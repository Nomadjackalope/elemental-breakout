using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FinalRewards : MonoBehaviour {

	private Dictionary<DataTypes.BiomeType, int> essence;
	private int bonus;
	private int score;
	private int coins;
	private float time;
	private int scoreBonusFromTime;

	// These are used to display a current count of score
	private int runningScore;
	private int runningCoins;

	public Text bonusCount;
	public Text scoreCount;
	public Text timeCount;
	public Text coinsCount;
	public Text highScore;
	public Text bestTime;
	public GameObject essenceCountLayout;

	public GameObject EssenceCountPrefab;
	public EssenceIcons icons;

	public Button continueButton;
	public Button retryButton;
	public Image skillsButtonAnim;

	private string ss, mm;
	
	private bool finishNow = false;

	void OnEnable() {
		if(GameManage.instance != null) {

			score = GameManage.instance.getScore();
			essence = GameManage.instance.getEssence();
			time = GameManage.instance.getTime();

			retryButton.gameObject.SetActive(false);

			timeCount.text = "0:00.00";
			bestTime.text = "";
			scoreCount.text = "0";
			highScore.text = "";

			scoreBonusFromTime = getScoreFromTime(); 
			
			coins = score / 16;// + getCoinsFromTime();
			StartCoroutine(runCounts());

			DataTypes.BiomeType curBiome = GameManage.instance.GetBiomeType();
			int curLevel = GameManage.instance.GetLevelId();

			string bestTimeText = MasterPlayerData.instance.getBestTime(curBiome, curLevel);
			if(bestTimeText != "" && curLevel >= 0) {
				bestTime.text = "Best: " + bestTimeText;
				retryButton.gameObject.SetActive(true);
			}

			string highScoreText = MasterPlayerData.instance.getHighScore(curBiome, curLevel);
			if(highScoreText != "") {
				highScore.text = "Best: " + highScoreText;
				retryButton.gameObject.SetActive(true);
			}
			
			bool newPlayer = MasterPlayerData.instance.getCoins() == 0;

			// Give coins after tally up animation
			MasterPlayerData.instance.addCoins(coins);
			
			foreach (DataTypes.BiomeType type in essence.Keys)
			{

				// give new player enough essence
				if(newPlayer && curBiome == type && essence[type] < 5) {
					essence[type] = 5;
				}

				addEssenceCount(type, essence[type]);
				MasterPlayerData.instance.addEssence(type, essence[type]);
			}

			// Ask player how many essence they want to give to the girl 
			//   and remove them here as a percentage of essence they gained this round

			

			MasterPlayerData.instance.changeHighScore(curBiome, curLevel, score);
			MasterPlayerData.instance.changeBestTime(curBiome, curLevel, time);
			// MasterPlayerData.instance.unlockNextLevel(curBiome);

			if(!GameManage.instance.hasNextLevel()) {
				continueButton.GetComponent<Button>().onClick.RemoveAllListeners();
				continueButton.GetComponent<Button>().onClick.AddListener(delegate {
					GameManage.instance.OnQuitToLevelSelect();
				});
			}
			
			// continueButton.gameObject.SetActive(GameManage.instance.hasNextLevel());
			

			Time.timeScale = 1;

			// If current paddle has no biome selected
			if(MasterPlayerData.instance.getActivePaddle().branchesPurchased.Count == 0) {
				skillsButtonAnim.GetComponent<Animator>().SetBool("skillsAvailable", true);
				print("setting true");
			} else {
				skillsButtonAnim.GetComponent<Animator>().SetBool("skillsAvailable", false);
				print("setting false");
			}

			// if(curLevel == 10) {
			// 	Elementalist.instance.sayEasyHappy("Congratulations! You beat the final " + curBiome + " challenge.");
			// }

		}
	}

	private IEnumerator runCounts() {
		yield return StartCoroutine(countUpTime(timeCount, 0, time, 1));
		
		if(!finishNow) {
			yield return bopTimeDown();
		}

		yield return StartCoroutine(countUp(scoreCount, scoreBonusFromTime, score, 1));

		if(!finishNow) {
			yield return bopScoreDown();
		}
		
		//yield return StartCoroutine(countUp(coinsCount, 0, coins, 2));
		coinsCount.text = coins.ToString();
	}

	private IEnumerator bopTimeDown() {
		GameObject clone = Instantiate(timeCount.gameObject, timeCount.transform);
		((RectTransform)clone.transform).anchoredPosition = Vector2.zero;
		clone.transform.SetParent(scoreCount.transform);

		yield return ((RectTransform)clone.transform).DOAnchorPos(Vector2.zero, 0.2f, false).WaitForCompletion();
		Destroy(clone);

		scoreCount.text = scoreBonusFromTime.ToString();
		Color prevColor = scoreCount.color;
		Sequence bopDown = DOTween.Sequence();
		yield return bopDown
			.Append(scoreCount.transform.DOPunchPosition(Vector3.down * 30f, 1f, 3, 1f))
			.Join(scoreCount.DOColor(new Color(0.584f, 0.8509f, 0.5725f), 1f))
			.Append(scoreCount.DOColor(prevColor, 0.5f)).WaitForCompletion();
	}

	private IEnumerator bopScoreDown() {
		GameObject clone = Instantiate(scoreCount.gameObject, scoreCount.transform);
		((RectTransform)clone.transform).anchoredPosition = Vector2.zero;
		clone.transform.SetParent(coinsCount.transform);

		Sequence shrinkDown = DOTween.Sequence();

		yield return shrinkDown
			.Append(((RectTransform)clone.transform).DOAnchorPos(Vector2.zero, 0.2f, false))
			.Join(clone.transform.DOScale(0, 0.2f)).WaitForCompletion();
		Destroy(clone);

		coinsCount.text = coins.ToString();
		Color prevColor = coinsCount.color;
		Sequence bopDown = DOTween.Sequence();
		yield return bopDown
			.Append(coinsCount.transform.DOPunchPosition(Vector3.down * 30f, 1f, 3, 1f))
			.Join(coinsCount.DOColor(new Color(0.584f, 0.8509f, 0.5725f), 1f))
			.Append(coinsCount.DOColor(prevColor, 0.5f)).WaitForCompletion();
	}

	public void OnPanelClicked() {
		finishNow = true;
	}

	IEnumerator countUp(Text textObj, int initVal, int finalVal, float time) {
		float curval = initVal;
		textObj.text = initVal.ToString();
		float stepsPerSecond = ((float)(finalVal - initVal))/time; // steps/second // 1000/10 = 100 steps/sec 

		while(curval < finalVal && !finishNow) {
			curval += stepsPerSecond / 60f;
			textObj.text = ((int) curval).ToString();
			yield return new WaitForSeconds(1f/60f);
		}

		textObj.text = finalVal.ToString();
	}

	IEnumerator countUpTime(Text textObj, int initVal, float finalVal, float time) {
		float curval = initVal;
		textObj.text = initVal.ToString();
		float stepsPerSecond = ((float)(finalVal - initVal))/time; // steps/second // 1000/10 = 100 steps/sec 

		while(curval < finalVal && !finishNow) {
			curval += stepsPerSecond / 60f;
			ss = (curval % 60).ToString("00.00");
			mm = (Mathf.Floor(curval / 60f) % 60).ToString();
			textObj.text = mm + ":" +  ss;
			yield return new WaitForSeconds(1f/60f);
		}


		ss = (finalVal % 60).ToString("00.00");
		mm = (Mathf.Floor(finalVal / 60f) % 60).ToString();

		textObj.text = mm + ":" + ss;
	}

	void addEssenceCount(DataTypes.BiomeType biome, int count) {
		GameObject essenceCountInstance = Instantiate(EssenceCountPrefab, essenceCountLayout.transform);
		essenceCountInstance.GetComponentInChildren<Text>().text = count.ToString();
		if(icons != null) {
			essenceCountInstance.GetComponentInChildren<Image>().sprite = icons.getIconFrom(biome);
		}
	}

	private int getCoinsFromTime() {
		return (int)Mathf.Pow(1.32f, (20-(time/12)));
	}

	private int getScoreFromTime() {
		return (int)Mathf.Pow(1.32f, (20-(time/12)))*10;
	}

}
