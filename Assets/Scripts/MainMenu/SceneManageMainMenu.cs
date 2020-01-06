using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class SceneManageMainMenu : MonoBehaviour {

	void Awake() {
		#if UNITY_EDITOR
		if(MasterManager.instance == null) {
			SceneManager.LoadScene("Master", LoadSceneMode.Additive);
		}
		
		#endif
	}

	void Start() {
		#if UNITY_EDITOR
		MasterManager.instance.SetCurrentSceneDEBUG("MainMenu");
		#endif
	}

	public void OnPlayClicked() {
		MasterManager.instance.SwitchSceneTo("LevelSelect");
	}

	public void OnShopClicked() {
		MasterManager.instance.SwitchSceneTo("Shop");
	}

    public void OnSkillTreeClicked()
    {
        MasterManager.instance.SwitchSceneTo("SkillTree");
    }

    public void OnLoadDataClicked() {
		MasterPlayerData.instance.loadPlayerDataAsync();
	}

	public void OnSettingsClicked() {
		MasterManager.instance.SwitchSceneTo("Settings");
	}

	public void OnHintsClicked() {
		MasterManager.instance.SwitchSceneTo("Hints");
	}
}
