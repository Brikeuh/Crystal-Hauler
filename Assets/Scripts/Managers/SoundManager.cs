using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundSound
{
    public AudioClip ClipName;
    public float Volume;
}

public class SoundManager : MonoBehaviour
{
    public List<BackgroundSound> MainMenuBgSounds;
    public List<BackgroundSound> GamePlayBgSounds;
    public float MusicVolume;
    public float SoundVolume;
    public static SoundManager Instance;
    public AudioSource BGSource, EffectsSource, UISource;
    public Sounds[] gameSounds;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        SoundVolume = PlayerPrefs.GetFloat("soundValue", 1);
    }

    private void OnEnable() { }
    private void OnDisable() { }

    public void UpdateSound(float v)
    {
        SoundVolume = v;
    }

    public void UpdateMusic(float v)
    {
        Debug.Log("NEW MUSIC:" + v);
    }

    public void PlayButtonSound()
    {
        PlaySound(SoundNames.ButtonClick, SoundType.Effect);
    }

    public void OnGameStart() { }

    public void PlaySound(SoundNames name, SoundType type, float volumeScale = 1f, bool isLooping = false)
    {
        if (type == SoundType.Effect || type == SoundType.UISource)
            volumeScale = volumeScale * SoundVolume;

        if (type == SoundType.BackGround)
        {
            BGSource.clip = gameSounds[(int)name].clip;
            BGSource.loop = isLooping;
            BGSource.Play();
        }
        else if (type == SoundType.Effect)
        {
            EffectsSource.loop = isLooping;
            EffectsSource.clip = gameSounds[(int)name].clip;
            EffectsSource.Play();
        }
        else if (type == SoundType.UISource)
        {
            UISource.clip = gameSounds[(int)name].clip;
            UISource.Play();
        }
    }

    private void Update() { }

    public void StopSound(SoundType type)
    {
        if (type == SoundType.BackGround) BGSource.Stop();
        else if (type == SoundType.Effect) EffectsSource.Stop();
        else if (type == SoundType.UISource) UISource.Stop();
    }

    public void PlayGameplaySound()
    {
        if (GamePlayBgSounds == null || GamePlayBgSounds.Count == 0) return;
        BackgroundSound c = GamePlayBgSounds[UnityEngine.Random.Range(0, GamePlayBgSounds.Count)];
        BGSource.clip = c.ClipName;
        BGSource.volume = c.Volume;
        BGSource.Play();
    }

    public void PlayMainMenuSound()
    {
        if (MainMenuBgSounds == null || MainMenuBgSounds.Count == 0) return;
        BackgroundSound c = MainMenuBgSounds[UnityEngine.Random.Range(0, MainMenuBgSounds.Count)];
        BGSource.volume = c.Volume;
        BGSource.clip = c.ClipName;
        BGSource.Play();
    }
}

[System.Serializable]
public struct Sounds
{
    public SoundNames name;
    public AudioClip clip;
}

public enum SoundNames
{
    ButtonClick,
    LevelFail,
    LevelComplete,
    CrystalPicked,
    CrystalPlaced,
BuzzerSound
}

public enum SoundType
{
    BackGround,
    Effect,
    UISource,
    Collision
}
