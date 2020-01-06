using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LevelLoader : MonoBehaviour {

	private static string gameDataProjectLocation = "/gameData.json";
	GameData gameData;

	public static LevelLoader instance;

    void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		LoadGameData();
	}

	private static string getAppDataPath() {
		#if UNITY_EDITOR
			return Application.dataPath + "/StreamingAssets";

		#elif UNITY_IOS
		 	return Application.dataPath + "/Raw";

		#elif UNITY_ANDROID
			return Application.streamingAssetsPath; 

        #else
            return Application.dataPath;
			
		#endif
	}

	private void LoadGameData() {
		if (Application.platform == RuntimePlatform.Android)
		{
			// WWW reader = new WWW(getAppDataPath() + gameDataProjectLocation);
			// IEnumerator enumerator = GetRequest(getAppDataPath() + gameDataProjectLocation);
			// enumerator.
			// Coroutine loading = StartCoroutine(GetRequest(getAppDataPath() + gameDataProjectLocation));
			// while (loading.)
			// {
				
			// }
			GetRequest(getAppDataPath() + gameDataProjectLocation);
			// while (!reader.isDone) { }

			// string jsonDataString = reader.text;
			//gameData = JsonUtility.FromJson<GameData>(jsonDataString);
		}
		else
		{
			string filePath = getAppDataPath() + gameDataProjectLocation;
			if (File.Exists(filePath))
			{
				string jsonDataString = File.ReadAllText(filePath);
				gameData = JsonUtility.FromJson<GameData>(jsonDataString);
			}
			else
			{
				Debug.LogWarning("No Game Data Found, Cannot Load");
				// fail gracefully here
			};
		}

		
	}


	public LevelData LoadLevelData(int levelId, DataTypes.BiomeType biome)
    {
		if(levelId == -1) {
			levelId = 1;
		}
		// Load in the gameData if we haven't before
		if(gameData == null) {
			LoadGameData();
		}

        //Get Data for selected level ID
        LevelData levelData = gameData.allLevelData.Find(x => x.levelId == levelId && x.biomeType == biome);
        return copyLevelData(levelData);
    }

	LevelData copyLevelData(LevelData levelData) {
		LevelData returnable = new LevelData();

		returnable.levelId = levelData.levelId;
		returnable.levelName = levelData.levelName;
		returnable.biomeType = levelData.biomeType;
		returnable.gridCellSize = new Vector3(levelData.gridCellSize.x, levelData.gridCellSize.y, levelData.gridCellSize.z);
		returnable.gridCellPosition = new Vector3(levelData.gridCellPosition.x, levelData.gridCellPosition.y, levelData.gridCellPosition.z);
		returnable.pointMultiplier = levelData.pointMultiplier;
		returnable.isBossLevel = levelData.isBossLevel;
		returnable.movementType = levelData.movementType;
		
		foreach (HexData hexData in levelData.hexData)
		{
			returnable.hexData.Add(copyHexData(hexData));
		}

		return returnable;
	}

	HexData copyHexData(HexData hexData) {
		HexData returnable = new HexData();

		returnable.hexType = hexData.hexType;
		returnable.health = hexData.health;
		returnable.points = hexData.points;

		returnable.hexScaleFactor = new Vector2(hexData.hexScaleFactor.x, hexData.hexScaleFactor.y);
		returnable.cellLocation = new Vector3Int(hexData.cellLocation.x, hexData.cellLocation.y, hexData.cellLocation.z);

		return returnable;
	}

	public List<int> getLevelList(DataTypes.BiomeType biome) {
		if(gameData == null) {
			LoadGameData();
		}

		List<int> levelIds = new List<int>();
		foreach (LevelData data in gameData.allLevelData)
		{
			if(data.biomeType == biome) {
				levelIds.Add(data.levelId);
			}
		}

		levelIds.Sort();

		return levelIds;
	}

	// void newRequest(string uri) {
	// 	StartCoroutine(GetRequest(uri));
	// }

	// IEnumerator GetRequest(string uri) {
	// 	using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
    //     {
    //         // Request and wait for the desired page.
    //         yield return webRequest.SendWebRequest();

    //         string[] pages = uri.Split('/');
    //         int page = pages.Length - 1;

    //         if (webRequest.isNetworkError)
    //         {
    //             Debug.Log(pages[page] + ": Error: " + webRequest.error);
    //         }
    //         else
    //         {
    //             gameData = JsonUtility.FromJson<GameData>(webRequest.downloadHandler.text);
	// 			Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
    //         }
    //     }
	// }

	// IEnumerator SendWebRequest(UnityWebRequest webRequest) {
	// 	yield return webRequest.SendWebRequest();
	// }

	// Returns true on success
	bool GetRequest(string uri) {
		UnityWebRequest webRequest = UnityWebRequest.Get(uri);

		webRequest.SendWebRequest();

		float timeToFinish = Time.realtimeSinceStartup + 5;

		// stall until the webrequest is done or 5 seconds have elapsed
		while(!webRequest.isDone) {
			if(timeToFinish - Time.realtimeSinceStartup < 0) {
				throw new FileLoadException("LevelLoader: Couldn't load Game Data in 5 seconds");
			}
		}

		string[] pages = uri.Split('/');
		int page = pages.Length - 1;

		if (webRequest.isNetworkError)
		{
			//Debug.Log(pages[page] + ": Error: " + webRequest.error);
			throw new FileLoadException("LevelLoader: webRequest network error");
		}
		else
		{
			gameData = JsonUtility.FromJson<GameData>(webRequest.downloadHandler.text);
			//Debug.Log("webRequest.text: " + webRequest.downloadHandler.text);
		}

		return true;

	}

	
}