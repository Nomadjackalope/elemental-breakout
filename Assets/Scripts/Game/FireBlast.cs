using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBlast : MonoBehaviour {

	public bool hasPassthrough = false;

	// Use this for initialization
	void Start () {
		Destroy(gameObject, 4);
	}

	void OnTriggerEnter2D(Collider2D collider) {
		if(collider.tag == "Hex") {
			collider.gameObject.GetComponent<Hex>().removeHealth(1);
			if(!hasPassthrough) {
				Destroy(gameObject);
			}
		}
	}
}
