using UnityEngine;

public class Branch : MonoBehaviour {

    public DataTypes.BiomeType biome;

    void Update() {
        if(MasterPlayerData.instance.branchEnabled(biome) || MasterPlayerData.instance.branchesAvailableOnActivePaddle() <= 0) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
        }
    }
    
    public void enableBranch() {
        SkillTree.instance.enableBranch(biome);
    }
}