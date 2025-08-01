using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("효과음 오디오 소스")]
    private AudioSource sfxSource;

    [Header("효과음 리스트")]
    public AudioClip buttonClick;
    public AudioClip archerHit;
    public AudioClip magicianHit;
    public AudioClip miss;
    public AudioClip warriorDash;
    public AudioClip playerDamaged;
    public AudioClip warning;

    private void Awake()
    {
        // 싱글톤 패턴
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // AudioSource 컴포넌트 추가 (효과음용)
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
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
    
}