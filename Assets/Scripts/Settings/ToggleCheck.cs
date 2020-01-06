using UnityEngine;
using UnityEngine.UI;

public class ToggleCheck : MonoBehaviour {

    public Image check;

    public void OnValueChanged(bool value) {
        check.enabled = value;
    }
}