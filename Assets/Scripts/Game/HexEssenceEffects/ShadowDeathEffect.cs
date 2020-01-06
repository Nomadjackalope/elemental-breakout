using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDeathEffect : HexDeathEffect
{

    public void showShadowAfterTime(float seconds) {
        Invoke("shadowDeath", seconds);
    }

    void shadowDeath() {
        if(!GameManage.instance.paddleData.runeIds.Contains(PowerUps.ShadowDisableShadow)) {
            GameManage.instance.showShadow();
        }
    }
}