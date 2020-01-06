#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

//Test Class for writing data into the Assets/streamingAssets/gameData.json file 
public class GameDataEditor :EditorWindow
{
    private string gameDataProjectLocation = "/StreamingAssets/gameData.json";
    private string scene_path = "Assets/Scenes/LevelEditor.unity";
    private string levelCanvas_path = "Assets/Prefabs/LevelCanvas.prefab";
    private string hexPrefab_path = "Assets/Prefabs/hex.prefab";
    public GameData gameData;
    //Object levelCanvas;
    int loadLevel_ind = 0;
    public string levelID = "0";
    public string levelName = "Level_X";
    public string[] savedLevels = null;
    private int biomeIndex = 0;
    private int loadBiome_ind = 0;
    public DataTypes.BiomeType biomeType = DataTypes.BiomeType.Earth;
    public float levelPointMultiplier;
    public bool bossLevel;
    public BossMovement movementType;

    [MenuItem ("Window/Game Data Editor")]
    static void Init()
    {
        GameDataEditor window = (GameDataEditor)EditorWindow.GetWindow(typeof(GameDataEditor));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUI.color = new Color(0.9f, 0.5f, 0.5f);
        if (GUILayout.Button("CreateNewLevel"))
        {
            //Just Clears Design Space, new level isn't created till the Save button pressed
            ClearLevelWorkSpace();
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUI.color = new Color(0.3f, 0.8f, 0.6f);
        GUILayout.Label("Level Attributes------------------------------");
        levelID = EditorGUILayout.TextField("LevelID:",levelID);
        levelName = EditorGUILayout.TextField("LevelName:", levelName);
        biomeIndex = EditorGUILayout.Popup("Level Biome: ", biomeIndex, System.Enum.GetNames(typeof(DataTypes.BiomeType)), GUILayout.Width(250));
        //levelCanvas = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(levelCanvas_path, typeof(GameObject))) as GameObject;
        //levelCanvas = EditorGUILayout.ObjectField("LevelCanvas", levelCanvas,typeof(GameObject),true);
        levelPointMultiplier = EditorGUILayout.FloatField("Level Point Multiplier: ", levelPointMultiplier);
        bossLevel = EditorGUILayout.Toggle("Is boss level: ", bossLevel);
        if(bossLevel) {
            movementType = (BossMovement) EditorGUILayout.EnumPopup("Boss Movement: ", movementType);
        }
        if (GUILayout.Button("Save Level"))
        {
            SaveGameData();
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        GUI.color = new Color(0.3f, 0.4f, 0.6f);
        try
        {
            savedLevels = (gameData.allLevelData).Select(x => x.levelId.ToString()).ToArray();
        }
        catch
        {
            //DO Nothing
        }
        if (gameData != null && savedLevels != null)
        {
            GUILayout.Label("LOAD EXISTING LEVEL-------------------------");
            loadBiome_ind = EditorGUILayout.Popup("Load Biome: ", loadBiome_ind, System.Enum.GetNames(typeof(DataTypes.BiomeType)), GUILayout.Width(250));
            savedLevels =(gameData.allLevelData).Where(x => x.biomeType == (DataTypes.BiomeType)loadBiome_ind).Select(x => x.levelId.ToString()).ToArray();
            loadLevel_ind = EditorGUILayout.Popup("Level ID: ", loadLevel_ind,savedLevels,GUILayout.Width(250));
            if (GUILayout.Button("Load Level"))
            {
                LoadGameData();
            }

        }
        GUILayout.EndVertical();

        GameObject hGrid = GameObject.FindGameObjectWithTag("HexGrid");
        if(hGrid != null) {
            GUILayout.Label("LEVEL STATS ---------------------------");
            GUILayout.Label("Blocks in level: " + hGrid.transform.childCount);
        }

    }

    private void ClearLevelWorkSpace()
    {
        //Load Level Editor Scene
        EditorSceneManager.OpenScene(scene_path);
        //Remove everything but the camera
        foreach (GameObject obj in SceneManager.GetSceneByPath(scene_path).GetRootGameObjects())
        {
            if ((obj.GetComponent("Camera") as Camera)==null){
                DestroyImmediate(obj);
            }
        }
        //Add back the Level Canvas Prefab
        //GameObject.Instantiate(levelCanvas);
        PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(levelCanvas_path, typeof(GameObject)) as GameObject);

    }

    private void SaveGameData()
    {
        //EditorSceneManager.OpenScene(scene_path);
        //Check if LevelID can be converted to an Int
        int level =0;
        if(int.TryParse(levelID,out level)){
            //Check if Level ID is already in use, if so ask user to override
            if (gameData != null)
            {
                if (((gameData.allLevelData).Where(x=>x.biomeType == (DataTypes.BiomeType)biomeIndex).Select(x => x.levelId).ToList()).Contains(level))
                {
                    if (EditorUtility.DisplayDialog("A level with ID: " + level.ToString() + " And Biome Type " + (DataTypes.BiomeType)biomeIndex + " Already Exists!\n", "Do you want to override this level?", "Yes", "Cancel"))
                    {
                        //Remove the level with same ID
                        (gameData.allLevelData).RemoveAll(x => x.levelId == level && x.biomeType == (DataTypes.BiomeType)biomeIndex);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
                gameData = new GameData();
            //SAVE LEVEL DATA
            LevelData levelData = Save2LevelData(level);
            if (levelData != null)
            {
                gameData.allLevelData.Add(levelData);
            }
        }
       
        //SAVE GAME DATA From Editor into .JSON
        string jsonDataString = JsonUtility.ToJson(gameData);
        string filePath = Application.dataPath + gameDataProjectLocation;
        File.WriteAllText(filePath, jsonDataString);
    }

    private LevelData Save2LevelData(int level)
    {
        LevelData levelData = null;
        GameObject hGrid = GameObject.FindGameObjectWithTag("HexGrid"); //THINK THE ERROR IS HERE ...USE TAGS AND SEARch ScENE...we don't want the HEXGRID script, we want the transform
        if (hGrid!= null)
        {
            Debug.Log("Grid Found!");
            levelData = new LevelData() { 
                levelId = level,
                levelName = levelName, 
                biomeType = (DataTypes.BiomeType)biomeIndex ,
                gridCellSize = (hGrid.GetComponent("Grid") as Grid).cellSize, 
                gridCellPosition = hGrid.transform.position, 
                pointMultiplier = levelPointMultiplier,
                isBossLevel = bossLevel,
                movementType = this.movementType };
            int childCount = hGrid.transform.childCount;
            Debug.Log("childCount" + childCount);
            for (int i = 0; i < childCount; i++)
            {
                Transform child = hGrid.transform.GetChild(i);
                if (child.gameObject.tag == "Hex")
                {
                    Hex hex = child.gameObject.GetComponent<Hex>();
                    levelData.hexData.Add(hex.getHexData());
                }
            }
        }
        else
        {
            EditorUtility.DisplayDialog("No HexGrid", "There is No HexGrid in the LevelEditor Scene - Cannot save level\nClick OK to Close window", "OK");
        }
        return levelData;
    }

    private void LoadGameData()
    {
        //LOAD GAME DATA FROM .JSON into Editor so we can modify
        string filePath = Application.dataPath + gameDataProjectLocation;
        if (File.Exists(filePath))
        {
            string jsonDataString = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(jsonDataString);
        }
        else
        {
            Debug.LogWarning("No Game Data Found, Cannot Load");
            //gameData = new GameData();
            
        };
        //Get Data for selected level ID
        int loadLevel = int.Parse(savedLevels[loadLevel_ind]);
        LevelData levelData = ((gameData.allLevelData).FirstOrDefault(x => x.levelId == loadLevel && x.biomeType == (DataTypes.BiomeType)loadBiome_ind));
        if ((gameData.allLevelData).Where(x => x.levelId == loadLevel && x.biomeType == (DataTypes.BiomeType)loadBiome_ind).ToList().Count() == 0)
        {
            EditorUtility.DisplayDialog("No Level Data for selected Biome/Level", "There is no data saved for level "+ loadLevel+ " in Biome "+ (DataTypes.BiomeType)loadBiome_ind + "\nClick OK to Close window", "OK");
        }
        levelID = levelData.levelId.ToString();
        levelName = levelData.levelName;
        biomeIndex = (int)levelData.biomeType;
        levelPointMultiplier = levelData.pointMultiplier;
        bossLevel = levelData.isBossLevel;
        movementType = levelData.movementType;
        //Debug.Log(levelData.levelId);
        BuildLoadedLevel(levelData);
    }

    private void BuildLoadedLevel(LevelData levelData)
    {
        ClearLevelWorkSpace();
        HexGrid hGrid = GameObject.FindGameObjectWithTag("HexGrid").GetComponent<HexGrid>();
        Grid grid = hGrid.GetComponent<Grid>();
        //Set hGrid cellSize and position
        hGrid.transform.position = levelData.gridCellPosition;
        hGrid.gridCellSize = levelData.gridCellSize;
        hGrid.UpdateGridSize();
        //Draw all Hex Objects
        foreach(HexData hexData in levelData.hexData)
        {
            GameObject hexObj =PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath(hexPrefab_path,typeof(GameObject))) as GameObject;
            Hex hex = hexObj.GetComponent<Hex>();
            
            hex.setHexData(hexData);
            hexObj.transform.SetParent(hGrid.transform);
            Debug.Log("Cell Location: " + hexData.cellLocation);
            Debug.Log(hexData.cellLocation);
            Debug.Log(hex.getHexData().cellLocation);
            hexObj.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated(hexData.cellLocation));
            hexObj.transform.localScale = new Vector3(hexData.hexScaleFactor.x * grid.cellSize.x, hexData.hexScaleFactor.y * grid.cellSize.y, 0);
        }

    }

}
#endif