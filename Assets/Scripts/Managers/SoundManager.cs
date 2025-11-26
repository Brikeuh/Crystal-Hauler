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
    [Header("Background Music")]
    public List<BackgroundSound> MainMenuBgSounds;
    public List<BackgroundSound> GamePlayBgSounds;

    [Header("Volume Settings")]
    public float MusicVolume;
    public float SoundVolume;

    [Header("Audio Sources")]
    public AudioSource BGSource;
    public AudioSource EffectsSource;
    public AudioSource UISource;

    [Header("Game Sounds")]
    public Sounds[] gameSounds;

    public static SoundManager Instance;

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

    public void UpdateSound(float volume)
    {
        SoundVolume = volume;
        PlayerPrefs.SetFloat("soundValue", volume);
    }

    public void UpdateMusic(float volume)
    {
        MusicVolume = volume;
        BGSource.volume = volume;
        Debug.Log("New Music Volume: " + volume);
    }

    public void PlayButtonSound()
    {
        PlaySound(SoundNames.ButtonClick, SoundType.Effect);
    }

    public void PlaySound(SoundNames name, SoundType type, float volumeScale = 1f, bool isLooping = false)
    {
        // Apply volume scaling for effects and UI
        if (type == SoundType.Effect || type == SoundType.UISource)
        {
            volumeScale *= SoundVolume;
        }

        switch (type)
        {
            case SoundType.BackGround:
                BGSource.clip = gameSounds[(int)name].clip;
                BGSource.loop = isLooping;
                BGSource.Play();
                break;

            case SoundType.Effect:
                EffectsSource.loop = isLooping;
                EffectsSource.volume = volumeScale;
                EffectsSource.clip = gameSounds[(int)name].clip;
                EffectsSource.Play();
                break;

            case SoundType.UISource:
                UISource.volume = volumeScale;
                UISource.clip = gameSounds[(int)name].clip;
                UISource.Play();
                break;
        }
    }

    public void StopSound(SoundType type)
    {
        switch (type)
        {
            case SoundType.BackGround:
                BGSource.Stop();
                break;

            case SoundType.Effect:
                EffectsSource.Stop();
                break;

            case SoundType.UISource:
                UISource.Stop();
                break;
        }
    }

    public void PlayGameplaySound()
    {
        if (GamePlayBgSounds == null || GamePlayBgSounds.Count == 0)
        {
            Debug.LogWarning("No gameplay background sounds assigned!");
            return;
        }

        BackgroundSound randomSound = GamePlayBgSounds[Random.Range(0, GamePlayBgSounds.Count)];
        BGSource.clip = randomSound.ClipName;
        BGSource.volume = randomSound.Volume;
        BGSource.loop = true;
        BGSource.Play();
    }

    public void PlayMainMenuSound()
    {
        if (MainMenuBgSounds == null || MainMenuBgSounds.Count == 0)
        {
            Debug.LogWarning("No main menu background sounds assigned!");
            return;
        }

        BackgroundSound randomSound = MainMenuBgSounds[Random.Range(0, MainMenuBgSounds.Count)];
        BGSource.volume = randomSound.Volume;
        BGSource.clip = randomSound.ClipName;
        BGSource.loop = true;
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
    CrystalPicked,
    CrystalPlaced,
    LevelWon,
    LevelLost,
    PlayerHurt,
    PlayerAttack,
    EnemyHurt,
    EnemyAttack
}

public enum SoundType
{
    BackGround,
    Effect,
    UISource
}