using OctoberStudio.Pool;
using OctoberStudio.Save;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace OctoberStudio.Audio
{
    /// <summary>
    /// AudioManager - Example of proper VContainer dependency injection for MonoBehaviours
    ///
    /// Key patterns:
    /// 1. NO static instance or singleton pattern - VContainer handles lifecycle
    /// 2. Dependencies injected via [Inject] Construct method
    /// 3. Initialization in Start() after all dependencies are injected
    /// 4. Registered as component in ProjectLifetimeScope (where it exists in scene)
    /// 5. DontDestroyOnLoad handled by component, not static instance
    ///
    /// This is the template for all manager/service MonoBehaviours in the project.
    /// </summary>
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        public static readonly int BUTTON_CLICK_HASH = "Button Click".GetHashCode();

        [SerializeField] AudioDatabase database;
        [SerializeField] GameObject audioSource;

        public AudioDatabase AudioDatabase => database;
        public AudioSource CurrentMusic => currentMusic;

        private PoolComponent<AudioSource> audioSourcePool;
        private List<AudioData> aliveSources = new List<AudioData>();
        private Dictionary<int, SoundContainer> sounds;
        private AudioSave save;
        private AudioSource currentMusic;

        // Injected dependencies
        private ISaveManager saveManager;

        [Inject]
        public void Construct(ISaveManager saveManager)
        {
            this.saveManager = saveManager;

            // Initialize immediately after injection
            Initialize();
        }

        private void Initialize()
        {
            // Initialize audio source pool
            if (audioSourcePool == null)
            {
                audioSourcePool = new PoolComponent<AudioSource>("audio source", audioSource, 2, null, true);
            }

            // Add AudioListener if needed
            if (GetComponent<AudioListener>() == null)
            {
                gameObject.AddComponent<AudioListener>();
            }

            // Initialize sounds dictionary
            InitializeSounds();

            // Load save data when SaveManager is ready
            if (saveManager != null)
            {
                if (saveManager.IsSaveLoaded)
                {
                    save = saveManager.GetSave<AudioSave>("Audio");
                }
                else
                {
                    // Default values until save is loaded
                    save = new AudioSave();
                    saveManager.OnSaveLoaded += OnSaveLoaded;
                }
            }
            else
            {
                // Fallback for non-DI scenarios
                save = new AudioSave();
            }

            Debug.Log("[AudioManager] Initialized with dependency injection");
        }

        private void OnSaveLoaded()
        {
            if (saveManager != null)
            {
                save = saveManager.GetSave<AudioSave>("Audio");
                saveManager.OnSaveLoaded -= OnSaveLoaded;
            }
        }

        private void InitializeSounds()
        {
            sounds = new Dictionary<int, SoundContainer>();

            for(int i = 0; i < database.Sounds.Count; i++)
            {
                var sound = database.Sounds[i];
                var hash = sound.Name.GetHashCode();

                if (sounds.ContainsKey(hash))
                {
                    Debug.LogError($"Audio clip with the name {sound.Name} has already been added. You should rename one of the entries with this name.");
                } else
                {
                    sound.Init(this); // Pass AudioManager reference to SoundContainer
                    sounds.Add(hash, sound);
                }
            }

            Debug.Log($"[AudioManager] Initialized {sounds.Count} sounds from database");
        }

        public float SoundVolume { 
            get => save.SoundVolume;
            set
            {
                save.SoundVolume = value;
                OnSoundVolumeChanged();
            }
        }

        public float MusicVolume
        {
            get => save.MusicVolume;
            set
            {
                save.MusicVolume = value;
                OnMusicVolumeChanged();
            }
        }

        private void Awake()
        {
            // VContainer will handle lifecycle - no singleton pattern needed
            // Just ensure we persist across scenes
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Initialization is now done in Construct method after dependencies are injected
            // This method is kept for any future Unity-specific setup
        }

        public AudioSource PlaySound(AudioClip clip, float volume = 1, float pitch = 1)
        {
            var source = audioSourcePool.GetEntity();
            source.clip = clip;

            source.loop = false;

            source.pitch = pitch;
            source.volume = volume * save.SoundVolume;

            var data = new AudioData() { source = source, volume = volume };
            aliveSources.Add(data);

            source.Play();

            return source;
        }

        public AudioSource PlaySound(int hash, float volume = 1, float pitch = 1)
        {
            if (sounds.ContainsKey(hash))
            {
                return sounds[hash].Play(false, volume, pitch);
            }

            Debug.LogWarning($"There are no sound with hash {hash} in the AudioDatabase");
            return null;
        }

        public AudioSource PlaySound(AudioClipData clipData, float volume = 1, float pitch = 1)
        {
            var source = audioSourcePool.GetEntity();
            source.clip = clipData.AudioClip;

            source.loop = false;

            source.pitch = clipData.Pitch * pitch;
            source.volume = clipData.Volume * save.SoundVolume * volume;

            var data = new AudioData() { source = source, volume = clipData.Volume };
            aliveSources.Add(data);

            source.Play();

            return source;
        }

        public AudioSource PlayMusic(AudioClipData clipData)
        {
            var source = audioSourcePool.GetEntity();
            source.clip = clipData.AudioClip;

            source.loop = true;

            source.pitch = clipData.Pitch;
            source.volume = clipData.Volume * save.MusicVolume;

            var data = new AudioData() { source = source, volume = clipData.Volume };
            aliveSources.Add(data);

            currentMusic = source;
            source.Play();

            return source;
        }

        public AudioSource PlayMusic(int hash)
        {
            if (sounds == null)
            {
                Debug.LogError("[AudioManager] Sounds dictionary not initialized - cannot play music");
                return null;
            }

            if (sounds.ContainsKey(hash))
            {
                currentMusic = sounds[hash].Play(true);
                return currentMusic;
            }

            Debug.LogWarning($"There are no sound with hash {hash} in the AudioDatabase");
            return null;
        }

        private void OnSoundVolumeChanged()
        {
            foreach(var source in aliveSources)
            {
                if (!source.source.loop)
                {
                    source.source.volume = source.volume * save.SoundVolume;
                }
            }
        }

        private void OnMusicVolumeChanged()
        {
            foreach (var source in aliveSources)
            {
                if (source.source.loop)
                {
                    source.source.volume = source.volume * save.MusicVolume;
                }
            }
        }

        private void Update()
        {
            for(int i = 0; i < aliveSources.Count; i++)
            {
                if (!aliveSources[i].source.isPlaying)
                {
                    aliveSources[i].source.gameObject.SetActive(false);
                    aliveSources.RemoveAt(i);
                    i--;
                }
            }
        }

        public class AudioData
        {
            public AudioSource source;
            public float volume;
        }
    }
}