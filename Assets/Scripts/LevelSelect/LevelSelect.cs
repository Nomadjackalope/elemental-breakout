using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{

    public List<GameObject> levelsToHide = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {   
        if(MasterPlayerData.instance.getNumLevelsBeat() == 0) {
            hideLevels();
        } else {
            Elementalist.instance.sayEasyHappyIfAvailable(ConvoIds.Biomes);
        }

    }

    void hideLevels() {
        foreach (GameObject level in levelsToHide)
        {
            level.SetActive(false);
        }
    }
}
