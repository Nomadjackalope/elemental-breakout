﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeleteAudio : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy(gameObject, GetComponent<AudioSource>().clip.length);
	}
}
