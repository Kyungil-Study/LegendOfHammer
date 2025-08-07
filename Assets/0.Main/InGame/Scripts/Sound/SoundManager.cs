using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("효과음 오디오 소스")]
    private AudioSource sfxSource;
    [Header("BGM 오디오 소스")]
    private AudioSource bgmSource;


    [Header("효과음 리스트")]
    public AudioClip buttonClick;
    public AudioClip archerHit;
    public AudioClip magicianHit;
    public AudioClip miss;
    public AudioClip warriorDash;
    public AudioClip playerDamaged;
    public AudioClip warning;

    public AudioClip gameBgm1;
    public AudioClip gameBgm2;
    public AudioClip gameBgm3;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 효과음용 AudioSource
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // 배경음악용 AudioSource
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.playOnAwake = false;
        bgmSource.loop = true;
        bgmSource.volume = 0.5f;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("SoundManager: 재생하려는 오디오 클립이 비어있음");
        }
    }

    /// 효과음을 이름으로 재생 (원한다면 switch로 확장 가능)
    public void PlayButtonClick() => PlaySFX(buttonClick);
    public void PlayArcherHit() => PlaySFX(archerHit); // ㅇㅇ
    public void PlayMagicianHit() => PlaySFX(magicianHit); // ㅇㅇ
    public void PlayWarning() => PlaySFX(warning);
    public void PlayWarriorDash() => PlaySFX(warriorDash); // DD
    public void PlayPlayerDamaged() => PlaySFX(playerDamaged);
    public void PlayMiss() => PlaySFX(miss);
    
    public void PlayRandomGameBgm()
    {
        AudioClip[] bgmList = new AudioClip[] { gameBgm1, gameBgm2, gameBgm3 };
        int index = Random.Range(0, bgmList.Length);
        AudioClip selectedBgm = bgmList[index];

        if (selectedBgm != null)
        {
            bgmSource.clip = selectedBgm;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning("SoundManager: 선택된 BGM이 null입니다.");
        }
    }
}