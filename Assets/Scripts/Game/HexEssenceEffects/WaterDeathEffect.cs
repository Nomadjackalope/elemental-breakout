using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDeathEffect : HexDeathEffect
{

    private float waterDuration = 1f;
    private float numSpins = 10;

    private bool spin = false;
    private GameObject ball;

    private float ballCapturedTime = 0;

    private AudioClip waterSpinSound;

    void Start() {
        numSpins += Random.value;
    }

    void Update() {
        spinBall();
    }

    void spinBall() {
        if(!spin) return;

        if(ball == null) {
            shootWaterSpunBall(Vector2.zero);
        } else {
            // spin ball and after number of seconds release
            float val = easeInQuart(ballCapturedTime, 0, 1, waterDuration) * 2 * Mathf.PI * numSpins;

            // convert val to some circular step function
            float x = Mathf.Sin(val) * 0.25f; // 0 to 0.25
            float y = Mathf.Cos(val) * 0.25f;

            ball.transform.SetPositionAndRotation(transform.position + new Vector3(x, y, 0), Quaternion.identity);

            // curve that slows ball spin down over time
            ballCapturedTime += Time.deltaTime;
            if(ballCapturedTime > waterDuration) {
                // get the angle that it ends
                Vector2 shootVector = ball.transform.position - transform.position;
                shootWaterSpunBall(shootVector);
                
            }
        }
    }

    public void setInits(GameObject ball, AudioClip waterSpinSound) {
        this.ball = ball;
        this.waterSpinSound = waterSpinSound;
        waterDeath();
    }

    private void waterDeath() {
        if(ball == null) {
            Destroy(gameObject);
            return;
        }

        // capture ball // whirlpool ball in
        ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    
        ball.transform.SetPositionAndRotation(transform.position, Quaternion.identity);

        if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.WaterPickElementDirection)) {
            GameManage.instance.pickWaterElementDirection(ball, this);
        } else {
            spin = true;

            if(waterSpinSound != null) {
                MasterEffectsSound.instance.PlayOneShot(waterSpinSound);
            }
        }
    }

    public void setWaterSpinAngle(float angle) {
        
        angle *= Mathf.Deg2Rad;
        Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

        shootWaterSpunBall(direction);
    }
    
    private void shootWaterSpunBall(Vector2 shootVector) {

        // launch ball
        if(ball != null) {
            ball.GetComponent<Rigidbody2D>().AddForce(shootVector.normalized * 12, ForceMode2D.Impulse);
        }

        spin = false;
        Destroy(gameObject);
    }

    // http://gizma.com/easing/#quart1
    float easeInQuart(float t, float b, float c, float d) {
        t /= d;
        return c * t * t + b;
    }
    
}