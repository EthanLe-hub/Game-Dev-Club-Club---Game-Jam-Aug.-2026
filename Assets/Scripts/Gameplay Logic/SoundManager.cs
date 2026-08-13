using System.Collections;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] AudioSource musicSource; // Looping background music.
    [SerializeField] AudioSource sfxSource;   // One-shot sound effects.

    [Header("Music")]
    [SerializeField] AudioClip defaultMusic;  // Title/menu track - starts playing on load.
    [SerializeField] AudioClip dayMusic;      // Played from DayStart (GameManager calls PlayDayMusic).
    [SerializeField] AudioClip nightMusic;    // Played from NightStart (GameManager calls PlayNightMusic).
    [SerializeField] float musicVolume = 0.4f; // Music sits UNDER gameplay - keep well below the SFX volume.
    [SerializeField] float sfxVolume = 0.25f;     // SFX sits OVER gameplay - keep at full volume.
    [SerializeField] float musicFadeSeconds = 1f; // Crossfade length when switching tracks.

    Coroutine musicFade; // The in-progress track switch, so a new switch can cancel it.

    [Header("Footsteps")]
    [SerializeField] AudioClip[] footstepClips; // A few variations; one is picked at random per step.
    [SerializeField] float footstepInterval = 0.45f; // Seconds between steps while walking.
    [SerializeField] float footstepVolume = 0.8f;

    [SerializeField] AudioClip imposterKills; 
    [SerializeField] AudioClip lockUpClip;

    float lastFootstepTime;

    void Awake()
    {
        if (Instance == null) // Create a new static instance if game was loaded up for the first time.
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Do not restart the music between scene reloads.
        }
        else // If an instance already exists, do not create a duplicate (destroy the new one, keep the old).
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicSource != null)
        {
            musicSource.volume = musicVolume; // The field is the single source of truth for music loudness.
        }

        if (musicSource != null && defaultMusic != null && !musicSource.isPlaying)
        {
            PlayMusic(defaultMusic);
        }
    }

    public void PlayDayMusic() => PlayMusic(dayMusic);
    public void PlayNightMusic() => PlayMusic(nightMusic);

    // Starts (or fades over to) a looping track. No-op if that track is already playing,
    // so music keeps running seamlessly across scene loads:
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        if (musicFade != null) StopCoroutine(musicFade);

        if (!musicSource.isPlaying)
        {
            // Nothing playing yet - just start at full music volume:
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource.Play();
            return;
        }

        musicFade = StartCoroutine(FadeToTrack(clip));
    }

    // Fades the current track out, swaps clips, and fades the new one in.
    // Uses UNSCALED time so the fade still runs while the game is paused (timeScale = 0):
    IEnumerator FadeToTrack(AudioClip clip)
    {
        float half = musicFadeSeconds * 0.5f;

        for (float t = 0; t < half; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(musicVolume, 0f, t / half);
            yield return null;
        }

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();

        for (float t = 0; t < half; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, musicVolume, t / half);
            yield return null;
        }

        musicSource.volume = musicVolume;
        musicFade = null;
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    // General one-shot sound effect (door knock, UI clicks, etc):
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayImposterKillsSFX()
    {
        if (sfxSource == null || imposterKills == null) return;
        sfxSource.PlayOneShot(imposterKills, sfxVolume);
    }

    // Called every frame while the player is walking (see Player.cs) - rate-limits itself
    // to one step per footstepInterval and picks a random clip for variation:
    public void PlayFootstep()
    {
        if (sfxSource == null || footstepClips == null || footstepClips.Length == 0) return;
        if (Time.time < lastFootstepTime + footstepInterval) return;

        lastFootstepTime = Time.time;
        sfxSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)], footstepVolume);
    }

    public void PlayLockUpSFX()
    {
        if (sfxSource == null || lockUpClip == null) return;
        sfxSource.PlayOneShot(lockUpClip, sfxVolume);
    }
}
