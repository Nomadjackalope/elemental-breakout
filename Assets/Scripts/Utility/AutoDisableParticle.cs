using UnityEngine;
using System.Collections;

public class AutoDisableParticle : MonoBehaviour {
	private ParticleSystem ps;


	public void Start() {
		ps = GetComponent<ParticleSystem>();
	}

	public void Update() {
		if(ps) {
			if(!ps.IsAlive()) {
				gameObject.SetActive(false);
			}
		}
	}
}