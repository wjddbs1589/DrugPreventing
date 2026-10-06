using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 환각 상태에서 강아지를 해친 사실을 깨닫는 장면.
/// 문이 열린 뒤 플레이어의 양손을 피 묻은 손 머티리얼로 바꾸고, 대사와 배경음으로 상황을 전달한다.
/// </summary>
public class DogDie : MonoBehaviour
{
    [Tooltip("대사를 표시할 말풍선")]
    public GameObject textCanvas;

    [Tooltip("열릴 문 오브젝트")]
    public GameObject Door;

    [Header("피 묻은 손 연출")]
    [Tooltip("왼손 모델 (SkinnedMeshRenderer)")]
    public GameObject LeftHand;

    [Tooltip("오른손 모델 (SkinnedMeshRenderer)")]
    public GameObject RightHand;

    [Tooltip("피 묻은 왼손 머티리얼")]
    public Material BHL;

    [Tooltip("피 묻은 오른손 머티리얼")]
    public Material BHR;

    // 문이 열리는 데 걸리는 시간 (초)
    private const float DOOR_OPEN_DURATION = 1.0f;

    // 문 회전 각도 (닫힘 → 열림)
    private const float DOOR_CLOSED_ANGLE = -180f;
    private const float DOOR_OPEN_ANGLE = -30f;

    private AudioSource _audioSource;
    private TextMeshProUGUI _text;
    private SkinnedMeshRenderer _leftHandRenderer;
    private SkinnedMeshRenderer _rightHandRenderer;

    /// <summary>참조를 캐싱하고 필로폰 화면 연출을 끈 뒤 장면을 시작한다.</summary>
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _text = textCanvas.GetComponentInChildren<TextMeshProUGUI>();
        _leftHandRenderer = LeftHand.GetComponent<SkinnedMeshRenderer>();
        _rightHandRenderer = RightHand.GetComponent<SkinnedMeshRenderer>();

        GameManager.Instance.Effect_Philopon.SetActive(false);
        StartCoroutine(OpenDoor());
    }

    /// <summary>효과음과 함께 문을 열고 대사 장면으로 넘어간다.</summary>
    private IEnumerator OpenDoor()
    {
        _audioSource.Play();
        CanvasManager.Instance.Player_SetActive(true);

        float elapsed = 0f;
        while (elapsed < DOOR_OPEN_DURATION)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(DOOR_CLOSED_ANGLE, DOOR_OPEN_ANGLE, elapsed / DOOR_OPEN_DURATION);
            Door.transform.rotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }
        Door.transform.rotation = Quaternion.Euler(0, DOOR_OPEN_ANGLE, 0);

        StartCoroutine(TypingCo());
    }

    /// <summary>양손을 피 묻은 손으로 바꾸고, 조작을 잠근 상태에서 대사를 출력한 뒤 다시 조작을 푼다.</summary>
    private IEnumerator TypingCo()
    {
        _leftHandRenderer.material = BHL;
        _rightHandRenderer.material = BHR;

        textCanvas.SetActive(true);
        GameManager.Instance.bgmController.PlayBGM_Dog();
        CanvasManager.Instance.Player_SetActive(false);

        yield return TypingEffect.Type(_text, " 이게 뭐야!!! 어째서 바닥에 피가...");

        yield return new WaitForSeconds(2.0f);
        CanvasManager.Instance.Player_SetActive(true);
        textCanvas.SetActive(false);
    }
}
