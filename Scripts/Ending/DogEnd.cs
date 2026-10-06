using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 강아지를 발견하는 엔딩 컷신. 플레이어가 트리거에 들어오면 한 번 실행된다.
///
/// 진행 순서:
///   조작 잠금 → 화면 어두워짐 → 카메라를 강아지 쪽으로 이동 → 화면 밝아짐
///   → 대사와 효과음 → 화면 어두워짐 → 배경을 숨기고 엔딩 DM(Telegram) 시작
/// </summary>
public class DogEnd : MonoBehaviour
{
    [Tooltip("대사를 표시할 말풍선")]
    public GameObject textCanvas;

    [Tooltip("엔딩 진입 시 숨길 집 배경")]
    public GameObject House;

    private AudioSource _audioSource;
    private TextMeshProUGUI _text;
    private Collider _trigger;

    /// <summary>효과음, 말풍선 텍스트, 트리거 콜라이더를 캐싱한다.</summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _text = textCanvas.GetComponentInChildren<TextMeshProUGUI>();
        _trigger = GetComponent<Collider>();
    }

    /// <summary>플레이어가 들어오면 트리거를 끄고 컷신을 시작한다.</summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 중복 실행 방지
        _trigger.enabled = false;
        StartCoroutine(EndingCo());
    }

    /// <summary>강아지 발견 컷신을 진행한 뒤 엔딩 DM으로 넘어간다.</summary>
    private IEnumerator EndingCo()
    {
        CanvasManager.Instance.Player_SetActive(false);
        GameManager.Instance.screenFader.FadeOut();
        yield return new WaitForSeconds(2.5f);

        // 화면이 어두운 동안 카메라를 강아지(자식 오브젝트) 쪽으로 옮긴다
        Camera.main.transform.LookAt(transform.GetChild(0));
        Camera.main.transform.position = transform.position + Vector3.up;

        GameManager.Instance.screenFader.FadeIn();
        yield return new WaitForSeconds(2f);

        textCanvas.SetActive(true);
        yield return TypingEffect.Type(_text, " 설마... 내가 뽀삐를?......");
        _audioSource.Play();

        yield return new WaitForSeconds(4.0f);
        textCanvas.SetActive(false);

        GameManager.Instance.screenFader.FadeOut();
        yield return new WaitForSeconds(4.0f);
        _audioSource.Stop();
        yield return new WaitForSeconds(4.0f);

        // 엔딩 DM이 잘 보이도록 배경을 숨기고 스카이박스를 어둡게 한 뒤 이동을 막는다
        RenderSettings.skybox.SetColor("_Tint", new Color(0, 0, 0));
        House.SetActive(false);
        CanvasManager.Instance.PlayerController.GetComponent<CharacterController>().enabled = false;

        GameManager.Instance.Finale.DM_Open();
    }
}
