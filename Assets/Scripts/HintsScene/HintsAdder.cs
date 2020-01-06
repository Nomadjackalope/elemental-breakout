using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintsAdder : MonoBehaviour {

    public GameObject hintPrefab;
    
    void Start() {

        populateHints();
    }

    private void populateHints() {

        foreach (HintIds hintId in MasterPlayerData.instance.GetPlayerData().seenHints)
        {
            Instantiate(hintPrefab, transform).GetComponentInChildren<Text>().text = Hints.getHintFrom(hintId);
        }
    }
}