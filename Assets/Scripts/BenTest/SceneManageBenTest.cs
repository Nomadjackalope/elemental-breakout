#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManageBenTest : MonoBehaviour {

	void Awake() {
		
		if(MasterManager.instance == null) {
			SceneManager.LoadScene("Master", LoadSceneMode.Additive);
		}
		
	}

	void Start() {
		MasterManager.instance.SetCurrentSceneDEBUG("MainMenu");
	}

}
#endif
