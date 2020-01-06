using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PlayerData {

	// Game version
	public string version { get; set; }

	public bool prettified = false;


	// Levels, unlocked or high score
	public Dictionary<DataTypes.BiomeType, LevelScoreData> levels = new Dictionary<DataTypes.BiomeType, LevelScoreData>{
		{ DataTypes.BiomeType.Fire, new LevelScoreData() },
		{ DataTypes.BiomeType.Water, new LevelScoreData() },
		{ DataTypes.BiomeType.Earth, new LevelScoreData() },
		{ DataTypes.BiomeType.Growth, new LevelScoreData() },
		{ DataTypes.BiomeType.Poison, new LevelScoreData() },
		{ DataTypes.BiomeType.Shadow, new LevelScoreData() }
	};



	// num coins
	public int coins = 0;

	// essence per type // cannot use dictionary with Unity.JSON
	public Dictionary<DataTypes.BiomeType, int> essences = new Dictionary<DataTypes.BiomeType, int>{
		{ DataTypes.BiomeType.Fire, 0 },
		{ DataTypes.BiomeType.Water, 0 },
		{ DataTypes.BiomeType.Earth, 0 },
		{ DataTypes.BiomeType.Growth, 0 },
		{ DataTypes.BiomeType.Poison, 0 },
		{ DataTypes.BiomeType.Shadow, 0 }
	};

	// paddles, their skill trees, and number of runes
	public Dictionary<int, PaddleData> paddles = new Dictionary<int, PaddleData>{
		{0, new PaddleData() }, 
		};

	public int activePaddle = 0;

	// progress through story? This is probably linked to number of levels completed and doesn't need its own value

	// evil - good meter // negative is bad, positive is good
	// public float moralityMeter;

	public DataTypes.BiomeType lastBiomePlayed;
	public int lastLevelIdPlayed;

	// public bool seenBasicExplanation;
	// public bool seenSkillTree;
	// public bool seenBoss;
	// public bool seenActiveExplanation;
	// public bool seenAutoEnd;
	// public bool seenCoins;
	
	// public bool seenFire;
	// public bool seenWater;
	// public bool seenEarth;
	// public bool seenShadow;
	// public bool seenPoison;
	// public bool seenGrowth;

	public bool seenBiomePurchaseArrow;
	public bool seenSkillPurchaseArrow;

	public bool seenMainMenuNewButtons;

	public List<HintIds> seenHints = new List<HintIds>();
	public List<ConvoIds> seenConvo = new List<ConvoIds>();

	// public bool seenBossConvo;
	// public bool seenFinalConvo;

}

[System.Serializable]
public class PaddleData {
	// Number of runes
	public int totalNumRunes = 0;

	// Purchased skills
	public List<PowerUps> skillAvailable = new List<PowerUps>();

	// Skills runes currently in
	public List<PowerUps> runeIds = new List<PowerUps>();

	// Branches available?
	public List<DataTypes.BiomeType> branchesPurchased = new List<DataTypes.BiomeType>();
}

[System.Serializable]
public class LevelScoreData {
	public Dictionary<int, int> score = new Dictionary<int, int>{
            { 1, 0 },
        };
	public Dictionary<int, float> time = new Dictionary<int, float>{
			
		};
	public Dictionary<int, List<PowerUps>> bestSkills = new Dictionary<int, List<PowerUps>> {

		};
}

// [System.Serializable]
// public class RandomScoreData 
// {
// 	public int score;
// 	public int numPlays;
// }
