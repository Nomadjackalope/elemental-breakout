using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonSpray : MonoBehaviour {

	public ParticleSystem ps;

	// Use this for initialization
	void Start () {
		Vector2 randomUp = new Vector2((Random.value - 0.5f) * 2, 1);

		GetComponent<Rigidbody2D>().AddForce(randomUp * 8, ForceMode2D.Impulse);

		// Destroy(gameObject, 4);
		StartCoroutine(waitToKill());
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if(collision.collider.tag == "Hex") {
			collision.collider.gameObject.GetComponent<Hex>().poison(0);
			GetComponent<Collider2D>().enabled = false;
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			StartCoroutine(killProjectile());
		}
	}

	IEnumerator waitToKill() {
		yield return new WaitForSeconds(4);

		yield return killProjectile();
	}

	IEnumerator killProjectile() {
		if(ps == null) {
			Destroy(gameObject);
		} else {
			var em = ps.emission;
			em.enabled = false;

			yield return new WaitForSeconds(4);

			Destroy(gameObject);
		}
	}
}
