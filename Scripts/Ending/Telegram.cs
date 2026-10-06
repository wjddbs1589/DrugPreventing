using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 엔딩 DM 연출. 마약 판매자와의 DM 이미지를 한 장씩 보여준 뒤 예방 안내 문구를 순서대로 표시하고 씬을 재시작한다.
///
/// 진행 순서:
///   DM_Open()        → 핸드폰 화면과 마무리 배경음, 첫 DM 표시
///   DM_Interactive() → 버튼을 누를 때마다 다음 DM 표시, 모두 보면 안내 문구 시작
///   Open_InfoText()  → 안내 문구 4개 → (광주 버전이면 추가 영상) → 씬 재시작
///
/// 납품처별 버전:
///   ForGwangJu를 체크하면 광주 버전으로 동작해 안내 문구 뒤에 추가 영상을 재생한다.
/// </summary>
public class Telegram : MonoBehaviour
{
    [Tooltip("DM 화면 오브젝트")]
    public GameObject Phone;

    [Header("안내 문구 (표시 순서대로)")]
    public GameObject InfoText1;
    public GameObject InfoText2;
    public GameObject InfoText3;
    public GameObject InfoText4;

    [Header("광주 버전 전용")]
    [Tooltip("체크하면 안내 문구 뒤에 추가 영상을 재생한다.")]
    public bool ForGwangJu = false;

    [Tooltip("광주 버전에서 재생할 추가 영상 오브젝트")]
    public GameObject InfoVideo1;

    [Header("DM")]
    [Tooltip("순서대로 표시할 DM 이미지")]
    public GameObject[] DM_Images;

    // 표시할 DM 개수
    private const int DM_IMAGE_COUNT = 7;

    // 광주 버전 추가 영상 길이 (초)
    private const float GWANGJU_VIDEO_DURATION = 40f;

    // DM 알림음
    private AudioSource _dmSource;

    // 지금까지 표시한 DM 수
    private int _dmCount = 0;

    /// <summary>DM 알림음을 캐싱한다.</summary>
    private void Awake()
    {
        _dmSource = GetComponent<AudioSource>();
    }

    /// <summary>엔딩을 시작한다. 핸드폰 화면을 켜고 첫 DM을 표시한다.</summary>
    public void DM_Open()
    {
        Phone.SetActive(true);
        GameManager.Instance.screenFader.FadeIn();
        GameManager.Instance.bgmController.PlayBGM_Finale();

        DM_Interactive();
    }

    /// <summary>DM 화면을 닫는다.</summary>
    public void DM_Close()
    {
        Phone.SetActive(false);
    }

    /// <summary>다음 DM을 표시한다. 모든 DM을 보여줬으면 화면을 닫고 안내 문구를 시작한다.</summary>
    public void DM_Interactive()
    {
        if (_dmCount >= DM_IMAGE_COUNT)
        {
            DM_Close();
            StartCoroutine(Open_InfoText());
            return;
        }

        _dmSource.Play();
        DM_Images[_dmCount].SetActive(true);
        _dmCount++;
    }

    /// <summary>안내 문구를 시간에 맞춰 순서대로 표시하고, 광주 버전이면 추가 영상을 재생한 뒤 씬을 재시작한다.</summary>
    private IEnumerator Open_InfoText()
    {
        InfoText1.SetActive(true);
        yield return new WaitForSeconds(4);

        InfoText1.SetActive(false);
        InfoText2.SetActive(true);
        yield return new WaitForSeconds(14);

        InfoText2.SetActive(false);
        InfoText3.SetActive(true);
        yield return new WaitForSeconds(13);

        InfoText3.SetActive(false);
        InfoText4.SetActive(true);
        yield return new WaitForSeconds(7);
        GameManager.Instance.screenFader.FadeOut();

        if (ForGwangJu)
        {
            yield return new WaitForSeconds(3);
            GameManager.Instance.screenFader.FadeIn();
            GameManager.Instance.bgmController.StopBgm();
            InfoText4.SetActive(false);
            InfoVideo1.SetActive(true);

            yield return new WaitForSeconds(GWANGJU_VIDEO_DURATION);
            GameManager.Instance.screenFader.FadeOut();
        }

        yield return new WaitForSeconds(2);

        // 어둡게 바꿔 둔 스카이박스를 원래 밝기로 되돌린다 (머티리얼 에셋 값이 유지되므로 재시작 전에 복구)
        RenderSettings.skybox.SetColor("_Tint", new Color(.5f, .5f, .5f));

        yield return new WaitForSeconds(1);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
