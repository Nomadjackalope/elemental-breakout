using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunShot : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody2D>().AddForce(Vector2.down * 10, ForceMode2D.Impulse);
		Destroy(gameObject, 5);
	}
	
	void OnTriggerEnter2D(Collider2D collider) {
		if(collider.tag == "Paddle") {
			collider.gameObject.GetComponent<Paddle>().stunThePaddle();
			Destroy(gameObject);
		} else if(collider.tag == "DeathBase") {
			Destroy(gameObject);
		}
	}
}
