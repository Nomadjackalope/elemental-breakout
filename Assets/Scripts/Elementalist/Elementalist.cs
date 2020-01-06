using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Elementalist : MonoBehaviour {

	public static Elementalist instance;

	public Text convoText;
	public GameObject convoBackground;
	public Image girlImage;
	public Image dark;
	public Button ContinueButton;

	public ElementalistEmotion emotions;

	bool isShowing = false;
	bool textRunning = false;
	bool isMultiple = false;
	bool next = false;
	bool activeSpeech;

	Sprite currentEmotion;

	private Vector2 girlFinalPos = new Vector2(175, 700);
	private Vector2 convoFinalPos = new Vector2(500, 150);

	private Vector2 girlOffPos = new Vector2(-190, 700);
	private Vector2 convoOffPos = new Vector3(500, -150);

	private float darkFinal = 0.5f;
	private float darkOff = 0;

	private Color hiddenColor = new Color(1,1,1,0);
	private Color shownColor = new Color(1,1,1,1);

	private string convoTextFinal;

	private List<Touch> curTouches = new List<Touch>();

	private Coroutine runningText;

	private bool touchToContinue = false;
	private bool finishText = false;

	private List<ElementalistDialog> dialogs = new List<ElementalistDialog>();

	void Awake() {
		if(instance == null) {
			instance = this;
		} else if(instance != this) {
			Destroy(gameObject);
		}

		// convoOffPos.y = -convoBackground.GetComponent<RectTransform>().rect.height / 2;
		// convoFinalPos.y = -convoOffPos.y;

		// girlOffPos.x = girlImage.GetComponent<RectTransform>().rect.width / 2;
		// girlFinalPos.x = -girlOffPos.x;

		girlImage.color = hiddenColor;
		convoBackground.GetComponent<Image>().color = hiddenColor;
		ContinueButton.gameObject.SetActive(false);
	}

	private IEnumerator say(string text, Sprite emotion) {
		// while(isShowing || activeSpeech) {
		// 	yield return null;
		// }

		activeSpeech = true;

		if(emotion != currentEmotion) {
			//girlImage.sprite = emotion;
			currentEmotion = emotion;
		}

		if(!isShowing) {
			isShowing = true;
			transform.root.GetComponent<GraphicRaycaster>().enabled = true;
			yield return StartCoroutine(showGirlAndText());
		}


		yield return StartCoroutine(changeText(text));

		activeSpeech = false;

		while(isShowing) {
			if(touchToContinue) {
				if (dialogs.Count > 0) {
					trySayNext();
				} else {
					StartCoroutine(hideGirlAndText());
				}
				touchToContinue = false;
			} else {
				yield return null;
			}
		}

	}

	// public void sayEasy(string text, Sprite emotion) {
	// 	// StartCoroutine(say(text, emotion));
	// 	dialogs.Add(new ElementalistDialog(text, emotion));
	// }

	public void sayEasyHappy(string text) {
		// StartCoroutine(say(text, emotions.happy));
		dialogs.Add(new ElementalistDialog(text, emotions.happy));
	}

	public bool sayEasyHappyIfAvailable(HintIds hint) {
		bool seenHint = MasterPlayerData.instance.checkSeenHint(hint);

		if(!seenHint) {
			Elementalist.instance.sayEasyHappy(Hints.getHintFrom(hint));
			MasterPlayerData.instance.seenHint(hint);
		}

		return seenHint;
	}

	public bool sayEasyHappyIfAvailable(ConvoIds convo) {
		bool seenHint = MasterPlayerData.instance.checkSeenConvo(convo);

		if(!seenHint) {
			Elementalist.instance.sayEasyHappy(Convos.getHintFrom(convo));
			MasterPlayerData.instance.seenConvo(convo);
		}

		return seenHint;

	}

	public void sayMultiple(List<string> texts, List<Sprite> emotions) {
		isMultiple = true;
	}
	
	public void trySayNext() {
		StartCoroutine(say(dialogs[0].text, dialogs[0].emotion));
		dialogs.RemoveAt(0);
	}

	public IEnumerator trySayNow(string text, Sprite emotion) {
		while(isShowing) {
			yield return null;
		}
		yield return StartCoroutine(say(text, emotion));
	}

	public void hide() {
		girlImage.transform.position = girlOffPos;
		convoBackground.transform.position = convoOffPos;
	}

	// Use this for initialization
	void Start () {

		//StartCoroutine(say("Hello, my name is .... and I am so glad you have decided to help me collect essence to be an elementalist.", emotions.happy));
	}

	void Update() {
		if(!isShowing) {
			if(dialogs.Count > 0) {
				trySayNext();
			}
			return;
		}
		
		curTouches = InputHelper.GetTouches();

		if(curTouches.Count > 0) {

			// Watch for ball launches on touch down
			if(curTouches[0].phase == TouchPhase.Began) {
				walkthroughTouches();
			}
		}
	}

	public void walkthroughTouches() {
		if(isShowing && textRunning) {
			if(runningText != null) {
				finishText = true;
			}
			// textRunning = false;
			// convoText.text = convoTextFinal;
		} else if(isShowing) {
			// StartCoroutine(hideGirlAndText());
			touchToContinue = true;
		}
	}
	

	IEnumerator changeText(string text) {
		convoText.text = "";
		convoTextFinal = text;
		runningText = StartCoroutine(gradualText(text, 0.03f));
		yield return runningText;
	}

	IEnumerator gradualText(string text, float interval) {
		textRunning = true;
 
		foreach (char letter in text.ToCharArray())
		{
			convoText.text += letter;
			if(!finishText) {
				yield return new WaitForSeconds(interval);
			}
		}

		finishText = false;
		textRunning = false;
	}

	IEnumerator showGirlAndText() {
		bool atPos = false;


		girlImage.enabled = true;
		convoBackground.gameObject.SetActive(true);

		// print("girlOff: " + girlOffPos);
		// print("convoOff: " + convoOffPos);
		// // make sure girl and text are off screen at the right place
		// Rect prevGirlRect = girlImage.GetComponent<RectTransform>().rect;
		// girlImage.GetComponent<RectTransform>().rect.Set(girlOffPos.x, girlOffPos.y, prevGirlRect.width, prevGirlRect.height);
		// convoBackground.transform.position = convoOffPos;
		Color nextColor;

		girlImage.color = hiddenColor;
		convoBackground.GetComponent<Image>().color = hiddenColor;
		//convoBackground.GetComponentInChildren<Text>().color = hiddenColor;

		dark.color = new Color(dark.color.r, dark.color.g, dark.color.b, darkOff);

		// yield break;
		while(!atPos) {
			//prevGirlRect = girlImage.GetComponent<RectTransform>().rect;

			//girlImage.transform.Translate(Vector3.right * (girlFinalPos - girlOffPos) / 0.5f * Time.deltaTime);
			//girlImage.GetComponent<RectTransform>().rect.Set((girlFinalPos.x - girlOffPos.x) / 0.5f * Time.deltaTime + prevGirlRect.x, prevGirlRect.y, prevGirlRect.width, prevGirlRect.height);
			//convoBackground.transform.Translate(Vector3.up * (convoFinalPos - convoOffPos) / 0.5f * Time.deltaTime);

			nextColor = new Color(1,1,1, girlImage.color.a + 1 / 0.5f * Time.deltaTime);

			girlImage.color = nextColor;
			convoBackground.GetComponent<Image>().color = nextColor;
			//convoBackground.GetComponentInChildren<Text>().enabled = false;

			dark.color = new Color(dark.color.r, dark.color.g, dark.color.b, dark.color.a + (darkFinal - darkOff) / 0.5f * Time.deltaTime);

			//if(((Vector2)girlImage.transform.position - girlFinalPos).magnitude > ((girlFinalPos - girlOffPos) / 0.5f * Time.deltaTime).x * 2) {
			//print("dark: " + (dark.color.a - darkFinal) + ", " + ((darkFinal - darkOff) * Time.deltaTime) );
			if((darkFinal - dark.color.a) > (darkFinal - darkOff) * 0.5f * Time.deltaTime * 2) { 
				yield return null; //new WaitForSeconds(0.01f);
			} else {

				//girlImage.transform.position = girlFinalPos;
				//convoBackground.transform.position = convoFinalPos;

				girlImage.color = shownColor;
				convoBackground.GetComponent<Image>().color = shownColor;
				//convoBackground.GetComponentInChildren<Text>().enabled = true;

				atPos = true;
			}
		}

		ContinueButton.gameObject.SetActive(true);
		ContinueButton.GetComponent<CanvasGroup>().alpha = 0;
		ContinueButton.GetComponent<CanvasGroup>().DOFade(1, 0.3f);
	}

	IEnumerator hideGirlAndText() {
		transform.root.GetComponent<GraphicRaycaster>().enabled = false;
		bool atPos = false;

		// make sure girl and text are off screen at the right place
		// girlImage.transform.position = girlOffPos;
		// convoBackground.transform.position = convoOffPos;

		DOTween.Sequence()
			.Append(ContinueButton.GetComponent<CanvasGroup>().DOFade(0, 0.3f))
			.AppendCallback(() => { ContinueButton.gameObject.SetActive(false); });
		
		
		Color nextColor;
		

		girlImage.color = shownColor;
		convoBackground.GetComponent<Image>().color = shownColor;

		while(!atPos) {

			// girlImage.transform.Translate(Vector3.right * (girlOffPos - girlFinalPos) / 0.5f * Time.deltaTime);
			// convoBackground.transform.Translate(Vector3.up * (convoOffPos - convoFinalPos) / 0.5f * Time.deltaTime);

			nextColor = new Color(1,1,1, girlImage.color.a + 1 / 0.5f * Time.deltaTime);

			girlImage.color = nextColor;
			convoBackground.GetComponent<Image>().color = nextColor;

			dark.color = new Color(dark.color.r, dark.color.g, dark.color.b, dark.color.a + (darkOff - darkFinal) / 0.5f * Time.deltaTime);

			//if(((Vector2)girlImage.transform.position - girlOffPos).magnitude > ((girlFinalPos - girlOffPos) / 0.5f * Time.deltaTime).x * 2) {
			// print("dark: " + (dark.color.a - darkOff) + ", " + ((darkFinal - darkOff) * Time.deltaTime) );
			if((dark.color.a - darkOff) > (darkFinal - darkOff) * 0.5f * Time.deltaTime * 2) { 
				yield return null; //new WaitForSeconds(0.01f);
			} else {

				// girlImage.transform.position = girlOffPos;
				// convoBackground.transform.position = convoOffPos;

				girlImage.color = hiddenColor;
				convoBackground.GetComponent<Image>().color = hiddenColor;


				atPos = true;
			}
		}

		girlImage.enabled = false;
		convoBackground.gameObject.SetActive(false);

		convoText.text = "";

		isShowing = false;
	}

	public bool getIsShowing() {
		return isShowing;
	}
}

public class ElementalistDialog {
	public string text;
	public Sprite emotion;

	public ElementalistDialog(string text, Sprite emotion) {
		this.text = text;
	}
}
