using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class SkillTree : MonoBehaviour {

	public static SkillTree instance;

	public GameObject DescriptionPanel;
	private Text skillName;
	private Text description;
	private Image skillIcon;
	// private Button purchaseButton;
	private Skill currentSkill;
	private Text isActive;

	public SkillDataList skillDataList;

	public GameObject essenceCountLayout;
	public GameObject EssenceCountPrefab;
	public EssenceIcons icons;
	public GameObject essenceAmount;

	public BiomePath path;

	private DataTypes.BiomeType biomeToAdd;
	private PowerUps powerUpToAdd;
	//private ToggleSkill.callback powerupCallback;

	public GameObject ActivePopUp;
	//public GameObject BiomePopUp; // not used
	public GameObject PurchaseActivePopUp;
	public GameObject BiomeInfoPopUp; // Simply tells user to buy a new skill set to equip different combinations of biomes

	bool activeCheckShown;
	//bool biomeCheckShown;

	private int costPerRow = 5;

	public Button continueButton;

	public GameObject skillExplanationPanel;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		if(!MasterPlayerData.instance.checkSeenHint(HintIds.SkillTree)) {
			Elementalist.instance.sayEasyHappy(Hints.getHintFrom(HintIds.SkillTree));
			Elementalist.instance.sayEasyHappy(Hints.getHintFrom(HintIds.SkillTreeSuggest) + getTopEssenceCountBiome());
			MasterPlayerData.instance.seenHint(HintIds.SkillTree);
		}

		skillExplanationPanel.gameObject.SetActive(false);

	}

	string getTopEssenceCountBiome() {
		int baseVal = 0;
		DataTypes.BiomeType biome = DataTypes.BiomeType.Fire;
		foreach (var item in MasterPlayerData.instance.getEssence())
		{
			if(item.Value > baseVal) {
				baseVal = item.Value;
				biome = item.Key;
			}
		} 

		return biome.ToString();
	}

	void Start() {
		// foreach (Transform t in DescriptionPanel.transform)
		// {
		// 	if(t.name == "Name") {
		// 		skillName = t.GetComponent<Text>();
		// 		skillName.text = "";
		// 	} else if(t.name == "Description") {
		// 		description = t.GetComponent<Text>();
		// 		description.text = "";
		// 	} else if(t.name == "Image") {
		// 		skillIcon = t.GetComponent<Image>();
		// 		skillIcon.enabled = false;
		// 	} else if(t.name == "Button") {
		// 		purchaseButton = t.GetComponent<Button>();
		// 		// purchaseButton.onClick.AddListener(() => purchasePowerUp());
		// 		purchaseButton.gameObject.SetActive(false);
		// 	} else if(t.name == "isActive") {
		// 		isActive = t.GetComponent<Text>();
		// 		isActive.text = "";
		// 	}
		// }

		// if(MasterPlayerData.instance.branchesAvailableOnActivePaddle() <= 0) {
		// 	disableOthers();
		// }

		//BiomePopUp.SetActive(false);
		ActivePopUp.SetActive(false);
		PurchaseActivePopUp.SetActive(false);
		BiomeInfoPopUp.SetActive(false);


		GenericDictionary message = MasterManager.instance.getMessage();
		if(continueButton != null && message != null && message.GetValue<object>("continueGame") != null && (bool)message.GetValue<object>("continueGame")) {
			continueButton.gameObject.SetActive(true);
		} else {
			continueButton.gameObject.SetActive(false);
		}
	}

	public void activatePowerUp(PowerUps powerUp) {
		MasterPlayerData.instance.addPowerUpToActivePaddle(powerUp);
	}

	public void deactivatePowerUp(PowerUps powerUp) {
		MasterPlayerData.instance.removePowerUpToActivePaddle(powerUp);
	}

	// public void OnPowerUpClicked(Skill data) {

	// 	currentSkill = data;
	// 	if(skillName != null) {
	// 		skillName.text = data.skillName;
	// 	}

	// 	if(description != null) {
	// 		description.text = data.description;
	// 	}

	// 	if(skillIcon != null) {
	// 		skillIcon.enabled = true;
	// 		skillIcon.sprite = data.icon;
	// 	}

	// 	if(purchaseButton != null) {
	// 		if(MasterPlayerData.instance.checkPowerupUnlocked(data.powerUp)) {
	// 			purchaseButton.gameObject.SetActive(false);
	// 			essenceCountLayout.gameObject.SetActive(false);
	// 		} else {

	// 			purchaseButton.gameObject.SetActive(true);
	// 			essenceCountLayout.gameObject.SetActive(true);

	// 			// can purchase if they don't already have it and they have one from previous tier and 
	// 			bool canPurchase = checkPreviousTier(data) && MasterPlayerData.instance.checkEssence(data.biome, data.Row * costPerRow) 
	// 				&& MasterPlayerData.instance.branchEnabled(data.biome);
	// 			purchaseButton.interactable = canPurchase;//gameObject.SetActive(canPurchase);

	// 			for(int i = essenceCountLayout.transform.childCount - 1; i >= 0; i--) {
	// 				Destroy(essenceCountLayout.transform.GetChild(i).gameObject);
	// 			}
	// 			addEssenceCount(data.biome, data.Row * 5, canPurchase, essenceCountLayout.transform);
	// 		}
	// 	}

	// 	if(essenceAmount != null) {
	// 		for(int i = essenceAmount.transform.childCount - 1; i >= 0; i--) {
	// 			Destroy(essenceAmount.transform.GetChild(i).gameObject);
	// 		}
	// 		addEssenceCount(data.biome, MasterPlayerData.instance.getEssence()[data.biome], true, essenceAmount.transform);
	// 	}

	// 	if(isActive != null) {
	// 		isActive.text = data.isAnActive ? "Active" : "Passive";
	// 	}
	// }

	// public void purchasePowerUp() {
	// 	if(MasterPlayerData.instance.subtractEssence(currentSkill.biome, currentSkill.Row * costPerRow)) {
	// 		if(currentSkill.isAnActive && MasterPlayerData.instance.activePaddleHasActive()) {
	// 			MasterPlayerData.instance.purchaseSkillToActivePaddle(currentSkill.powerUp, false);
	// 			showActiveCheckPanel(currentSkill.powerUp);
	// 		} else {
	// 			MasterPlayerData.instance.purchaseSkillToActivePaddle(currentSkill.powerUp);
	// 		}

	// 		purchaseButton.gameObject.SetActive(false);
	// 		essenceCountLayout.gameObject.SetActive(false);

	// 		if(currentSkill.isAnActive && !MasterPlayerData.instance.GetPlayerData().seenActiveExplanation) {
	// 			Elementalist.instance.sayEasyHappy(Hints.getHintFrom(HintIds.ActiveExplanation));
	// 			MasterPlayerData.instance.GetPlayerData().seenActiveExplanation = true;
	// 			MasterPlayerData.instance.savePlayerDataAsync();
	// 		}
	// 	}
	// }


	// Called from Skillbar 
	public void purchasePowerUp(Skill skill) {
		if(MasterPlayerData.instance.getEssence()[skill.biome] >= skill.Row * costPerRow) {//subtractEssence(skill.biome, skill.Row * costPerRow)) {
			if(skill.isAnActive && MasterPlayerData.instance.activePaddleHasActive()) {
				showPurchaseActiveCheckPanel(skill.powerUp);
				return;
			}
			
				
			turnOffOtherSkillOnRow(skill.biome, skill.Row);

			if(MasterPlayerData.instance.purchaseSkillToActivePaddle(skill.powerUp)) {
				MasterPlayerData.instance.subtractEssence(skill.biome, skill.Row * costPerRow);
			}
			

			// purchaseButton.gameObject.SetActive(false);
			essenceCountLayout.gameObject.SetActive(false);

			Elementalist.instance.sayEasyHappyIfAvailable(HintIds.Skills);

			if(skill.isAnActive) {
				Elementalist.instance.sayEasyHappyIfAvailable(HintIds.ActiveExplanation);
			}
		}
	}

	public void activateSkill(Skill skill) {
		if(skill.isAnActive && MasterPlayerData.instance.activePaddleHasActive()) {
			showActiveCheckPanel(skill.powerUp);
		} else {
			turnOffOtherSkillOnRow(skill.biome, skill.Row);
		}

		MasterPlayerData.instance.addPowerUpToActivePaddle(skill.powerUp);
	}

	void turnOffOtherSkillOnRow(DataTypes.BiomeType biome, int row) {
		List<PowerUps> toRemove = new List<PowerUps>();
		// Remove powerup with same row as new powerup from runeids
		foreach (PowerUps item in MasterPlayerData.instance.getActivePaddle().runeIds)
		{
			if(MasterPlayerData.instance.skillDataList.getSkillData(item).Row == row
				&& MasterPlayerData.instance.skillDataList.getSkillData(item).biome == biome) {
				toRemove.Add(item);
			}
		}

		foreach(PowerUps item in toRemove) {
			MasterPlayerData.instance.removePowerUpToActivePaddle(item);
		}
	}

	public void deactivateSkill(PowerUps powerUp) {
		MasterPlayerData.instance.removePowerUpToActivePaddle(powerUp);
	}

	public int getSkillCost(Skill skillData) {
		return costPerRow * skillData.Row;
	}

	public void enableBranch(DataTypes.BiomeType biome) {
		if(MasterPlayerData.instance.branchesAvailableOnActivePaddle() > 0) {
			showBiomeCheckPanel(biome);
		} else {
			showBiomeInfoPanel();
		}
	}

	public bool checkPreviousTier(Skill data) {
		if(data.Row == 1) {
			// first tier skills can always be purchased
			return true;
		} else {
			// if a skill in the previous row has been purchased return true
			List<Skill> possibleSkills = skillDataList.getSkillList(data.biome).FindAll(x => x.Row == data.Row - 1);

			foreach (Skill skill in possibleSkills)
			{
				if(MasterPlayerData.instance.checkPowerupUnlocked(skill.powerUp)) {
					return true;
				}
			}

		}

		return false;
	}

	void addEssenceCount(DataTypes.BiomeType biome, int count, bool canPurchase, Transform layout) {
		GameObject essenceCountInstance = Instantiate(EssenceCountPrefab, layout);
		essenceCountInstance.GetComponentInChildren<Text>().text = count.ToString();
		if(icons != null) {
			essenceCountInstance.GetComponentInChildren<Image>().sprite = icons.getIconFrom(biome);
		}
		
		essenceCountInstance.GetComponentInChildren<Image>().color = canPurchase ? Color.white : new Color(0.5f, 0.5f, 0.5f);
		essenceCountInstance.GetComponentInChildren<Text>().color = canPurchase ? Color.white : new Color(0.5f, 0.5f, 0.5f);
		
	}

	// void disableOthers() {
	// 	List<DataTypes.BiomeType> activeBranches = MasterPlayerData.instance.getActivePaddle().branchesPurchased;

	// 	if(!activeBranches.Contains(DataTypes.BiomeType.Fire)) {
	// 		// FirePanel.gameObject.SetActive(false);
	// 		Destroy(FirePanel.gameObject);
	// 	}

	// 	if(!activeBranches.Contains(DataTypes.BiomeType.Water)) {
	// 		// WaterPanel.gameObject.SetActive(false);
	// 		Destroy(WaterPanel.gameObject);
	// 	}

	// 	if(!activeBranches.Contains(DataTypes.BiomeType.Earth)) {
	// 		// EarthPanel.gameObject.SetActive(false);
	// 		Destroy(EarthPanel.gameObject);

	// 	}

	// 	if(!activeBranches.Contains(DataTypes.BiomeType.Growth)) {
	// 		// GrowthPanel.gameObject.SetActive(false);
	// 		Destroy(GrowthPanel.gameObject);
	// 	}

	// 	if(!activeBranches.Contains(DataTypes.BiomeType.Shadow)) {
	// 		// ShadowPanel.gameObject.SetActive(false);
	// 		Destroy(ShadowPanel.gameObject);
	// 	}

	// 	if(!activeBranches.Contains(DataTypes.BiomeType.Poison)) {
	// 		// PoisonPanel.gameObject.SetActive(false);
	// 		Destroy(PoisonPanel.gameObject);
	// 	}

	// 	//StartCoroutine(resetScreensNextFrame());
	// }

	// IEnumerator resetScreensNextFrame() {
	// 	GameObject snappersObject = snapper.gameObject;
	// 	Destroy(snapper);
	// 	yield return null;
	// 	snappersObject.AddComponent<HorizontalScrollSnap>();

	// }

	public void showBiomeCheckPanel(DataTypes.BiomeType biome) {
		biomeToAdd = biome;
		confirmBiome();
		// BiomePopUp.GetComponentInChildren<BiomeConfirmation>().SetBiomeType(biome);
		// BiomePopUp.SetActive(true);
	}

	public void showBiomeInfoPanel() {
		BiomeInfoPopUp.SetActive(true);
	}

	public void showActiveCheckPanel(PowerUps powerUp) {//, ToggleSkill.callback func) {
		powerUpToAdd = powerUp;
		ActivePopUp.SetActive(true);
	}

	public void showPurchaseActiveCheckPanel(PowerUps powerUp) {
		powerUpToAdd = powerUp;
		PurchaseActivePopUp.SetActive(true);
	}

	public void confirmBiome() {
		MasterPlayerData.instance.enableBranchOnActivePaddle(biomeToAdd);

		if(MasterPlayerData.instance.branchesAvailableOnActivePaddle() <= 0) {
			//disableOthers();
		}

		path.setDirty();

        // BiomePopUp.SetActive(false);
    }

    // public void rejectBiome() {
    //     BiomePopUp.SetActive(false);
    // }

	public void confirmActive() {
		MasterPlayerData.instance.switchActiveOnActivePaddle(powerUpToAdd);
		ActivePopUp.SetActive(false);
		path.setDirty();

		
		// if(powerupCallback != null) {
		// 	powerupCallback(powerUpToAdd);
		// }
	}

	public void rejectActive() {
		ActivePopUp.SetActive(false);
	}

	public void confirmActivePurchase() {
		Skill skill = MasterPlayerData.instance.skillDataList.getSkillData(powerUpToAdd);

		if(MasterPlayerData.instance.purchaseSkillToActivePaddle(powerUpToAdd, true)) {
			MasterPlayerData.instance.subtractEssence(skill.biome, skill.Row * costPerRow);
		}
		PurchaseActivePopUp.SetActive(false);

		if(skill.isAnActive) {

			Elementalist.instance.sayEasyHappyIfAvailable(HintIds.ActiveExplanation);
		}

		path.setDirty();
	}

	public void rejectPurchaseActive() {
		PurchaseActivePopUp.SetActive(false);
	}

	public void OpenSkillExplanation(SkillBar skillBar) {
		skillExplanationPanel.gameObject.SetActive(true);
		skillExplanationPanel.GetComponent<SkillExplanation>().setData(skillBar.skillData);
	}
}
