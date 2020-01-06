using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeInfoPopUp : MonoBehaviour
{
    List<Touch> curTouches;

    void Update() {
        curTouches = InputHelper.GetTouches();

		if(curTouches.Count > 0) {

			// Watch for ball launches on touch down
			if(curTouches[0].phase == TouchPhase.Began) {
				gameObject.SetActive(false);
			}
		}
    }
}