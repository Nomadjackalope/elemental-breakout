using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class DataController : MonoBehaviour {

    private List<LevelData> allLevelData;
    private string gameDataFileName = "gameData.json";
	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(this.gameObject);
        LoadGameData();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void LoadGameData()
    {
        //DOES Streaming Asset folder work on Android? Do you we need to use persistant Data path?
        string filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);
        if (File.Exists(filePath))
        {
            Debug.LogWarning("We found File!");
            string jsonDataString = File.ReadAllText(filePath);
            GameData loadedData = JsonUtility.FromJson<GameData>(jsonDataString);
            allLevelData = loadedData.allLevelData;
        }
        else
        {
            Debug.LogError("Could not locate GameData");
        }
    }
}
