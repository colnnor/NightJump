using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using Eflatun.SceneReference;
using UnityEditor;

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
    private static AudioManager instance;
    public static AudioManager Instance
    { 
        get
        {
            if(instance == null)
            {
                instance = FindFirstObjectByType<AudioManager>();
                if(instance == null)
                {
#if UNITY_EDITOR
                    GameObject go = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/Audio Manager.prefab"));
                    go.name = "AudioManger_autocreated";

                    instance = go.GetComponent<AudioManager>();
#else
                    throw new System.Exception("No AudioManager found in scene");
#endif

                }
            }
            return instance;
        }
    }
    [Title("Settings")]
    [SerializeField] private float crossFadeDuration;
    [SerializeField] private AudioSettings audioSettings;
    [SerializeField] private bool playMusicOnAwake;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] bool overrideMixer;

    [Title("Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSourceA;
    [SerializeField] private AudioSource musicSourceB;

    [Title("Clips")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip gamePlayMusicClip;
    [SerializeField] private AudioClip buttonClickClipPositive;
    [SerializeField] private AudioClip buttonClickClipNegative;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip enemyJumpClip;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip collectClip;
    [SerializeField] private AudioClip enemyCollectClip;

    private AudioSource currentSource;
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
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            Debug.LogWarning("Multiple instances of AudioManager detected. Destroying duplicate...");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);  
    }

    private void OnEnable()
    {
        LevelManager.OnSceneLoaded += SceneLoaded;
        GameManager.OnGameResume += ResetAudio;
        GameManager.OnGamePause += PausedAudio;
    }
    private void OnDisable()
    {
        LevelManager.OnSceneLoaded -= SceneLoaded;
        GameManager.OnGameResume -= ResetAudio;
        GameManager.OnGamePause -= PausedAudio;
    }
    private void Start()
    {
        currentSource = musicSourceA;
        if (playMusicOnAwake)
        { 
            PlayMusic(musicClip); 
        }

        InitializeDictionary();
        ResetSourceVolumes();
    }

    void SceneLoaded(SceneType scene)
    {
        AudioClip clip = musicClip;
        
        if(scene == SceneType.GamePlay)
            clip = gamePlayMusicClip;
        

        CrossFade(clip, crossFadeDuration);
        
        ResetAudio();
    }


    private void ResetAudio()
    {
        ResetLowpass();
        audioMixer.SetFloat(MASTER_VOLUME, audioSettings.MasterVolume);
    }


    private void PausedAudio()
    {
        EnforceLowpass();
        float volume = audioSettings.MasterVolume < 0 ? audioSettings.MasterVolume * 2 : audioSettings.MasterVolume / 2;
        audioMixer.SetFloat(MASTER_VOLUME, volume);
    }

    private void ResetLowpass()
    {
        audioMixer.SetFloat(LOWPASS_CUTOFF, LOWPASS_NORMAL_VALUE);
    }

    private void EnforceLowpass()
    {
        audioMixer.SetFloat(LOWPASS_CUTOFF, LOWPASS_CUTTOFF_VALUE);
    }

    public void CrossFade(AudioClip clipToFadeTo, float fadeDuration)
    {
        Debug.Log($"Fading to {clipToFadeTo.name} in {fadeDuration} seconds");
        AudioSource fadeInSource = currentSource == musicSourceA ? musicSourceB : musicSourceA;
        AudioSource fadeOutSource = currentSource;

        fadeInSource.clip = clipToFadeTo;
        fadeInSource.volume = 0;
        fadeInSource.Play();

        fadeInSource.DOFade(audioSettings.MusicVolume, fadeDuration);
        currentSource.DOFade(0, fadeDuration).OnComplete(() => fadeOutSource.Stop());
        
        currentSource = fadeInSource;



        //fade out current
        //fade in new based on settings volume
        //update current music source
        //stop old
    }

    public void ResetSourceVolumes()
    {
        if (!overrideMixer) return;

        audioSettings.MasterVolume = 0;
        InitializeSourceVolumes();
    }
    

    void InitializeSourceVolumes()
    { 
        sfxSource.volume = audioSettings.SFXVolume;
        musicSourceA.volume = audioSettings.MusicVolume;
        musicSourceB.volume = audioSettings.MusicVolume;

        audioMixer.SetFloat(MASTER_VOLUME, audioSettings.MasterVolume);
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
            .SetVolume(volume ?? audioSettings.SFXVolume)
            .SetRandomPitch(randomPitch)
            .Play();
    }


    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1.0f)
    {
        currentSource.clip = clip;
        currentSource.loop = loop;
        currentSource.volume = volume;
        currentSource.Play();
    }

    #region change volume
    public void UpdateMusicVolume(float volume)
    {
        if (!overrideMixer) return;

        audioSettings.MusicVolume = volume;
        musicSourceA.volume = volume;
        musicSourceB.volume = volume;
    }

    public void UpdateSFXVolume(float volume)
    {
        if (!overrideMixer) return;

        audioSettings.SFXVolume = volume;
        sfxSource.volume = volume;
    }

    public void UpdateMasterVolume(float volume)
    {
        if (!overrideMixer) return;

        audioSettings.MasterVolume = volume;
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
