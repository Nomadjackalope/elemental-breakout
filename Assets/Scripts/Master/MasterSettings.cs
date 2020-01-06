using UnityEngine;

public class MasterSettings : MonoBehaviour {

    public static MasterSettings instance;

    public float musicVolume;
    public float effectsVolume;

    public bool skillIconsOnBot = true;
    
    void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
	}

    void Start() {
        // load settings from player prefs;
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
        effectsVolume = PlayerPrefs.GetFloat("effectsVolume", 1.0f);
        skillIconsOnBot = PlayerPrefs.GetInt("skillIconsOnBot", 1) == 1 ? true : false;

        MasterMusic.instance.setVolume(musicVolume);
        MasterEffectsSound.instance.source.volume = effectsVolume;
    }

    public void setMusicVolume(float value) {
        musicVolume = Mathf.Min(value, 1.0f);
        MasterMusic.instance.setVolume(musicVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }

    public void setEffectsVolume(float value) {
        effectsVolume = Mathf.Min(value, 1.0f);
        MasterEffectsSound.instance.source.volume = effectsVolume;
        PlayerPrefs.SetFloat("effectsVolume", effectsVolume);
    }

    public void setSkillIconsBot(bool value) {
        skillIconsOnBot = value;
        PlayerPrefs.SetInt("skillIconsOnBot", value ? 1 : 0);
    }
}