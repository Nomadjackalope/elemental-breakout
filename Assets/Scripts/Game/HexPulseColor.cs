using System.Collections;
using UnityEngine;
using DG.Tweening;

public class HexPulseColor : MonoBehaviour {

    public float fadeTo = 0.75f;
    public float duration = 5;

    void Start() {
        duration *= Random.Range(0.5f, 1);
        // Sequence pulse = DOTween.Sequence();
        // pulse
        //     .Append(GetComponent<SpriteRenderer>().DOFade(fadeTo, duration / 2))
        //     .Append(GetComponent<SpriteRenderer>().DOFade(1, duration / 2)).SetLoops(-1);
        StartCoroutine(startAfterTime(Random.value * duration));
    }

    IEnumerator startAfterTime(float seconds) {
        yield return new WaitForSeconds(seconds);
        GetComponent<SpriteRenderer>().DOFade(fadeTo, duration / 2).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }


}