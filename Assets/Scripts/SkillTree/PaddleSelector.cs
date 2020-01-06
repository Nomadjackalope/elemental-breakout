using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PaddleSelector : MonoBehaviour {

	public Text paddleName;
	public Button left, right;

	int currentPaddleId;

	public GameObject SkillTreePaddlePrefab;

	public GameObject content;

	PaddleSelection paddleSelection;

	public GameObject realPaddle;
	public GameObject buyPaddle;

	private int paddleCost = 3600;

	// Use this for initialization
	void Start () {
		currentPaddleId = MasterPlayerData.instance.getActivePaddleId();

		paddleSelection = content.transform.GetChild(0).GetComponent<PaddleSelection>();
		updatePaddleId();

		if(MasterPlayerData.instance.getPaddles().Count > 2) {
			paddleCost = 4800;
		}
	}

	public void RightClicked() {
		currentPaddleId++;
		if(currentPaddleId >= MasterPlayerData.instance.getPaddles().Count) {
			currentPaddleId = -1;
		}

		updatePaddleId();
		
	}

	public void LeftClicked() {
		currentPaddleId--;
		if(currentPaddleId < -1) {
			currentPaddleId = MasterPlayerData.instance.getPaddles().Count - 1;
		}

		updatePaddleId();
	}

	void updatePaddleId() {
		if(currentPaddleId == -1) {
			paddleName.text = "New Skill Set";
			showBuyPaddle();
		} else {
			showPaddle();
			paddleSelection.setPaddleId(currentPaddleId);
			MasterPlayerData.instance.setActivePaddle(currentPaddleId);
			paddleName.text = "Skill Set " + (currentPaddleId + 1);
		}
	}

	void showBuyPaddle() {
		realPaddle.SetActive(false);
		buyPaddle.SetActive(true);

		buyPaddle.GetComponentInChildren<Text>().text = paddleCost + " coins";
	}

	void showPaddle() {
		realPaddle.SetActive(true);
		buyPaddle.SetActive(false);
	}

	public void OnBuyPaddleClicked() {
		if(purchasePaddle()) {
			// instantiate coins particle system going with text
			LeftClicked();
		}

		//updateCoins();
	}

	public bool purchasePaddle() {
		if(MasterPlayerData.instance.subtractCoins(paddleCost)) {
			MasterPlayerData.instance.addPaddle();
			return true;
		}

		return false;
	}
}
