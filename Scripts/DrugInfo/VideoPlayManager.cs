using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

/// <summary>
/// 마약 4종(LSD, 나비약, 펜타닐, 합성대마)의 정보 확인과 부작용 영상 시청을 관리한다.
///
/// 진행 흐름:
///   Select_Drug()  → 반응 대사 출력, 약물 설명창 표시, 아직 안 본 약물이면 재생 버튼 표시
///   Play_Video()   → LSD는 영상 대신 화면 연출(Effect_LSD), 나머지는 배경을 숨기고 부작용 영상 재생
///   OnVideoEnd()   → UI 복구, 시청 완료 표시
///   4종을 모두 시청하면 다음 단계(필로폰)로 넘어간다
///
/// 영상 컨트롤:
///   일시정지/재생, 2배속, 스킵 버튼을 지원한다.
/// </summary>
public class VideoPlayManager : MonoBehaviour
{
    private const int DRUG_COUNT = 4;
    private const int LSD_INDEX = 0;

    // 약물을 선택했을 때 출력할 반응 대사 (버튼 인덱스 순서)
    private static readonly string[] DrugComments =
    {
        "예쁜 스티커처럼 생겼네? 혀 끝에 붙여보라고 했던 것 같은데...",
        "나는 왜 모태마름이 아닐까.. 나도 날씬하고 예쁘고 싶다. 쉽게 살 빼는 약이라던데?",
        "병원에서 처방 받는 약이라고 들었어! 그럼 괜찮은 거 아냐?",
        "그냥 전자 담배 카트리지인 줄 알았어..합성대마라고?"
    };

    [Header("비디오 플레이어")]
    [Tooltip("영상을 표시할 RawImage 오브젝트 (자식에 VideoPlayer 포함)")]
    public GameObject screen;

    [Header("비디오 텍스쳐")]
    [Tooltip("약물별 영상 출력용 RenderTexture (버튼 인덱스 순서)")]
    public RenderTexture[] VideoTextures;

    [Header("마약 부작용 영상 (LSD, 나비약, 펜타닐, 합성대마)")]
    public VideoClip[] clips = new VideoClip[DRUG_COUNT];

    [Header("마약 선택 버튼")]
    public Button[] SelectBtns = new Button[DRUG_COUNT];

    [Tooltip("선택 버튼 묶음 (설명창이 열리면 숨긴다)")]
    public GameObject Buttons;

    [Header("마약 선택 유도 화살표")]
    [Tooltip("아직 시청하지 않은 약물을 가리키는 화살표 (버튼 인덱스 순서)")]
    public GameObject[] ImageArrow = new GameObject[DRUG_COUNT];

    [Header("설명창")]
    [Tooltip("약물 정보를 표시하는 설명창")]
    public GameObject ExplainBoard;

    [Tooltip("부작용 영상 재생 버튼")]
    public GameObject playButton;

    [Header("말풍선")]
    public GameObject textObj;
    public TextMeshProUGUI text;

    [Header("비디오 컨트롤 버튼")]
    [Tooltip("일시정지/재생 버튼 이미지")]
    public Image playBtn;

    [Tooltip("배속 버튼 이미지")]
    public Image speedBtn;

    [Header("비디오 컨트롤 아이콘")]
    public Sprite playImage;
    public Sprite stopImage;
    public Sprite speedImage;

    [Header("영상 재생 시 숨길 배경")]
    public GameObject House;

    private VideoPlayer _videoPlayer;

    // 약물별 시청 완료 여부
    private readonly bool[] _playCheck = new bool[DRUG_COUNT];

    // 각 선택 버튼의 약물 정보 컴포넌트
    private readonly ActiveInfoText[] _infos = new ActiveInfoText[DRUG_COUNT];

    // 현재 선택된 약물 인덱스 (-1 = 선택 전)
    private int _videoIndex = -1;

    // 진행 중인 대사 타이핑 (다른 약물을 고르면 중단하고 새로 출력)
    private Coroutine _typingCoroutine;

