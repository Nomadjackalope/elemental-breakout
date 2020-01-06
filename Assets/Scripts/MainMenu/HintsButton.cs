using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintsButton : MonoBehaviour {

    public GameObject hintButton;

    void Start() {
        hintButton.SetActive(false);
        if(MasterPlayerData.instance.GetPlayerData().seenHints.Count > 0) {
            hintButton.SetActive(true);
        }
    }


}