using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallOutBarrier : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D collision) {
		if(collision.collider.tag == "Ball" || collision.collider.tag == "Hex") {
			Destroy(gameObject);
		}
	}
}
