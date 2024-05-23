using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public enum SFXType
{
    ButtonClickPositive,    
    ButtonClickNegative,
    Jump,
    EnemyJump,
    Damage,
    Death,
    Collect,
    EnemyCollect
}


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] bool overrideMixer;
    [SerializeField] private AudioMixer audioMixer;

    [Title("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Title("Clips")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip buttonClickClipPositive;
    [SerializeField] private AudioClip buttonClickClipNegative;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip enemyJumpClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioClip enemyCollectClip;

    [Title("Settings")]
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private bool playMusicOnAwake;
    private Dictionary<SFXType, AudioClip> clips = new();

    private const string MASTER_VOLUME = "MasterVolume";
    private const string LOWPASS_CUTOFF = "LowpassCutoff";
    private const float LOWPASS_NORMAL_VALUE = 22000f;
    private const float LOWPASS_CUTTOFF_VALUE = 5000f;

    private AudioSource SFXSource
    {
        get
        {
            if(!sfxSource) sfxSource = gameObject.AddComponent<AudioSource>();
            return sfxSource;
        }
    }
    private AudioSource MusicSource
    {
        get
        {
            if(!musicSource) musicSource = gameObject.AddComponent<AudioSource>();
            return musicSource;
        }
    }
    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple instances of AudioManager detected. Destroying duplicate...");
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);  
    }

    private void OnEnable()
    {
        GameManager.OnGameResume += Resume;
        GameManager.OnGamePause += Pause;
    }

    private void Resume()
    {
        ResetLowpass();
        audioMixer.SetFloat(MASTER_VOLUME, audioSettings.masterVolume);
    }


    private void Pause()
    {
        EnforceLowpass();
        float volume = audioSettings.masterVolume < 0 ? audioSettings.masterVolume * 2 : audioSettings.masterVolume / 2;
        audioMixer.SetFloat(MASTER_VOLUME, volume);
    }

    private void OnDisable()
    {
        GameManager.OnGameResume -= Resume;
        GameManager.OnGamePause -= Pause;
    }
    private void ResetLowpass()
    {
        audioMixer.SetFloat(LOWPASS_CUTOFF, LOWPASS_NORMAL_VALUE);
    }

    private void EnforceLowpass()
    {
        audioMixer.SetFloat(LOWPASS_CUTOFF, LOWPASS_CUTTOFF_VALUE);
    }

    private void Start()
    {
        if (playMusicOnAwake)
        { 
            PlayMusic(musicClip); 
        }


        InitializeDictionary();
        ResetSourceVolumes();
    }

    public void ResetSourceVolumes()
    {
        if (!overrideMixer) return;

        audioSettings.sfxVolume = 1.0f;
        audioSettings.musicVolume = 1.0f;
        audioSettings.masterVolume = 0;
        InitializeSourceVolumes();
    }
    

    void InitializeSourceVolumes()
    { 
        sfxSource.volume = audioSettings.sfxVolume;
        musicSource.volume = audioSettings.musicVolume;

        audioMixer.SetFloat(MASTER_VOLUME, audioSettings.masterVolume);
        ResetLowpass();
    }
    private void InitializeDictionary()
    {
        clips.Add(SFXType.ButtonClickPositive, buttonClickClipPositive);
        clips.Add(SFXType.ButtonClickNegative, buttonClickClipNegative);
        clips.Add(SFXType.Jump, jumpClip);
        clips.Add(SFXType.EnemyJump, enemyJumpClip);
        clips.Add(SFXType.Damage, hurtClip);
        clips.Add(SFXType.Death, deathClip);
        clips.Add(SFXType.Collect, collectClip);
        clips.Add(SFXType.EnemyCollect, enemyCollectClip);
    }

    public void PlayOneShot(SFXType type, float? volume = null, Vector3? location = null, bool randomPitch = false)
    {
        if (!clips.TryGetValue(type, out AudioClip clip)) throw new System.Exception("Audio clip not found for type");
        new Builder(SFXSource)
            .SetClip(clip)
            .SetLocation(location ?? Vector3.zero)
            .SetVolume(volume ?? audioSettings.sfxVolume)
            .SetRandomPitch(randomPitch)
            .Play();
    }


    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1.0f)
    {
        MusicSource.clip = clip;
        MusicSource.loop = loop;
        MusicSource.volume = volume;
        MusicSource.Play();
    }

    #region change volume
    public void UpdateMusicVolume(float volume)
    {
        if (!overrideMixer) return;

        audioSettings.musicVolume = volume;
        musicSource.volume = volume;
    }

    public void UpdateSFXVolume(float volume)
    {
        if (!overrideMixer) return;

        audioSettings.sfxVolume = volume;
        sfxSource.volume = volume;
    }

    public void UpdateMasterVolume(float volume)
    {
        if (!overrideMixer) return;

        audioSettings.masterVolume = volume;
        audioMixer.SetFloat(MASTER_VOLUME, volume);
    }
#endregion

    public class Builder
    {
        private AudioSource source;
        private AudioClip clip;
        private Vector3? location;
        private float volume = 1.0f;
        private bool randomPitch = false;

        public Builder(AudioSource source)
        {
            this.source = source;
        }
        public Builder SetClip(AudioClip clip)
        {
            this.clip = clip;
            return this;
        }
        public Builder SetLocation(Vector3 location)
        {
            this.location = location;
            return this;
        }
        public Builder SetVolume(float volume)
        {
            this.volume = volume;
            return this;
        }
        public Builder SetRandomPitch(bool randomPitch)
        {
            this.randomPitch = randomPitch;
            return this;
        }


        public void Play()
        {
            if(!clip) throw new System.Exception($"You must set a clip before calling {nameof(Play)}");
            
            source.transform.position = location ?? Vector3.zero;
            source.pitch = randomPitch ? Random.Range(.95f, 1.05f) : 1;
            source.PlayOneShot(clip, volume);
        }
    
    }
}
