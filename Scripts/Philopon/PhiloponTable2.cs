using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 벌레 환각 이후의 금단 단계.
/// 후회하는 대사 → 속삭임 나레이션과 자막 → 두 번째 필로폰 등장 순서로 진행한다.
///
/// 나레이션 자막:
///   문장마다 나레이션 음성 길이를 지정하고 TypingEffect.TypeOverDuration()으로 출력 간격을 계산해,
///   자막이 음성과 함께 끝나도록 맞춘다.
/// </summary>
public class PhiloponTable2 : MonoBehaviour
{
    // 속삭임 나레이션 문장과 각 문장의 음성 길이 (초)
    private static readonly string[] WhisperLines =
    {
        "우울해... 너무 우울해....",
        "모든것이 의미가 없어...",
        "다시 한 번 마약을 줘.. 네 몸이 원하잖아...",
        "마약을 안 하니까 고통스러워..."
    };
    private static readonly float[] WhisperDurations = { 4.0f, 2.0f, 3.5f, 3.0f };

    // 속삭임이 시작된 뒤 두 번째 필로폰이 나타나기까지의 시간 (초)
    private const float SPAWN_DELAY = 18f;

    // 두 번째 필로폰 테이블 (자식)
    private GameObject _philoponTable;

    // 일반 말풍선
    private GameObject _normalTextObj;
    private TextMeshProUGUI _normalText;

    // 속삭임 자막
    private GameObject _whisperTextObj;
    private TextMeshProUGUI _whisperText;

    // 속삭임 나레이션 음성
    private AudioSource _whisper;

    /// <summary>말풍선, 자막, 나레이션 참조를 캐싱하고 후회 대사를 시작한다.</summary>
    private void Awake()
    {
        _normalTextObj = CanvasManager.Instance.TalkBoxUI;
        _normalText = _normalTextObj.GetComponentInChildren<TextMeshProUGUI>();

        _whisperTextObj = CanvasManager.Instance.WhisperUI;
        _whisperText = _whisperTextObj.GetComponentInChildren<TextMeshProUGUI>();

        _philoponTable = transform.GetChild(0).gameObject;
        _whisper = GetComponent<AudioSource>();

        StartCoroutine(Normal_TypingCo());
    }

    /// <summary>화면이 밝아지면 후회하는 대사를 출력하고 3초 뒤 금단 단계로 넘어간다.</summary>
    public IEnumerator Normal_TypingCo()
    {
        yield return new WaitForSeconds(3.5f);
        GameManager.Instance.screenFader.FadeIn();

        _normalTextObj.SetActive(true);
        yield return TypingEffect.Type(_normalText, "생각보다 마약이 기분 좋은게 아니구나. 두 번 다시 손대지 말아야겠다.");

        yield return new WaitForSeconds(3f);
        close_TalkBox();
    }

    /// <summary>말풍선을 닫고 금단 단계를 시작한다.</summary>
    public void close_TalkBox()
    {
        _normalTextObj.SetActive(false);
        StartCoroutine(Spawn_Philopon());
    }

    /// <summary>
    /// 속삭임 나레이션을 재생하고 플레이어가 움직일 수 있게 한 뒤,
    /// SPAWN_DELAY가 지나면 두 번째 필로폰을 등장시키고 미션을 안내한다.
    /// </summary>
    private IEnumerator Spawn_Philopon()
    {
        yield return new WaitForSeconds(2);
        _whisper.Play();
        CanvasManager.Instance.Player_SetActive(true);

        // 나레이션이 나오는 동안은 방향 화살표를 숨긴다
        GameManager.Instance.indicator.moving = false;

        StartCoroutine(Whisper_TypingCo());

        yield return new WaitForSeconds(SPAWN_DELAY);
        GameManager.Instance.indicator.moving = true;
        _philoponTable.SetActive(true);

        GameManager.Instance.Mission_Change("다른 필로폰을 찾으세요");
    }

    /// <summary>속삭임 나레이션 문장을 음성 길이에 맞춰 순서대로 자막으로 출력한다.</summary>
    public IEnumerator Whisper_TypingCo()
    {
        _whisperTextObj.SetActive(true);

        for (int i = 0; i < WhisperLines.Length; i++)
            yield return TypingEffect.TypeOverDuration(_whisperText, WhisperLines[i], WhisperDurations[i]);

        yield return new WaitForSeconds(2f);
        _whisperTextObj.SetActive(false);
    }
}
