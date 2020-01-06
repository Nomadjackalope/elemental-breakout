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

    public Sprite h1;
    public Sprite h2;
    public Sprite h3;
    public Sprite h4;

    public Sprite hR1_Outer;
    public Sprite hR2_Outer;
    public Sprite hR3_Outer;
    public Sprite hR4_Outer;

    public Sprite hR1_Inner;
    public Sprite hR2_Inner;
    public Sprite hR3_Inner;
    public Sprite hR4_Inner;

    // public Sprite earthHex;
    // public Sprite fireHex;
    // public Sprite waterHex;
    // public Sprite fireBiomeHex;
    // public Sprite poisonHex;
    // public Sprite growthHex;
    // public Sprite shadowHex;

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
        //Sprite[] terrainHex = Resources.LoadAll<Sprite>("Sprites/TerrainHex");
        //Sprite[] testHex = Resources.LoadAll<Sprite>("Sprites/basicHexSizeTest");
        // Debug.Log("Located some terrain: " + terrainHex.Length);
        DataTypes.HexType hexType = gameObject.GetComponent<Hex>().getHexType();

        HexBuilderData hexBuilderData = GetHexBuilderDataFrom(hexType);

        // Change this to change sprite based on health
        if(hexBuilderData != null) {
            spriteR.sprite = hexBuilderData.sprite;
            // if(hex.getHealth() == 1 && hexBuilderData.spriteHealth1 != null) {
            //     spriteR.sprite = hexBuilderData.spritehealth1;
            // }

            // if(hex.getHealth() == 2 && hexBuilderData.spriteHealth2 != null) {
            //     spriteR.sprite = hexBuilderData.spriteHealth2;
            // }

            // if(hex.getHealth() == 3 && hexBuilderData.spriteHealth3 != null) {
            //     spriteR.sprite = hexBuilderData.spriteHealth3;
            // }

            // if(hex.getHealth() == 4 && hexBuilderData.spriteHealth4 != null) {
            //     spriteR.sprite = hexBuilderData.spriteHealth4;
            // }

            // if(hex.getHealth() == 5 && hexBuilderData.spriteHealth5 != null) {
            //     spriteR.sprite = hexBuilderData.spriteHealth5;
            // }

            if(DataTypes.IsBiome(hex.getHexType())) {
                //healthSprite.sprite = getHealthOverlaySprite(hex.getHealth());
                
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

        

        // switch (hexType)
        // {
        //     case DataTypes.HexType.Growth:
        //         spriteR.sprite = growthHex;
        //         hex.hexData.health = 3;
        //         break;
        //     case DataTypes.HexType.Earth:
        //         // spriteR.sprite = terrainHex[6];
        //         spriteR.sprite = earthHex;
        //         break;
        //     case DataTypes.HexType.Fire:
        //         spriteR.sprite = fireHex;
        //         break;
        //     case DataTypes.HexType.Water:
        //         spriteR.sprite = waterHex;
        //         break;
        //     case DataTypes.HexType.FireBiome:
        //         spriteR.sprite = fireBiomeHex;
        //         break;
        //     case DataTypes.HexType.Poison:
        //         spriteR.sprite = poisonHex;
        //         break;
        //     case DataTypes.HexType.Shadow:
        //         spriteR.sprite = shadowHex;
        //         break;
        //     default:
        //         spriteR.sprite = testHex[0];
        //         break;
        // }

        needsUpdate = false;
    }

    HexBuilderData GetHexBuilderDataFrom(DataTypes.HexType hexType) {
        return builderData.Find(x => x.hexType == hexType);
    }

    Sprite getHealthOverlaySprite(int health) {
        if(health == 1) {
            return h1;
        } else if(health == 2) {
            return h2;
        } else if(health == 3) {
            return h3;
        } else if(health == 4) {
            return h4;
        } else if(health > 4) {
            return h4;
        } else {
            return null;
        }
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
