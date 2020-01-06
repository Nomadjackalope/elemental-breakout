using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelIconButton : MonoBehaviour {

	public float maxEffectDist = 400f;

	public Text levelsWonText;

	public DataTypes.BiomeType biome;

	public bool isMain = false;

	public LevelSelectLoader levels;

	public Image backgroundColor;

	float lerpTime = 0;
	Vector2 originPos;
	Vector2 endPos;
	float journeyLength;
	float speed = 1.0f;

	public bool printThis = false;

	float distToCenter, halfWidth, distLeft;

	// Use this for initialization
	void Start () {
		endPos = transform.parent.parent.parent.position;

		if(MasterPlayerData.instance.getLastBiomePlayed() == biome) {
			// transform.parent.position = new Vector3(0, transform.parent.position.y);
			//print("Content position: " + transform.parent.position);			
			Vector2 distanceToMove = new Vector2(-transform.localPosition.x + 540, 0);
			((RectTransform)transform.parent).anchoredPosition += distanceToMove;
			//print("Distance to move: " + distanceToMove.x);
			
			checkMain();
		}
	}
	
	// Update is called once per frame
	void Update () {
		distToCenter = Mathf.Abs(posInScrollView().x);//transform.position.x - transform.parent.parent.parent.position.x);
		// print("distToCenter: " + distToCenter);
		halfWidth = ((RectTransform)transform).rect.width / 2;
		//printActive("distToCenter: " + distToCenter);

		if(distToCenter > halfWidth) {
			isMain = false;
			// normalize distance from 0.5 to 1
			distToCenter = Mathf.Min(maxEffectDist, distToCenter); // halfWidth to maxEffectDist
			distToCenter -= halfWidth; // 0 to (maxEffectDist - halfWidth)
			distToCenter /= (maxEffectDist - halfWidth); // 0 to 1
			distToCenter /= 2f; // 0 to 0.5;
			distToCenter = 1f - distToCenter; // 1 to 0.5

			GetComponent<CanvasGroup>().alpha = distToCenter;
			transform.localScale = new Vector3(distToCenter * 0.5f + 0.5f, distToCenter * 0.5f + 0.5f, 1); // 0.75 to 1
		} else {
			GetComponent<CanvasGroup>().alpha = 1;
			transform.localScale = Vector3.one;

			checkMain();
			snapToCenter();
		}

		levelsWonText.text = Mathf.Min(MasterPlayerData.instance.getLevelsUnlockedIn(biome), 10) + " / 10";
	}

	void checkMain() {
		if(isMain == false) {
			// Just switched
			levels.switchBiome(biome);
			backgroundColor.CrossFadeColor(DataTypes.GetColorFrom(biome), 0.5f, true, false);

		}

		isMain = true;
	}

	void snapToCenter() {
		if(InputHelper.GetTouches().Count == 0) {
			distLeft = posInScrollView().x;
			//printActive("distLeft: " + distLeft);

			if(Mathf.Abs(posInScrollView().x) > 5f) {
				transform.parent.Translate(new Vector2( (-distLeft / ((RectTransform)transform).rect.width / 2) * 50, 0)); // -Mathf.Sign(distLeft) * // * journeyLength, 0));
			}
			
		}
	}

	Vector2 posInScrollView() {
		return transform.parent.parent.parent.InverseTransformPoint(transform.position);
	}

	void printActive(string text) {
		if(printThis) print(biome + "| " + text);
	}
}
