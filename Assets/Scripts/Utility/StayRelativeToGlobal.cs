using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayRelativeToGlobal : MonoBehaviour {

    Vector3 globalOriginPosition;

    void Start() {
        globalOriginPosition = transform.position;
    }

    void Update() {
        if(transform.position != globalOriginPosition) {
            transform.SetPositionAndRotation(globalOriginPosition, transform.rotation);
        }
    }
}