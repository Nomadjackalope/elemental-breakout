using UnityEngine;
using System.Collections;

public class AutoDeleteParticle : MonoBehaviour {
	private ParticleSystem ps;


	public void Start() {
		ps = GetComponent<ParticleSystem>();
	}

	public void Update() {
		if(ps) {
			if(!ps.IsAlive()) {
				Destroy(gameObject);
			}
		}
	}
}
