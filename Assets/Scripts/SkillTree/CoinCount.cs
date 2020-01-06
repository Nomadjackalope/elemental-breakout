using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCount : MonoBehaviour {
	
	void Start() {
		updateCoinCountText();
	}

	// Update is called once per frame
	void Update () {
		updateCoinCountText();
	}

	void updateCoinCountText() {
		GetComponent<Text>().text = "Current coins: " + MasterPlayerData.instance.getCoins();
	}
}
