using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthDeathEffect : HexDeathEffect
{
    Rigidbody2D rb;
    Collider2D collider2d;

    bool hitPaddle = true;
    bool killingEscapedEarthHex;

    void Awake() {
        gameObject.AddComponent<SpriteRenderer>();

        collider2d = gameObject.AddComponent<CircleCollider2D>();
        
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.mass = 10;
        rb.simulated = true;
        gameObject.layer = 11; // DyingEarthHex
    }

     void FixedUpdate() {
        // do gravity to it
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -9.81f));
        if(transform.position.y < -10 && !killingEscapedEarthHex) {
            killingEscapedEarthHex = true;
            Destroy(gameObject);
        }
    }

    public void SetInits(PhysicsMaterial2D material2D, Sprite sprite) {
        rb.sharedMaterial = material2D;
        rb.simulated = true;

        GetComponent<SpriteRenderer>().sprite = sprite;
        
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.CompareTag("Hex")) {
                Hex hex = collision.collider.gameObject.GetComponent<Hex>();
                hex.removeHealth(hex.getHexData().health);
        } else if (collision.collider.CompareTag("DeathBase")) {
               Destroy(gameObject);
        } else if (collision.collider.CompareTag("Paddle")) {
                if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.EarthElementPaddleBounce) && !hitPaddle) {
                    GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                    hitPaddle = true;
                } else {
                    Destroy(gameObject);
                }
        } else if (collision.collider.CompareTag("StickyOuter")) {
            Destroy(gameObject);
        }  
    } 
}