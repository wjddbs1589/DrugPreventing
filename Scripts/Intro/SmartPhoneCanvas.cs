using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 스마트폰 DM 화면 UI. 체험의 도입부로, 마약 판매 DM을 받는 상황을 보여준다.
///
/// 진행 순서:
///   On_Screen()       → 조작 잠금, 초기 화면과 대사 출력
///   Change_Screen()   → DM 화면으로 전환, 대사 출력 후 다음 버튼 표시
///   On_DrugCanvas()   → DM 화면을 닫고 마약 선택 단계로 넘어간다
/// </summary>
public class SmartPhoneCanvas : MonoBehaviour
{
    [Header("화면")]
    [Tooltip("핸드폰 UI 배경")]
    public GameObject Phone_BG;

    [Tooltip("핸드폰 초기 화면")]
    public GameObject SmartPhone1;

    [Tooltip("핸드폰 DM 화면")]
    public GameObject SmartPhone2;

    [Header("대사")]
    [Tooltip("말풍선")]
    public GameObject TextObj;

    [Tooltip("DM 대사가 끝나면 표시할 다음 버튼")]
    public GameObject nextButton;

    // 말풍선 텍스트
    private TextMeshProUGUI _text;

    /// <summary>말풍선 텍스트를 캐싱한다.</summary>
    private void Awake()
    {
        _text = TextObj.GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>핸드폰 화면을 켜고 플레이어 조작을 잠근다.</summary>
    public void On_Screen()
    {
        CanvasManager.Instance.Player_SetActive(false);
        Phone_BG.SetActive(true);
        StartCoroutine(TypingCo_1());
    }

    /// <summary>DM 알림을 받은 반응 대사를 출력한다.</summary>
    private IEnumerator TypingCo_1()
    {
        yield return TypingEffect.Type(_text, "디엠이 왔네!!!! 누구지???");
    }

    /// <summary>초기 화면에서 DM 화면으로 전환한다.</summary>
    public void Change_Screen()
    {
        SmartPhone1.SetActive(false);
        SmartPhone2.SetActive(true);
        StartCoroutine(TypingCo_2());
    }

    /// <summary>DM을 읽은 반응 대사 두 문장을 이어서 출력한 뒤 다음 버튼을 표시한다.</summary>
    private IEnumerator TypingCo_2()
    {
        yield return TypingEffect.Type(_text, "아이스? 크리스탈? 요즘 유행하는 사탕인가?");

        // 앞 문장을 1초간 보여준 뒤 다음 문장을 바로 출력한다
        yield return new WaitForSeconds(1f);
        yield return TypingEffect.Type(_text, "트렌드에 뒤떨어 질 수 없지! 나도 하나 구매 해야겠다!!", startDelay: 0f);

        yield return new WaitForSeconds(1f);
        nextButton.SetActive(true);
    }

    /// <summary>다음 버튼을 누르면 DM 화면을 닫고 마약 선택 단계로 넘어간다.</summary>
    public void On_DrugCanvas()
    {
        CanvasManager.Instance.audioSource.Play();

        SmartPhone2.SetActive(false);
        TextObj.SetActive(false);
        nextButton.SetActive(false);
        Phone_BG.SetActive(false);

        CanvasManager.Instance.SelectDrugBtn.SetActive(true);
        CanvasManager.Instance.DrugCanvas.Btn_AnimOn();
    }
}
