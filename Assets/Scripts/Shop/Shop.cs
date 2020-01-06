using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.Advertisements;

public class Shop : MonoBehaviour {//, IUnityAdsListener {

	private int paddleCost = 3600;
	private int singleEssenceCost = 120;
	private int tenEssenceCost = 1000;
	private int randomEssencePer = 80;
	private int randomPackCost = 800;
	private int randAdCooldownSeconds = 60 * 10; // 10 minutes

	public Text singleEssenceLabel;
	public Text bulkEssenceLabel;

	public Text coinCountText;
	public Text fireEssenceCostText;
	public Text fireBulkEssenceCostText;
	public Text paddleCostText;
	public Text randomEssenceCostText;

	public Button singleEssenceButton;
	public Button tenEssenceButton;
	public Button randomEssenceButton;
	

	private Dictionary<DataTypes.BiomeType, int> essence;
	public GameObject essenceCountLayout;
	public GameObject EssenceCountPrefab;
	public EssenceIcons icons;
	private Dictionary<DataTypes.BiomeType, GameObject> essenceCounts = new Dictionary<DataTypes.BiomeType, GameObject>();

	DataTypes.BiomeType selectedBiome = DataTypes.BiomeType.Fire;

	private Dictionary<DataTypes.BiomeType, Toggle> essenceToggles = new Dictionary<DataTypes.BiomeType, Toggle>();

	Coroutine levelsLoading;
	public GameObject randAdButton;

	void Start() {

		if(MasterPlayerData.instance.getPaddles().Count > 2) {
			paddleCost = 4800;
		}

		// initialize amounts
		updateCoins();
		loadEssence();
		essenceCountLayout.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
		updateCosts();
		randAdButton.GetComponent<Button>().enabled = checkRandPurchaseAvailable();
		if(!checkRandPurchaseAvailable()) {
			randAdButton.GetComponentInChildren<Text>().text = getTimeSinceLastRandPurchase();
			StartCoroutine(updateTime());
		}
		randomEssenceCostText.text = randomPackCost + " Coins";

		Elementalist.instance.sayEasyHappyIfAvailable(HintIds.Coins);

		// Advertisement.AddListener(this);
	}

	void updateCosts() {
		fireEssenceCostText.text = singleEssenceCost + " Coins";
		fireBulkEssenceCostText.text = tenEssenceCost + " Coins";
		paddleCostText.text = paddleCost + " Coins";
	}

	public void OnBuySingleEssenceClicked() {
		if(buyEssence(selectedBiome, 1)) {
			// instantiate coins particle system going with text
		}

		updateCoins();
	}

	public void OnBuyBulkEssenceClicked() {
		if(buyEssence(selectedBiome, 10)) {
			// instantiate coins particle system going with text
		}

		updateCoins();
	}

	public void OnBuyRandomPackCoinClicked() {
		if(buyRandomEssence(10, randomPackCost)) {
			// instantiate coins particle system going with text
		}

		updateCoins();
	}

	public void OnBuyRandomPackAdClicked() {
		if(checkRandPurchaseAvailable()) {
		
			ShowAd();
			
		}
	}

	public string placementId = "randomessence";


	public void ShowAd() {
		// if (Advertisement.IsReady(placementId))
		// {
		// 	//var options = new ShowOptions { resultCallback = HandleShowResult };
		// 	Advertisement.Show(placementId);//, options);
		// }

		buyRandomEssence(10, 0);
		randAdButton.GetComponent<Button>().enabled = false;
		PlayerPrefs.SetString("lastRandPurchase", getTimeSinceEpoch().ToString());
		StartCoroutine(updateTime());

	}


	bool checkRandPurchaseAvailable() {
		return getTimeSinceEpoch() - getLastRandPurchase() > randAdCooldownSeconds;
	}

	double getTimeSinceEpoch() {
		System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
 		return (System.DateTime.UtcNow - epochStart).TotalSeconds;
	}

	double getLastRandPurchase() {
		return double.Parse(PlayerPrefs.GetString("lastRandPurchase", "0"));
	}

	string getTimeSinceLastRandPurchase() {
		double secondsLeft = randAdCooldownSeconds - (getTimeSinceEpoch() - getLastRandPurchase());
		int hours = Mathf.FloorToInt((float)(secondsLeft / (60 * 60)));
		int minutes = Mathf.FloorToInt((float)((secondsLeft - hours * (60 * 60)) / 60));
		int seconds = Mathf.FloorToInt((float)((secondsLeft - hours * (60 * 60)) - minutes * 60));
		
		string timeUntil = "";

		timeUntil += hours > 0 ? hours.ToString() + ":" : "";
		timeUntil += minutes > 0 ? minutes.ToString() + ":" : "";
		timeUntil += seconds.ToString();

		return timeUntil;
	}

