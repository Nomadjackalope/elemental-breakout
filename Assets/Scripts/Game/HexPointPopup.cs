using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HexPointPopup : MonoBehaviour {

    Sequence popUp;
    
    public void ResetAndRunAnim(int value) {
        // Reset 
		//transform.localScale = Vector3.zero;
		GetComponent<Text>().text = "+" + value;
        GetComponent<Text>().color = Color.white;

        RunAnimation();
    }

    // void OnDisable() {
        // print("disabling");
    // }

    void RunAnimation() {
        if(popUp != null) {
            popUp.Kill();
        }

        popUp = DOTween.Sequence();

		popUp
			// .Append(transform.DOScale(Vector3.one, 0.1f))
			// .AppendInterval(0.3f)
            .AppendInterval(1f)
			.Append(GetComponent<Text>().DOFade(0, 0.1f))
			.AppendCallback(() => { gameObject.SetActive(false); });
    }
}