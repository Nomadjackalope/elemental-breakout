using UnityEngine;
using UnityEngine.UI;

public class BiomeConfirmation : MonoBehaviour {
    
    private string biomeTypeText;

    void OnEnable() {
        int branchesLeft = MasterPlayerData.instance.branchesAvailableOnActivePaddle();
        print(branchesLeft);

        GetComponent<Text>().text = "This will permanently apply " + biomeTypeText + " as your " + (branchesLeft == 2 ? "first" : "second") + " of two biomes.";
     
        transform.GetChild(0).gameObject.SetActive(branchesLeft == 1);
 
        //  "Only two biomes are allowed per paddle. \n" 
        //     + ( branchesLeft == 2 ? "This is your first." : "This is your second.");
    }

    public void SetBiomeType(DataTypes.BiomeType biomeType) {
        biomeTypeText = biomeType.ToString();
    }
}