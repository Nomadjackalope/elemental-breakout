using System.Collections;
using UnityEngine;

public class DeathTrail : MonoBehaviour {

    void OnEnable() {
        StartCoroutine(deactivate());
    }

    IEnumerator deactivate() {
        yield return new WaitForSeconds(3);

        gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "Hex") {
            collider.GetComponent<Hex>().removeHealth(1);
        }
    }

}