using OctoberStudio.Pool;
using OctoberStudio.Save;
using System.Collections.Generic;
using UnityEngine;

namespace OctoberStudio.Audio
{
    /// <summary>
    /// AudioService - Pure C# implementation of IAudioManager
    /// Created and managed entirely by VContainer
    /// This is the gold standard for DI services
    /// </summary>
    public class AudioService : IAudioManager
    {
        public static readonly int BUTTON_CLICK_HASH = "Button Click".GetHashCode();

        private readonly ISaveManager saveManager;
        private readonly AudioDatabase database;
        private readonly GameObject audioSourcePrefab;

        private PoolComponent<AudioSource> audioSourcePool;
        private List<AudioData> aliveSources = new List<AudioData>();
        private Dictionary<int, SoundContainer> sounds;
        private AudioSave save;
        private Transform poolParent;

        public AudioDatabase AudioDatabase => database;

        public AudioSource CurrentMusic
        {
            get
            {
                // Find the first looped (music) source that's playing
                foreach (var data in aliveSources)
                {
                    if (data.source.loop && data.source.isPlaying)
                    {
                        return data.source;
                    }
                }
                return null;
            }
        }

        public float SoundVolume
        {
            get => save?.SoundVolume ?? 1f;
            set
            {
                if (save != null)
                {
                    save.SoundVolume = value;
                    OnSoundVolumeChanged();
                }
            }
        }

        public float MusicVolume
        {
            get => save?.MusicVolume ?? 1f;
            set
            {
                if (save != null)
                {
                    save.MusicVolume = value;
                    OnMusicVolumeChanged();
                }
            }
        }

        public AudioService(ISaveManager saveManager, AudioDatabase database, GameObject audioSourcePrefab)
        {
            this.saveManager = saveManager;
            this.database = database;
            this.audioSourcePrefab = audioSourcePrefab;

            Initialize();
        }

        private void Initialize()
        {
            // Create a parent GameObject for the audio system
            var audioSystemGO = new GameObject("[AudioSystem]");
            Object.DontDestroyOnLoad(audioSystemGO);
            poolParent = audioSystemGO.transform;

            // Add AudioListener
            audioSystemGO.AddComponent<AudioListener>();

            // Initialize pool
            audioSourcePool = new PoolComponent<AudioSource>("audio source", audioSourcePrefab, 2, poolParent, true);

            // Initialize sounds
            InitializeSounds();

            // Load save data
            if (saveManager != null)
            {
                if (saveManager.IsSaveLoaded)
                {
                    save = saveManager.GetSave<AudioSave>("Audio");
                }
                else
                {
                    save = new AudioSave();
                    saveManager.OnSaveLoaded += OnSaveLoaded;
                }
            }
            else
            {
                save = new AudioSave();
            }

            // Debug.Log("[AudioService] Initialized via dependency injection");
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

            if (database?.Sounds != null)
            {
                for (int i = 0; i < database.Sounds.Count; i++)
                {
                    var sound = database.Sounds[i];
                    var hash = sound.Name.GetHashCode();

                    if (sounds.ContainsKey(hash))
                    {
                        Debug.LogError($"Audio clip with the name {sound.Name} has already been added");
                    }
                    else
                    {
                        sound.Init(this);
                        sounds.Add(hash, sound);
                    }
                }

                // Debug.Log($"[AudioService] Initialized {sounds.Count} sounds from database");
            }
        }

        public AudioSource PlaySound(AudioClip clip, float volume = 1, float pitch = 1)
        {
            var source = audioSourcePool.GetEntity();
            source.clip = clip;
            source.loop = false;
            source.pitch = pitch;
            source.volume = volume * SoundVolume;

            var data = new AudioData() { source = source, volume = volume };
            aliveSources.Add(data);

            source.Play();
            return source;
        }

        public AudioSource PlaySound(int hash, float volume = 1, float pitch = 1)
        {
            if (sounds != null && sounds.ContainsKey(hash))
            {
                return sounds[hash].Play(false, volume, pitch);
            }

            Debug.LogWarning($"There is no sound with hash {hash} in the AudioDatabase");
            return null;
        }

        public AudioSource PlaySound(AudioClipData clipData, float volume = 1, float pitch = 1)
        {
            var source = audioSourcePool.GetEntity();
            source.clip = clipData.AudioClip;
            source.loop = false;
            source.pitch = clipData.Pitch * pitch;
            source.volume = clipData.Volume * SoundVolume * volume;

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
            source.volume = clipData.Volume * MusicVolume;

            var data = new AudioData() { source = source, volume = clipData.Volume };
            aliveSources.Add(data);

            source.Play();
            return source;
        }

        public AudioSource PlayMusic(int hash)
        {
            if (sounds == null)
            {
                Debug.LogError("[AudioService] Sounds dictionary not initialized");
                return null;
            }

            if (sounds.ContainsKey(hash))
            {
                return sounds[hash].Play(true);
            }

            Debug.LogWarning($"There is no sound with hash {hash} in the AudioDatabase");
            return null;
        }

        private void OnSoundVolumeChanged()
        {
            foreach (var source in aliveSources)
            {
                if (!source.source.loop)
                {
                    source.source.volume = source.volume * SoundVolume;
                }
            }
        }

        private void OnMusicVolumeChanged()
        {
            foreach (var source in aliveSources)
            {
                if (source.source.loop)
                {
                    source.source.volume = source.volume * MusicVolume;
                }
            }
        }

        public void Update()
        {
            for (int i = 0; i < aliveSources.Count; i++)
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