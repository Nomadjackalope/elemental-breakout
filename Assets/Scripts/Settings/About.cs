using UnityEngine;
using UnityEngine.UI;

public class About : MonoBehaviour {


    void Start() {
        
    }

    public void OnMarkLinkClicked() {
        Application.OpenURL("https://themarkhorton.com/");
    }
    
}