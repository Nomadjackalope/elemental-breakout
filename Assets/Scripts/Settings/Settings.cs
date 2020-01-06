using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour {

    public Slider MusicVolume;
    public Slider EffectVolume;
    public Toggle skillIconsOnBot;

    public GameObject LicensePanel;

    void Start() {
        MusicVolume.value = MasterSettings.instance.musicVolume;
        EffectVolume.value = MasterSettings.instance.effectsVolume;

        skillIconsOnBot.isOn = MasterSettings.instance.skillIconsOnBot;

        hideLicenses();
    }

    public void changeMusicVolume(float value) {
        MasterSettings.instance.setMusicVolume(value);
    }

    public void changeEffectsVolume(float value) {
        MasterSettings.instance.setEffectsVolume(value);
    }

    public void SkillBotValueChanged(bool value) {
        MasterSettings.instance.setSkillIconsBot(value);
    }

    public void showLicenses() {
        LicensePanel.SetActive(true);
    }

    public void hideLicenses() {
        LicensePanel.SetActive(false);
    }

    public void OpenPrivacyPolicy() {
        Application.OpenURL("http://www.nomadjackalope.com/ElementalBreakout/EBPrivacyPolicy.htm");
    }

    public void OpenMarkHorton() {
        Application.OpenURL("https://themarkhorton.com/");
    }

    public void OpenAnnaMarie() {
        Application.OpenURL("http://www.annamarieart.net/");
    }
    
}