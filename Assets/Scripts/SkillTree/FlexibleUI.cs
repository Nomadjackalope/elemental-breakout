using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class FlexibleUI : MonoBehaviour {

	public Skill skillData;

	protected virtual void OnSkinUI() {

	}

	// Use this for initialization
	public virtual void Awake () {
		OnSkinUI();
	}
	
	// #if UNITY_EDITOR
	// // Update is called once per frame
	// void Update () {
	// 	if(!Application.isPlaying) {
	// 		OnSkinUI();
	// 	}
	// }
	// #endif
}
