using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillExplanation : MonoBehaviour
{

    public Image skillImage;
    public Text skillName;
    public Text skillDescription;
    public Text skillCooldown;

    List<Touch> curTouches;

    public void setData(Skill skillData) {
        skillImage.sprite = skillData.icon;
        skillName.text = skillData.skillName;
        skillDescription.text = skillData.description;
        skillCooldown.text = "";

        // if(skillData.whileCoolDown == CoolDownType.TimeBased) {
        //     skillCooldown.text = "Cooldown time: " + skillData. seconds", 5);
        // }

    }

    void Update() {
        curTouches = InputHelper.GetTouches();

		if(curTouches.Count > 0) {

			// Watch for ball launches on touch down
			if(curTouches[0].phase == TouchPhase.Began) {
				gameObject.SetActive(false);
			}
		}
    }
}
