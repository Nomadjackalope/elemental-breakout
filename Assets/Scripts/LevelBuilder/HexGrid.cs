using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    public GameObject hexPrefab;
    public Vector3 gridCellSize =new Vector3(1f, 1f, 0f);
    protected Grid grid;
    List<Hex> hexScripts;

    public void UpdateGridSize()
    {
        grid = this.GetComponent("Grid") as Grid;
        GetHexAttributes();
        if (gridCellSize != grid.cellSize)
        {
            Debug.Log("Changed Cell size");
            DestroyHexs();
            grid.cellSize = gridCellSize;
            RedrawHexs();
        }
    }


    protected void GetHexAttributes()
    {
        hexScripts = new List<Hex>();
        //Go through each child and check if hex exists in the clicked cell's position
        int childCount = this.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = this.transform.GetChild(i);
            if (child.gameObject.tag == "Hex")
            {
                (child.gameObject.GetComponent("Hex") as Hex).getHexData().cellLocation = Vector3Int.RoundToInt(grid.WorldToLocal(grid.LocalToCell(child.position)));
                hexScripts.Add(child.gameObject.GetComponent("Hex") as Hex);
            }

        }
    }
    
    protected void DestroyHexs(){
        List<GameObject> all_hexes = new List<GameObject>();
        foreach (Transform child in this.transform)
        {
            if (child.gameObject.tag == "Hex"){
                all_hexes.Add(child.gameObject);   
            }
        }
        foreach (GameObject hex in all_hexes){
            DestroyImmediate(hex);
        }
    }

    protected void RedrawHexs()
    {
        //Debug.Log("Draw Hexes");
        int hexCount = hexScripts.Count;
        Debug.Log(hexCount);
        for (int i = 0; i < hexCount; i++)
        {
            #if (UNITY_EDITOR) 
            GameObject gameObject = PrefabUtility.InstantiatePrefab(hexPrefab) as GameObject;
            #else
            Instantiate(hexPrefab);
            #endif
            Hex hex = (gameObject.GetComponent("Hex") as Hex);
            hex.setHexData((hexScripts[i]).getHexData());
            //hex.hexData.hexType = (hexScripts[i]).hexData.hexType;
            //hex.hexData.hexScaleFactor = (hexScripts[i]).hexData.hexScaleFactor;
            //hex.hexData.cellLocation = (hexScripts[i]).hexData.cellLocation;
            gameObject.transform.SetParent(this.transform);
            gameObject.transform.position = grid.LocalToWorld(grid.CellToLocalInterpolated((hexScripts[i]).getHexData().cellLocation));
            gameObject.transform.localScale = new Vector3((hexScripts[i]).getHexData().hexScaleFactor.x*grid.cellSize.x, (hexScripts[i]).getHexData().hexScaleFactor.y * grid.cellSize.y,0);
        }
    }
}
