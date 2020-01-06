using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SelectBiomeLevel: MonoBehaviour {
 
	private Animator p1Anim;
	//private Animator p2Anim;
	private bool swapPanel = false;

	//public RectTransform panel1;
	//public RectTransform panel2;

	// Use this for initialization
	void Start () {
		// p1Anim = panel1.GetComponent<Animator>();
		p1Anim = GetComponent<Animator>();
		//p2Anim = panel2.GetComponent<Animator>();
	}
	
	public void IveBeenClicked()
	{
		swapPanel = !swapPanel;

		p1Anim.SetBool("open", swapPanel);
		//p2Anim.SetBool("open", swapPanel);
	}
}