    /// <summary>약물 정보 컴포넌트와 VideoPlayer를 캐싱하고 영상 종료 이벤트를 등록한다.</summary>
    private void Awake()
    {
        for (int i = 0; i < DRUG_COUNT; i++)
            _infos[i] = SelectBtns[i].GetComponent<ActiveInfoText>();

        _videoPlayer = screen.GetComponentInChildren<VideoPlayer>();
        _videoPlayer.loopPointReached += OnVideoEnd;
    }

    /// <summary>모든 선택 버튼의 강조 애니메이션을 시작한다.</summary>
    public void Btn_AnimOn()
    {
        foreach (var btn in SelectBtns)
            btn.GetComponent<Animator>().SetTrigger("On");
    }

    // ── 약물 선택 ────────────────────────────────────────

    /// <summary>
    /// 약물 버튼을 눌렀을 때 호출된다.
    /// 반응 대사를 출력하고 설명창을 열며, 이미 시청한 약물이면 재생 버튼을 숨긴다.
    /// </summary>
    public void Select_Drug(int num)
    {
        _videoIndex = num;

        ShowDrugComment();

        _infos[_videoIndex].Set_explainBoard();
        Set_ExplainBoard(true);

        playButton.SetActive(!_playCheck[_videoIndex]);
    }

    /// <summary>선택한 약물의 반응 대사를 타이핑 효과로 출력한다. 출력 중이던 대사는 중단한다.</summary>
    private void ShowDrugComment()
    {
        if (_typingCoroutine != null)
            StopCoroutine(_typingCoroutine);

        textObj.SetActive(true);
        _typingCoroutine = StartCoroutine(TypingEffect.Type(text, DrugComments[_videoIndex]));
    }

    /// <summary>설명창과 선택 버튼 중 하나만 보이도록 전환한다.</summary>
    public void Set_ExplainBoard(bool isOn)
    {
        CanvasManager.Instance.audioSource.Play();
        ExplainBoard.SetActive(isOn);
        Buttons.SetActive(!isOn);
    }

    // ── 영상 재생 ────────────────────────────────────────

    /// <summary>
    /// 재생 버튼을 눌렀을 때 호출된다.
    /// LSD는 영상 대신 화면 연출(Effect_LSD)을 실행하고, 나머지는 배경을 숨긴 뒤 부작용 영상을 재생한다.
    /// </summary>
    public void Play_Video()
    {
        CanvasManager.Instance.audioSource.Play();

        HideSelectUI();

        // LSD는 영상 대신 체험 연출로 진행하고, 연출이 끝나면 End_LSD()가 호출된다
        if (_videoIndex == LSD_INDEX)
        {
            GameManager.Instance.bgmController.PlayBGM_LSD();
            GameManager.Instance.Effect_LSD.SetActive(true);
            return;
        }

        GameManager.Instance.bgmController.audioSource.Stop();

        // 약물별 RenderTexture로 영상 출력 대상을 바꾼다
        screen.GetComponent<RawImage>().texture = VideoTextures[_videoIndex];
        _videoPlayer.targetTexture = VideoTextures[_videoIndex];
        Map_Off();

        _videoPlayer.clip = clips[_videoIndex];
        screen.SetActive(true);
        _videoPlayer.playbackSpeed = 1.0f;
        _videoPlayer.Play();
    }

    /// <summary>영상이나 연출이 진행되는 동안 말풍선, 설명창, 선택 버튼을 모두 숨긴다.</summary>
    private void HideSelectUI()
    {
        textObj.SetActive(false);
        playButton.SetActive(false);
        Buttons.SetActive(false);
        ExplainBoard.SetActive(false);

        foreach (Button button in SelectBtns)
            button.gameObject.SetActive(false);
    }

    /// <summary>LSD 연출이 끝나면 LSDeffect에서 호출한다. 영상 종료와 같은 처리를 한다.</summary>
    public void End_LSD()
    {
        OnVideoEnd(_videoPlayer);
    }

