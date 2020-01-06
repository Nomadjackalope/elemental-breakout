using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class SceneManageSettings : MonoBehaviour {

	void Awake() {
		#if UNITY_EDITOR
		if(MasterManager.instance == null) {
			SceneManager.LoadScene("Master", LoadSceneMode.Additive);
		}
		#endif
	}

	public void OnBackClicked() {
		MasterManager.instance.Back();
	}
}
