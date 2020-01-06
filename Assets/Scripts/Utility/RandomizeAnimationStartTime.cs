
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RandomizeAnimationStartTime : MonoBehaviour {

    void Start() {
        GetComponent<Animator>().enabled = false;

        StartCoroutine(StartAnimatorIn(Random.value * 5));
    }

    IEnumerator StartAnimatorIn(float seconds) {
        yield return new WaitForSeconds(seconds);
        GetComponent<Animator>().enabled = true;
    }
}