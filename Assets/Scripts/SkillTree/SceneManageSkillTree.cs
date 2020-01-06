using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class SceneManageSkillTree : MonoBehaviour {

	void Awake() {
		#if UNITY_EDITOR
		if(MasterManager.instance == null) {
			SceneManager.LoadScene("Master", LoadSceneMode.Additive);
		}
		#endif
	}
	void Start() {
		#if UNITY_EDITOR
		MasterManager.instance.SetCurrentSceneDEBUG("SkillTree");
		#endif
	}

	public void OnMainMenuClicked() {
		MasterManager.instance.SwitchSceneTo("MainMenu");
	}

	public void OnBackClicked() {
		MasterManager.instance.Back();
	}

	public void OnContinueClicked() {
		object levelId = MasterManager.instance.getMessage().GetValue<object>("levelId");
		object biome = MasterManager.instance.getMessage().GetValue<object>("biome");
		bool continueGame = (bool) MasterManager.instance.getMessage().GetValue<object>("continueGame");
		
		if(levelId != null && biome != null && continueGame) {
			MasterManager.instance.SwitchSceneTo("Game", MasterManager.instance.getMessage());
			// Since the message is meant to be received here it will be consumed
			MasterManager.instance.getMessage().Remove("continueGame");
		}
	}
}
