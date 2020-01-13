using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowDeathEffect : HexDeathEffect
{
    private float seconds;

    public void Start() {
        Invoke("shadowDeath", seconds);
    }

    public void showShadowAfterTime(float seconds) {
        this.seconds = seconds;
    }

    void shadowDeath() {
        if(!GameManage.instance.paddleData.runeIds.Contains(PowerUps.ShadowDisableShadow)) {
            GameManage.instance.showShadow();
        }
    }
}