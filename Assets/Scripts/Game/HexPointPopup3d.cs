using UnityEngine;
using DG.Tweening;

public class HexPointPopup3d : MonoBehaviour {

    Sequence popUp;
    
    public void ResetAndRunAnim(int value) {
        // Reset 
		//transform.localScale = Vector3.zero;
		GetComponent<TextMesh>().text = "+" + value;
        GetComponent<TextMesh>().color = Color.white;

        RunAnimation();
    }

    void OnDisable() {
        print("disabling");
    }

    void RunAnimation() {
        if(popUp != null) {
            popUp.Kill();
        }

        popUp = DOTween.Sequence();

		popUp
			//.Append(transform.DOScale(Vector3.one, 0.1f))
			//.AppendInterval(0.3f)
			//.Append(GetComponent<TextMesh>().color.)
            .AppendInterval(0.4f)
			.AppendCallback(() => { gameObject.SetActive(false); });
    }
}