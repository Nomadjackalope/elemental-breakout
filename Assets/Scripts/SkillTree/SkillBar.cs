using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Toggle has four states. Unpurchaseable, Purchasable, Available, Active
// Purchaseable is an extension of Available that buys the skill and enables it
// Unpurchaseable and Active are not interactable but different texts

public class SkillBar : FlexibleUI {

	enum SkillState
	{
		Unpurchaseable,
		Purchasable,
		Available,
		Active
	}

	public Text skillName;
	public Text skillDescription;
	public Image skillIcon;
	public Image isActive;
	// public Toggle toggle;
	public Button button;
	public Graphic Checkmark;

	public SkillBarToggle skillBarToggle;

	SkillState currentState;
	
	public Color defaultActiveColor;

	public PaddleSelection paddleSelection;
	

	// Use this for initialization
	void Start () {
		//button = GetComponentInChildren<Button>();
		
		if(!SkillTree.instance.checkPreviousTier(skillData)) {
			button.interactable = false;
		}

		button.onClick.AddListener(delegate {
			ButtonClicked();
		});
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Application.isPlaying) {
			UpdateCurrentState();
		}
	}

	void UpdateCurrentState() {
		if(skillData == null || skillData.powerUp == PowerUps.Placeholder) {
			return;
		}

		if(paddleSelection.getRuneIds().Contains(skillData.powerUp)) {
			currentState = SkillState.Active;
		} else if(paddleSelection.getSkillAvailable().Contains(skillData.powerUp)) {
			currentState = SkillState.Available;
		} else if(MasterPlayerData.instance.getEssence()[skillData.biome] >= SkillTree.instance.getSkillCost(skillData)) {
			currentState = SkillState.Purchasable;
		} else {
			currentState = SkillState.Unpurchaseable;
		}

		button.interactable = true;
		Checkmark.enabled = false;

		switch (currentState)
		{
			

			case SkillState.Active:
				//print("skill state is active");
				skillBarToggle.enableText(skillData.isAnActive ? "Active" : "Enabled");
				Checkmark.enabled = true;
				// turnToggle(true);
				break;
			case SkillState.Available:
				skillBarToggle.enableText("Select");
				break;
			case SkillState.Purchasable:
				skillBarToggle.enablePurchase(SkillTree.instance.getSkillCost(skillData), true, skillData.biome);
				break;
			case SkillState.Unpurchaseable:
				//print("skill state is unpurchaseable");			
				skillBarToggle.enablePurchase(SkillTree.instance.getSkillCost(skillData), false, skillData.biome);
				button.interactable = false;
				break;
			default:
				break;
		}
	}

	void ButtonClicked() {
		MasterEffectsSound.instance.playButtonClick();
		//print("toggle value changed: " + change.isOn);
		OnPowerUpClicked();
        // if(change.isOn && !paddleSelection.getRuneIds().Contains(skillData.powerUp)) {
        //     if(skillData.isAnActive && MasterPlayerData.instance.activePaddleHasActive()) {
        //         Debug.LogWarning("You have multiple actives enabled");
        //         //SkillTree.instance.activatePowerUp(skillData.powerUp);
        //         SkillTree.instance.showActiveCheckPanel(skillData.powerUp); //, enableActive);
        //     } else {
        //         SkillTree.instance.activatePowerUp(skillData.powerUp);
        //     }
        // } else if(!change.isOn && paddleSelection.getRuneIds().Contains(skillData.powerUp)) {
        //     SkillTree.instance.deactivatePowerUp(skillData.powerUp);
        // }
	}

	protected override void OnSkinUI() {
		base.OnSkinUI();

		skillName.text = skillData.skillName;
		skillDescription.text = skillData.description;
		skillIcon.sprite = skillData.icon;

		if (skillData.isAnActive) {
			isActive.color = defaultActiveColor;
		} else {
			isActive.color = Color.clear;
		}
	}

	public void OnPowerUpClicked() {
		if(currentState == SkillState.Purchasable) {
			SkillTree.instance.purchasePowerUp(skillData);
		} else if(currentState == SkillState.Available) {
			SkillTree.instance.activateSkill(skillData);
		} else if(currentState == SkillState.Active) {
			SkillTree.instance.deactivatePowerUp(skillData.powerUp);
		}

		transform.parent.parent.GetComponentInChildren<BiomePath>().setDirty();


		SkillBars.instance.changeActiveToggle(this);

		UpdateCurrentState();
	}

	public void DisableSkill() {
		SkillTree.instance.deactivatePowerUp(skillData.powerUp);

		UpdateCurrentState();
	}

	public void setSkillData(Skill skill) {
		// if(skillData != skill) {
		// 	turnToggle(false);
		// }

		skillData = skill;
		OnSkinUI();
	}

	// void turnToggle(bool on) {
	// 	//toggle.onValueChanged.RemoveAllListeners();
	// 	toggle.interactable = false;
	// 	toggle.isOn = on;
	// 	toggle.interactable = true;
	// 	// toggle.onValueChanged.AddListener(delegate {
	// 	// 	ToggleValueChanged(toggle);
	// 	// });

	// }
}
