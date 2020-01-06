using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class MasterPlayerData : MonoBehaviour {

	public static MasterPlayerData instance;

	private PlayerData playerData = new PlayerData();

	public SkillDataList skillDataList;

	private string friendlyDev = "// Thank you for playing Elemental Breakout. \n// We have left this easily editable for you. Hopefully you don\'t break too much ;) \n// If you like this app please tell your friends about it. \n\n";

	void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		// setupJsonSettings();

		// Check if any save files exist
		checkForSaveAndCreate();
	}

	void setupJsonSettings() {
		// JsonSerializerSettings settings = new JsonSerializerSettings();
		//settings.MissingMemberHandling = MissingMemberHandling.Error;

		// var
	}

	// Do this in background or something?
	void checkForSaveAndCreate() {
		print("made it here");
		if(!File.Exists(Application.persistentDataPath + "/playerData.json")) {
			savePlayerDataAsync();
		} else {
			loadPlayerDataAsync();
		}
	}

	public void loadPlayerDataAsync() {
		print(Application.persistentDataPath);

		string jsonData = "";
		
		jsonData = File.ReadAllText(Application.persistentDataPath + "/playerData.json");

		print(jsonData);


		if(jsonData != "") {
			playerData = JsonConvert.DeserializeObject<PlayerData>(jsonData);

			print("coins: " + playerData.coins);
		} else {
			print("json does not exist");
		}

	}

	public void savePlayerDataAsync() {
		// iterate over multiple saves

		string jsonData = friendlyDev + JsonConvert.SerializeObject(playerData, playerData.prettified ? Formatting.Indented : Formatting.None);

		File.WriteAllText(Application.persistentDataPath + "/playerData.json", jsonData);

		// string dataPath = Application.persistentDataPath + "/playerData.json";

		// // Background
		// System.Threading.Thread thread = new System.Threading.Thread(delegate () {
		// 	string jsonData = friendlyDev + JsonConvert.SerializeObject(playerData, playerData.prettified ? Formatting.Indented : Formatting.None);

		// 	try
		// 	{
		// 		File.WriteAllText(dataPath, jsonData);
		// 	}
		// 	catch (System.IO.IOException)
		// 	{
		// 		// Debug.LogError("File failed to save");
				
		// 	}

		// });

		// thread.Start();
	}

	// Accessing player data

	public void addCoins(int coins) {
		playerData.coins += coins;

		savePlayerDataAsync();

	}

	public bool subtractCoins(int amount) {
		if(playerData.coins < amount) {
			return false;
		}

		playerData.coins -= amount;

		savePlayerDataAsync();

		return true;
	}

	public int getCoins() {
		return playerData.coins;
	}

	public void addEssence(DataTypes.BiomeType type, int amount) {
		playerData.essences[type] += amount;

		savePlayerDataAsync();

	}

	// Fails if essence will be < 0
	public bool subtractEssence(DataTypes.BiomeType type, int amount) {

		if(playerData.essences[type] < amount) {
			return false;
		}

		playerData.essences[type] -= amount;

		savePlayerDataAsync();

		return true;
	}

	public bool checkEssence(DataTypes.BiomeType type, int amount) {
		return playerData.essences[type] >= amount;
	}

	public Dictionary<DataTypes.BiomeType, int> getEssence() {
		return playerData.essences;
	}

	public void unlockLevel(DataTypes.BiomeType type, int levelId) {
		if(levelId > 10 || (playerData.levels[type].score.ContainsKey(levelId) && playerData.levels[type].score[levelId] > 0)) {
			return;
		}

		playerData.levels[type].score[levelId] = 0;

		savePlayerDataAsync();

	}

	public void unlockNextLevel(DataTypes.BiomeType type) {
		List<int> levelList = LevelLoader.instance.getLevelList(type);

		int curLevel = 0;
		
		foreach(KeyValuePair<int, int> entry in playerData.levels[type].score) {
			if(curLevel < entry.Key) {
				curLevel = entry.Key;
			}
		}

		for (int i = 0; i < levelList.Count; i++)
		{
			if(levelList[i] > curLevel) {
				playerData.levels[type].score[levelList[i]] = 0;
				break;
			}
		}

		savePlayerDataAsync();
	}

	public bool checkIfLevelUnlocked(DataTypes.BiomeType type, int levelId) {
		// make sure level has >= 0 for score
		return playerData.levels[type].score.ContainsKey(levelId) && playerData.levels[type].score[levelId] >= 0;
	}

	public int getLevelsUnlockedIn(DataTypes.BiomeType biome) {
		int levelCount = 0;
		foreach (KeyValuePair<int, int> entry in playerData.levels[biome].score)
		{
			if(entry.Key != -1 && entry.Value > 0) {
				levelCount++;
			}
		}

		return levelCount;

		// if(playerData.levels[biome].score[playerData.levels[biome].score.Count - 1] == 0) {
		// 	return playerData.levels[biome].score.Count - 1;
		// } else {
		// 	return playerData.levels[biome].score.Count;
		// }
	}

	// returns true if high score is greater
	public bool changeHighScore(DataTypes.BiomeType type, int levelId, int score) {

		if(levelId == -1) {
			if(!playerData.levels[type].time.ContainsKey(levelId)) {
				playerData.levels[type].time[levelId] = 0;
			}
			if(!playerData.levels[type].score.ContainsKey(levelId)) {
				playerData.levels[type].score[levelId] = 0;
			}
			playerData.levels[type].time[levelId]++;
			playerData.levels[type].score[levelId] = Mathf.FloorToInt((playerData.levels[type].score[levelId] * playerData.levels[type].time[levelId] + score) / playerData.levels[type].time[levelId]);

			return true;
		}


		// if current score > new score this fails
		if(playerData.levels[type].score.ContainsKey(levelId)) {
			if(playerData.levels[type].score[levelId] >= score) {
				return false;
			}
		}

		changeBestSkills(type, levelId);

		playerData.levels[type].score[levelId] = score;

		savePlayerDataAsync();

		return true;
	}

	public bool changeBestTime(DataTypes.BiomeType type, int levelId, float time) {
		if(levelId == -1) return false;

		// if current time < new time this fails
		if(playerData.levels[type].time.ContainsKey(levelId)) {
			if(playerData.levels[type].time[levelId] <= time) {
				return false;
			}
		}

		playerData.levels[type].time[levelId] = time;

		savePlayerDataAsync();

		return true;
	}

	public bool changeBestSkills(DataTypes.BiomeType type, int levelId) {
		List<PowerUps> bestSkills = new List<PowerUps>(getActivePaddle().runeIds);
		playerData.levels[type].bestSkills[levelId] = bestSkills;
		return true;
	}

	public PaddleData getActivePaddle() {
		return playerData.paddles[playerData.activePaddle];
	}

	public int getActivePaddleId() {
		return playerData.activePaddle;
	}

	public void setActivePaddle(int id) {
		playerData.activePaddle = id;

		savePlayerDataAsync();
	}

	public void setActivePaddle(PaddleData paddleData) {
		foreach (int id in playerData.paddles.Keys)
		{	
			if(playerData.paddles[id] == paddleData) {
				playerData.activePaddle = id;
				return;
			}
		}

		savePlayerDataAsync();

	}

	public PaddleData addPaddle() {
		PaddleData newPaddle = new PaddleData();
		playerData.paddles[getNextPaddleId()] = newPaddle;
		setActivePaddle(newPaddle);

		savePlayerDataAsync();


		return newPaddle;
	}

	private int getNextPaddleId() {
		int highId = 0;
		foreach (int id in playerData.paddles.Keys)
		{
			if(id > highId) {
				highId = id;
			}
		}
		return highId + 1;
	}

	public bool addPowerUpToActivePaddle(PowerUps powerUp) {
		if(playerData.paddles[playerData.activePaddle].runeIds.Contains(powerUp)) {
			return false;
		}
		
		playerData.paddles[playerData.activePaddle].runeIds.Add(powerUp);

		savePlayerDataAsync();

		return true;
	}

	public bool switchActiveOnActivePaddle(PowerUps newPowerUp) {
		bool success = false;

		foreach (PowerUps powerup in playerData.paddles[playerData.activePaddle].runeIds)
		{
			if(skillDataList.getSkillData(powerup).isAnActive) {
				playerData.paddles[playerData.activePaddle].runeIds.Remove(powerup);

				print("removing current active: " + powerup);
				success = true;
				break;
			}
		}

		if(success) {
			if(!playerData.paddles[playerData.activePaddle].runeIds.Contains(newPowerUp)) {
				playerData.paddles[playerData.activePaddle].runeIds.Add(newPowerUp);
				print("adding new active: " + newPowerUp);
			}
		}
		
		savePlayerDataAsync();

		return success;
	}

	public PowerUps getActiveOnPaddle() {
		foreach (PowerUps powerup in playerData.paddles[playerData.activePaddle].runeIds)
		{
			if(skillDataList.getSkillData(powerup).isAnActive) {
				return powerup;;
			}
		}

		return PowerUps.Placeholder;
	}


	public bool removePowerUpToActivePaddle(PowerUps powerUp) {
		bool success = playerData.paddles[playerData.activePaddle].runeIds.Remove(powerUp);

		savePlayerDataAsync();

		return success;
	}

	public bool purchaseSkillToActivePaddle(PowerUps powerUp, bool alsoActivate) {
		if(playerData.paddles[playerData.activePaddle].skillAvailable.Contains(powerUp)) {
			return false;
		}

		playerData.paddles[playerData.activePaddle].skillAvailable.Add(powerUp);

		// also activate it
		if(alsoActivate) {
			if(skillDataList.getSkillData(powerUp).isAnActive && getActiveOnPaddle() != PowerUps.Placeholder) {
				switchActiveOnActivePaddle(powerUp);
			} else {
				playerData.paddles[playerData.activePaddle].runeIds.Add(powerUp);
			}
		}

		savePlayerDataAsync();

		return true;
	}

	public bool purchaseSkillToActivePaddle(PowerUps powerUp) {
		return purchaseSkillToActivePaddle(powerUp, true);
	}

	public bool checkPowerupUnlocked(PowerUps powerUp) {
		return getActivePaddle().skillAvailable.Contains(powerUp);
	}

	public void updatePaddle(int paddleId, PaddleData paddleData) {
		playerData.paddles[paddleId] = paddleData;

		savePlayerDataAsync();

	}

	public Dictionary<int, PaddleData> getPaddles() {
		return playerData.paddles;
	}

	public bool activePaddleHasActive() {
		foreach (PowerUps powerup in getActivePaddle().runeIds)
		{
			if(skillDataList.getSkillData(powerup).isAnActive) {
				return true;
			}
		}

		return false;
	}

	public int branchesAvailableOnActivePaddle() {
		return 2 - getActivePaddle().branchesPurchased.Count;
	}

	public void enableBranchOnActivePaddle(DataTypes.BiomeType biome) {
		if(!branchEnabled(biome)) {
			getActivePaddle().branchesPurchased.Add(biome);
		}

		savePlayerDataAsync();
	}

	public bool branchEnabled(DataTypes.BiomeType biome) {
		return getActivePaddle().branchesPurchased.Contains(biome);
	}

	public PlayerData GetPlayerData() {
		return playerData;
	}

	public void setLastLevelPlayed(DataTypes.BiomeType biome, int levelId) {
		playerData.lastBiomePlayed = biome;
		playerData.lastLevelIdPlayed = levelId;
		
		// savePlayerDataAsync();
	}

	public DataTypes.BiomeType getLastBiomePlayed() {
		return playerData.lastBiomePlayed;
	}

	public int getLastLevelIdPlayed() {
		return playerData.lastLevelIdPlayed;
	}

	public string getBestTime(DataTypes.BiomeType type, int levelId) {
		// if current score > new score this fails
		if(!playerData.levels[type].time.ContainsKey(levelId)) {
			return "";
		}

		float time = playerData.levels[type].time[levelId];

		
		string ss = (time % 60).ToString("00.00");
		string mm = (Mathf.Floor(time / 60f) % 60).ToString();
		string final = mm + ":" +  ss;
		

		return final;
	}

	public float getBestTimeSeconds(DataTypes.BiomeType type, int levelId) {
		if(!playerData.levels[type].time.ContainsKey(levelId)) {
			return -1;
		}

		return playerData.levels[type].time[levelId];
	}

	public string getHighScore(DataTypes.BiomeType type, int levelId) {
		if(!playerData.levels[type].score.ContainsKey(levelId)) {
			return "";
		}
		
		int score = playerData.levels[type].score[levelId];

		if(score == 0) {
			return "";
		}

		return score.ToString();

	}

	public List<PowerUps> getBestSkills(DataTypes.BiomeType type, int levelId) {
		if(!playerData.levels[type].bestSkills.ContainsKey(levelId)) {
			return new List<PowerUps>();
		}

		return playerData.levels[type].bestSkills[levelId];
	}

	public bool seenBiomeHint(DataTypes.BiomeType biome) {
		return checkSeenHint(Hints.getHintIdFromBiome(biome));
	}

	public void seenBiomeHint(DataTypes.BiomeType biome, bool setValue) {
		if(!setValue) {
			return;
		}

		seenHint(Hints.getHintIdFromBiome(biome));

	}

	public bool checkSeenHint(HintIds hint) {
		return playerData.seenHints.Contains(hint);
	}

	public void seenHint(HintIds hint) {
		if(!checkSeenHint(hint)) {
			playerData.seenHints.Add(hint);
		}
		savePlayerDataAsync();
	}

	public bool checkSeenConvo(ConvoIds convo) {
		return playerData.seenConvo.Contains(convo);
	}

	public void seenConvo(ConvoIds convo) {
		if(!checkSeenConvo(convo)) {
			playerData.seenConvo.Add(convo);
		}
		savePlayerDataAsync();
	}

	// Might not work the way you are expecting
	public int getNumLevelsLeft() {
		int levelsLeft = 60;

		levelsLeft -= playerData.levels[DataTypes.BiomeType.Fire].score.Count;
		levelsLeft -= playerData.levels[DataTypes.BiomeType.Water].score.Count;
		levelsLeft -= playerData.levels[DataTypes.BiomeType.Earth].score.Count;
		levelsLeft -= playerData.levels[DataTypes.BiomeType.Growth].score.Count;
		levelsLeft -= playerData.levels[DataTypes.BiomeType.Poison].score.Count;
		levelsLeft -= playerData.levels[DataTypes.BiomeType.Shadow].score.Count;

		return levelsLeft;
	}

	// Returns the number of levels with a score > 0
	public int getNumLevelsBeat() {
		int levelsBeat = 0;

		levelsBeat += getNumLevelsBeat(DataTypes.BiomeType.Fire);
		levelsBeat += getNumLevelsBeat(DataTypes.BiomeType.Water);
		levelsBeat += getNumLevelsBeat(DataTypes.BiomeType.Earth);
		levelsBeat += getNumLevelsBeat(DataTypes.BiomeType.Growth);
		levelsBeat += getNumLevelsBeat(DataTypes.BiomeType.Poison);
		levelsBeat += getNumLevelsBeat(DataTypes.BiomeType.Shadow);

		return levelsBeat;
	}

	private int getNumLevelsBeat(DataTypes.BiomeType biome) {
		int levelsBeat = 0;

		foreach (KeyValuePair<int, int> entry in playerData.levels[biome].score)
		{
			if(entry.Value > 0) {
				levelsBeat++;
			}
		}

		return levelsBeat;
	}
}