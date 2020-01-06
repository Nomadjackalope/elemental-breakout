using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelSelectLoader : MonoBehaviour {

	private DataTypes.BiomeType curBiome = DataTypes.BiomeType.Default;

	public RectTransform content;
	public GameObject levelButtonPrefab;

	Coroutine levelsLoading;

	public SelectBiomeLevel selectBiome;

	public GameObject MainButtons;
	public GameObject BiomeIconsContent;
	public GameObject SafePanel;
	public GameObject IconPrefab;

	void Start() {
		//StartCoroutine(delayStart());
	}

	IEnumerator delayStart() {
		yield return new WaitForSeconds(0.2f);
		levelsLoading = StartCoroutine(reloadList());
	}

	public void switchBiome(string biomeName) {
		switchBiome(DataTypes.GetBiomeFrom(biomeName));
	}

	public void switchBiome(DataTypes.BiomeType biome) {
		print("biome: " + biome);
		//selectBiome.IveBeenClicked();
		if(curBiome != biome) {
			curBiome = biome;
			if(levelsLoading != null) {
				StopCoroutine(levelsLoading);
			}
			levelsLoading = StartCoroutine(reloadList());
		}
	}

	private IEnumerator reloadList() {
		// Clear List
		int childCount = content.transform.childCount;
 
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(content.transform.GetChild(i).gameObject);
        }

		List<int> levelIds = LevelLoader.instance.getLevelList(curBiome);

		if(MasterPlayerData.instance.checkIfLevelUnlocked(curBiome, 10)) {
			GameObject button = Instantiate(levelButtonPrefab, content);
			button.GetComponent<LevelButtonAlt>().setLevelData(curBiome, -1);
			button.GetComponent<LevelButtonAlt>().setButtonDelegate(delegate() { StartGame(-1); });
		}

		// iterate through levels making buttons
		foreach (int levelId in levelIds)
		{
			if(!MasterPlayerData.instance.checkIfLevelUnlocked(curBiome, levelId) || levelId == -1) {
				continue;
			}
			GameObject button = Instantiate(levelButtonPrefab, content);
			button.GetComponent<LevelButtonAlt>().setLevelData(curBiome, levelId);
			button.GetComponent<LevelButtonAlt>().setButtonDelegate(delegate() { StartGame(levelId); });
			// button.GetComponentInChildren<Text>().text = levelId.ToString();
			// Component[] texts = button.GetComponentsInChildren<Text>();
			// if(texts != null) {
			// 	foreach (Text text in texts)
			// 	{
			// 		if(text.gameObject.tag == "TimeText") {
			// 			text.text = MasterPlayerData.instance.getBestTime(curBiome, levelId);
			// 		}

			// 		if(text.gameObject.tag == "ScoreText" ) {
			// 			text.text = MasterPlayerData.instance.getHighScore(curBiome, levelId);
			// 		}
			// 	}
			// }

			// button.GetComponent<Button>().onClick.AddListener(delegate() { StartGame(levelId); });
			// if(!MasterPlayerData.instance.checkIfLevelUnlocked(curBiome, levelId)) {
			// 	button.GetComponent<Button>().interactable = false;
			// 	foreach (Transform child in button.transform)
			// 	{
			// 		if(child.name == "Image"){
			// 			child.GetComponent<Image>().enabled = true;
			// 		}
			// 	}
			// }
			// button.GetComponent<Image>().color = DataTypes.GetButtonColorFrom(curBiome);
			// if(curBiome == DataTypes.BiomeType.Water || curBiome == DataTypes.BiomeType.Shadow) {
			// 	if(texts != null) {
			// 		foreach (Text text in texts)
			// 		{
			// 			text.color = Color.white;
			// 		}
			// 	}
			// }
			yield return new WaitForSeconds(0.04f);

		}
	}

	private IEnumerator unloadList() {
		int childCount = content.transform.childCount;
 
        for (int i = childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(content.transform.GetChild(i).gameObject);
			yield return new WaitForSeconds(0.04f);
        }
	}

	public void StartGame(int levelId) {
		// MasterPlayerData.instance.setLastLevelPlayed(curBiome, levelId);

		// GenericDictionary level = new GenericDictionary();
		// level.Add("levelId", (object) levelId);
		// level.Add("biome", (object) curBiome);
		// MasterManager.instance.SwitchSceneTo("Game", level);
		StartCoroutine(StartTransition(levelId));
	}

	private IEnumerator StartTransition(int levelId) {
		yield return StartCoroutine(unloadList());

		GameObject biomeIcon = getMainBiomeIcon();
		if(biomeIcon != null) {
			Vector2 sizeDelta = ((RectTransform)biomeIcon.transform).sizeDelta;

			GetComponent<CanvasGroup>().alpha = 0;
			BiomeIconsContent.GetComponent<CanvasGroup>().alpha = 0;
			MainButtons.GetComponent<CanvasGroup>().alpha = 0;

			GetComponent<CanvasGroup>().interactable = false;
			BiomeIconsContent.GetComponent<CanvasGroup>().interactable = false;
			MainButtons.GetComponent<CanvasGroup>().interactable = false;

			GameObject biomeIconCopy = Instantiate(IconPrefab, biomeIcon.transform.position, biomeIcon.transform.rotation, MasterManager.instance.masterCanvas.transform);
			MasterManager.instance.transitionIconObject = biomeIconCopy;
			biomeIconCopy.GetComponent<TransitionIcon>().setBackground(curBiome);
			biomeIconCopy.GetComponentInChildren<Text>().text = levelId >= 0 ? "Level " + levelId : "?";
			biomeIconCopy.GetComponentInChildren<Shadow>().effectColor = DataTypes.GetAccentColorFrom(curBiome);
			
			RectTransform rectTransform = ((RectTransform)biomeIconCopy.transform);
			rectTransform.sizeDelta = sizeDelta;

			RectTransform maskTransform = ((RectTransform)biomeIconCopy.transform.GetChild(0).GetChild(0).transform);
			RectTransform backgroundTransform = ((RectTransform)biomeIconCopy.transform.GetChild(0).GetChild(0).GetChild(0).transform);
			backgroundTransform.anchorMin = Vector2.one * 0.5f;
			backgroundTransform.anchorMax = Vector2.one * 0.5f;

	
			Sequence expandLevelIcon = DOTween.Sequence();
			yield return expandLevelIcon
				.Append(backgroundTransform.DOSizeDelta(new Vector2(2149, 3200), 0.5f))
				// .Append(rectTransform.DOSizeDelta(new Vector2(2149, 3200), 0.5f))
				.Join(rectTransform.DOAnchorPos(Vector2.zero, 0.5f))
				.Join(maskTransform.DOSizeDelta(new Vector2(2000, 2000), 0.5f))
				.WaitForCompletion();
		}

		MasterPlayerData.instance.setLastLevelPlayed(curBiome, levelId);

		GenericDictionary level = new GenericDictionary();
		level.Add("levelId", (object) levelId);
		level.Add("biome", (object) curBiome);
		MasterManager.instance.SwitchSceneTo("Game", level);

	}

	private GameObject getMainBiomeIcon() {
		LevelIconButton[] buttons = BiomeIconsContent.GetComponentsInChildren<LevelIconButton>();
		foreach (LevelIconButton button in buttons)
		{
			if(button.isMain) {
				return button.gameObject;
			}
		}

		return null;
	}
}
