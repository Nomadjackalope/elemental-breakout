using UnityEngine;


public class CameraSingleton : MonoBehaviour {

    void Start() {
        if(MasterManager.instance.masterCamera != null) {
            gameObject.SetActive(false);
        }
    }
}