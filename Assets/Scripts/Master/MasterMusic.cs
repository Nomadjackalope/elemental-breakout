using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MasterMusic : MonoBehaviour {

    public static MasterMusic instance;

    private SceneAudio sceneAudio;

    public AudioSource introSource;
    public AudioSource loopSource;

    public BiomeAudioList biomeAudioList;
    public SceneAudio menuAudio;

    void Awake() {
        if(instance == null) {
            instance = this;
        } else if(instance != this) {
            Destroy(gameObject);
        }
	}

    private void switchAudio(SceneAudio audio) {
        if(sceneAudio != audio) {
            //StartCoroutine(switchToNextClip(audio.intro));
            StartCoroutine(switchToNextScene(audio));
            sceneAudio = audio;
            
        }
    }

    IEnumerator switchToNextScene(SceneAudio audio) {
        yield return StartCoroutine(switchToNextClip(audio.intro));
        loopSource.clip = audio.loop;
        loopSource.loop = true;
        loopSource.PlayScheduled(AudioSettings.dspTime + audio.intro.length);

    }

    IEnumerator switchToNextClip(AudioClip clip) {
        AudioSource source = introSource.isPlaying ? introSource : loopSource;

        if(source.isPlaying) {
            yield return StartCoroutine(fadeOut());
        }

        introSource.volume = MasterSettings.instance.musicVolume; // needs to be whatever user chose
        introSource.clip = clip;
        introSource.loop = false;
        introSource.Play();
    }

    IEnumerator fadeOut() {
        AudioSource source = introSource.isPlaying ? introSource : loopSource;

        float volume = source.volume;
        for (float i = volume; i >= 0; i -= 0.05f)
        {
            source.volume = i;
            yield return new WaitForSeconds(0.01f);
        }
        source.Stop();

        introSource.volume = MasterSettings.instance.musicVolume;
        loopSource.volume = MasterSettings.instance.musicVolume;
    }

    public void switchMusic(string sceneName, GenericDictionary message) {
		if(MasterManager.isMenu(sceneName)) {
			switchAudio(menuAudio);
            return;
		}

        if(message == null) {
            Debug.LogError("Message is null and we are supposed to play biome music");
            return;
        }

		object levelId = message.GetValue<object>("levelId");
		object biome = message.GetValue<object>("biome");
		if(levelId != null && biome != null) {
			LevelData levelData = LevelLoader.instance.LoadLevelData((int) levelId, (DataTypes.BiomeType) biome);

			if(levelData != null) {
                if(levelData.isBossLevel) {
                    switchAudio(biomeAudioList.bossAudio);
                } else {
                    switchAudio(biomeAudioList.GetAudioForBiome((DataTypes.BiomeType) biome));
                }
			}
		}
	}

    public void setVolume(float volume) {
        introSource.volume = volume;
        loopSource.volume = volume;
    }
    
}