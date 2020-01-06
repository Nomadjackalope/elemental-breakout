using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDeathEffect : HexDeathEffect
{
    void Start() {
        Invoke("ExplodeDelayed", 0.5f);
    }

    private void ExplodeDelayed() {
        
        // get this tile's location in map
        Vector3Int cellPos = Hex.cellPosFromLocal(transform.localPosition);

        // check for colliders surrounding this tile
        List<Vector3> cellsToExplode = Hex.getHexRingInWorldPos(cellPos, 1);

        if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.FireExplosionUp)) {
            cellsToExplode.AddRange(Hex.getHexRingInWorldPos(cellPos, 2));
        }

        foreach (Vector3 cell in cellsToExplode) {
            ExplodeCell(cell);
        }
        
        Destroy(gameObject);
    }

    private void ExplodeCell(Vector3 worldPos) {
        Hex hex = Hex.getHexAt(worldPos);
        if(hex != null) {
            hex.removeHealth(1);
        }
    }
}