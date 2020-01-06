using UnityEngine;
using UnityEngine.UI;

public class PaddleToggleButton : MonoBehaviour {
    public Transform powerUpsLeft;
    public Transform powerUpsRight;

    public DataTypes.BiomeType left = DataTypes.BiomeType.Default;
    public DataTypes.BiomeType right = DataTypes.BiomeType.Default;

    public Image biomeLeft;
    public Image biomeRight;

    public Image right1;
    public Image right2;
    public Image right3;

    public Image left1;
    public Image left2;
    public Image left3;


    public void loadData(PaddleData data) {
        if(data.branchesPurchased.Count > 0) {
            biomeLeft.color = DataTypes.GetButtonColorFrom(data.branchesPurchased[0]);
            left = data.branchesPurchased[0];

        } else {
            biomeLeft.color = Color.white;
        }

        if(data.branchesPurchased.Count > 1) {
            biomeRight.color = DataTypes.GetButtonColorFrom(data.branchesPurchased[1]);
            right = data.branchesPurchased[1];
        } else {
            biomeRight.color = Color.white;
        }

        int leftIndex = 0;
        int rightIndex = 0;

        foreach (PowerUps powerUp in data.runeIds)
        {
            print("powerup: " + powerUp);
            Skill skill = MasterPlayerData.instance.skillDataList.getSkillData(powerUp);
            if(skill.biome == left) {
                if(leftIndex == 0) {
                    left1.sprite = skill.icon;
                    left1.enabled = true;
                } else if(leftIndex == 1) {
                    left2.sprite = skill.icon;
                    left2.enabled = true;
                } else if(leftIndex == 2) {
                    left3.sprite = skill.icon;
                    left3.enabled = true;
                }

                leftIndex++;
                
            } else {
                if(rightIndex == 0) {
                    right1.sprite = skill.icon;
                    right1.enabled = true;
                } else if(rightIndex == 1) {
                    right2.sprite = skill.icon;
                    right2.enabled = true;
                } else if(rightIndex == 2) {
                    right3.sprite = skill.icon;
                    right3.enabled = true;
                }

                rightIndex++;

            }
        }        

    }
}