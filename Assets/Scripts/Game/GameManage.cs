using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class GameManage : MonoBehaviour {

	public static GameManage instance;

	public LevelData level;

	public GameObject hexPrefab;
	public GameObject paddlePrefab;
	public GameObject ballPrefab;
	public Transform ballSpawnLocation;
	// public Text countDown;
	List<GameObject> ballInstances = new List<GameObject>();
	public GameObject gridInstance;
	Grid grid;
	HexGrid hexGrid;
	public GameObject shadowTilemapPrefab;
	Tilemap shadowTilemap;
	bool shadowOn = false;
	public Tile blackTile;
	Coroutine shadowOffCountdown;
	
	// some of these we can make private once they are finessed
	float ballInitSpeed = 9f;
	public int timeBeforeBallLaunch = 3;
	float ballSpacing = 0.75f;

	public int numBalls = 0;
	int shadowBalls = 0;
	int numHexesDestroyed = 0;
	int destructibleHexes;
	int score = 0;
	private int lastScore = 0;
	float currentCombo = 1.0f;
	float additionalComboForXHits = 0.1f;
	public int lives = 3;
	float maxCombo = 4.0f;
	float baseCombo = 1.0f;

	public Text ScoreText;
	public Text EssenceText;
	public Text LivesText;
	public Text ComboText;
	public Text TimeText;

	public Text loseLevelLabel;
	public Text winLevelLabel;

    public GameObject hexParticle;

	public GameObject baseCanvas;

	public GameObject pointPopUpsObject;
	public GameObject pointPopUpPrefab;
	public GameObject pointPopUp3dPrefab;

	// Score variables
	int livesLost = 0;
	float timeUsed;
	string ss, mm;

	float countDownTimeLeft;

	public GameObject pauseMenu;
	public GameObject LosePanel;
	public GameObject WinPanel;
	public GameObject pauseButton;
	public GameObject CountDownHolder;

	Dictionary<DataTypes.BiomeType, int> essenceCollected = new Dictionary<DataTypes.BiomeType, int>();
	int essenceCollectedCount = 0;
	List<GameObject> essenceInstances = new List<GameObject>();
	bool collectingEssence = false;

	bool gameAutoEnding = false;

	public PaddleData paddleData;
	public List<GameObject> paddles;

	private float timeOfGameStart = 0;
	private bool firstLaunch = true;
	private bool adLaunch = false;

	// PowerUp cooldowns ---------------------

	int ballHitCount = 0;


	public GameObject fireBlastPrefab;
	int fireBlastCountdown;
	int hitsBetweenFireBlast = 3;

	int lastQuakingHit = 0;
	int hitsBetweenQuakingHit = 3;

	int lastShadowDisable = 0;
	int hitsBetweenShadowDisable = 3;

	int lastSpawnedShadowBall = 0;
	int hitsBetweenShadowBall = 6;

	int lastDeathTrail = 0;
	int hitsBetweenDeathTrail = 8;
	bool deathTrailActive = false;

	int lastAllPoisonBalls = 0;
	int hitsBetweenAllPoisonBalls = 10;
	bool poisonAllActive = false;

	float lastMagPaddle = -20;
	float secondsMagPaddleLasts = 10;
	float secondsBetweenMagPaddle = 10;
	bool magPaddleAvailable = false;
	bool magPaddle = false;

	float lastBallControl = -20;
	float secondsBallControlLasts = 10;
	float secondsBetweenBallControl = 10;
	bool ballControlAvailable = false;
	bool ballControl = false;

	bool poisonHexPowerActive = false;
	float secondsHexPowerLasts = 10;
	float lastHexPower = 0;
	float secondsBetweenHexPower = 10;

	float lastShootFireBall = 0;
	float secondsBetweenShootFireBall = 45;

	float lastCreateFireElement = 0;
	float secondsBetweenCreateFireElement = 10;

	float lastEarthQuake = 0;
	float secondsBetweenEarthQuake = 10;

	float lastCreateGrowthElement = 0;
	float secondsBetweenCreateGrowthElement = 10;

	float lastSlowTime = 0;
	float secondsSlowTimeLasts = 5;
	float secondsBetweenSlowTime = 10;

	float lastMultipleBalls = 0;
	float secondsBetweenMultipleBalls = 10;

	float lastPoisonSpray = 0;
	float secondsBetweenPoisonSpray = 10;


	public GameObject poisonSprayPrefab;
	private int poisonSprayNum = 15;



	bool nextHitSwitchHexFire = false;
	bool nextHitSwitchHexGrowth = false;

	float lastTouchBegan = 0;
	float lastTouchEnded = 0;

	LevelData levelData;
	
	int spawnEarthTry = 0;
	int spawnGrowthTry = 0;
	public Vector3Int tryOrigin = new Vector3Int(-3, 10, 0);
	Vector3Int largeVec = new Vector3Int(-100000, -100000, -100000);

	public GameObject FallOutBarrier;

	public bool ballStuckToPaddle = false;
	bool readyToLaunchBall = true;

	int lastSpawnUnder = 0;

	public GameObject bouncePredictorPrefab;

	bool geyserLaunching = false;
	float geyserAngle = 90.0f;

	bool pickingWaterElementDirection = false;

	float lastTouchLength = 0;

	private List<Touch> curTouches = new List<Touch>();

	public List<Hex> hexes = new List<Hex>();
	public GameObject bossBattlePrefab;
	public BossMovement movementType;

	public Background background;

	private bool usedRewardedAd = false;
	public GameObject rewardedButton;

	private List<float> prevTimeScaleStack = new List<float>();

    public Text countDownPrefab;
    private bool countingDown = false;
	private Text countDownUI;

	private bool gameWon = false;
	private bool gameEnding = false;

	private bool runningVideoAd = false;

	public GameObject dragTipPrefab;
	public GameObject WorldCanvasPanel;
	private GameObject dragTipInstance;

	private float lastTimeUpdate, ssI, hunI;
	static readonly string[] leftover = {"01", "98", "37", "76", "52", "46", "83", "21"};

	// private Vector2 deltaPosWorld;
	// private Vector2 controlDirection;

	#if UNITY_EDITOR
		public bool overrideLevel = false;
	#endif

	void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		setCountdowns();

		// Make sure game can play
		setTimeScale(1);
	}

	private void setCountdowns() {
		fireBlastCountdown = hitsBetweenFireBlast;
	}

	// Use this for initialization
	void Start () {

		if(MasterPlayerData.instance != null) {
			paddleData = MasterPlayerData.instance.getActivePaddle();
		} else {
			Debug.LogWarning("MasterPlayerData is null. Likely you are not playing the real game.");
		}

		hexGrid = gridInstance.GetComponent<HexGrid>();
        grid = gridInstance.GetComponent<Grid>();

		// Generate hexes from layout
		#if UNITY_EDITOR
		if(!overrideLevel) {
		#endif
			object levelId = MasterManager.instance.getMessage().GetValue<object>("levelId");
			object biome = MasterManager.instance.getMessage().GetValue<object>("biome");
			if(levelId != null && biome != null) {

				if((int)levelId == -1) {
					level = RandomLevelGenerator.GenerateLevel((DataTypes.BiomeType) biome);
				} else {
					level = LevelLoader.instance.LoadLevelData((int) levelId, (DataTypes.BiomeType) biome);
				}


				BuildLoadedLevel(level);

				movementType = level.movementType;
				background.setBiome(level.biomeType);
			}

		#if UNITY_EDITOR
		} else {
			destructibleHexes = grid.transform.childCount;
		}
		#endif

		if(paddleData.runeIds.Contains(PowerUps.WaterAdditionalPaddle)) {
			// move current paddle left
			paddles[0].transform.Translate(-paddles[0].transform.localScale.x * 2f, 0, 0);
			GameObject paddleInstance = Instantiate(paddles[0], transform);
			paddleInstance.transform.parent = null;
			paddleInstance.transform.position = paddles[0].transform.position;
			paddles.Add(paddleInstance);
			//paddleInstance.transform.Translate(1.5f * paddles[0].transform.localScale.x * 2f, 0, 0);
			
		} else {
			paddles[0].transform.position = new Vector2(0, paddles[0].transform.position.y);
		}

		// Countdown then spawn ball
		spawnBall();

		if(paddleData.runeIds.Contains(PowerUps.ShadowLaunch)) {
			spawnBall(true, true);
		}

		// Remove physics gravity
		Physics2D.gravity = Vector2.zero;

		// Hide any panels
		if(WinPanel != null)
			WinPanel.SetActive(false);

		if(LosePanel != null)
			LosePanel.SetActive(false);
		
		if(pauseMenu != null)
			pauseMenu.SetActive(false);

		if(paddleData.runeIds.Contains(PowerUps.EarthFallOutBarrier) && FallOutBarrier != null) {
			Instantiate(FallOutBarrier, this.gameObject.transform);
		}

		if(paddleData.runeIds.Contains(PowerUps.GrowthSideSticky)) {
			setStickyZones(true);
		}

		if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.GrowthPaddleWidthUp)) {
			foreach (GameObject paddleInstance in paddles)
			{
				paddleInstance.GetComponent<Paddle>().changeSize(1.3f);
			}
		}

		if(paddles.Count > 1) {
			// Resize paddles
			for (int i = 0; i < paddles.Count; i++)
			{
				float offset = paddles[i].transform.localScale.x * i == 0 ? -2f : 2f;
				paddles[i].transform.position = new Vector3(offset, paddles[i].transform.position.y, paddles[i].transform.position.z);
			}
		}
		

		// Extra Lives
		if(paddleData.runeIds.Contains(PowerUps.WaterExtraLife1)) {
			lives++;
		}

		if(paddleData.runeIds.Contains(PowerUps.WaterExtraLife2)) {
			lives++;
		}
		
		if(paddleData.runeIds.Contains(PowerUps.WaterExtraLife3)) {
			lives++;
		}

		if(paddleData.runeIds.Contains(PowerUps.WaterGeyserLaunch)) {
			geyserLaunching = true;
			foreach (GameObject ballInstance in ballInstances)
			{
				ballInstance.GetComponent<Ball>().geyserDirection.SetActive(true);
			}
		}

		if(paddleData.runeIds.Contains(PowerUps.PoisonIncreaseBaseCombo)) {
			baseCombo = 2.0f;
		}

		currentCombo = baseCombo;

		if(paddleData.runeIds.Contains(PowerUps.GrowthHighPointSpawn)) {
			spawnHighPointBlocks();
		}

		if(levelData.isBossLevel) {
			Instantiate(bossBattlePrefab, transform);

			Elementalist.instance.sayEasyHappyIfAvailable(HintIds.Boss);
		}

		Elementalist.instance.sayEasyHappyIfAvailable(HintIds.BasicExplanation);
		
		Elementalist.instance.sayEasyHappyIfAvailable(Hints.getHintIdFromBiome(level.biomeType));

		// Set all texts
		if(EssenceText != null) 
			EssenceText.text = "Essence: " + essenceCollectedCount;
		
		if(LivesText != null) {
			//LivesText.text = "Lives: " + lives;
			StartCoroutine(popInIntro());			
		}

		if(ScoreText != null)
			ScoreText.text = "Score: " + score;

		if(ComboText != null)
			ComboText.text = "Combo: " + currentCombo;
		
		if(TimeText != null)
			TimeText.text = "0:00.00";
		// string results = "";
		// for (int i = 0; i < 100; i++)
		// {
		// 	results += getHealthForHex(1) + "\n";
		// }
		// print(results);

		// results = "";
		// for (int i = 0; i < 100; i++)
		// {
		// 	results += getHealthForHex(5) + "\n";
		// }
		// print(results);

		// results = "";
		// for (int i = 0; i < 100; i++)
		// {
		// 	results += getHealthForHex(10) + "\n";
		// }
		// print(results);

		if(paddleData.runeIds.Count < 10) {
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			int i = 0;
			foreach (PowerUps item in paddleData.runeIds)
			{
				dictionary.Add("slot" + i, item);
				i++;
			}
		}

		StartCoroutine(showDragTip());
	}

	private IEnumerator popInIntro() {
		yield return StartCoroutine(setInitLives(LivesText, lives));
		PowerUpsIcons pIcons = FindObjectOfType<PowerUpsIcons>();
		if(pIcons != null) {
			yield return StartCoroutine(pIcons.popIcons());
		}
	}

	void OnApplicationPause(bool pauseStatus) {
		if(pauseStatus && !pauseMenu.activeSelf && !WinPanel.activeSelf && !LosePanel.activeSelf && (!gameWon && !firstLaunch && !adLaunch)) {
			OnPauseClicked();
		}
	}

	private void BuildLoadedLevel(LevelData levelData)
    {
		this.levelData = levelData;

        
        //Set hexGrid cellSize and position
        hexGrid.transform.position = levelData.gridCellPosition;
        hexGrid.gridCellSize = levelData.gridCellSize;
        hexGrid.UpdateGridSize();

		destructibleHexes = levelData.hexData.Count;

        //Draw all Hex Objects
        foreach(HexData hexData in levelData.hexData)
        {
			spawnHex(hexData);
        }

		if(paddleData.runeIds.Contains(PowerUps.EarthSpawn3Elements)) {
			spawnEarthBlocks();
		}

    }

	private void spawnEarthBlocks() {
		int spawnLeft = 3;
			int tries = 50;

			HexData earthHexData = new HexData();
			earthHexData.hexType = DataTypes.HexType.Earth;
			earthHexData.hexScaleFactor = levelData.hexData[0].hexScaleFactor;

			while(spawnLeft > 0 && tries > 0) {
				Vector3Int position = trySpawn(spawnEarthTry, 0.7f);
				spawnEarthTry++;
				if(position != largeVec) {
					earthHexData.cellLocation = position;
					spawnHex(earthHexData);

					// space them out
					spawnEarthTry++;

					spawnLeft--;

					destructibleHexes++;
				}
				tries--;
			}
	}

	private void spawnHighPointBlocks() {
		int spawnLeft = 3;
		int tries = 50;

		HexData highPointHexData = new HexData();
		highPointHexData.hexType = DataTypes.HexType.HighPointGrowth;
		highPointHexData.hexScaleFactor = levelData.hexData[0].hexScaleFactor;

		while(spawnLeft > 0 && tries > 0) {
			float spawnPressure = (1 - tries / 50) * 10 + 1; // Increase chance of spawn as tries goes down
			Vector3Int position = trySpawn(spawnGrowthTry, 0.01f * spawnPressure);
			spawnGrowthTry++;
			if(position != largeVec) {
				highPointHexData.cellLocation = position;
				Hex newHex = spawnHex(highPointHexData).GetComponent<Hex>();

				//StartCoroutine(newHex.startDeathCountDown(10));

				// space them out
				spawnGrowthTry++;

				spawnLeft--;

				destructibleHexes++;
			}
			tries--;
		}
	}

	private void spawnPlantBlocksUnder(Transform ballTransform) {

		HexData plantBlockData = new HexData();
		plantBlockData.hexType = DataTypes.getHexFrom(levelData.biomeType);
		plantBlockData.hexScaleFactor = levelData.hexData[0].hexScaleFactor;

		Vector3Int ballCellPos = grid.WorldToCell(ballTransform.position);

		List<Vector3> surroundingHexes = Hex.getHexRingInWorldPos(ballCellPos, 2);

		foreach (Vector3 pos in surroundingHexes)
		{
			Vector3Int cellPos = grid.WorldToCell(pos);
			if(cellPos.y < ballCellPos.y - 1 && checkForCellEmptyAt(pos)) {
				//print("below");

				plantBlockData.cellLocation = cellPos;
				Hex newHex = spawnHex(plantBlockData).GetComponent<Hex>();
				newHex.GetComponent<HexBuilder>().overrideColor = true;
				newHex.addVines(null);
				
				newHex.countTowardHexesDestroyed = false;

				//destructibleHexes++;

			}
		}
	}

	private GameObject spawnHex(HexData hexData) {
		GameObject hexObj = Instantiate(hexPrefab);
		Hex hex = hexObj.GetComponent<Hex>();
		
		hex.setHexData(hexData);
		hexObj.transform.SetParent(hexGrid.transform);
		//Debug.Log("Cell Location: " + hexData.cellLocation);
		hexObj.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(hexData.cellLocation));
		hexObj.transform.localScale = new Vector3(hexData.hexScaleFactor.x * grid.cellSize.x, hexData.hexScaleFactor.y * grid.cellSize.y, 0);

		hexes.Add(hex);

		// if(hex.hexData.hexType != DataTypes.HexType.Growth) {
		// 	hex.hexData.health = getHealthForHex(levelData.levelId);
		// 	// hexObj.GetComponent<SpriteRenderer>().color = new Color((float)hex.hexData.health / 3, 0, 0);
		// }

        if(DataTypes.IsElement(hex.getHexType()))
        {
            Instantiate(hexParticle, hex.transform);
        }

		return hexObj;
	}

	public int getHealthForHex(int levelCount) {
		double u, v, S;

		do
		{
			u = 2.0 * Random.value - 1.0;
			v = 2.0 * Random.value - 1.0;
			S = u * u + v * v; 
		} while (S >= 1.0);

		float fac = Mathf.Sqrt(-2.0f * (Mathf.Log((float)S) / (float)S));
		int health = Mathf.RoundToInt((float)(u*fac) + (float)levelCount / 3);
		health = Mathf.Max(health, 1);
		return health;
	}

	public Vector3Int trySpawn(int tryType, float chanceOfSpawn) {
		int deltaX = tryType % 8;
		int deltaY = Mathf.FloorToInt(tryType / 8);
		Vector2 loc = (Vector2)grid.CellToWorld(tryOrigin) + new Vector2(deltaX, -deltaY * 2);

		if(checkForCellEmptyAt(loc)) {
			if(Random.value < chanceOfSpawn) {
				return grid.WorldToCell(loc);
			}
		}

		Vector3Int cellPos = Hex.cellPosFromLocal(loc);

		List<Vector3> extendedOptions = Hex.getHexRingInWorldPos(cellPos, 1);
		List<Vector3Int> foundEmpty = new List<Vector3Int>();

		foreach (Vector3 pos in extendedOptions)
		{
			
			if(checkForCellEmptyAt(pos)) {
				foundEmpty.Add(grid.WorldToCell(pos));
			}
		}

		if(foundEmpty.Count != 0) {
			// choose one of the surrounding options
			int randIndex = Random.Range(0, foundEmpty.Count - 1);
			if(Random.value < chanceOfSpawn) {
				return foundEmpty[randIndex];
			}
		}

		return largeVec;

	}

	// Returns true if cell is empty
	public static bool checkForCellEmptyAt(Vector3 worldPos) {
		RaycastHit2D originHit = Physics2D.Raycast(worldPos, Vector2.zero);

		return originHit.collider == null || originHit.collider.tag != "Hex";
	}

	private delegate void finalizeGameCallback();

	private void OnLoseGame() {

		updateTime(true);
		
		if(countDownUI != null && countDownUI.isActiveAndEnabled) {
			Destroy(countDownUI.gameObject);
		}

		GameFailedEvent.Invoke();

		// if(!MasterPlayerData.instance.GetPlayerData().seenAutoEnd) {
		// 	StartCoroutine(explainAutoEnd(OnLoseGame));
		// }

		// if(!MasterPlayerData.instance.GetPlayerData().seenCoins && MasterPlayerData.instance.getNumLevelsLeft() <= 57) {
		// 	StartCoroutine(explainCoins(OnLoseGame));
		// }

		Pause();
		
		// give chance to continue with rewarded ad
		rewardedButton.SetActive(!usedRewardedAd);

		// show lose panel
		LosePanel.SetActive(true);

		if(loseLevelLabel != null) {
			loseLevelLabel.text = "Level " + levelData.levelId;
		}

		// release all essesnce from paddle?
	}

	private void OnWinGame() {
		updateTime(true);

		gameWon = true;

		// Unlock next level
		if(MasterPlayerData.instance != null)
			MasterPlayerData.instance.unlockLevel(levelData.biomeType, levelData.levelId + 1);//unlockNextLevel(levelData.biomeType);

		// auto collect to paddle
		collectingEssence = true;
		
		// destroy balls
		destroyAllBalls();

		if(!gameEnding) {
			StartCoroutine(endGame(true));
		}
		

		// Calculate total score with bonuses

		// Give coins and essence

		// Girl asks how much essence you can give her
	}

    public void ShowAd()
    {
		adLaunch = true;
		lives = 3;

		LosePanel.SetActive(false);
		usedRewardedAd = true;

		goToPrevTimeScale();

		if(LivesText != null)
			StartCoroutine(setInitLives(LivesText, lives));
    }

    private IEnumerator endGame(bool win) {
		gameEnding = true;

		while (collectingEssence == true)
		{
			yield return null;
		}

		if(!MasterPlayerData.instance.checkSeenHint(HintIds.AutoEnd)) {
			yield return StartCoroutine(explainAutoEnd(null));
		}
		if(!MasterPlayerData.instance.checkSeenHint(HintIds.Coins) && MasterPlayerData.instance.getNumLevelsBeat() >= 3) {
			yield return StartCoroutine(explainCoins(null));
		}
		if(!MasterPlayerData.instance.checkSeenConvo(Convos.getHintIdFromBiome(levelData.biomeType)) && !hasNextLevel()) {
			yield return StartCoroutine(wonBossConvo(null));
		}
		if(!MasterPlayerData.instance.checkSeenConvo(ConvoIds.Final) && MasterPlayerData.instance.getNumLevelsBeat() >= 59) {
			yield return StartCoroutine(wonGameConvo(null));
		}

		//Pause();

		
		// yield return runAdThenWin();

		if(win) {
			WinPanel.SetActive(true);

			if(winLevelLabel != null) {
				winLevelLabel.text = levelData.levelId >= 0 ? "Level " + levelData.levelId : "Random Level";
			}

			// if(checkAllLevelsWon()) {
			// 	setTimeScale(1);
			// 	Elementalist.instance.sayEasyHappy("Congratulations! You have beaten all the bosses");
			// }
		}
	}

	private IEnumerator runAdThenWin() {

		// var options = new ShowOptions { resultCallback = HandleVideoResult };
		// runningVideoAd = MasterManager.instance.ShowAd(options);

		while(runningVideoAd) {
			yield return null;
		}

	}

	// private void HandleVideoResult(ShowResult result) {
	// 	runningVideoAd = false;
	// }

	private bool checkAllLevelsWon() {
		foreach (DataTypes.BiomeType biome in MasterPlayerData.instance.GetPlayerData().levels.Keys)
		{
			if(!MasterPlayerData.instance.GetPlayerData().levels[biome].score.ContainsKey(10) ||
			MasterPlayerData.instance.GetPlayerData().levels[biome].score[10] == 0) {
				return false;
			}
		}

		return true;
	}

	private void destroyAllBalls() {
		foreach (GameObject ball in ballInstances)
		{
			Destroy(ball);
		}
	}

	public void OnQuitToMainMenuClicked() {
		// Transition out slower
		MasterManager.instance.SwitchSceneTo("MainMenu");
	}

	public void OnQuitToLevelSelect() {
		MasterManager.instance.SwitchSceneTo("LevelSelect");
	}

	public void OnRetryClicked() {
		// Transition out slower
		MasterManager.instance.SwitchSceneTo("Game", MasterManager.instance.getMessage());
	}

	public void OnSkillTreeClicked() {
		GenericDictionary message = getNextLevel();
		if(message != null) {
			message.Add("continueGame", (object) true);
			MasterManager.instance.SwitchSceneTo("SkillTree", message);
		}
	}

	public void OnSkillTreeClickedLose() {
		GenericDictionary message = MasterManager.instance.getMessage();
		if(message != null) {
			message.Add("continueGame", (object) true);
			MasterManager.instance.SwitchSceneTo("SkillTree", message);
		}
	}

	public void OnContinueClicked() {
		// go to next level

		GenericDictionary nextLevelMessage;
		nextLevelMessage = getNextLevel();

		if(nextLevelMessage != null) {
			MasterManager.instance.SwitchSceneTo("Game", nextLevelMessage);
		}

	}

	private GenericDictionary getNextLevel() {
		if(levelData.levelId == -1) {
			return MasterManager.instance.getMessage();
		}

		int nextLevel = -1;
		List<int> levelList = LevelLoader.instance.getLevelList(levelData.biomeType);

		foreach (int id in levelList)
		{
			if(id > levelData.levelId) {
				nextLevel = id;
				break;
			}
		}

		if(nextLevel > 0) {
			GenericDictionary message = new GenericDictionary();
			message.Add("levelId", (object) nextLevel);
			message.Add("biome", (object) levelData.biomeType);
			return message;
		}

		return null;
	}

	public bool hasNextLevel() {
		int nextLevel = -1;
		List<int> levelList = LevelLoader.instance.getLevelList(levelData.biomeType);

		foreach (int id in levelList)
		{
			if(id > levelData.levelId) {
				nextLevel = id;
				break;
			}
		}

		return nextLevel != -1;
	}

	public void OnPauseClicked() {
		Pause();

		// show menu
		pauseMenu.SetActive(true);
	}

	public void OnResumeClicked() {
		Resume();

		// show menu
		pauseMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

		if(Elementalist.instance.getIsShowing()) {
			return;
		}
		
		curTouches = InputHelper.GetTouches();

		if(curTouches.Count > 0) {
			Touch myTouch = curTouches[0];

			// Watch for ball launches on touch down
			if(myTouch.phase == TouchPhase.Began) {
				print("touch began");

				if(Time.realtimeSinceStartup - lastTouchBegan < 0.25f) {
					print("doubletapped");
					// On doubletap check these things
	
					if(pickingWaterElementDirection) {
						finalizeWaterElementDirection();
					} else if(ballStuckToPaddle) {
						launchBall();
						setStickyZones(false);
					} else if(readyToLaunchBall) {
						launchBall();
					} else if(paddleData.runeIds.Contains(PowerUps.FireMassiveFireball) && checkShootFireBall()) {
						launchMassiveFireball();
					} else if(paddleData.runeIds.Contains(PowerUps.FireCreateFireElement) && checkCreateFireElement()) {
						setNextHitSwitchHexFire(true);
					} else if(paddleData.runeIds.Contains(PowerUps.EarthQuake) && checkEarthQuake()) {
						earthQuake();
					} else if(paddleData.runeIds.Contains(PowerUps.GrowthCreateGrowthElement) && checkCreateGrowthElement()) {
						setNextHitSwitchHexGrowth(true);
					} else if(paddleData.runeIds.Contains(PowerUps.ShadowSlowTime) && checkSlowTime()) {
						StartCoroutine(slowTime());
					} else if(paddleData.runeIds.Contains(PowerUps.PoisonMaxCombo) && checkHexPower()) {
						StartCoroutine(startHexPower());
					} else if(paddleData.runeIds.Contains(PowerUps.EarthMagneticPaddle)) {
						//print("attempting magPaddle: " + magPaddleAvailable + ", " + (lastMagPaddle + secondsMagPaddleLasts + magPaddleCooldown) + " : " + Time.time);
						//  if(!magPaddleAvailable && checkMagPaddle()) {
						StartCoroutine(activateMagPaddle());
						// } else if (magPaddleAvailable) {
						// 	switchMagPaddle(true);
						// }
					} else if(paddleData.runeIds.Contains(PowerUps.WaterControlBall)) {
						//print("attemping ballControl: " + ballControlAvailable + ", " + (lastBallControl + secondsBallControlLasts + secondsBetweenBallControl) + " : " + Time.time);	
						if(!ballControlAvailable && checkControlBall()) {
							StartCoroutine(activateBallControl());
						} else if(ballControlAvailable) {
							switchBallControl(true);
						}
					} else if(paddleData.runeIds.Contains(PowerUps.ShadowMultipleBalls) && checkMultipleBalls()) {
						spawnNewBall(false);
						TriggerEvent(PowerUps.ShadowMultipleBalls, PowerUpState.CoolDown, secondsBetweenMultipleBalls);
						lastMultipleBalls = Time.time;
					} else if(paddleData.runeIds.Contains(PowerUps.PoisonSpray) && checkPoisonSpray()) {
						sprayPoison();
					}

					// fake winning on double tap
					//OnWinGame();
				}
				

				lastTouchBegan = Time.realtimeSinceStartup;

			} else if(myTouch.phase == TouchPhase.Moved) {

				if(geyserLaunching) {
					geyserAngle = Vector2.Angle(Vector2.right, (Vector2)MasterManager.instance.masterCamera.ScreenToWorldPoint(Input.mousePosition) - (Vector2)paddles[0].transform.position);
					
					bool shadowLaunching = paddleData.runeIds.Contains(PowerUps.ShadowLaunch);
					foreach (GameObject ballInstance in ballInstances)
					{
						float angleOffset = 0;
						if(shadowLaunching) {
							if(ballInstance.GetComponent<Ball>().isShadowBall) {
								angleOffset = -45;
							} else {
								angleOffset = 45;
							}
						}
						Vector3 v = ballInstance.GetComponent<Ball>().geyserDirection.transform.localRotation.eulerAngles;
						ballInstance.GetComponent<Ball>().geyserDirection.transform
							.localRotation = Quaternion.Euler(v.x, v.y, geyserAngle - 90 + angleOffset);
					}

				} else if(pickingWaterElementDirection) {
					if(MasterManager.instance.masterCamera.ScreenToWorldPoint(myTouch.position).y < waterDeathEffect.transform.position.y) {
						waterElementAngle = Vector2.Angle(Vector2.left, (Vector2)MasterManager.instance.masterCamera.ScreenToWorldPoint(myTouch.position) - (Vector2)waterDeathEffect.transform.position);
						waterElementAngle += 180;
					} else {
						waterElementAngle = Vector2.Angle(Vector2.right, (Vector2)MasterManager.instance.masterCamera.ScreenToWorldPoint(myTouch.position) - (Vector2)waterDeathEffect.transform.position);
					}

					if(waterElementPickingBall != null) {
						Vector3 v = waterElementPickingBall.geyserDirection.transform.localRotation.eulerAngles;

						waterElementPickingBall.geyserDirection.transform.localRotation = Quaternion.Euler(v.x, v.y, waterElementAngle - 90);
					} else {
						finalizeWaterElementDirection();
					}


				} else {
					for (int i = 0; i < paddles.Count; i++) {
						float offset = paddles[i].transform.localScale.x * (paddles.Count == 1 ? 0 : i == 0 ? -2f : 2f);
						paddles[i].GetComponent<Paddle>().checkForTouchMove(offset);
					}
				}

				if(magPaddle) {
					// print("doing mag paddle");
					if(hexGrid.transform.childCount > 0) {
						int i = 0;
						foreach (GameObject ballInstance in ballInstances)
						{
							Vector2 direction = hexGrid.transform.GetChild(i).position - ballInstance.transform.position;
							//Vector2 direction = paddles[0].transform.position - ballInstance.transform.position;
							ballInstance.GetComponent<Rigidbody2D>().AddForce(direction.normalized, ForceMode2D.Impulse );

							if(i + 1 < hexGrid.transform.childCount) {
								i++;
							} else {
								i = 0;
							}
						}
					}
				}

				if(ballControl) {
					// print("doing ball control");
					foreach (GameObject ballInstance in ballInstances)
					{
						// float screenUnitPerWorldUnit = Mathf.Abs(MasterManager.instance.masterCamera.WorldToScreenPoint(Vector2.right).x);

						//print("deltaPos: " + myTouch.deltaPosition.x);
						//print("screen unit per world unit: " + screenUnitPerWorldUnit);
						// Vector2 direction = new Vector2(myTouch.deltaPosition.x /screenUnitPerWorldUnit, 0) * 20;
						//print("direction: " + direction);

						// deltaPosWorld = MasterManager.instance.masterCamera.ScreenToWorldPoint(myTouch.deltaPosition);

						// print("deltaPosWorld.y: " + deltaPosWorld);
						// print("myTouch.deltaPosition: " + myTouch.deltaPosition);

						// controlDirection.x = paddles[0].transform.position.x - ballInstance.transform.position.x;
						// controlDirection.y = myTouch.deltaPosition.y; //deltaPosWorld.y);

						ballInstance.GetComponent<Rigidbody2D>().AddForce(myTouch.deltaPosition.normalized, ForceMode2D.Impulse);//direction.normalized, ForceMode2D.Impulse);
					}
				}

				checkBouncePredictions();
			} else if(myTouch.phase == TouchPhase.Ended) {
				// switchMagPaddle(false);
				switchBallControl(false);
				
				lastTouchEnded = Time.realtimeSinceStartup;
			}
		}

		if(shadowOn) {
			checkBallPositionsAndRemoveShadow();
		}

		if(collectingEssence) {
			collectLeftoverEssence();
		}

		//if game hasn't finished && it past first launch or past adlaunch
		if(!gameWon && !firstLaunch && !adLaunch) { 
			timeUsed += Time.deltaTime;
		}

		updateScore();
		updateTime(false);
	}

	private IEnumerator slowTime() {
		setTimeScale(0.5f);

		yield return new WaitForSeconds(secondsSlowTimeLasts);

		StartCoroutine(endSlowTime());
	}

	private IEnumerator endSlowTime() {
		while(Time.timeScale < prevTimeScaleStack[prevTimeScaleStack.Count - 2]) {
			Time.timeScale += 0.05f;
			yield return null;
		}

		goToPrevTimeScale();
		TriggerEvent(PowerUps.ShadowSlowTime, PowerUpState.CoolDown, secondsBetweenSlowTime);
	}

	void spawnBall(bool isShadow, bool isStart) {
		// Create ball game object
		GameObject ballInstance = Instantiate(ballPrefab, ballSpawnLocation);
		ballInstances.Add(ballInstance);
		ballInstance.gameObject.layer = 12;

		// Correct paddle placement if it's outside of launchable area
		if(paddleData.runeIds.Contains(PowerUps.WaterGeyserLaunch) && Mathf.Abs(ballInstance.transform.position.x) > 5.4f) {
			float deltaX = (5.4f - Mathf.Abs(ballInstance.transform.position.x)) * Mathf.Sign(ballInstance.transform.position.x);
			ballInstance.transform.parent.parent.Translate(new Vector3(deltaX, 0, 0));
		}

		Ball ball = ballInstance.GetComponent<Ball>();

		if(!isShadow) {
			numBalls++;
		} else {
			shadowBalls++;
		}

		if(isStart) {
			readyToLaunchBall = true;
		}
		

		if(shadowOn) {
			toggleBallShadowSprites(true);
		}

		if(paddleData.runeIds.Contains(PowerUps.EarthHeavyBall)) {
			ballInstance.GetComponent<Rigidbody2D>().mass = 2;
		}

		if(paddleData.runeIds.Contains(PowerUps.GrowthBallSizeUp1)) {
			ball.changeSize(1);
		}

		if(paddleData.runeIds.Contains(PowerUps.GrowthBallSizeUp2)) {
			ball.changeSize(1);
		}

		ball.heavyBall = paddleData.runeIds.Contains(PowerUps.EarthHeavyBall);

		if(isShadow) {
			ball.setShadowBall();
		}

		if(ballInstances.Count > 1 && isStart) {
			layoutBalls();
		}

		if(paddleData.runeIds.Contains(PowerUps.PoisonStartPoisoned)) {
			ball.poison(false);
		}

		checkBouncePredictions();

		StartCoroutine(waitThenCheckPredictions());
	}

	void spawnBall() {
		spawnBall(false, true);
	}

	IEnumerator waitThenCheckPredictions() {
		yield return new WaitForSeconds(0.02f);

		checkBouncePredictions();
	}

	void layoutBalls() {
		int uniqueBallCount = 0;
		foreach (GameObject ballInstance in ballInstances)
		{
			Ball ball = ballInstance.GetComponent<Ball>();
			if(!ball.isShadowBall) {
				uniqueBallCount++;
			}
		}

		float startPosOffset = 0;
		if(uniqueBallCount % 2 == 0) {
			startPosOffset = (uniqueBallCount - 1) * ballSpacing / 2;
		} else {
			startPosOffset = (uniqueBallCount - 1) / 2 * ballSpacing;
		}

		int uniqueBall = 0;
		for (int i = 0; i < ballInstances.Count; i++)
		{
			GameObject ballInstance = ballInstances[i];
			Ball ball = ballInstance.GetComponent<Ball>();
			if(!ball.isShadowBall) {
				ballInstance.transform.position = ballSpawnLocation.position + new Vector3(-startPosOffset + uniqueBall * ballSpacing, 0, 0);
				uniqueBall++;
			} else {
				ballInstance.transform.position = ballSpawnLocation.position + new Vector3(-startPosOffset + (uniqueBall - 1) * ballSpacing, 0, 0);
			}
		}
	}

	public void launchBall() {
		foreach (GameObject ballInstance in ballInstances)
		{
			if(Mathf.Abs(ballInstance.transform.position.x) > 5.4f) {
				return;
			}
		}

		if(firstLaunch) {
			// start initial countdown on actives
			lastShootFireBall = 0;
			lastCreateFireElement = 0;
			lastMagPaddle = 0;
			lastEarthQuake = 0;
			lastCreateGrowthElement = 0;
			lastHexPower = 0;
			lastPoisonSpray = 0;
			lastMultipleBalls = Time.time + secondsBetweenMultipleBalls;
			lastSlowTime = 0;
			lastBallControl = 0;

			PowerUps activePowerup = MasterPlayerData.instance.getActiveOnPaddle();
			if(activePowerup != PowerUps.Placeholder) {
				TriggerEvent(activePowerup, PowerUpState.CoolDown, getTimeBetween(activePowerup));
			}
		}

		float ballModifiedSpeed = getBallModifiedSpeed();
		//print("ballModifiedSpeed = " + ballModifiedSpeed);
		Vector2 ballModifiedDirection = Vector2.up;

		if(geyserLaunching) {
			ballModifiedDirection = rotateVectorBy(Vector2.right, geyserAngle);

			foreach (GameObject ballInstance in ballInstances)
			{
				ballInstance.GetComponent<Ball>().geyserDirection.SetActive(false);
			}
		}

		int ballNum = 0;
		// launch all balls
		foreach (GameObject ballInstance in ballInstances)
		{
			if(ballInstance != null) {
				if(paddleData.runeIds.Contains(PowerUps.ShadowLaunch)) {
					if(ballNum % 2 == 0) {
						ballInstance.GetComponent<Ball>().launch(ballModifiedSpeed, rotateVectorBy(ballModifiedDirection, -45));
					} else {
						ballInstance.GetComponent<Ball>().launch(ballModifiedSpeed, rotateVectorBy(ballModifiedDirection, 45));
					}
				} else {
					ballInstance.GetComponent<Ball>().launch(ballModifiedSpeed, ballModifiedDirection);
				}
			}
			ballNum++;
		}

		readyToLaunchBall = false;
		ballStuckToPaddle = false;
		
		BouncePredictor.RemoveAll();

		firstLaunch = false;
		adLaunch = false;
		geyserLaunching = false;
	}

	public float getBallModifiedSpeed() {
		float speedMultiplier = 0;

		if(paddleData.runeIds.Contains(PowerUps.FireBallSpeedUp1)) {
			speedMultiplier += 1;
		}

		if(paddleData.runeIds.Contains(PowerUps.FireBallSpeedUp2)) {
			speedMultiplier += 1;
		}

		return ballInitSpeed + (speedMultiplier * 2);
	}

	public Vector2 rotateVectorBy(Vector2 original, float angleDeg) {
		float theta = Mathf.Deg2Rad * angleDeg;

		float cos = Mathf.Cos(theta);
		float sin = Mathf.Sin(theta);

		float px = original.x * cos - original.y * sin;
		float py = original.x * sin + original.y * cos;

		return new Vector2(px, py);
	}

	public void checkBouncePredictions() {
		BouncePredictor.RemoveAll();

		if(paddleData.runeIds.Contains(PowerUps.WaterBouncePredictor) && readyToLaunchBall) {
			foreach (GameObject ball in ballInstances)
			{
				// Fire bouncePredictor that has the ball sized collider and goes out when paddle is moved
				BouncePredictor predictor = BouncePredictor.GetPredictor(transform);

				if(geyserLaunching) {
					if(paddleData.runeIds.Contains(PowerUps.ShadowLaunch)) {
						if(ball.GetComponent<Ball>().isShadowBall) {
							predictor.launch(rotateVectorBy(Vector2.right, geyserAngle + 45), ball.transform, ball.GetComponent<CircleCollider2D>().radius);
						} else {
							predictor.launch(rotateVectorBy(Vector2.right, geyserAngle - 45), ball.transform, ball.GetComponent<CircleCollider2D>().radius);
						}
					} else {
						predictor.launch(rotateVectorBy(Vector2.right, geyserAngle), ball.transform, ball.GetComponent<CircleCollider2D>().radius);
					}
				} else {
					predictor.launch(Vector2.up, ball.transform, ball.GetComponent<CircleCollider2D>().radius);
				}
			}
		}
	}

	public void ballDestroyed(GameObject ball) {
		// print("ball died");
		ballInstances.Remove(ball);
		
		if(!ball.GetComponent<Ball>().isShadowBall) {
			numBalls--;
		} else {
			shadowBalls++;
		}
		
		if(numBalls <= 0) {
			despawnAllBalls();

			livesLost++;
			lives--;

			setCurrentCombo(baseCombo);

			if(LivesText != null)
				updateLifeText(LivesText, lives);//LivesText.text = "Lives: " + lives;

			if(lives <= 0) {
				OnLoseGame();
			}

			spawnBall();

			if(paddleData.runeIds.Contains(PowerUps.ShadowLaunch)) {
				spawnBall(true, true);
			}

			if(paddleData.runeIds.Contains(PowerUps.WaterGeyserLaunch)) {
				geyserLaunching = true;
				foreach (GameObject ballInstance in ballInstances)
				{
					ballInstance.GetComponent<Ball>().geyserDirection.SetActive(true);
				}
			}
		}
	}

	private void despawnAllBalls() {
		// print("ballInstances: " + ballInstances.Count);
		for (int i = ballInstances.Count - 1; i >= 0; i--)
		{
			ballInstances[i].GetComponent<Ball>().onDeath(false);
		}
		
		ballInstances.Clear();
	}

	public void hexHit(Vector3 hexPosition) {
		addPointsAt(10, hexPosition);
	}

	public void hexDestroyed(int pointVal, bool countTowardDestroyed, Vector3 hexPosition) {
		// if all hexes are gone you win
		if(countTowardDestroyed) {
			numHexesDestroyed++;
		}

		//float combo = currentCombo * levelData.pointMultiplier * (poisonHexPowerActive ? 2 : 1);

		
		addPointsAt(pointVal, hexPosition);
		


		if(numHexesDestroyed >= destructibleHexes) {
			OnWinGame();
		}

		// if(numHexesDestroyed == 1 && paddleData.runeIds.Contains(PowerUps.GrowthHighPointSpawn)) {
		// 	spawnHighPointBlocks();
		// }

        if(numHexesDestroyed >= destructibleHexes - 3)
        {
           	endGameAfterDelay();
        }
	}

	private void addPointsAt(int points, Vector3 hexPosition) {
		int hexPoints = Mathf.CeilToInt((float)points * currentCombo * levelData.pointMultiplier * (poisonHexPowerActive ? 2 : 1));

		score += hexPoints;

		// if(points != 0) {
		// 	popUpScore(hexPosition, hexPoints);//, combo));
		// 	//popUpScore3d(hexPosition, hexPoints);
			
		// }
	}

	private void popUpScore3d(Vector3 position, int value) {
		// if(GetComponent<PoolManager>() == null || !GetComponent<PoolManager>().isActiveAndEnabled) {
			GameObject popup = Instantiate(pointPopUp3dPrefab, position, Quaternion.identity);
			popup.transform.SetParent(transform, true);//pointPopUpsObject.transform, true);
			popup.GetComponent<HexPointPopup3d>().ResetAndRunAnim(value);

			return;
		// }

		// GameObject clone = GameManage.instance.GetComponent<PoolManager>().GetObjectByName("HexPointPopup");
		
		// // Place in the correct place
		// clone.transform.SetPositionAndRotation(MasterManager.instance.masterCamera.WorldToScreenPoint(position), Quaternion.identity);
		// clone.transform.SetParent(pointPopUpsObject.transform);

		// clone.GetComponent<HexPointPopup>().ResetAndRunAnim(value);
	} 

	private void popUpScore(Vector3 position, int value) {//}, float combo) {
		if(GetComponent<PoolManager>() == null || !GetComponent<PoolManager>().isActiveAndEnabled) {
			GameObject popup = Instantiate(pointPopUpPrefab, MasterManager.instance.masterCamera.WorldToScreenPoint(position), Quaternion.identity);
			popup.transform.SetParent(transform, true);//pointPopUpsObject.transform, true);
			popup.GetComponent<HexPointPopup>().ResetAndRunAnim(value);

			return;
		}

		GameObject clone = GameManage.instance.GetComponent<PoolManager>().GetObjectByName("HexPointPopup");
		//((RectTransform)clone.transform).anchoredPosition = Vector2.zero;
		
		// Place in the correct place
		clone.transform.SetPositionAndRotation(MasterManager.instance.masterCamera.WorldToScreenPoint(position), Quaternion.identity);
		clone.transform.SetParent(pointPopUpsObject.transform);

		clone.GetComponent<HexPointPopup>().ResetAndRunAnim(value);

		// + " x" + combo.ToString("0.0");

 
		//yield return ((RectTransform)clone.transform).DOAnchorPos(Vector2.zero, 0.2f, false).WaitForCompletion();
		
	}

	private IEnumerator explainAutoEnd(finalizeGameCallback callback) {

		yield return Elementalist.instance.trySayNow(Hints.getHintFrom(HintIds.AutoEnd), Elementalist.instance.emotions.happy);

		MasterPlayerData.instance.seenHint(HintIds.AutoEnd);
		
		if(callback != null) {
			callback();
		}
	}

	private IEnumerator explainCoins(finalizeGameCallback callback) {
		 
		yield return Elementalist.instance.trySayNow(Hints.getHintFrom(HintIds.Coins), Elementalist.instance.emotions.happy);

		MasterPlayerData.instance.seenHint(HintIds.Coins);

		if(callback != null) {
			callback();
		}
	}

	private IEnumerator wonBossConvo(finalizeGameCallback callback) {

		//yield return Elementalist.instance.trySayNow(Convos.getHintFrom(ConvoIds.Boss), Elementalist.instance.emotions.happy);

		yield return Elementalist.instance.trySayNow(Convos.getHintStringFromBiome(levelData.biomeType), Elementalist.instance.emotions.happy);

		yield return Elementalist.instance.trySayNow(Convos.getHintFrom(ConvoIds.Reward) + levelData.biomeType + " essence!", Elementalist.instance.emotions.happy);

		MasterPlayerData.instance.seenConvo(Convos.getHintIdFromBiome(levelData.biomeType));

		CollectedEssence(levelData.biomeType, 100);
		//MasterPlayerData.instance.addEssence(levelData.biomeType, 100);
		
		if(callback != null) {
			callback();
		}
	}

	private IEnumerator wonGameConvo(finalizeGameCallback callback) {

		yield return Elementalist.instance.trySayNow(Convos.getHintFrom(ConvoIds.Final), Elementalist.instance.emotions.happy);

		MasterPlayerData.instance.seenConvo(ConvoIds.Final);
		
		if(callback != null) {
			callback();
		}
	}


    private void endGameAfterDelay()
    {
        // Begin countdown
        if (!countingDown)
        {
            countingDown = true;
            StartCoroutine(countDown());
        }
    }

    private IEnumerator countDown()
    {
        // give player 5 seconds before starting
        yield return new WaitForSeconds(5f);

		if(numHexesDestroyed >= destructibleHexes) {
			yield break;
		}

        float countDownTime = 5;
        countDownUI = Instantiate(countDownPrefab, CountDownHolder.transform).GetComponent<Text>();
		HexAutoEndEvent.Invoke();

        float beginTime = Time.time;
        while (Time.time - beginTime < countDownTime)
        {
            yield return new WaitForSeconds(0.1f);
			if(countDownUI != null) {
            	countDownUI.text = "" + (int) (countDownTime + 1 - (Time.time - beginTime));
			}

			if(numHexesDestroyed >= destructibleHexes) {
				Destroy(countDownUI.gameObject);
				yield break;
			}
        }

        Destroy(countDownUI.gameObject);

		foreach (GameObject ballObj in ballInstances)
		{
			ballObj.GetComponent<Rigidbody2D>().simulated = false;
			StartCoroutine(ballPop(ballObj));
		}

		foreach (Hex hex in hexes)
		{
			if(hex != null && hex.gameObject.activeSelf) {
				while (hex.getPoisoned() || hex.spinningBall)
				{
					yield return null;
				}
			}
		}

		yield return new WaitForSeconds(1);

		if(WinPanel.activeSelf) {
			yield break;
		}

		OnWinGame();
    }

	IEnumerator ballPop(GameObject ballObj) {
		yield return new WaitForSeconds(Random.value);
		Destroy(ballObj);
	}

	private void updateScore() {
		//calculateScore();
		if(ScoreText != null)
			//ScoreText.text = "Score: " + score;
			updateScoreText(ScoreText, score);
	}

	StringBuilder sb = new StringBuilder(7, 20);
	
	float mmF;

	private void updateTime(bool real) {
		// This creates a lot of garbage so we only do it if it's required
		if(real) {
			ss = (timeUsed % 60).ToString("00.00");
			mm = (Mathf.Floor(timeUsed / 60f) % 60).ToString();
			TimeText.text = mm + ":" +  ss;
			return;
		}

		// We don't want to create garbage for a fairly useless piece of the game (every frame)
		if(timeUsed - lastTimeUpdate < 0.04f) {
			return;
		}
		// // hunF = timeUsed % 6000;
		// //123.45658 -> 2:03.45

		// ssF = timeUsed % 60; // 03.45
		// mmF = (Mathf.Floor(timeUsed / 60f) % 60); // 2

		hunI = (float)System.Math.Truncate((timeUsed - System.Math.Truncate(timeUsed)) * 100);
		ssI = Mathf.Floor(timeUsed % 60);
		mmF = (Mathf.Floor(timeUsed / 60f) % 60);

		sb.Length = 0;
		sb.Append(mmF);
		sb.Append(":");
		if(ssI < 10) sb.Append("0");
		sb.Append(ssI);
		sb.Append(".");
		if(hunI < 10) sb.Append("0");
		sb.Append(hunI);

		TimeText.text = sb.ToString();

		// // lastTimeUpdate = timeUsed;
		



		// if(TimeText != null) {
		// 	if(real) {
		// 		ss = (timeUsed % 60).ToString("00.00");
		// 		mm = (Mathf.Floor(timeUsed / 60f) % 60).ToString();
		// 		TimeText.text = mm + ":" +  ss;
		// 	} else {
		// 		if(timeUsed - lastTimeUpdate < 0.04f) {
		// 			return;
		// 		}

		// 		hunI = (float)System.Math.Truncate((timeUsed - System.Math.Truncate(timeUsed)) * 100);
		// 		ssI = Mathf.Floor(timeUsed % 60);
		// 		mm = (Mathf.Floor(timeUsed / 60f) % 60).ToString();
		
		// 		TimeText.text = mm + ":" + (ssI < 10 ? "0" : "") +  ssI.ToString() + "." + hunI;//leftover[UnityEngine.Random.Range(0, leftover.Length)];
		// 	}
			
			
		// 	lastTimeUpdate = timeUsed;
		// }
	}

	// private void calculateScore() {
	// 	score = ballHitCount + ;
	// }

	public void showShadow() {
		// Another shadow has been killed so we get restart the countdown
		if(shadowOffCountdown != null) {
			StopCoroutine(shadowOffCountdown);

			// Do we want to reset the shadows here?
		}

		if(shadowTilemap == null) {
			shadowTilemap = Instantiate(shadowTilemapPrefab, gridInstance.transform).GetComponent<Tilemap>();
		}

		shadowOn = true;
		if(shadowTilemap != null) {
			// remove element shadows

			// fade it in
			shadowTilemap.enabled = true;
			shadowTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = true;

		}

		toggleBallShadowSprites(true);
		
		

		shadowOffCountdown = StartCoroutine(shadowOffTimer(10));
	}

	private void turnOffShadow() {
		shadowOn = false;
		if(shadowTilemap != null) {
			Destroy(shadowTilemap.gameObject);
			shadowTilemap = null;
			// shadowTilemap.enabled = false;
			// shadowTilemap.gameObject.GetComponent<TilemapRenderer>().enabled = false;
		}

		toggleBallShadowSprites(false);
	}

	private void toggleBallShadowSprites(bool on) {
		foreach (GameObject ballInstance in ballInstances)
		{
			ballInstance.GetComponent<Ball>().toggleShadowBallSprite(on);
		}
	}

	public void removeShadow(Vector3 worldPos) {
		if(shadowTilemap != null && shadowOn) {

			// remove shadow at the location
			shadowTilemap.SetTile(shadowTilemap.WorldToCell(worldPos), null);
		}
	}

	private void checkBallPositionsAndRemoveShadow() {
		foreach (GameObject ballInstance in ballInstances)
		{
			if(ballInstance != null) {
				Vector3Int cellPos = shadowTilemap.WorldToCell(ballInstance.transform.position);
				if(shadowTilemap.HasTile(cellPos) && shadowTilemap.GetTile(cellPos) == blackTile) {
					shadowTilemap.SetTile(cellPos, null);
				}
			}
		}
	}

	IEnumerator shadowOffTimer(float seconds) {
		yield return new WaitForSeconds(seconds);

		turnOffShadow();
	}

	public void Pause() {
		// pause game
		setTimeScale(0);
		
		// hide pause button
		if(pauseButton != null)
			pauseButton.SetActive(false);
		
	}

	public void Resume() {
		// resume game
		goToPrevTimeScale();

		// show pause button
		if(pauseButton != null)
			pauseButton.SetActive(true);
		
	}

	void setTimeScale(float scale) {
		prevTimeScaleStack.Add(scale);
		Time.timeScale = scale;
	}

	public void goToPrevTimeScale() {
		prevTimeScaleStack.RemoveAt(prevTimeScaleStack.Count - 1);
		Time.timeScale = prevTimeScaleStack[prevTimeScaleStack.Count - 1];
	}

	public void EssenceLaunched(GameObject essence) {
		essenceInstances.Add(essence);
	}

	public void EssenceRemoved(GameObject essence) {
		essenceInstances.Remove(essence);

		if(collectingEssence && essenceInstances.Count == 0) {
			collectingEssence = false;
		} 
	}

	private void collectLeftoverEssence() {
		foreach(GameObject essence in essenceInstances) {
			if(essence != null) {
				Rigidbody2D rb = essence.GetComponent<Rigidbody2D>();

				Vector2 essenceToPaddle = paddles[0].transform.position - essence.transform.position; 

				rb.AddForce(essenceToPaddle);

				essence.GetComponent<Drop>().SetFloating(false);
			}
		}

		if(collectingEssence && essenceInstances.Count == 0) {
			collectingEssence = false;
		}
	}

	public void CollectedEssence(DataTypes.BiomeType type) {
		CollectedEssence(type, 1);
	}

	private void CollectedEssence(DataTypes.BiomeType type, int amount) {
		if(!essenceCollected.ContainsKey(type)) {
			essenceCollected[type] = 0;
		}

		essenceCollected[type] += amount;

		essenceCollectedCount += amount;

		EssenceText.text = "Essence: " + essenceCollectedCount;
	}

	public int addBallHit() {
		ballHitCount++;

		checkFireBlast();

		checkDisableShadow();

		checkXHitsShadowBall();

		checkDeathTrail();

		checkAllPoisonBalls();

		checkQuakingHit();

		return ballHitCount;
	}

	private void checkFireBlast() {
		
		if(!paddleData.runeIds.Contains(PowerUps.FireBlast)) {
			return;
        }

		fireBlastCountdown--;

		if(fireBlastCountdown <= 0) {
			fireBlastCountdown = hitsBetweenFireBlast; // number of hits between blasts
			
			launchFireBlast();
		}

		TriggerEvent(PowerUps.FireBlast, PowerUpState.CoolDown, fireBlastCountdown);
	}

	private void launchFireBlast() {
		foreach (GameObject paddle in paddles)
		{
			GameObject fireBlastInstance = Instantiate(fireBlastPrefab, paddle.transform);
			fireBlastInstance.transform.parent = null;
			fireBlastInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
		}
	}

	private void checkDisableShadow() {
		if(paddleData.runeIds.Contains(PowerUps.ShadowDisableShadow)) {
			if(shadowOn && lastShadowDisable + hitsBetweenShadowDisable < ballHitCount) {
				turnOffShadow();
				lastShadowDisable = ballHitCount;
			}

			TriggerEvent(PowerUps.ShadowDisableShadow, PowerUpState.CoolDown, lastShadowDisable + hitsBetweenShadowDisable - ballHitCount);
		}
	}

	private void checkXHitsShadowBall() {
		if(paddleData.runeIds.Contains(PowerUps.ShadowXHitsShadowBall)) {
			if(lastSpawnedShadowBall + hitsBetweenShadowBall < ballHitCount && ballInstances.Count < 3) {

				spawnNewBall(true);
				lastSpawnedShadowBall = ballHitCount;
			}
			TriggerEvent(PowerUps.ShadowDisableShadow, PowerUpState.CoolDown, lastSpawnedShadowBall + hitsBetweenShadowBall - ballHitCount);
		}
	}

	private void spawnNewBall(bool isShadow) {
		spawnBall(isShadow, false);
		GameObject ballInstance = ballInstances[ballInstances.Count - 1];

		ballInstance.transform.parent = null;
		ballInstance.transform.position = ballInstances[0].transform.position;
		ballInstance.GetComponent<Ball>().launch(getBallModifiedSpeed());//ballInstances[0].GetComponent<Rigidbody2D>().velocity.magnitude);
	}

	private void checkDeathTrail() {
		if(paddleData.runeIds.Contains(PowerUps.ShadowTrail) && !deathTrailActive) {
			if(lastDeathTrail + hitsBetweenDeathTrail <= ballHitCount) {
				foreach (GameObject ballInstance in ballInstances)
				{
					ballInstance.GetComponent<Ball>().enableDeathTrail();
				}

				TriggerEvent(PowerUps.ShadowTrail, PowerUpState.Active, 3);

				StartCoroutine(triggerTrailDone(3));
			} else {
				TriggerEvent(PowerUps.ShadowTrail, PowerUpState.CoolDown, lastDeathTrail + hitsBetweenDeathTrail - ballHitCount);
			}
		}
	}

	IEnumerator triggerTrailDone(float seconds) {
		deathTrailActive = true;
		yield return new WaitForSeconds(seconds);
		deathTrailActive = false;
		lastDeathTrail = ballHitCount;
		TriggerEvent(PowerUps.ShadowTrail, PowerUpState.CoolDown, lastDeathTrail + hitsBetweenDeathTrail - ballHitCount);
	}

	private void checkAllPoisonBalls() {
		if(paddleData.runeIds.Contains(PowerUps.PoisonXHitsPoison) && !poisonAllActive) {
			if(lastAllPoisonBalls + hitsBetweenAllPoisonBalls <= ballHitCount) {
				float seconds = 0;
				foreach (GameObject ballInstance in ballInstances)
				{
					seconds = ballInstance.GetComponent<Ball>().poison();
				}

				TriggerEvent(PowerUps.PoisonXHitsPoison, PowerUpState.Active, seconds);

				StartCoroutine(triggerPoisonDone(seconds));

			} else {
				TriggerEvent(PowerUps.PoisonXHitsPoison, PowerUpState.CoolDown, lastAllPoisonBalls + hitsBetweenAllPoisonBalls - ballHitCount);
			}

		}
	}

	IEnumerator triggerPoisonDone(float seconds) {
		poisonAllActive = true;
		yield return new WaitForSeconds(seconds);
		poisonAllActive = false;
		lastAllPoisonBalls = ballHitCount;
		TriggerEvent(PowerUps.PoisonXHitsPoison, PowerUpState.CoolDown, lastAllPoisonBalls + hitsBetweenAllPoisonBalls - ballHitCount);
	}

	private void launchMassiveFireball() {
		foreach (GameObject paddle in paddles)
		{
			GameObject fireBlastInstance = Instantiate(fireBlastPrefab, paddle.transform);
			fireBlastInstance.transform.parent = null;
			fireBlastInstance.transform.localScale *= 3;
			fireBlastInstance.GetComponent<FireBlast>().hasPassthrough = true;
			fireBlastInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 10), ForceMode2D.Impulse);
		}

		lastShootFireBall = Time.time;
		TriggerEvent(PowerUps.FireMassiveFireball, PowerUpState.CoolDown, secondsBetweenShootFireBall);
	}

	private void setStickyZones(bool value) {
		foreach (GameObject paddle in paddles)
		{
			paddle.GetComponent<Paddle>().setStickyZones(value);	
		}
	}

	public void spawnBlocksUnder(Transform transform) {
		bool afterXHits = ballHitCount >= lastSpawnUnder + 3;
		if(afterXHits) {

			spawnPlantBlocksUnder(transform);


			lastSpawnUnder = ballHitCount;
		}
	}

	public void wormHoleBall(GameObject hexToExclude, GameObject ball, Vector2 prevVel) {
        // Find another element hex to send this ball to
		foreach (Transform child in grid.transform)
		{
			Hex hex = child.gameObject.GetComponent<Hex>();
			if(hex != null && hex.gameObject != hexToExclude && Hex.isElement(hex.getHexType()) && !hex.isDying()) {
				
				//hex.GetComponent<Collider2D>().enabled = false;
				hex.ignoreWormHole = true;
				
				// print("ball: " + ball);
				// print("hex: " + hex);
				
				ball.transform.position = hex.transform.position;

				hex.removeHealth(hex.getHealth());

				//if(prevVel == Vector2.positiveInfinity) {
					ball.GetComponent<Rigidbody2D>().velocity = ball.GetComponent<Ball>().prevVelocity;
				// } else {
				// 	ball.GetComponent<Rigidbody2D>().velocity = prevVel;
				// }


				break;
			}
		}

    }

	public void wormHoleBall(GameObject hexToExclude, GameObject ball) {
		wormHoleBall(hexToExclude, ball, Vector2.positiveInfinity);
	}

	public bool getNextHitSwitchHexFire() {
		return nextHitSwitchHexFire;
	}

	public void setNextHitSwitchHexFire(bool value) {
		if(nextHitSwitchHexFire != value && value == false) {
            TriggerEvent(PowerUps.FireCreateFireElement, PowerUpState.CoolDown, secondsBetweenCreateFireElement);
		}

		nextHitSwitchHexFire = value;
	}

	public bool getNextHitSwitchHexGrowth() {
		return nextHitSwitchHexGrowth;
	}

	public void setNextHitSwitchHexGrowth(bool value) {
		if(nextHitSwitchHexFire != value && value == false) {
            TriggerEvent(PowerUps.FireCreateFireElement, PowerUpState.CoolDown, secondsBetweenCreateGrowthElement);
		}

		nextHitSwitchHexGrowth = value;
	}

	private void earthQuake() {
		foreach (Transform child in gridInstance.transform)
		{
			Hex hex = child.GetComponent<Hex>();
			if(hex != null) {
				if(hex.getHexType() == DataTypes.HexType.Earth) {
					hex.removeHealth(hex.getHealth());
				}
			}
		}
		TriggerEvent(PowerUps.EarthQuake, PowerUpState.CoolDown, secondsBetweenEarthQuake);
	}

	public void checkQuakingHit() {
		if(lastQuakingHit + hitsBetweenQuakingHit <= ballHitCount) {
			TriggerEvent(PowerUps.EarthQuakingHit, PowerUpState.Active, 0);
		} else {
			TriggerEvent(PowerUps.EarthQuakingHit, PowerUpState.CoolDown, lastQuakingHit + hitsBetweenQuakingHit - ballHitCount);
		}
	}

	public bool QuakingHitAvailable() {
		return lastQuakingHit + hitsBetweenQuakingHit < ballHitCount;
	}

	public void quakingHit(bool succeeded) {
		if(succeeded) {
			lastQuakingHit = ballHitCount;
			TriggerEvent(PowerUps.EarthQuakingHit, PowerUpState.CoolDown, lastQuakingHit + hitsBetweenQuakingHit - ballHitCount);
		}
	}

	//Hex waterElementPickingHex;
	WaterDeathEffect waterDeathEffect;
	Ball waterElementPickingBall;
	float waterElementAngle;

	public void pickWaterElementDirection(GameObject ballInstance, WaterDeathEffect effectToCallBack) {//  Hex hexToCallBack) {
		setTimeScale(0);
		pickingWaterElementDirection = true;
		waterElementPickingBall = ballInstance.GetComponent<Ball>();
		waterElementPickingBall.geyserDirection.SetActive(true);
		// waterElementPickingHex = hexToCallBack;
		waterDeathEffect = effectToCallBack;
	}

	public void finalizeWaterElementDirection() {
		if(waterDeathEffect != null) {
			waterDeathEffect.setWaterSpinAngle(waterElementAngle);
		}
		if(waterElementPickingBall != null) {
			waterElementPickingBall.geyserDirection.SetActive(false);
		}
		goToPrevTimeScale();
		pickingWaterElementDirection = false;
	}

	private void sprayPoison() {
		for (int p = 0; p < paddles.Count; p++) {
			for (int i = 0; i < poisonSprayNum; i++)
			{
				GameObject poisonSprayInstance = Instantiate(poisonSprayPrefab, paddles[p].transform);
				poisonSprayInstance.transform.parent = null;
			}
		}

		lastPoisonSpray = Time.time;

		TriggerEvent(PowerUps.PoisonSpray, PowerUpState.CoolDown, secondsBetweenPoisonSpray);
	}

	public void setBallStuckToPaddle() {
		ballStuckToPaddle = true;
	}

	public int getScore() {
		return score; 
	}

	public Dictionary<DataTypes.BiomeType, int> getEssence() {
		return essenceCollected;
	}

	public float getTime() {
		return timeUsed;
	}

	public DataTypes.BiomeType GetBiomeType() {
		return levelData.biomeType;
	}

	public int GetLevelId() {
		return levelData.levelId;
	}

	public int getBallHitCount() {
		return ballHitCount;
	}

	public IEnumerator showDragTip() {
		// Don't show tip if player has beaten a level
		if(MasterPlayerData.instance.getCoins() > 0) {
			yield break;
		}

		int i = 0;
		int lastShowI = 0;
		float checkTime = 0.05f;

		while(score < 3000) {
			yield return new WaitForSeconds(checkTime);

			i++;

			if(curTouches.Count > 0) {
				lastShowI = i;
			} else if((i - lastShowI) * checkTime > 5 && dragTipInstance == null) {
				lastShowI = i;
				dragTipInstance = Instantiate(dragTipPrefab, WorldCanvasPanel.transform);//WinPanel.transform.parent);	
			}
			
		}
	}

	public void increaseCurrentCombo() {
		increaseCurrentCombo(additionalComboForXHits);
	}

	private void increaseCurrentCombo(float increase) {
		setCurrentCombo(increase + currentCombo);
	}

	private void setCurrentCombo(float combo) {
		currentCombo = Mathf.Min(combo, maxCombo);

		if(poisonHexPowerActive) {
			if(ComboText != null) {
				ComboText.text = "Combo: " + currentCombo.ToString("0.#") + " x2";
			}
		} else {
			if(ComboText != null) {
				ComboText.text = "Combo: " + currentCombo.ToString("0.#");
			}
		}
	}

	IEnumerator startHexPower() {
		poisonHexPowerActive = true;
		TriggerEvent(PowerUps.PoisonMaxCombo, PowerUpState.Active, secondsHexPowerLasts);
		yield return new WaitForSeconds(secondsHexPowerLasts);
		poisonHexPowerActive = false;
		lastHexPower = Time.time;
		TriggerEvent(PowerUps.PoisonMaxCombo, PowerUpState.CoolDown, 0);
	}

	private IEnumerator activateMagPaddle() {
		// print("turning on magnetic paddle");
		lastMagPaddle = Time.time;
		magPaddleAvailable = true;
		magPaddle = true;
		TriggerEvent(PowerUps.EarthMagneticPaddle, PowerUpState.Active, secondsMagPaddleLasts);
		yield return new WaitForSeconds(secondsMagPaddleLasts);
		magPaddleAvailable = false;
		magPaddle = false;
		// print("turning off magnetic paddle");
		TriggerEvent(PowerUps.EarthMagneticPaddle, PowerUpState.CoolDown, 10);
	}

	// private void switchMagPaddle(bool state) {
	// 	if(magPaddle != state) {
	// 		magPaddle = state;
	// 		if(magPaddleAvailable && magPaddle) {
	// 			TriggerEvent(PowerUps.EarthMagneticPaddle, PowerUpState.Active, -1);
	// 		} else if(magPaddleAvailable) {
	// 			TriggerEvent(PowerUps.EarthMagneticPaddle, PowerUpState.Available, -1);
	// 		}
	// 	}
	// }

	private IEnumerator activateBallControl() {
		// print("Turning on ball control");
		lastBallControl = Time.time;
		ballControlAvailable = true;
		ballControl = true;
		TriggerEvent(PowerUps.WaterControlBall, PowerUpState.Active, secondsBallControlLasts);
		yield return new WaitForSeconds(secondsBallControlLasts);
		ballControlAvailable = false;
		ballControl = false;
		// print("turning off ball control");
		TriggerEvent(PowerUps.WaterControlBall, PowerUpState.CoolDown, 10);
	}

	private void switchBallControl(bool state) {
		if(ballControl != state) {
			ballControl = state;
			if(ballControlAvailable && ballControl) {
				TriggerEvent(PowerUps.WaterControlBall, PowerUpState.Active, -1);	
			} else if(ballControlAvailable) {
				TriggerEvent(PowerUps.WaterControlBall, PowerUpState.Available, -1);	
			}
		}
	}

	private bool checkShootFireBall() {
		return lastShootFireBall + secondsBetweenShootFireBall < Time.time;
	}

	private bool checkCreateFireElement() {
		return lastCreateFireElement + secondsBetweenCreateFireElement < Time.time;
	}

	private bool checkEarthQuake() {
		return lastEarthQuake + secondsBetweenEarthQuake < Time.time;
	}

	private bool checkMagPaddle() {
		return lastMagPaddle + secondsMagPaddleLasts + secondsBetweenMagPaddle < Time.time;
	}

	private bool checkControlBall() {
		return lastBallControl + secondsBallControlLasts + secondsBetweenBallControl < Time.time;
	}

	private bool checkCreateGrowthElement() {
		return lastCreateGrowthElement + secondsBetweenCreateGrowthElement < Time.time;
	}

	private bool checkSlowTime() {
		return lastSlowTime + secondsSlowTimeLasts + secondsBetweenSlowTime < Time.time;
	}

	private bool checkHexPower() {
		return lastHexPower + secondsHexPowerLasts < Time.time;
	}

	private bool checkMultipleBalls() {
		return lastMultipleBalls + secondsBetweenMultipleBalls < Time.time;
	}

	private bool checkPoisonSpray() {
		return lastPoisonSpray + secondsBetweenPoisonSpray < Time.time;
	}


	private float getTimeBetween(PowerUps power) {
		switch (power)
		{
			case PowerUps.FireMassiveFireball:
				return secondsBetweenShootFireBall;
			case PowerUps.FireCreateFireElement:
				return secondsBetweenCreateFireElement;
			case PowerUps.EarthMagneticPaddle:
				return secondsBetweenMagPaddle;
			case PowerUps.EarthQuake:
				return secondsBetweenEarthQuake;
			case PowerUps.GrowthCreateGrowthElement:
				return secondsBetweenCreateGrowthElement;
			case PowerUps.PoisonMaxCombo:
				return secondsBetweenHexPower;
			case PowerUps.PoisonSpray:
				return secondsBetweenPoisonSpray;
			case PowerUps.ShadowMultipleBalls:
				return secondsBetweenMultipleBalls;
			case PowerUps.ShadowSlowTime:
				return secondsBetweenSlowTime;
			case PowerUps.WaterControlBall:
				return secondsBetweenBallControl;
			default:
				return 10;
		}
	}



	void OnDestroy() {
		BouncePredictor.RemoveAll();
	}

	// DoTween sequences

	private IEnumerator setInitLives(Text intText, int num) {
		int value = 0;
		while(value < num) {
			value++;
			Text it = intText;
			Sequence upLife = DOTween.Sequence();
			upLife.Append(it.DOColor(new Color(0.584f, 0.8509f, 0.5725f), 0.2f))
				.Join(it.transform.DOScale(Vector3.one * 1.5f, 0.2f))
				.Append(it.DOText("x" + value.ToString(), 0f))
				.Append(it.DOColor(Color.white, 0.01f))
				.Join(it.transform.DOScale(Vector3.one, 0.01f))
				.Join(it.transform.DOPunchPosition(Vector3.up * 30f, 0.15f));
			yield return new WaitForSeconds(0.55f);
		}
	}

	private void updateLifeText(Text intText, int value) {
		Text it = intText;
		Sequence redLife = DOTween.Sequence();
		redLife.Append(it.DOColor(new Color(0.867f, 0.4178f, 0.4178f), 0.2f))
			.Join(it.transform.DOScale(Vector3.one * 1.5f, 0.4f))
			.Append(it.DOText("x" + value.ToString(), 0f))
			.Append(it.DOColor(Color.white, 0.01f))
			.Join(it.transform.DOScale(Vector3.one, 0.01f))
			.Join(it.transform.DOPunchRotation(Vector3.forward * 30f, 0.3f));
		
	}

	Sequence popText;

	private void updateScoreText(Text intText, int value) {

		if(value != lastScore) {
			if(popText == null) {
				popText = DOTween.Sequence();
			} else {
				intText.text = "Score: " + value.ToString();
				popText.Kill();
				// intText.text = "reset";
				intText.color = Color.white;
				intText.transform.localScale = Vector3.one;

				popText = DOTween.Sequence();
			}

			lastScore = value;

			popText.Append(intText.DOColor(new Color(0.584f, 0.8509f, 0.5725f), 0.2f))
				.Join(intText.transform.DOScale(Vector3.one * 1.25f, 0.2f))
				.Append(intText.DOText("Score: " + value.ToString(), 0f))
				.Append(intText.DOColor(Color.white, 0.01f))
				.Join(intText.transform.DOScale(Vector3.one, 0.01f));
		}
	}

	// Power-up events

	Dictionary<string, UnityEvent<PowerUps, PowerUpState, float>> eventDictionary = new Dictionary<string, UnityEvent<PowerUps, PowerUpState, float>>();


	public static void StartListening(string eventName, UnityAction<PowerUps, PowerUpState, float> listener) {
        UnityEvent<PowerUps, PowerUpState, float> thisEvent = null;

        if(instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.AddListener(listener);
        } else {
            thisEvent = new PowerUpEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<PowerUps, PowerUpState, float> listener) {
        // if(eventManager == null) return;

        UnityEvent<PowerUps, PowerUpState, float> thisEvent = null;

        if(instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(PowerUps powerup, PowerUpState state, float progress) {
        UnityEvent<PowerUps, PowerUpState, float> thisEvent = null;

        if(instance.eventDictionary.TryGetValue(powerup.ToString(), out thisEvent)) {
            thisEvent.Invoke(powerup, state, progress);
        }
    }

    [System.Serializable]
	public class PowerUpEvent : UnityEvent<PowerUps, PowerUpState, float> {}

	public UnityEvent HexAutoEndEvent;
	public UnityEvent GameFailedEvent;
}
