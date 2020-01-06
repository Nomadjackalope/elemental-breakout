using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour {

	Rigidbody2D rb;

	private float sideWaysModifier = 1;
	private float downModifier = 4;
	private float airReistModifier = 3;
	private float maxYVelocity = 2;

	public float life = 0;

	private float frequency = 0.2f;
	private float amplitude = 0.3f;

	private DataTypes.BiomeType biomeType;

	private bool doFloatDown = true;

	public AudioClip collected;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D>();
		if(rb != null) {
			rb.mass = 5f;
			rb.AddForce(new Vector3((Random.value - 0.5f) * 2, 3), ForceMode2D.Impulse);
		}

		GameManage.instance.EssenceLaunched(gameObject);
	}
	
	void FixedUpdate () {
		if(doFloatDown) {
			floatDown();
		}
	}

	void OnTriggerEnter2D(Collider2D collider) {

		if(collider.tag == "DeathBase") {
			OnDeath();
		} else if (collider.tag == "Paddle") {
			// add the essence to final
			GameManage.instance.CollectedEssence(biomeType);

			if(collected != null) {
            	MasterEffectsSound.instance.PlayOneShot(collected);
        	}

			OnDeath();
		}
	}

	void OnDeath() {
		GameManage.instance.EssenceRemoved(gameObject);
		Destroy(gameObject);
	}

	void floatDown() {
		if(rb != null) {
			if(Mathf.Abs(rb.velocity.y) > maxYVelocity) return;

			rb.AddForce(new Vector3(0, -1 * downModifier));

			if(Mathf.Abs(transform.position.x) > 5f) {
				
			}

			//rb.AddForce(new Vector3(0, rb.velocity.y * -airReistModifier));

			// Add some randomness to this
			//rb.velocity = new Vector3(Mathf.Sin (Time.fixedTime * Mathf.PI * frequency) * amplitude, rb.velocity.y);
		}
	}

	IEnumerator moveBack() {
		while(transform.position.x > 5) {
			rb.AddForce(new Vector3(Mathf.Sign(transform.position.x) * -1 * 10, 0));
			yield return null;
		}
	}

	public void setDropType(DataTypes.BiomeType biome) {
		biomeType = biome;

		ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
		ParticleSystem.MainModule ma = ps.main;
		ma.startColor = DataTypes.GetEssenceColorFrom(biome);
	}

	public void SetFloating(bool val) {
		doFloatDown = val;
	}
}
