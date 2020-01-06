using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

public class SceneManageLevelSelect : MonoBehaviour {



	DataTypes.HexType biome;

	void Awake() {
		#if UNITY_EDITOR
		if(MasterManager.instance == null) {
			SceneManager.LoadScene("Master", LoadSceneMode.Additive);
		}
		#endif
	}

	// Use this for initialization
	void Start () {
		biome = DataTypes.HexType.Fire;

	
		#if UNITY_EDITOR
		MasterManager.instance.SetCurrentSceneDEBUG("LevelSelect");
		#endif
	}

	void changeBiome(DataTypes.HexType biome) {
		this.biome = biome;
	}

	// When Biome is changed this is called
	private void fillLevels() {
		// Create level list
		switch (biome)
		{
			case DataTypes.HexType.Earth:
				// Get levels list from where it is saved

				break;
			default:
				break;
		}

		// Lock the ones that aren't accessible

	}

	void OnFireBiomeClicked() { changeBiome(DataTypes.HexType.Fire); }

	public void OnBackButtonClicked() {
		MasterManager.instance.Back();
	}

	public void OnMainMenuClicked() {
		MasterManager.instance.SwitchSceneTo("MainMenu");
	}

}