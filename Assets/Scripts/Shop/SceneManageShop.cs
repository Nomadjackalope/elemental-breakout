using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class SceneManageShop : MonoBehaviour {

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

	public void OnBackClicked() {
		MasterManager.instance.SwitchSceneTo("MainMenu");
	}
}
