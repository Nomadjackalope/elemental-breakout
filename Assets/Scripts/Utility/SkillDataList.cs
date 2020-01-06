using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill Data List")]
public class SkillDataList : ScriptableObject {

	public List<Skill> skills;

	public Skill getSkillData(PowerUps powerUp) {
		return skills.Find(x => x.powerUp == powerUp);
	}

	public List<Skill> getSkillList(DataTypes.BiomeType biome) {
		return skills.FindAll(x => x.biome == biome);
	}

	public List<Skill> getSkillRow(DataTypes.BiomeType biome, int row) {
		return getSkillList(biome).FindAll(x => x.Row == row);
	}

}
