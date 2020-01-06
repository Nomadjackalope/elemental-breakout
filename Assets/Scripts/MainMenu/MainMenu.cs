using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

	public GameObject skillTreeButton, shopButton;

	public GameObject buttonShine;

	// Use this for initialization
	void Start () {
		//print("numLevels " + MasterPlayerData.instance.getNumLevelsBeat());
		if(MasterPlayerData.instance.getNumLevelsBeat() == 0) {
			skillTreeButton.SetActive(false);
			shopButton.SetActive(false);
		} else if(!MasterPlayerData.instance.GetPlayerData().seenMainMenuNewButtons) {
			Instantiate(buttonShine, skillTreeButton.transform);
			Instantiate(buttonShine, shopButton.transform);
			
			MasterPlayerData.instance.GetPlayerData().seenMainMenuNewButtons = true;
			MasterPlayerData.instance.savePlayerDataAsync();
		}
	}
	
}
