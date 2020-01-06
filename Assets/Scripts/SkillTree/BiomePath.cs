using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BiomePath : MonoBehaviour {

	public Image biome1Image;
	public Image biome1Frame;
	public Image b1skill1;
	public Image b1skill2;
	public Image b1skill3;

	public Image b1spacer1;
	public Image b1spacer2;
	public Image b1spacer3;

	public Image biome2Image;
	public Image biome2Frame;
	public Image b2skill1;
	public Image b2skill2;
	public Image b2skill3;

	public Image b2spacer1;
	public Image b2spacer2;
	public Image b2spacer3;

	public Image biome1Essence;
	public Image biome2Essence;

	DataTypes.BiomeType biome1;
	DataTypes.BiomeType biome2;

	public Sprite fire;
	public Sprite water;
	public Sprite earth;
	public Sprite poison;
	public Sprite shadow;
	public Sprite growth;
	public Sprite placeholder;

	public Sprite placeholderSkill;

	public GameObject biomeSelect;
	public GameObject ThreeSkills;
	public GameObject spacer;

	private GameObject prevBlinkingObject;

	public PaddleSelection paddleSelection;

	public GameObject biomeArrow;
	public GameObject skillArrow;
	public GameObject FireBiomeArrow;
	public GameObject fireBlastArrow;
	public ArrowState arrowState;
	private ArrowState prevArrowState = ArrowState.none;
	public enum ArrowState
	{
		biome,
		skill,
		fireBiome,
		fireblast,
		none
	}

	public bool isDirty;


	void Start() {
		addToggleListener(b1skill1); //, biome1, 1);
		addToggleListener(b1skill2); //, biome1, 2);
		addToggleListener(b1skill3); //, biome1, 3);
		addToggleListener(b2skill1); //, biome2, 1);
		addToggleListener(b2skill2); //, biome2, 2);
		addToggleListener(b2skill3); //, biome2, 3);

		biome1Image.GetComponent<Button>().onClick.AddListener(delegate {
			biomeButtonClicked();
			blink(biome1Frame.gameObject);
		});

		biome2Image.GetComponent<Button>().onClick.AddListener(delegate {
			biomeButtonClicked();
			blink(biome2Frame.gameObject);
		});
		
		hideAll();

		// biomeArrow.SetActive(!MasterPlayerData.instance.GetPlayerData().seenBiomePurchaseArrow);
		// skillArrow.SetActive(!MasterPlayerData.instance.GetPlayerData().seenSkillPurchaseArrow);
	}

	void addToggleListener(Image image) {
		image.gameObject.GetComponent<Button>().onClick.AddListener(delegate {
			onRowButtonClicked(image);
			blink(image.GetComponentInChildren<Animator>().gameObject);
		});
	}
	
	// Update is called once per frame
	void Update () {
		if(!isDirty && !paddleSelection.updatedId) return;

		resetImages();

		if(paddleSelection.updatedId) {
			hideAll();
			blink(null);
			paddleSelection.updatedId = false;
		}

		checkArrowState();

		if(paddleSelection.getBranchesPurchased().Count > 0) {
			biome1 = paddleSelection.getBranchesPurchased()[0];
			biome1Image.sprite = getBiomeSpriteFrom(biome1);
			biome1Image.GetComponent<Button>().interactable = true;//false;
			biome1Image.GetComponent<Button>().onClick.RemoveAllListeners();
			biome1Image.GetComponent<Button>().onClick.AddListener(delegate {
				SkillTree.instance.showBiomeInfoPanel();
			});

			b1skill1.color = Color.white;
			b1spacer1.color = Colors.WoodBrown;
			b1skill1.transform.GetChild(0).GetComponent<Image>().color = Color.white;
			b1skill1.GetComponent<Button>().interactable = true;

			biome1Essence.gameObject.SetActive(true);
			biome1Essence.sprite = SkillTree.instance.icons.getIconFrom(biome1);
			biome1Essence.GetComponentInChildren<Text>().text = MasterPlayerData.instance.getEssence()[biome1].ToString();

			// hideArrows();

			// if(MasterPlayerData.instance.getPaddles().Count <= 1) {
			// 	biomeArrow.GetComponent<Image>().enabled = false;
			// 	skillArrow.GetComponent<Image>().enabled = true;
			// }

		}
		
		if(paddleSelection.getBranchesPurchased().Count > 1) {
			biome2 = paddleSelection.getBranchesPurchased()[1];
			biome2Image.sprite = getBiomeSpriteFrom(biome2);
			biome2Image.GetComponent<Button>().interactable = true; //false;
			biome2Image.GetComponent<Button>().onClick.RemoveAllListeners();
			biome2Image.GetComponent<Button>().onClick.AddListener(delegate {
				SkillTree.instance.showBiomeInfoPanel();
			});

			b2skill1.color = Color.white;
			b2spacer1.color = Colors.WoodBrown;
			b2skill1.transform.GetChild(0).GetComponent<Image>().color = Color.white;
			b2skill1.GetComponent<Button>().interactable = true;

			biome2Essence.gameObject.SetActive(true);
			biome2Essence.sprite = SkillTree.instance.icons.getIconFrom(biome2);
			biome2Essence.GetComponentInChildren<Text>().text = MasterPlayerData.instance.getEssence()[biome2].ToString();
		}

		foreach (PowerUps powerup in paddleSelection.getSkillAvailable())
		{
			Skill skill = MasterPlayerData.instance.skillDataList.getSkillData(powerup);
			if(skill == null) {
				print("powerup: " + powerup);
			}
			int row = skill.Row;
			if(skill.biome == biome1) {
				if(row == 1) {
					b1skill2.color = Color.white;
					b1spacer2.color = Colors.WoodBrown;
					b1skill2.transform.GetChild(0).GetComponent<Image>().color = Color.white;
					//skillArrow.GetComponent<Image>().color = Color.clear;
					b1skill2.GetComponent<Button>().interactable = true;
				} else if(row == 2) {
					b1skill3.color = Color.white;
					b1spacer3.color = Colors.WoodBrown;
					b1skill3.transform.GetChild(0).GetComponent<Image>().color = Color.white;
					b1skill3.GetComponent<Button>().interactable = true;
				}
			} else if(skill.biome == biome2) {
				if(row == 1) {
					b2skill2.color = Color.white;
					b2spacer2.color = Colors.WoodBrown;
					b2skill2.transform.GetChild(0).GetComponent<Image>().color = Color.white;
					b2skill2.GetComponent<Button>().interactable = true;
				} else if(row == 2) {
					b2skill3.color = Color.white;
					b2spacer3.color = Colors.WoodBrown;
					b2skill3.transform.GetChild(0).GetComponent<Image>().color = Color.white;
					b2skill3.GetComponent<Button>().interactable = true;
				}
			}
		}

		foreach (PowerUps powerup in paddleSelection.getRuneIds())
		{
			Skill skill = MasterPlayerData.instance.skillDataList.getSkillData(powerup);
			int row = skill.Row;
			if(skill.biome == biome1) {
				if(row == 1) {
					b1skill1.sprite = skill.icon;
				} else if(row == 2) {
					b1skill2.sprite = skill.icon;
				} else if(row == 3) {
					b1skill3.sprite = skill.icon;
				}
			} else if(skill.biome == biome2) {
				if(row == 1) {
					b2skill1.sprite = skill.icon;
				} else if(row == 2) {
					b2skill2.sprite = skill.icon;
				} else if(row == 3) {
					b2skill3.sprite = skill.icon;
				}
			}
		}

		setDirty(false);
	}

	Sprite getBiomeSpriteFrom(DataTypes.BiomeType biome) {
		switch (biome)
		{
			case DataTypes.BiomeType.Fire:
				return fire;
			case DataTypes.BiomeType.Water:
				return water;
			case DataTypes.BiomeType.Earth:
				return earth;
			case DataTypes.BiomeType.Poison:
				return poison;
			case DataTypes.BiomeType.Shadow:
				return shadow;
			case DataTypes.BiomeType.Growth:
				return growth;
			default:
				return placeholder;
		}
	}

	void resetImages() {
		biome1Image.sprite = placeholder;
		biome2Image.sprite = placeholder;

		biome1Image.GetComponent<Button>().interactable = true;
		biome2Image.GetComponent<Button>().interactable = true;

		biome1Image.GetComponent<Button>().onClick.RemoveAllListeners();
		biome2Image.GetComponent<Button>().onClick.RemoveAllListeners();

		biome1Image.GetComponent<Button>().onClick.AddListener(delegate {
			biomeButtonClicked();
			blink(biome1Frame.gameObject);
		});

		biome2Image.GetComponent<Button>().onClick.AddListener(delegate {
			biomeButtonClicked();
			blink(biome2Frame.gameObject);
		});

		b1skill1.sprite = placeholderSkill;
		b1skill2.sprite = placeholderSkill;
		b1skill3.sprite = placeholderSkill;
		b2skill1.sprite = placeholderSkill;
		b2skill2.sprite = placeholderSkill;
		b2skill3.sprite = placeholderSkill;

		b1skill1.color = Color.clear;
		b1skill2.color = Color.clear;
		b1skill3.color = Color.clear;
		b2skill1.color = Color.clear;
		b2skill2.color = Color.clear;
		b2skill3.color = Color.clear;

		b1skill1.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
		b1skill2.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
		b1skill3.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
		b2skill1.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
		b2skill2.transform.GetChild(0).GetComponent<Image>().color = Color.clear;
		b2skill3.transform.GetChild(0).GetComponent<Image>().color = Color.clear;

		b1skill1.GetComponent<Button>().interactable = false;
		b1skill2.GetComponent<Button>().interactable = false;
		b1skill3.GetComponent<Button>().interactable = false;
		b2skill1.GetComponent<Button>().interactable = false;
		b2skill2.GetComponent<Button>().interactable = false;
		b2skill3.GetComponent<Button>().interactable = false;

		b1spacer1.color = Color.clear;
		b1spacer2.color = Color.clear;
		b1spacer3.color = Color.clear;

		b2spacer1.color = Color.clear;
		b2spacer2.color = Color.clear;
		b2spacer3.color = Color.clear;

		biome1Essence.gameObject.SetActive(false);
		biome2Essence.gameObject.SetActive(false);

		// biomeArrow.GetComponent<Image>().enabled = false;
		// skillArrow.GetComponent<Image>().enabled = true;
	}

	void biomeButtonClicked() {
		ThreeSkills.SetActive(false);
		spacer.SetActive(false);
		biomeSelect.SetActive(true);
		setDirty();
	}

	void showRows() {
		ThreeSkills.SetActive(true);
		biomeSelect.SetActive(false);
		spacer.SetActive(false);
		setDirty();
	}

	void hideAll() {
		spacer.SetActive(true);
		ThreeSkills.SetActive(false);
		biomeSelect.SetActive(false);
		setDirty();
	}

	void onRowButtonClicked(Image button) {
		showRows();

		if(button == b1skill1) {
			SkillBars.instance.changeSkillBiomeRow(biome1, 1);
		} else if(button == b1skill2) {
			SkillBars.instance.changeSkillBiomeRow(biome1, 2);
		} else if(button == b1skill3) {
			SkillBars.instance.changeSkillBiomeRow(biome1, 3);
		} else if(button == b2skill1) {
			SkillBars.instance.changeSkillBiomeRow(biome2, 1);
		} else if(button == b2skill2) {
			SkillBars.instance.changeSkillBiomeRow(biome2, 2);
		} else if(button == b2skill3) {
			SkillBars.instance.changeSkillBiomeRow(biome2, 3);
		}

		setDirty();
	}

	void blink(GameObject toBlink) {
		if(prevBlinkingObject != null) {
			prevBlinkingObject.GetComponent<Animator>().SetBool("blinkOn", false);
		}
		
		if(toBlink != null) {
			toBlink.GetComponent<Animator>().SetBool("blinkOn", true);
		}

		prevBlinkingObject = toBlink;
	}

	void checkArrowState() {
		
		if(MasterPlayerData.instance.getPaddles().Count <= 1) {
			if(paddleSelection.getBranchesPurchased().Count == 0) {
				if(!biomeSelect.activeSelf) {
					arrowState = ArrowState.biome;
				} else {
					arrowState = ArrowState.fireBiome;
				}
			} else if(paddleSelection.getSkillAvailable().Count == 0) {
				if(!ThreeSkills.activeSelf) {
					arrowState = ArrowState.skill;
				} else {
					arrowState = ArrowState.fireblast;
				}
			} else {
				arrowState = ArrowState.none;
			}
		} else {
			arrowState = ArrowState.none;
		}
		// print(arrowState);

		hideArrows();

		switch (arrowState)
		{
			case ArrowState.biome:
				biomeArrow.GetComponent<Image>().enabled = true;
				break;
			case ArrowState.fireBiome:
				FireBiomeArrow.GetComponent<Image>().enabled = true;
				break;
			case ArrowState.skill:
				skillArrow.GetComponent<Image>().enabled = true;
				break;
			case ArrowState.fireblast:
				fireBlastArrow.GetComponent<Image>().enabled = true;
				break;
			case ArrowState.none:
				break;
			default:
				break;
		}
		
		if(arrowState != prevArrowState) {
			setDirty();
		}

		prevArrowState = arrowState;
	}

	void hideArrows() {
		biomeArrow.GetComponent<Image>().enabled = false;
		skillArrow.GetComponent<Image>().enabled = false;
		FireBiomeArrow.GetComponent<Image>().enabled = false;
		fireBlastArrow.GetComponent<Image>().enabled = false;
	}

	public void setDirty(bool state = true) {
		isDirty = state;
	}
}
