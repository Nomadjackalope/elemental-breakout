using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBarToggle : MonoBehaviour {

	public Image cost;
	public Text costText;
	public Text stateText;

	Color disabledColor = new Color(0.636f, 0.636f, 0.636f);
	Color enabledColor = Color.white;

	public void enablePurchase(int costAmount, bool hasMoney, DataTypes.BiomeType biome) {
		stateText.gameObject.SetActive(false);
		cost.gameObject.SetActive(true);

		costText.text = costAmount.ToString();
		cost.sprite = SkillTree.instance.icons.getIconFrom(biome);
		if(!hasMoney) {
			cost.color = disabledColor;
			costText.color = disabledColor;
		} else {
			cost.color = enabledColor;
			costText.color = enabledColor;
		}
	}

	public void enableText(string message) {
		stateText.gameObject.SetActive(true);
		cost.gameObject.SetActive(false);

		stateText.text = message;
	}
}
