using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonAlt : MonoBehaviour {

    public Text LevelNum, BestScore, BestTime;
    public Image skill1, skill2, skill3, skill4, skill5, skill6;
    public Button play;
    
    
    private Sprite powerUpSprite;

	public void setLevelData(DataTypes.BiomeType biome, int levelId) {
        // If level is a random level then
        if(levelId == -1) {
            LevelNum.text = "?";

            if(MasterPlayerData.instance.getHighScore(biome, levelId) != "") {
                BestScore.text = "Average Score: " + MasterPlayerData.instance.getHighScore(biome, levelId);
                BestTime.text = "Levels played: " + MasterPlayerData.instance.getBestTimeSeconds(biome, levelId);
            } else {
                BestScore.text = "";
                BestTime.text = "";
            }
            
        } else {

            LevelNum.text = levelId.ToString();
            if(MasterPlayerData.instance.getHighScore(biome, levelId) != "") {
                BestScore.text = "Best Score: " + MasterPlayerData.instance.getHighScore(biome, levelId);
                BestTime.text = "Best Time: " + MasterPlayerData.instance.getBestTime(biome, levelId);
            } else {
                BestScore.text = "";
                BestTime.text = "";
            }
        }

        fillSkills(biome, levelId);

        setNumTextColor(biome);
    }

    void fillSkills(DataTypes.BiomeType biome, int levelId) {
        // clear skill icons
        skill1.color = Color.clear;
        skill2.color = Color.clear;
        skill3.color = Color.clear;
        skill4.color = Color.clear;
        skill5.color = Color.clear;
        skill6.color = Color.clear;

        List<PowerUps> levelBestSkills = MasterPlayerData.instance.getBestSkills(biome, levelId);

        int i = 0;
        foreach (PowerUps powerUp in levelBestSkills)
        {
            powerUpSprite = MasterPlayerData.instance.skillDataList.getSkillData(powerUp).icon;

            switch (i)
            {
                case 0:
                    skill1.sprite = powerUpSprite;
                    skill1.color = Color.white;
                    break;
                case 1:
                    skill2.sprite = powerUpSprite;
                    skill2.color = Color.white;
                    break;
                case 2:
                    skill3.sprite = powerUpSprite;
                    skill3.color = Color.white;
                    break;
                case 3:
                    skill4.sprite = powerUpSprite;
                    skill4.color = Color.white;
                    break;
                case 4:
                    skill5.sprite = powerUpSprite;
                    skill5.color = Color.white;
                    break;
                case 5:
                    skill6.sprite = powerUpSprite;
                    skill6.color = Color.white;
                    break;
                default:
                    break;
            }

            i++;
        }
    }

    void setNumTextColor(DataTypes.BiomeType biome) {
        LevelNum.GetComponent<Shadow>().effectColor = DataTypes.GetButtonColorFrom(biome);
    }

    public void setButtonDelegate(UnityEngine.Events.UnityAction buttonEvent) {
        play.onClick.AddListener(buttonEvent);
    }
}