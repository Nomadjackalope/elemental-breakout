using UnityEngine;

public class HexDeathAnimation : MonoBehaviour
{
    private static string MAIN_TEXTURE = "_MainTex";

    public MaterialPropertyBlock block;
    public SpriteRenderer _renderer;
    //public Texture2D mainTex;
    
    public Sprite normalTex;
    private Texture2D lastNormalTex;

    public Sprite colorTex;

    public HexData hexData;
    public Material crystal;

    public RuntimeAnimatorController largeAnimatorController;

    void Start() {
        if(getBiome()) {
            return;
        }

        transform.parent.GetComponent<Animator>().enabled = false;//.runtimeAnimatorController = largeAnimatorController;


        block = new MaterialPropertyBlock();
        _renderer = GetComponentInParent<SpriteRenderer>();
        transform.parent.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

        _renderer.material = crystal;

        //mainTex = GetSpriteTexture();
        //block.SetTexture(MAIN_TEXTURE, mainTex);
        // block.SetColor("_Color", Color.white);
        // block.SetTexture("_PassthroughTex", normalTex.texture);
        // _renderer.SetPropertyBlock(block);
    }   

    void Update() {
        if(getBiome()) {
            return;
        }

        _renderer.sprite = colorTex;     

        if(normalTex == null) return;

        //if(lastNormalTex != normalTex) {
            _renderer.GetPropertyBlock(block);
            // block.SetColor("_Color", Color.green);
            // block.SetTexture(MAIN_TEXTURE, mainTex);
            block.SetTexture("_PassthroughTex", normalTex.texture);
            _renderer.SetPropertyBlock(block);
            lastNormalTex = normalTex.texture;

        //}
    }

    // private Texture2D GetSpriteTexture () {
    //     // Get main texture trough MaterialPropertyBlock from sprite renderer
    //     MaterialPropertyBlock getBlock = new MaterialPropertyBlock ();
    //     _renderer.GetPropertyBlock (getBlock);
    //     return (Texture2D)getBlock.GetTexture (MAIN_TEXTURE);
    // }

    public void setHexData(HexData hexData) {
        this.hexData = hexData;
    }

    bool getBiome() {
        if(DataTypes.IsBiome(hexData.hexType)) {
            enabled = false;
            //DestroyImmediate(this.gameObject);
            return true;
        }

        return false;
    }
}