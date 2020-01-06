#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

[CreateAssetMenu]
[CustomGridBrush(false,true,false, "Prefab Brush / HexBrush")]
public class HexBrush : GridBrushBase
{
    public GameObject hexPrefab; //could we hardcode to the hexprefab but have an option for the type of hex?
    public DataTypes.HexType hexType;
    public bool replaceExisting = false;
    public int zPos = 0;
    public float xScale = 1.0f;
    public float yScale = 1.0f;
    [Range(1,10)]
    public int health = 1;

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        //First Check and make sure the brush Target is a HexGrid
        if((brushTarget.GetComponent("HexGrid") as HexGrid) != null){
            //Debug.Log("Found Hex Grid");
            Vector3Int cellPosition = new Vector3Int(position.x, position.y, zPos);
            Transform cellObject = GetObjectInCell(gridLayout, brushTarget.transform, new Vector3Int(position.x, position.y, position.z));
            if (cellObject!=null && !replaceExisting)
            {
                Debug.LogWarning("Hex Already exists here, will not duplicate in this location"+ position.x + " " + position.y);
                return;
            }
            else
            {
                //If we want to replace currect cells object, delete the exiting one
                if (cellObject != null && replaceExisting)
                    DestroyImmediate(cellObject.gameObject);
                //Create the new cell object using the provided hexPrefab
                GameObject gameObject = PrefabUtility.InstantiatePrefab(hexPrefab) as GameObject; // instead of Instantiate(hexPrefab); in order to keep prefab link
                Hex hex = (gameObject.GetComponent("Hex") as Hex);
                hex.setHexType(hexType);
                hex.getHexData().hexScaleFactor.x = (float)System.Math.Round(xScale,2);
                hex.getHexData().hexScaleFactor.y = (float)System.Math.Round(yScale, 2);
                hex.getHexData().cellLocation = cellPosition;
                hex.setHealth(health);
                gameObject.transform.SetParent(brushTarget.transform);//BrushTarget will be the tilemap selected
                gameObject.transform.position = gridLayout.LocalToWorld(gridLayout.CellToLocalInterpolated(cellPosition));
                gameObject.transform.localScale = new Vector3(((brushTarget.transform.localScale.x *gridLayout.cellSize.x) * xScale), brushTarget.transform.localScale.y *gridLayout.cellSize.y, brushTarget.transform.localScale.z *gridLayout.cellSize.z);
            }
        }
        else
        {
            Debug.Log("Can only draw on GameObjects with the HexGrid attached");
        }
    }

    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        Transform cellObject = GetObjectInCell(gridLayout, brushTarget.transform, new Vector3Int(position.x, position.y, position.z));
        if (cellObject != null)
            DestroyImmediate(cellObject.gameObject);
    }

    private Transform GetObjectInCell(GridLayout grid, Transform parent,Vector3Int position)
    {
        //Get all children objects of Grid containing TileMap
        int childCount = parent.childCount;
        //Go through each child and check if hex exists in the clicked cell's position
        for (int i=0; i < childCount; i++)
        {
            Transform child = parent.GetChild(i);
            //Convert the location of each child into a cell position (has to be cast to int)
            // Vector3 childCell_f = grid.WorldToLocal(grid.LocalToCell(child.position));
            //Vector3Int childCell_i = new Vector3Int((int)childCell_f.x, (int)childCell_f.y, (int)childCell_f.z);
            Vector3Int childCell_i = (child.GetComponent<Hex>()).getHexData().cellLocation;
            if (((child.GetComponent("Hex") as Hex) != null) && position == childCell_i)
                return child;
        }
        //Debug.Log("Valid P =" + position.x + " " + position.y);
        return null;
    }

    public void lowerHealth() {
        health--;
        health = Mathf.Max(1, health);
    }

     public void raiseHealth() {
        health++;
        health = Mathf.Min(10, health);
    }

}
#endif