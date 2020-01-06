using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomeSelect : MonoBehaviour {

	public GameObject fire;
	public GameObject water;
	public GameObject earth;
	public GameObject shadow;
	public GameObject poison;
	public GameObject growth;

	public Sprite basicFrameSprite;
	public Sprite decorFrameSprite;

	public PaddleSelection paddleSelection;

	// Use this for initialization
	void Start () {
		addButtonListener(fire, DataTypes.BiomeType.Fire);
		addButtonListener(water, DataTypes.BiomeType.Water);
		addButtonListener(earth, DataTypes.BiomeType.Earth);
		addButtonListener(shadow, DataTypes.BiomeType.Shadow);
		addButtonListener(poison, DataTypes.BiomeType.Poison);
		addButtonListener(growth, DataTypes.BiomeType.Growth);
	}

	void addButtonListener(GameObject go, DataTypes.BiomeType biome) {
		go.GetComponent<Button>().onClick.AddListener(delegate {
			SkillTree.instance.enableBranch(biome);
			transform.parent.GetComponentInChildren<BiomePath>().setDirty();
		});
	}

	void Update() {
		resetImages();

		if(paddleSelection.getBranchesPurchased().Count > 0) {
			applyDecorFrame(paddleSelection.getBranchesPurchased()[0]);

			if(paddleSelection.getBranchesPurchased().Count > 1) {
				applyDecorFrame(paddleSelection.getBranchesPurchased()[1]);

				fire.GetComponent<Button>().interactable = false;
				water.GetComponent<Button>().interactable = false;
				earth.GetComponent<Button>().interactable = false;
				shadow.GetComponent<Button>().interactable = false;
				poison.GetComponent<Button>().interactable = false;
				growth.GetComponent<Button>().interactable = false;
			}
		}
	}

	void resetImages() {
		switchFrameToBasic(fire.transform.Find("Frame"));
		switchFrameToBasic(water.transform.Find("Frame"));
		switchFrameToBasic(earth.transform.Find("Frame"));
		switchFrameToBasic(shadow.transform.Find("Frame"));
		switchFrameToBasic(poison.transform.Find("Frame"));
		switchFrameToBasic(growth.transform.Find("Frame"));
	}

	void applyDecorFrame(DataTypes.BiomeType biome) {
		switch (biome)
		{
			case DataTypes.BiomeType.Fire:
				switchFrameToDecor(fire.transform.Find("Frame"));
				break;
			case DataTypes.BiomeType.Water:
				switchFrameToDecor(water.transform.Find("Frame"));
				break;
			case DataTypes.BiomeType.Earth:
				switchFrameToDecor(earth.transform.Find("Frame"));
				break;
			case DataTypes.BiomeType.Shadow:
				switchFrameToDecor(shadow.transform.Find("Frame"));
				break;
			case DataTypes.BiomeType.Poison:
				switchFrameToDecor(poison.transform.Find("Frame"));
				break;
			case DataTypes.BiomeType.Growth:
				switchFrameToDecor(growth.transform.Find("Frame"));
				break;
			default:
				break;
		}
	}

	void switchFrameToDecor(Transform transform) {
		if(transform == null) return;

		transform.GetComponent<Image>().sprite = decorFrameSprite;
		transform.parent.GetComponent<Button>().interactable = false;
	}

	void switchFrameToBasic(Transform transform) {
		if(transform == null) return;

		transform.GetComponent<Image>().sprite = basicFrameSprite;
		transform.parent.GetComponent<Button>().interactable = true;	
	}
	
}