	IEnumerator updateTime() {
		while(!checkRandPurchaseAvailable()) {
			randAdButton.GetComponentInChildren<Text>().text = getTimeSinceLastRandPurchase();
			yield return new WaitForSecondsRealtime(1);
		}

		randAdButton.GetComponent<Button>().enabled = true;
		randAdButton.GetComponentInChildren<Text>().text = "Advert";
	}


	void updateCoins() {
		coinCountText.text = "Coins: " + MasterPlayerData.instance.getCoins();

		singleEssenceButton.interactable = MasterPlayerData.instance.getCoins() >= singleEssenceCost;
		tenEssenceButton.interactable = MasterPlayerData.instance.getCoins() >= tenEssenceCost;
		randomEssenceButton.interactable = MasterPlayerData.instance.getCoins() >= randomPackCost;
	}

	public bool buyEssence(DataTypes.BiomeType type, int amount) {
		if(amount == 1){
			if(MasterPlayerData.instance.subtractCoins(singleEssenceCost)) {
				MasterPlayerData.instance.addEssence(type, amount);
				updateEssence();
				return true;
			}
		} else { 
			if(MasterPlayerData.instance.subtractCoins(tenEssenceCost)) {
				MasterPlayerData.instance.addEssence(type, amount);
				updateEssence();
				return true;
			}
		}

		return false;
	}

	public bool buyRandomEssence(int amount, int coins) {
		if(MasterPlayerData.instance.subtractCoins(coins)) {
			for (int i = 0; i < amount; i++)
			{	
				MasterPlayerData.instance.addEssence((DataTypes.BiomeType)Random.Range(0, 6), 1);
			}

			updateEssence();
			return true;
		}

		return false;
	}

	void updateEssence() {
		foreach (DataTypes.BiomeType biome in essenceCounts.Keys)
		{
			essenceCounts[biome].GetComponentInChildren<Text>().text = essence[biome].ToString();
		}
	}

	void loadEssence() {
		essence = MasterPlayerData.instance.getEssence();

		foreach (DataTypes.BiomeType type in essence.Keys)
		{
			addEssenceCount(type, essence[type]);
		}
	}

	void addEssenceCount(DataTypes.BiomeType biome, int count) {
		GameObject essenceCountInstance = Instantiate(EssenceCountPrefab, essenceCountLayout.transform);
		essenceCountInstance.GetComponentInChildren<Text>().text = count.ToString();
		if(icons != null) {
			essenceCountInstance.transform.GetChild(1).GetComponent<Image>().sprite = icons.getIconFrom(biome);
		}
		essenceCountInstance.GetComponent<Toggle>().group = essenceCountLayout.GetComponent<ToggleGroup>();
		essenceCountInstance.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool value) { switchElement(biome); });

		essenceCounts.Add(biome, essenceCountInstance);
	}

	void switchElement(DataTypes.BiomeType biome) {
		selectedBiome = biome;

		singleEssenceLabel.text = "1 " + biome.ToString() + " Essence";
		bulkEssenceLabel.text = "10 " + biome.ToString() + " Essence";
	}


	DataTypes.BiomeType getTypeFromString(string type) {
		switch (type)
		{
			case "Fire":
				return DataTypes.BiomeType.Fire;
			case "Earth":
				return DataTypes.BiomeType.Earth;
			case "Water":
				return DataTypes.BiomeType.Water;
			case "Growth":
				return DataTypes.BiomeType.Growth;
			case "Shadow":
				return DataTypes.BiomeType.Shadow;
			case "Poison":
				return DataTypes.BiomeType.Poison;
			default:
				return DataTypes.BiomeType.Fire;
		}
	}

    // public void OnUnityAdsReady(string placementId)
    // {
    // }

    // public void OnUnityAdsDidError(string message)
    // {
    // }

    // public void OnUnityAdsDidStart(string placementId)
    // {
    // }

    // public void OnUnityAdsDidFinish(string placementId, ShowResult result)
    // {
	// 	switch (result)
	// 	{
	// 		case ShowResult.Finished:
	// 		Debug.Log("The ad was successfully shown.");
			
	// 		buyRandomEssence(10, 0);
	// 		randAdButton.GetComponent<Button>().enabled = false;
	// 		PlayerPrefs.SetString("lastRandPurchase", getTimeSinceEpoch().ToString());
	// 		StartCoroutine(updateTime());
			
	// 		break;
	// 		case ShowResult.Skipped:
	// 		Debug.Log("The ad was skipped before reaching the end.");
	// 		break;
	// 		case ShowResult.Failed:
	// 		Debug.LogError("The ad failed to be shown.");
	// 		break;
	// 	}
    // }
}
