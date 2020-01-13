using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class HexBuilder : MonoBehaviour {

    public List<HexBuilderData> builderData;

    public Sprite defaultHexSprite;
    public SpriteRenderer healthSprite;

    public Material crystal, basic;

    public bool needsUpdate = true;
    public bool overrideColor = false;

    public Sprite hR1_Outer;
    public Sprite hR2_Outer;
    public Sprite hR3_Outer;
    public Sprite hR4_Outer;

    public Sprite hR1_Inner;
    public Sprite hR2_Inner;
    public Sprite hR3_Inner;
    public Sprite hR4_Inner;

    private void Awake()
    {
        AssignAttributes();
    }
    private void Update()
    {
        if(needsUpdate) {
            AssignAttributes();
        }

    }

    private void AssignAttributes()
    {
        SpriteRenderer spriteR = gameObject.GetComponent<SpriteRenderer>();
        Hex hex = gameObject.GetComponent<Hex>();
        DataTypes.HexType hexType = gameObject.GetComponent<Hex>().getHexType();

        HexBuilderData hexBuilderData = GetHexBuilderDataFrom(hexType);

        // Change this to change sprite based on health
        if(hexBuilderData != null) {
            spriteR.sprite = hexBuilderData.sprite;

            if(DataTypes.IsBiome(hex.getHexType())) {
                
                //Trying new artstyle
                healthSprite.sprite = getHexRedoneInnerSprite(hex.getHealth());
                spriteR.sprite = getHexRedoneOuterSprite(hex.getHealth());

                healthSprite.color = DataTypes.GetAccentColorFrom(DataTypes.GetBiomeFrom(hex.getHexType()));

                if(!overrideColor) {
                    spriteR.color = DataTypes.GetBackgroundColorFrom(DataTypes.GetBiomeFrom(hex.getHexType()));
                }
                
                healthSprite.enabled = true;
                spriteR.material = basic;

                if(DataTypes.GetBiomeFrom(hex.getHexType()) == DataTypes.BiomeType.Fire) {
                    hex.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Colors.FireShadowColor;
                } else if(DataTypes.GetBiomeFrom(hex.getHexType()) == DataTypes.BiomeType.Poison) {
                    hex.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Colors.PoisonShadowColor;
                }
            } else if(hex.getHexType() == DataTypes.HexType.HighPointGrowth) {
                spriteR.color = DataTypes.GetAccentColorFrom(DataTypes.BiomeType.Growth);
                spriteR.material = crystal;
            } else {
                healthSprite.enabled = false;
                spriteR.material = crystal;
            }

            if(hexBuilderData.pointValue > -1) {
                hex.setPointValue(hexBuilderData.pointValue);
            }

            if(hexBuilderData.onBreak != null) {
                hex.onBreak = hexBuilderData.onBreak;
            }

        } else {
            spriteR.sprite = defaultHexSprite;
        }

        needsUpdate = false;
    }

    HexBuilderData GetHexBuilderDataFrom(DataTypes.HexType hexType) {
        return builderData.Find(x => x.hexType == hexType);
    }

    Sprite getHexRedoneInnerSprite(int health) {
        if(health == 1) {
            return hR1_Inner;
        } else if(health == 2) {
            return hR2_Inner;
        } else if(health == 3) {
            return hR3_Inner;
        } else if(health == 4) {
            return hR4_Inner;
        } else if(health > 4) {
            return hR4_Inner;
        } else {
            return null;
        }
    }

    Sprite getHexRedoneOuterSprite(int health) {
        if(health == 1) {
            return hR1_Outer;
        } else if(health == 2) {
            return hR2_Outer;
        } else if(health == 3) {
            return hR3_Outer;
        } else if(health == 4) {
            return hR4_Outer;
        } else if(health > 4) {
            return hR4_Outer;
        } else {
            return null;
        }
    }
}