    /// <summary>
    /// 영상(또는 LSD 연출)이 끝나면 호출된다.
    /// UI와 배경을 복구하고 시청 완료를 표시하며, 4종을 모두 봤으면 다음 단계로 넘어간다.
    /// </summary>
    public void OnVideoEnd(VideoPlayer vp)
    {
        GameManager.Instance.bgmController.PlayBGM_Drug();

        // 말풍선, 설명창, 선택 버튼 복구
        textObj.SetActive(true);
        text.text = "";
        playButton.SetActive(true);
        Map_On();
        ExplainBoard.SetActive(false);
        Buttons.SetActive(true);

        // 시청 완료 처리: 유도 화살표 제거, 강조 애니메이션 중단, 버튼 색 어둡게
        _playCheck[_videoIndex] = true;
        ImageArrow[_videoIndex].SetActive(false);
        SelectBtns[_videoIndex].GetComponent<Animator>().SetTrigger("Off");
        SelectBtns[_videoIndex].GetComponentInChildren<ActiveInfoText>().Btn_Used();

        // 강조 애니메이션으로 앞으로 나와 있던 버튼을 원래 깊이로 되돌린다
        RectTransform rectTransform = SelectBtns[_videoIndex].GetComponent<RectTransform>();
        Vector3 position = rectTransform.localPosition;
        position.z = 0;
        rectTransform.localPosition = position;

        screen.SetActive(false);
        _videoPlayer.clip = null;

        foreach (Button button in SelectBtns)
            button.gameObject.SetActive(true);

        // 영상 컨트롤 초기화 (일시정지 아이콘, 1배속)
        playBtn.sprite = stopImage;
        _videoPlayer.playbackSpeed = 1.0f;
        speedBtn.sprite = speedImage;

        // 4종을 모두 시청하면 필로폰 단계로 넘어간다
        if (AllVideosWatched())
        {
            FindObjectOfType<EndVideo>().Change_table();
            GameManager.Instance.bgmController.audioSource.Stop();
            gameObject.SetActive(false);
        }
    }

    /// <summary>모든 약물의 영상을 시청했는지 확인한다.</summary>
    private bool AllVideosWatched()
    {
        foreach (bool watched in _playCheck)
        {
            if (!watched) return false;
        }
        return true;
    }

    // ── 영상 컨트롤 버튼 ─────────────────────────────────

    /// <summary>배속 버튼. 1배속과 2배속을 전환하고 아이콘을 바꾼다.</summary>
    public void Set_VideoSpeed()
    {
        if (_videoPlayer.playbackSpeed == 1.0f)
        {
            _videoPlayer.playbackSpeed = 2.0f;
            speedBtn.sprite = playImage;    // 2배속 중에는 일반 속도 아이콘 표시
        }
        else
        {
            _videoPlayer.playbackSpeed = 1.0f;
            speedBtn.sprite = speedImage;   // 1배속 중에는 2배속 아이콘 표시
        }
    }

    /// <summary>일시정지 버튼. 재생 중이면 일시정지하고, 멈춰 있으면 다시 재생한다.</summary>
    public void StopAndPlay()
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();
            playBtn.sprite = playImage;
        }
        else
        {
            _videoPlayer.Play();
            playBtn.sprite = stopImage;
        }
    }

    /// <summary>스킵 버튼. 영상을 멈추고 종료 처리를 바로 실행한다.</summary>
    public void StopVideoAndCallOnVideoEnd()
    {
        _videoPlayer.Stop();
        OnVideoEnd(_videoPlayer);
    }

    // ── 배경 전환 ────────────────────────────────────────

    /// <summary>영상 재생 중에는 플레이어 이동을 막고 집 배경을 숨기며 스카이박스를 어둡게 한다.</summary>
    private void Map_Off()
    {
        CanvasManager.Instance.PlayerController.GetComponent<CharacterController>().enabled = false;
        House.SetActive(false);
        RenderSettings.skybox.SetColor("_Tint", new Color(0, 0, 0));
    }

    /// <summary>영상이 끝나면 스카이박스와 배경, 플레이어 이동을 되돌린다.</summary>
    private void Map_On()
    {
        RenderSettings.skybox.SetColor("_Tint", new Color(.5f, .5f, .5f));
        House.SetActive(true);
        CanvasManager.Instance.PlayerController.GetComponent<CharacterController>().enabled = true;
    }
}
