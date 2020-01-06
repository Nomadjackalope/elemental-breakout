using UnityEngine;
using UnityEngine.UI;

public class TransitionIcon : MonoBehaviour {

    public Image background;

    public void setBackground(DataTypes.BiomeType biome) {
        background.sprite = GetComponent<Background>().getBackground(biome);
    }

}