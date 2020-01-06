using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using UnityEngine.Advertisements;
using DG.Tweening;

public class MasterManager : MonoBehaviour {

	public static MasterManager instance;

	string previousScene;
	string currentScene;

	private GenericDictionary message;

	public Animator transitionAnim;
	private bool isLoading;

	public GameObject transitionIconObject;
	public Canvas masterCanvas;

	public Camera masterCamera;

	private int numGamesSinceLastAd = 0;
	private float timeSinceLastAd = 0;

	public Font testFont;
	public bool overrideFont = true;

	public Sprite testSprite;
	public bool overrideSprite = true;

	// #if UNITY_IOS
	// string gameId = "YOUR_GAMEID";
	// #else
	// string gameId = "YOUR_GAMEID";
	// #endif

	// bool testMode = true;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		Application.targetFrameRate = 60;

		// Advertisement.Initialize(gameId, testMode);
	}
	// Use this for initialization
	void Start () {
		// #if UNITY_EDITOR
		// 	Debug.LogWarning("Master scene added for debugging");
		// #else 
			SwitchSceneTo("MainMenu");
		// #endif
		
	}

	public void SwitchSceneTo(string sceneName) {
		SwitchSceneTo(sceneName, null);
	}

	public void SwitchSceneTo(string sceneName, GenericDictionary message) {
		if(isLoading) {
			return;
		}

		// Make sure other scenes aren't affected by game
		Time.timeScale = 1;

		MasterMusic.instance.switchMusic(sceneName, message);
		
		StartCoroutine(switchToNextSceneTry2(sceneName));
	

		//Debug.Log("Switching to " + sceneName + " scene");
		this.message = message;
	}

	

	private IEnumerator switchToNextSceneTry2(string sceneName) {
		isLoading = true;
		if(transitionAnim.isActiveAndEnabled) {
			yield return StartCoroutine(playStartTransition());
		}

		if(currentScene != null && currentScene.Length != 0) {
			yield return StartCoroutine(unloadCurrentScene());
		}

		yield return StartCoroutine(loadNextScene2(sceneName));

		if(transitionIconObject != null) {
			yield return new WaitForSeconds(0.5f);
			yield return transitionIconObject.GetComponent<CanvasGroup>().DOFade(0, 0.5f).WaitForCompletion();
			Destroy(transitionIconObject);
		}

		if(transitionAnim.isActiveAndEnabled) {
			yield return StartCoroutine(playEndTransition());
		}

		previousScene = currentScene;
		currentScene = sceneName;

		replaceFont();
		replaceSprite();

		isLoading = false;
	}

	IEnumerator playStartTransition() {
		print("starting transition");
		transitionAnim.SetTrigger("start");
		yield return new WaitForSeconds(0.5f);
	}

	IEnumerator playEndTransition() {
		print("ending transition");
		transitionAnim.SetTrigger("end");
		yield return new WaitForSeconds(0.5f);
	}


	IEnumerator loadNextScene2(string sceneName) {
		AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

		while(!async.isDone) {

			yield return null;

		}
	}

	IEnumerator unloadCurrentScene() {
		// Debug.LogWarning("ASYNC LOAD STARTED - " +
        //     "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");

		yield return SceneManager.UnloadSceneAsync(currentScene);
	}


	public void Back() {
		if(currentScene != null && currentScene.Length != 0) {
			SwitchSceneTo(previousScene);
		} else {
			Debug.LogWarning("No previous scene to return to");
		}
	}

	#if UNITY_EDITOR
	public void SetCurrentSceneDEBUG(string currentScene) {
		this.currentScene = currentScene;
	}
	#endif

	public GenericDictionary getMessage() {
		return message;
	}

	public static bool isMenu(string sceneName) {
		return sceneName == "MainMenu" ||
			sceneName == "Settings" ||
			sceneName == "Shop" ||
			sceneName == "SkillTree" ||
			sceneName == "LevelSelect" ||
			sceneName == "Hints";
	}

	// public string placementId = "video";


    // public bool ShowAd(ShowOptions options)
    // {
	// 	numGamesSinceLastAd++;


	// 	// print("gameSinceAd: " + numGamesSinceLastAd);

	// 	if(numGamesSinceLastAd >= 3 && Time.realtimeSinceStartup - timeSinceLastAd > 5 * 60) {
	// 		// print("tried to show ad");
	// 		numGamesSinceLastAd = 0;
	// 		timeSinceLastAd = Time.realtimeSinceStartup;
		
    //     	if (Advertisement.IsReady(placementId))
    //     	{
	// 			Advertisement.Show(placementId, options);
	// 			return true;
    //     	}
	// 	}

	// 	return false;
	// }

	void replaceFont() {
		if(overrideFont) {
			Text[] texts = Resources.FindObjectsOfTypeAll<Text>();

			foreach (Text text in texts)
			{
				if(text.gameObject.name != "Title") {
					text.font = testFont;
				}
			}
		}
	}

	void replaceSprite() {
		if(overrideSprite) {
			SpriteRenderer[] sprites = Resources.FindObjectsOfTypeAll<SpriteRenderer>();

			foreach (SpriteRenderer renderer in sprites)
			{
				
				renderer.sprite = testSprite;
				
			}
		}
	}
}
