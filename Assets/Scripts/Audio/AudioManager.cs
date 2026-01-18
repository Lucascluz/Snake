using UnityEngine;
using System.Collections;

/// <summary>
/// Procedural audio manager - generates all sounds through code, no audio files needed!
/// Creates retro-style 8-bit sound effects and simple background music.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.4f;

    // Audio sources
    private AudioSource sfxSource;
    private AudioSource musicSource;

    // Generated audio clips
    private AudioClip eatSound;
    private AudioClip gameOverSound;
    private AudioClip menuSelectSound;
    private AudioClip menuClickSound;
    private AudioClip portalSound;
    private AudioClip obstacleHitSound;
    private AudioClip countdownBeep;
    private AudioClip backgroundMusic;

    // Constants for audio generation
    private const int SAMPLE_RATE = 44100;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
        GenerateAllSounds();
    }

    private void Start()
    {
        // Don't auto-start music - let scenes control when to play
        // Music is controlled by Game.cs during gameplay
    }

    private void SetupAudioSources()
    {
        // SFX source
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;

        // Music source
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume * masterVolume;
    }

    private void GenerateAllSounds()
    {
        // Generate all sound effects
        eatSound = GenerateEatSound();
        gameOverSound = GenerateGameOverSound();
        menuSelectSound = GenerateMenuSelectSound();
        menuClickSound = GenerateMenuClickSound();
        portalSound = GeneratePortalSound();
        obstacleHitSound = GenerateObstacleHitSound();
        countdownBeep = GenerateCountdownBeep();
        backgroundMusic = GenerateBackgroundMusic();
    }

    #region Sound Effect Generators

    /// <summary>
    /// Eat food - quick rising tone (happy sound)
    /// </summary>
    private AudioClip GenerateEatSound()
    {
        float duration = 0.1f;
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = (float)i / samples;
            
            // Rising frequency from 400Hz to 800Hz
            float freq = Mathf.Lerp(400f, 800f, progress);
            
            // Square wave with envelope
            float envelope = 1f - progress; // Fade out
            float wave = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * freq * t));
            
            data[i] = wave * envelope * 0.3f;
        }

        return CreateClip("EatSound", data);
    }

    /// <summary>
    /// Game over - descending sad tones
    /// </summary>
    private AudioClip GenerateGameOverSound()
    {
        float duration = 0.8f;
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];

        float[] notes = { 440f, 349f, 294f, 220f }; // A4, F4, D4, A3 (descending)
        float noteLength = duration / notes.Length;

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            int noteIndex = Mathf.Min((int)(t / noteLength), notes.Length - 1);
            float noteProgress = (t % noteLength) / noteLength;
            
            float freq = notes[noteIndex];
            float envelope = Mathf.Exp(-noteProgress * 3f); // Decay
            
            // Triangle wave for softer sound
            float phase = (t * freq) % 1f;
            float wave = 4f * Mathf.Abs(phase - 0.5f) - 1f;
            
            data[i] = wave * envelope * 0.4f;
        }

        return CreateClip("GameOverSound", data);
    }

    /// <summary>
    /// Menu select - short blip when hovering/selecting
    /// </summary>
    private AudioClip GenerateMenuSelectSound()
    {
        float duration = 0.05f;
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = (float)i / samples;
            
            float freq = 600f;
            float envelope = 1f - progress;
            float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
            
            data[i] = wave * envelope * 0.25f;
        }

        return CreateClip("MenuSelectSound", data);
    }

    /// <summary>
    /// Menu click - confirmation sound
    /// </summary>
    private AudioClip GenerateMenuClickSound()
    {
        float duration = 0.12f;
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = (float)i / samples;
            
            // Two quick tones
            float freq = progress < 0.5f ? 500f : 700f;
            float envelope = 1f - (progress * 2f % 1f);
            envelope = Mathf.Max(0, envelope);
            
            float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
            
            data[i] = wave * envelope * 0.3f;
        }

        return CreateClip("MenuClickSound", data);
    }

    /// <summary>
    /// Portal teleport - wobbly sci-fi sound
    /// </summary>
    private AudioClip GeneratePortalSound()
    {
        float duration = 0.25f;
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = (float)i / samples;
            
            // Wobbling frequency
            float freq = 300f + Mathf.Sin(t * 40f) * 150f;
            float envelope = Mathf.Sin(progress * Mathf.PI); // Fade in and out
            
            float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
            
            data[i] = wave * envelope * 0.35f;
        }

        return CreateClip("PortalSound", data);
    }

    /// <summary>
    /// Obstacle hit - short thud
    /// </summary>
    private AudioClip GenerateObstacleHitSound()
    {
        float duration = 0.15f;
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = (float)i / samples;
            
            // Descending frequency for impact
            float freq = Mathf.Lerp(200f, 60f, progress);
            float envelope = Mathf.Exp(-progress * 8f);
            
            // Noise + tone for impact feel
            float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
            float noise = (Random.value * 2f - 1f) * 0.3f;
            
            data[i] = (wave + noise * (1f - progress)) * envelope * 0.4f;
        }

        return CreateClip("ObstacleHitSound", data);
    }

    /// <summary>
    /// Countdown beep
    /// </summary>
    private AudioClip GenerateCountdownBeep()
    {
        float duration = 0.08f;
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float progress = (float)i / samples;
            
            float freq = 880f; // A5
            float envelope = 1f - progress;
            float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
            
            data[i] = wave * envelope * 0.25f;
        }

        return CreateClip("CountdownBeep", data);
    }

    #endregion

    #region Background Music Generator

    /// <summary>
    /// Generate simple looping chiptune-style background music
    /// </summary>
    private AudioClip GenerateBackgroundMusic()
    {
        float bpm = 120f;
        float beatDuration = 60f / bpm;
        int bars = 8;
        int beatsPerBar = 4;
        float duration = bars * beatsPerBar * beatDuration;
        int samples = (int)(SAMPLE_RATE * duration);
        float[] data = new float[samples];

        // Simple chord progression: C - Am - F - G (I - vi - IV - V)
        float[][] chordFreqs = new float[][]
        {
            new float[] { 130.81f, 164.81f, 196f },      // C (C3, E3, G3)
            new float[] { 110f, 130.81f, 164.81f },      // Am (A2, C3, E3)
            new float[] { 87.31f, 110f, 130.81f },       // F (F2, A2, C3)
            new float[] { 98f, 123.47f, 146.83f }        // G (G2, B2, D3)
        };

        // Bass line notes
        float[] bassNotes = { 65.41f, 55f, 43.65f, 49f }; // C2, A1, F1, G1

        // Simple melody (quarter notes)
        float[] melody = { 523.25f, 587.33f, 659.25f, 523.25f, 493.88f, 440f, 392f, 440f,
                          349.23f, 392f, 440f, 349.23f, 392f, 440f, 493.88f, 523.25f };

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / SAMPLE_RATE;
            float beatPosition = t / beatDuration;
            int currentBar = (int)(beatPosition / beatsPerBar) % bars;
            int chordIndex = (currentBar / 2) % 4; // Change chord every 2 bars
            float beatInBar = beatPosition % beatsPerBar;

            float sample = 0f;

            // Pad/Chord layer (soft sustained chords)
            float[] chord = chordFreqs[chordIndex];
            foreach (float freq in chord)
            {
                float chordWave = Mathf.Sin(2f * Mathf.PI * freq * t);
                sample += chordWave * 0.08f;
            }

            // Bass layer (plays on beats 1 and 3)
            float bassFreq = bassNotes[chordIndex];
            float bassEnvelope = 0f;
            float beatFrac = beatInBar % 1f;
            if (beatInBar < 1f || (beatInBar >= 2f && beatInBar < 3f))
            {
                bassEnvelope = Mathf.Exp(-beatFrac * 4f);
            }
            float bassWave = Mathf.Sin(2f * Mathf.PI * bassFreq * t);
            sample += bassWave * bassEnvelope * 0.15f;

            // Simple arpeggio layer
            int arpIndex = (int)(beatPosition * 2) % 3; // 8th note arpeggios
            float arpFreq = chord[arpIndex] * 2f; // One octave up
            float arpEnvelope = Mathf.Exp(-(beatPosition * 2 % 1f) * 6f);
            float arpWave = Mathf.Sin(2f * Mathf.PI * arpFreq * t);
            sample += arpWave * arpEnvelope * 0.06f;

            // Hi-hat layer (8th notes)
            float hihatEnvelope = Mathf.Exp(-(beatPosition * 2 % 1f) * 20f);
            float hihat = (Random.value * 2f - 1f) * hihatEnvelope * 0.03f;
            sample += hihat;

            data[i] = Mathf.Clamp(sample, -1f, 1f);
        }

        return CreateClip("BackgroundMusic", data);
    }

    #endregion

    #region Helper Methods

    private AudioClip CreateClip(string name, float[] data)
    {
        AudioClip clip = AudioClip.Create(name, data.Length, 1, SAMPLE_RATE, false);
        clip.SetData(data, 0);
        return clip;
    }

    #endregion

    #region Public Methods - Play Sounds

    public void PlayEatSound()
    {
        PlaySFX(eatSound);
    }

    public void PlayGameOverSound()
    {
        PlaySFX(gameOverSound);
        // Optionally lower music volume during game over
        StartCoroutine(FadeMusicVolume(0.2f, 0.5f));
    }

    public void PlayMenuSelectSound()
    {
        PlaySFX(menuSelectSound);
    }

    public void PlayMenuClickSound()
    {
        PlaySFX(menuClickSound);
    }

    public void PlayPortalSound()
    {
        PlaySFX(portalSound);
    }

    public void PlayObstacleHitSound()
    {
        PlaySFX(obstacleHitSound);
    }

    public void PlayCountdownBeep()
    {
        PlaySFX(countdownBeep);
    }

    public void PlayBackgroundMusic()
    {
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public bool IsMusicPlaying()
    {
        return musicSource != null && musicSource.isPlaying;
    }

    public void PauseBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.Pause();
        }
    }

    public void ResumeBackgroundMusic()
    {
        if (musicSource != null)
        {
            musicSource.UnPause();
        }
    }

    private void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
        }
    }

    private IEnumerator FadeMusicVolume(float targetVolume, float duration)
    {
        float startVolume = musicSource.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume * masterVolume, elapsed / duration);
            yield return null;
        }

        musicSource.volume = targetVolume * masterVolume;
    }

    public void RestoreMusicVolume()
    {
        StartCoroutine(FadeMusicVolume(musicVolume, 0.5f));
    }

    #endregion

    #region Volume Controls

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }

    public void ToggleMute()
    {
        if (masterVolume > 0)
        {
            SetMasterVolume(0);
        }
        else
        {
            SetMasterVolume(0.7f);
        }
    }

    #endregion
}
