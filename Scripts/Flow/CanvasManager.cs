using BNG;
using System.Collections;
using UnityEngine;

/// <summary>
/// 시작 메뉴부터 첫 미션까지의 UI 흐름과 플레이어 조작 잠금을 관리하는 싱글톤.
///
/// 진행 순서:
///   시작 메뉴 → 튜토리얼 → 주의사항 → 첫 미션("핸드폰을 찾으세요")
///
/// 조작 잠금:
///   대사, 영상, 컷신이 진행되는 동안 Player_SetActive(false)로 이동·회전과 방향 화살표를 함께 끈다.
///
/// 주의:
///   모든 캔버스는 씬에서 켜 둔 상태로 시작해야 한다.
///   InitializeUI()에서 필요한 UI만 남기고 끄며, 처음부터 꺼져 있으면 이후 활성화 시 상호작용이 되지 않는다.
/// </summary>
public class CanvasManager : MonoBehaviour
{
    private static CanvasManager instance;
    public static CanvasManager Instance => instance;

    [Header("캔버스")]
    [Tooltip("시작 메뉴 캔버스")]
    public GameObject StartCanvas;

    [Tooltip("체험할 마약을 선택하는 버튼 묶음")]
    public GameObject SelectDrugBtn;

    [Tooltip("마약 정보와 부작용 영상을 관리하는 캔버스")]
    public VideoPlayManager DrugCanvas;

    [Header("UI")]
    [Tooltip("메뉴 뒷배경")]
    public GameObject BackgroundUI;

    [Tooltip("시작 메뉴")]
    public GameObject StartUI;

    [Tooltip("튜토리얼")]
    public GameObject TutorialUI;

    [Tooltip("주의사항")]
    public GameObject CautionUI;

    [Tooltip("일반 대사 말풍선")]
    public GameObject TalkBoxUI;

    [Tooltip("속삭임 나레이션 자막")]
    public GameObject WhisperUI;

    [Tooltip("UI 버튼 클릭음")]
    public AudioSource audioSource;

    [Tooltip("옵션 메뉴")]
    public OptionButton option;

    [Header("플레이어")]
    [Tooltip("플레이어 캐릭터 (BNG PlayerRotation, SmoothLocomotion 포함)")]
    public GameObject PlayerController;

    // 플레이어 회전과 이동 컴포넌트
    private PlayerRotation _playerRotation;
    private SmoothLocomotion _playerMove;

    /// <summary>싱글톤을 등록한다.</summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>플레이어 조작 컴포넌트를 캐싱하고 시작 메뉴 상태로 UI를 맞춘다.</summary>
    private void Start()
    {
        _playerRotation = PlayerController.GetComponent<PlayerRotation>();
        _playerMove = PlayerController.GetComponent<SmoothLocomotion>();

        InitializeUI();
    }

    /// <summary>시작 메뉴와 배경만 남기고 튜토리얼과 주의사항을 끈다.</summary>
    private void InitializeUI()
    {
        StartCanvas.SetActive(true);

        StartUI.SetActive(true);
        BackgroundUI.SetActive(true);
        TutorialUI.SetActive(false);
        CautionUI.SetActive(false);
    }

    // ── 시작 UI 흐름 ─────────────────────────────────────

    /// <summary>시작 버튼을 누르면 튜토리얼을 연다.</summary>
    public void OpenCanvas_Tutorial()
    {
        audioSource.Play();
        StartUI.SetActive(false);
        TutorialUI.SetActive(true);
    }

    /// <summary>튜토리얼을 닫으면 주의사항을 연다.</summary>
    public void OpenCanvas_CautionText()
    {
        audioSource.Play();
        TutorialUI.SetActive(false);
        BackgroundUI.SetActive(false);
        CautionUI.SetActive(true);
    }

    /// <summary>주의사항을 닫으면 시작 캔버스를 닫고 초기 상태로 되돌린 뒤 체험을 시작한다.</summary>
    public void CloseCanvas_Tutorial()
    {
        audioSource.Play();

        // 다음에 메뉴를 다시 열 때를 위해 UI를 초기 상태로 되돌려 둔다
        StartCanvas.SetActive(false);
        StartUI.SetActive(true);
        BackgroundUI.SetActive(true);
        TutorialUI.SetActive(false);
        CautionUI.SetActive(false);

        Player_SetActive(true);
        StartCoroutine(First_Mission());
    }

    /// <summary>1초 뒤 첫 미션을 안내한다.</summary>
    private IEnumerator First_Mission()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.Mission_Change("핸드폰을 찾으세요");
    }

    // ── 플레이어 조작 ────────────────────────────────────

    /// <summary>플레이어의 이동·회전과 목표 방향 화살표를 함께 켜거나 끈다.</summary>
    public void Player_SetActive(bool On)
    {
        GameManager.Instance.indicator.moving = On;
        _playerRotation.enabled = On;
        _playerMove.b_Moveable = On;
    }

    /// <summary>플레이어의 이동 가능 여부를 변경한다.</summary>
    public void Player_SetMove(bool On)
    {
        _playerMove.enabled = On;
    }

    /// <summary>플레이어의 회전 가능 여부를 변경한다.</summary>
    public void Player_SetRotation(bool On)
    {
        _playerRotation.enabled = On;
    }
}
