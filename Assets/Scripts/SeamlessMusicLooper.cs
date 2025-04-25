using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Settings")]
    public AudioClip musicClip;
    public float defaultVolume = 0.7f;

    [Header("UI Volume Control")]
    public Slider volumeSlider;

    private AudioSource audioA;
    private AudioSource audioB;
    private bool isPlayingA = true;
    private double nextStartTime;
    private double clipLength;

    private void Awake()
    {
        // Đảm bảo singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SetupAudioSources();
    }

    private void Start()
    {
        PlayLoopedMusic();

        // Gắn slider nếu có
        if (volumeSlider != null)
        {
            volumeSlider.value = defaultVolume;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    void SetupAudioSources()
    {
        audioA = gameObject.AddComponent<AudioSource>();
        audioB = gameObject.AddComponent<AudioSource>();

        foreach (var audio in new[] { audioA, audioB })
        {
            audio.clip = musicClip;
            audio.playOnAwake = false;
            audio.volume = defaultVolume;
        }

        clipLength = musicClip.length;
    }

    void PlayLoopedMusic()
    {
        nextStartTime = AudioSettings.dspTime + 0.1;
        audioA.PlayScheduled(nextStartTime);
        nextStartTime += clipLength;
        InvokeRepeating(nameof(ScheduleNextLoop), (float)clipLength, (float)clipLength);
    }

    void ScheduleNextLoop()
    {
        if (isPlayingA)
            audioB.PlayScheduled(nextStartTime);
        else
            audioA.PlayScheduled(nextStartTime);

        nextStartTime += clipLength;
        isPlayingA = !isPlayingA;
    }

    public void SetVolume(float volume)
    {
        audioA.volume = volume;
        audioB.volume = volume;
    }

    public float GetVolume()
    {
        return audioA.volume;
    }
}
