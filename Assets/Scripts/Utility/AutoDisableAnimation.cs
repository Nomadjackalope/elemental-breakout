using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AutoDisableAnimation : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(DisableAfter(this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length));
		
	}

    IEnumerator DisableAfter(float seconds) {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}