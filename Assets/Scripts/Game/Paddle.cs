using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Paddle : MonoBehaviour {

	Vector2 prevLoc;
	public float paddleDrag = 1;

	private Rigidbody2D rb;

	private bool isStunned = false;
	private float paddleSpeed = 4;

	public bool stun;

	public SpriteRenderer paddleSprite;

	private Vector2 stunnedDistance;

	private Sequence paddleBlink;

	void Awake() {
		rb = gameObject.GetComponent<Rigidbody2D>();

	}

	// FixedUpdate is called as close to a set time apart
	void FixedUpdate() {
		#if UNITY_EDITOR
		if(stun) {
			StartCoroutine(stunPaddle());
			stun = false;
		}
		#endif
	}

	public void checkForTouchMove(float offset) {
		
		if(Input.GetMouseButton(0)) {

			// Get mouse position from in screen pixels then convert that to world coordinates
			Vector2 mousePosInWorld = MasterManager.instance.masterCamera.ScreenToWorldPoint(Input.mousePosition);

			if(isStunned) {
				

				//rb.AddForce(new Vector2((mousePosInWorld.x - transform.position.x) * paddleSpeed, 0));
				// get direction
				stunnedDistance.x = mousePosInWorld.x + offset - transform.position.x;
				
				// d = vt & velocity = 4 units/sec
				stunnedDistance.x = 2 * Time.deltaTime * stunnedDistance.x;

				
				// rb.MovePosition(new Vector2(mousePosInWorld.x + offset, transform.position.y));
				rb.MovePosition(new Vector2(stunnedDistance.x + transform.position.x, transform.position.y));


			} else {
				


				// Move the paddle in the next physics update to this position. Apparently this doesn't give it velocity.
				rb.MovePosition(new Vector2(mousePosInWorld.x + offset, transform.position.y));

				// Restrict paddle movement to inside screen ?
			}

		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if(collision.collider.tag == "Ball") {
			//collision.rigidbody.AddForce(new Vector2(rb.velocity.x * paddleDrag, 0));
		} else if(collision.collider.gameObject.layer == 11 && collision.otherCollider.tag != "StickyOuter") {
				if(!GameManage.instance.paddleData.runeIds.Contains(PowerUps.EarthNoPaddleStun)) {
					// slow paddle
					StartCoroutine(stunPaddle());
				}
		}
	}

	public void stunThePaddle() {
		StartCoroutine(stunPaddle());
	}

	private IEnumerator stunPaddle() {
		isStunned = true;
		//rb.bodyType = RigidbodyType2D.Dynamic;

		if(paddleBlink != null) {
			print("blink Active. blink playing: " + paddleBlink.active + " " + !paddleBlink.IsPlaying());
		}

		if(paddleBlink == null || (paddleBlink.active && !paddleBlink.IsPlaying())) {
			float blinkTime = 0.1f;
			paddleBlink = DOTween.Sequence();
			paddleBlink.Append(paddleSprite.DOFade(0, blinkTime))
				.Append(paddleSprite.DOFade(1, blinkTime))
				.SetLoops((int)(1.5f/blinkTime)).SetAutoKill(false);
		}

		yield return new WaitForSeconds(3);

		//rb.velocity = Vector2.zero;
		isStunned = false;
		//rb.bodyType = RigidbodyType2D.Kinematic;
	}

	public void setStickyZones(bool value) {
		foreach (Transform child in transform)
		{
			if(child.name == "StickyOuters") {
				child.gameObject.SetActive(value);
			}
		}
	}

	public void changeSize(float size) {
		GetComponent<BoxCollider2D>().size *= new Vector2(size, 1);
		foreach (Transform child in transform)
		{
			if(child.GetComponent<SpriteRenderer>() != null) {
				
				child.localScale = new Vector3(child.localScale.x * size, child.localScale.y, child.localScale.z);

			} else if(child.name == "StickyOuters") {

				Transform left = child.transform.GetChild(0);
				Transform right = child.transform.GetChild(1);

				left.localPosition = new Vector3(-transform.localScale.x * size - left.localScale.x, left.localPosition.y, left.localPosition.z);
				right.localPosition = new Vector3(transform.localScale.x * size + right.localScale.x, right.localPosition.y, right.localPosition.z);

				// left.localScale= new Vector3(left.localScale.x * size, left.localScale.y, left.localScale.z);
				// right.localScale = new Vector3(right.localScale.x * size, right.localScale.y, right.localScale.z);
			}
		} 
	}
}
