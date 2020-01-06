using UnityEngine;

// Allows sound effects to have volume controlled

public class MasterEffectsSound : MonoBehaviour {

    public static MasterEffectsSound instance;
    
    public AudioClip buttonClick;
    public AudioSource source;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
	}

    void Start() {
        source = GetComponent<AudioSource>();
    }

    public void playButtonClick() {
        source.PlayOneShot(buttonClick);
    }

    public void PlayOneShot(AudioClip clip) {
        source.PlayOneShot(clip);
    }

}