using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour {

    void Awake() {
        GetComponent<Button>().onClick.AddListener(delegate() { playButtonClick(); });
    }

    public void playButtonClick() {
        //print("clicking");
        MasterEffectsSound.instance.playButtonClick();
    }

}