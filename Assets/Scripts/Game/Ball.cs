using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {

	// Properties
	// int attack;
	// effect?
	// float radius;
	//bool attachedToPaddle = true;
	private Rigidbody2D rb;
	public GameObject hitParticles;
	public float xPaddleMultiplier = 1;
	public SpriteRenderer ghostlySprite;

	public bool isPoisoned = false;

	public List<AudioClip> ballHits = new List<AudioClip>();
	public AudioClip ballStick;

	public float minVelocity = 2f;
	public float additionalForce = 0.1f;

	private int passThroughStrength = 3;
	public int passThroughStrengthLeft;
	private float launchSpeed = 1;

	public Vector2 prevVelocity;

	private int sizeMultiplier = 0;
	private float baseColliderSize;

	public bool isShadowBall = false;
	public int age = 0;
	int shadowLifetime = 7;

	private int hitsBetweenTeleport = 4;
	private int lastTeleport = 0;

	public GameObject deathTrail;
	public bool heavyBall;

	public GameObject geyserDirection;

	private int numConsecutivePaddleHits;

	public GameObject TeleportSwirl;

	void Awake() {
		rb = GetComponent<Rigidbody2D>();
		baseColliderSize = GetComponent<CircleCollider2D>().radius;
	}

	// Use this for initialization
	void Start () {
        // rb.velocity = new Vector2(1.0f, -20.0f);

		passThroughStrengthLeft = passThroughStrength;

	}

	void FixedUpdate() {
		if(heavyBall && rb.bodyType == RigidbodyType2D.Dynamic) {
			// manage speed
			if(rb.velocity.magnitude > (launchSpeed / rb.mass)) {
				rb.AddForce(rb.velocity.normalized * -0.5f);
			} else {
				rb.AddForce(rb.velocity.normalized * 1f);
			}
		} else if(rb.bodyType == RigidbodyType2D.Dynamic) {
			if(rb.velocity.magnitude > launchSpeed * 3) {
				rb.velocity = rb.velocity.normalized * launchSpeed / rb.mass;
			} if(rb.velocity.magnitude > (launchSpeed / rb.mass)) {
				rb.AddForce(rb.velocity * -0.5f);
			} else {
				rb.AddForce(rb.velocity.normalized * 1f);
			}
		}
		prevVelocity = rb.velocity;
	}

	// Methods
	void OnCollisionEnter2D(Collision2D collision) {
	
		// not quite right
		MasterEffectsSound.instance.PlayOneShot(ballHits[(int)((ballHits.Count - 1) * Random.value)]);

		// Spawn hitParticle where ball makes contact with anything
		if(collision.contactCount > 0) {
			Vector3 colPoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
			PoolManager poolManager = GameManage.instance.GetComponent<PoolManager>();

			if(poolManager != null && poolManager.isActiveAndEnabled) {
				Transform ballHitTransform = poolManager.GetObjectByName("Ball hit").transform;
				ballHitTransform.position = colPoint;
			} else {
				Instantiate(hitParticles, colPoint, Quaternion.identity);
			}
		}

		if(collision.collider.tag == "DeathBase") {
			numConsecutivePaddleHits = 0;
			onDeath();
		} else if(collision.collider.tag == "Paddle") {
			numConsecutivePaddleHits++;

			if(numConsecutivePaddleHits > 3) {
				rb.AddForce(Vector3.up * 2);
			}

			if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.GrowthStickyPaddle)) {
				stickBall(collision.collider.gameObject);
			}
			else if(collision.contactCount > 0) {
				// Convert collision location to paddle coordinates system
				Transform paddleTransform = collision.transform;
				Vector2 paddleCoords = paddleTransform.InverseTransformPoint(collision.GetContact(0).point);

				// Keep velocity ~constant while making ball move more in x depending on ball collision location on paddle
				// get magnitude
				float magnitude = rb.velocity.magnitude;
				// switch percentage // This should be a percent of total paddle width?
				Vector2 newPercent = new Vector2(rb.velocity.x + (paddleCoords.x * xPaddleMultiplier), rb.velocity.y);
				newPercent *= 1000; // force magnitude up high
				// clamp magnitude back down
				newPercent = Vector2.ClampMagnitude(newPercent, magnitude);

				// print("Magnitude before " + magnitude + " and after " + newPercent.magnitude);

				rb.velocity = newPercent;
			}

			if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.EarthHeavyBall)) {
				passThroughStrengthLeft = passThroughStrength;
				GameManage.TriggerEvent(PowerUps.EarthHeavyBall, PowerUpState.Active, passThroughStrengthLeft);
        	}

			if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.ShadowTeleporterPaddle)) {
				if(lastTeleport + hitsBetweenTeleport <= GameManage.instance.getBallHitCount()) {
					StartCoroutine(visualTeleport(new Vector3(transform.position.x, 9, transform.position.z), prevVelocity));
				}
			}

		} else if(collision.collider.tag == "Border") {
			numConsecutivePaddleHits = 0;
			if(Mathf.Abs(rb.velocity.y) < minVelocity) {
				// print("y changed");
				rb.AddForce(new Vector2(0, Mathf.Sign(rb.velocity.y) * additionalForce), ForceMode2D.Impulse);
			}

			if(collision.collider.name == "Top") {
				if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.GrowthSpawnUnderTop)) {
					GameManage.instance.spawnBlocksUnder(transform);
				}
			}
		} else if(collision.collider.tag == "StickyOuter") {
			numConsecutivePaddleHits = 0;
			if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.GrowthSideSticky)) {
				stickBall(collision.collider.gameObject);
			}
		} else if(collision.collider.tag == "Hex") {
			numConsecutivePaddleHits = 0;
		}
	}



	// Hex calls ball in some situations to find out how much to get hurt
	// public int getDamage() {
	// 	return passThroughStrengthLeft;
	// }

	// public void setDamage(int leftOver) {
	// 	passThroughStrengthLeft = Mathf.Max(leftOver, 1);
	// 	if(passThroughStrengthLeft <= 0) {
	// 		GameManage.TriggerEvent(PowerUps.EarthHeavyBall, PowerUpState.CoolDown, 0);
	// 	} else {
	// 		GameManage.TriggerEvent(PowerUps.EarthHeavyBall, PowerUpState.Active, passThroughStrengthLeft);
	// 	}
	// }

	public void onDeath() {
		// Let game manager know ball died
		GameManage.instance.ballDestroyed(gameObject);
		
		onDeath(false);
	}

	public void onDeath(bool notifyManager) {
		
		// Kill ball
		Destroy(gameObject);
	}

	public void launch(float speed) {
		launch(speed, Vector2.up);
		
	}

	public void launch(float speed, Vector2 direction) {
		speed = speed == 0.0f ? 1 : GameManage.instance.getBallModifiedSpeed();
		direction = (direction == Vector2.zero) ?  Vector2.up : direction;

		if(rb.bodyType == RigidbodyType2D.Kinematic) {

			// Ball will no longer move relative to paddle
			transform.parent = null;
			
			// Make the ball move from outside forces
			rb.bodyType = RigidbodyType2D.Dynamic;

			// Add force to launch ball vertically
			rb.AddForce(direction * speed, ForceMode2D.Impulse);

			launchSpeed = speed;

			if(isPoisoned) {
				poison();
			}

			gameObject.layer = 8;
		}
	}

	public void changeSize(int multiplier) {
		sizeMultiplier += multiplier;

		GetComponent<CircleCollider2D>().radius = baseColliderSize * ((sizeMultiplier * 0.25f) + 1);
		foreach (Transform child in transform)
		{
			child.localScale = Vector3.one * ((sizeMultiplier * 0.3f) + 1);
		}
	}
	
	public float poison() {
		return poison(true);
	}

	public float poison(bool autoStop) {
		isPoisoned = true;
		if(isShadowBall) {
			GetComponentInChildren<SpriteRenderer>().color = new Color(0.5f, 0, 0.06f);
		} else {
			GetComponentInChildren<SpriteRenderer>().color = new Color(1, 0, 0.2f);
		}

		float seconds = 5;
		if(GameManage.instance.paddleData.runeIds.Contains(PowerUps.PoisonExtendDuration)) {
			seconds += 5;
		}

		
		if(autoStop) {
			StartCoroutine(stopPoison(seconds));
		}

		return seconds;
	}

	private IEnumerator stopPoison(float seconds) {
		yield return new WaitForSeconds(seconds);

		isPoisoned = false;
		if(isShadowBall) {
			GetComponentInChildren<SpriteRenderer>().color = new Color(0.33f, 0.33f, 0.33f);
		} else {
			GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1f);		
		}
	}

	public void toggleShadowBallSprite(bool on) {
		ghostlySprite.enabled = on;
	}

	private void stickBall(GameObject paddle) {
		if(ballStick != null) {
			AudioSource.PlayClipAtPoint(ballStick, Vector3.zero);
		}

		GameManage.instance.setBallStuckToPaddle();
		stopBall();
		transform.parent = paddle.transform;
		gameObject.layer = 12;
	}

	private void stopBall() {
		rb.bodyType = RigidbodyType2D.Kinematic;
		rb.velocity = Vector2.zero;
	}

	private IEnumerator visualTeleport(Vector3 finalPos, Vector3 prevVelocitySaved) {
		
		rb.simulated = false;
		GetComponent<CircleCollider2D>().enabled = false;
		bool disabledDeathTrail = deathTrail.activeSelf;
		if(disabledDeathTrail) disableDeathTrail();

		GameObject teleportSwirl = Instantiate(TeleportSwirl, transform);

		SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		Color[] colors = new Color[spriteRenderers.Length];
		Color lowAlphaColor;
		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			lowAlphaColor = spriteRenderers[i].color;
			colors[i] = new Color(lowAlphaColor.r, lowAlphaColor.g, lowAlphaColor.b, lowAlphaColor.a);

			lowAlphaColor.a = 0.2f;
			spriteRenderers[i].color = lowAlphaColor;
		}

		yield return StartCoroutine(MoveToPosition(finalPos, 0.25f));

		for (int i = 0; i < spriteRenderers.Length; i++)
		{
			spriteRenderers[i].color = colors[i];
		}

		Destroy(teleportSwirl);
		
		if(disabledDeathTrail) enableDeathTrail();
		rb.simulated = true;
		GetComponent<CircleCollider2D>().enabled = true;

		rb.velocity = prevVelocitySaved;
		lastTeleport = GameManage.instance.getBallHitCount();
	}

	private IEnumerator MoveToPosition(Vector3 newPosition, float time)
	{
		float elapsedTime = 0;
		Vector3 startingPos = transform.position;
		while (elapsedTime < time)
		{
			transform.position = Vector3.Lerp(startingPos, newPosition, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	public void setShadowBall() {
		GetComponentInChildren<SpriteRenderer>().color = new Color(0.33f, 0.33f, 0.33f);
		isShadowBall = true;
	}

	public int getShadowLifetime() {
		return shadowLifetime;
	}

	public void enableDeathTrail() {
		deathTrail.SetActive(true);
	}

	public void disableDeathTrail() {
		deathTrail.SetActive(false);
	}
}
