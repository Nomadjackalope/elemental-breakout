using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBars : MonoBehaviour {

	public static SkillBars instance;

	public SkillBar skill1;
	public SkillBar skill2;
	public SkillBar skill3;

	void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}
	}

	public void changeSkillBiomeRow(DataTypes.BiomeType biome, int row) {
		List<Skill> skills = new List<Skill>(3);

		skills = MasterPlayerData.instance.skillDataList.getSkillRow(biome, row);

		// foreach (Skill skill in skills)
		// {
		// 	print(skill);
		// }

		// print("skills: " + skills.Count);

		if(skills.Count < 3) return;

		skill1.setSkillData(skills[0]);
		skill2.setSkillData(skills[1]);
		skill3.setSkillData(skills[2]);
		
		 
	}

	public void changeActiveToggle(SkillBar skillBar) {
		if(skill1 != skillBar) {
			skill1.DisableSkill();
		}

		if(skill2 != skillBar) {
			skill2.DisableSkill();
		}

		if(skill3 != skillBar) {
			skill3.DisableSkill();
		}
	}
}
