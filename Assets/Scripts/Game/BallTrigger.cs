using UnityEngine;

public class BallTrigger : MonoBehaviour {

    private int passThroughStrength = 3;
	private int passThroughStrengthLeft;

    void Start() {
        passThroughStrengthLeft = passThroughStrength;
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "Hex") {
            if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.EarthHeavyBall)) {
                collider.gameObject.GetComponent<Hex>().removeHealth(Damage(collider.gameObject.GetComponent<Hex>().getHealth()));
            }
        } else if(collider.tag == "Paddle") {
            print("doing this");
            passThroughStrengthLeft = passThroughStrength;
        }
    }

    int Damage(int damageNeeded) {
        // if the hex has less health than we have damage, just return the health
        if(passThroughStrengthLeft - damageNeeded >= 0) {
            passThroughStrengthLeft -= damageNeeded;
            print("damage: " + damageNeeded);
            return damageNeeded;
        } else {
            int damage = passThroughStrengthLeft;
            passThroughStrengthLeft = 0;
            
            return damage;
        }
    }
}