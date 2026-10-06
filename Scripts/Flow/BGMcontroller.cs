using UnityEngine;

/// <summary>
/// 체험 장면에 맞춰 배경음을 전환한다.
/// 장면별 공개 메서드(PlayBGM_Normal 등)는 다른 스크립트에서 호출하며, 내부적으로 PlayClip() 하나로 처리한다.
/// </summary>
public class BGMcontroller : MonoBehaviour
{
    /// <summary>배경음 재생용 AudioSource</summary>
    [HideInInspector] public AudioSource audioSource;

    [Header("기본")]
    public AudioClip BGM_Normal;

    [Header("마약 구매")]
    public AudioClip BGM_Drug;

    [Header("LSD")]
    public AudioClip BGM_LSD;

    [Header("벌레 등장")]
    public AudioClip BGM_Bug;

    [Header("강아지 사망")]
    public AudioClip BGM_Dog;

    [Header("마무리")]
    public AudioClip BGM_Finale;

    /// <summary>AudioSource를 캐싱하고 기본 배경음을 재생한다.</summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        PlayBGM_Normal();
    }

    /// <summary>기본 배경음을 재생한다.</summary>
    public void PlayBGM_Normal() => PlayClip(BGM_Normal);

    /// <summary>마약 거래 장면의 배경음을 재생한다.</summary>
    public void PlayBGM_Drug() => PlayClip(BGM_Drug);

    /// <summary>LSD 체험 중 배경음을 재생한다.</summary>
    public void PlayBGM_LSD() => PlayClip(BGM_LSD);

    /// <summary>벌레 환각 장면의 배경음을 재생한다.</summary>
    public void PlayBGM_Bug() => PlayClip(BGM_Bug);

    /// <summary>강아지 사망 장면의 배경음을 재생한다.</summary>
    public void PlayBGM_Dog() => PlayClip(BGM_Dog);

    /// <summary>마무리 장면의 배경음을 재생한다.</summary>
    public void PlayBGM_Finale() => PlayClip(BGM_Finale);

    /// <summary>배경음을 멈추고 클립을 비운다.</summary>
    public void StopBgm()
    {
        audioSource.Stop();
        audioSource.clip = null;
    }

    /// <summary>재생 중인 배경음을 멈추고 지정한 클립을 처음부터 재생한다.</summary>
    private void PlayClip(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.Play();
    }
}
