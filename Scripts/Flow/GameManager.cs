using BNG;
using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 체험 전체에서 공통으로 사용하는 참조와 미션 안내 UI를 관리하는 싱글톤.
///
/// 보관하는 참조:
///   BGM 컨트롤러, 화면 페이드(OVRScreenFade), 엔딩(Telegram), 목표 방향 화살표(DirectionIndicator),
///   약물별 화면 연출 오브젝트(LSD, 필로폰)
///
/// 미션 안내:
///   Mission_Change()로 새 목표를 타이핑 효과로 표시하고, Mission_Complete()로 숨긴다.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    [Header("연결 대상")]
    [Tooltip("장면별 배경음 컨트롤러")]
    public BGMcontroller bgmController;

    [Tooltip("LSD 체험 시 활성화할 화면 연출 오브젝트 (URP Volume 포함)")]
    public GameObject Effect_LSD;

    [Tooltip("필로폰 체험 시 활성화할 화면 연출 오브젝트")]
    public GameObject Effect_Philopon;

    [Header("미션 안내")]
    [Tooltip("미션 문구를 표시할 UI 오브젝트 (자식에 TextMeshProUGUI, 부모에 CanvasGroup 필요)")]
    public GameObject MissionInfo;

    /// <summary>화면 페이드 (CenterEyeAnchor의 OVRScreenFade)</summary>
    [HideInInspector] public OVRScreenFade screenFader;

    /// <summary>엔딩 DM 연출</summary>
    [HideInInspector] public Telegram Finale;

    /// <summary>현재 미션 대상을 가리키는 화살표</summary>
    [HideInInspector] public DirectionIndicator indicator;

    // 미션 문구 텍스트와 표시 여부를 조절하는 CanvasGroup
    private TextMeshProUGUI _missionText;
    private CanvasGroup _missionCanvasGroup;

    /// <summary>싱글톤을 등록하고 공통 참조를 초기화한다.</summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>미션 UI와 씬에 하나씩 있는 공통 오브젝트를 찾아 저장한다.</summary>
    private void Initialize()
    {
        _missionText = MissionInfo.GetComponentInChildren<TextMeshProUGUI>();
        _missionCanvasGroup = _missionText.GetComponentInParent<CanvasGroup>();

        screenFader = FindObjectOfType<OVRScreenFade>();
        Finale = FindObjectOfType<Telegram>();
        indicator = FindObjectOfType<DirectionIndicator>();
    }

    // ── 미션 안내 UI ─────────────────────────────────────

    /// <summary>미션 안내 문구를 표시하고 타이핑 효과로 출력한다.</summary>
    public void Mission_Change(string mission)
    {
        StartCoroutine(ShowMissionCo(mission));
    }

    /// <summary>미션 안내 UI를 보이게 한 뒤 문구를 타이핑 효과로 출력한다.</summary>
    private IEnumerator ShowMissionCo(string mission)
    {
        _missionCanvasGroup.alpha = 1f;
        yield return TypingEffect.Type(_missionText, mission);
    }

    /// <summary>미션을 완료하면 안내 문구를 즉시 숨긴다.</summary>
    public void Mission_Complete()
    {
        _missionCanvasGroup.alpha = 0f;
    }
}
