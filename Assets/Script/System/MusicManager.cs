using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MusicManager : PersistentManager<MusicManager>
{
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    [SerializeField] private MusicClipRefsSO musicClipRefsSO;

    private AudioSource backgroundMusicSource;
    private AudioSource gameMusicSource;

    private float volume = 0.3f;
    private float fadeDuration = 1f;

    protected override void Awake()
    {
        base.Awake();

        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.3f);

        backgroundMusicSource = gameObject.AddComponent<AudioSource>();
        gameMusicSource = gameObject.AddComponent<AudioSource>();

        SetupMusicSource(backgroundMusicSource);
        SetupMusicSource(gameMusicSource);


        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void SetupMusicSource(AudioSource source)
    {
        source.loop = true;
        source.playOnAwake = false;
        source.volume = volume;
    }

    private void GameManager_OnStateChanged(GameManager.State oldState, GameManager.State newState)
    {
        switch (newState)
        {
            case GameManager.State.WaitingToStart:
                Debug.Log("GameManager_OnStateChanged: Game is waiting to start, switching to background music.");
                PlayBackgroundMusic();
                break;
            case GameManager.State.GamePlaying:
                if (oldState != GameManager.State.Paused)
                {
                    Debug.Log("GameManager_OnStateChanged: Starting game music.");
                    PlayGameMusic();
                }
                else
                {
                    Debug.Log("GameManager_OnStateChanged: Resuming game music.");
                    gameMusicSource.Play();
                }
                break;
            case GameManager.State.LevelWon:
            case GameManager.State.LevelLost:
                Debug.Log("GameManager_OnStateChanged: Game is over, stopping music.");
                StopAllCoroutines();
                gameMusicSource.Stop();
                break;
            case GameManager.State.Paused:
                Debug.Log("GameManager_OnStateChanged: Game is paused, stopping music.");
                gameMusicSource.Pause();
                break;
        }
    }

    private void PlayBackgroundMusic()
    {
        AudioClip newClip = GetRandomClip(musicClipRefsSO.background);
        if (newClip == null) return;

        StopAllCoroutines();
        StartCoroutine(SwitchMusic(gameMusicSource, backgroundMusicSource, newClip));
        Debug.Log("Playing background music: " + newClip.name);
    }

    private void PlayGameMusic()
    {
        AudioClip newClip = GetRandomClip(musicClipRefsSO.inGame);
        if (newClip == null) return;

        StopAllCoroutines();
        StartCoroutine(SwitchMusic(backgroundMusicSource, gameMusicSource, newClip));
        Debug.Log("Playing game music: " + newClip.name);
    }

    private IEnumerator SwitchMusic(AudioSource fromSource, AudioSource toSource, AudioClip newClip)
    {
        yield return StartCoroutine(FadeOut(fromSource, fadeDuration));
        yield return StartCoroutine(FadeIn(toSource, newClip, fadeDuration));
    }

    private IEnumerator FadeOut(AudioSource source, float duration)
    {
        if (source.isPlaying)
        {
            float startVolume = source.volume;
            float time = 0f;

            while (time < duration)
            {
                source.volume = Mathf.Lerp(startVolume, 0f, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            source.Stop();
            source.volume = volume;
        }
    }

    private IEnumerator FadeIn(AudioSource source, AudioClip clip, float duration)
    {
        source.clip = clip;
        source.volume = 0f;
        source.Play();

        float time = 0f;
        while (time < duration)
        {
            source.volume = Mathf.Lerp(0f, volume, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        source.volume = volume;
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning("No music clips provided.");
            return null;
        }

        return clips[Random.Range(0, clips.Length)];
    }

    public void SetVolume(float newVolume)
    {
        volume = newVolume;

        backgroundMusicSource.volume = volume;
        gameMusicSource.volume = volume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
