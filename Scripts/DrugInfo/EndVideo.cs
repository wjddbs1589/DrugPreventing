using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 부작용 영상 4종을 모두 시청한 뒤 필로폰 단계로 넘어가는 전환 처리.
/// 화면을 어둡게 했다가 필로폰을 배치하고 다시 밝힌 뒤, 대사와 함께 다음 미션을 안내한다.
/// </summary>
public class EndVideo : MonoBehaviour
{
    [Tooltip("다음 단계에서 사용할 필로폰 오브젝트 (시작 시 숨김)")]
    public GameObject philopon;

    [Tooltip("말풍선")]
    public GameObject TextObj;

    // 말풍선 텍스트
    private TextMeshProUGUI _text;

    /// <summary>말풍선 텍스트를 캐싱하고 필로폰을 숨긴다.</summary>
    private void Awake()
    {
        _text = TextObj.GetComponentInChildren<TextMeshProUGUI>();
        philopon.SetActive(false);
    }

    /// <summary>모든 영상을 시청하면 VideoPlayManager에서 호출한다.</summary>
    public void Change_table()
    {
        StartCoroutine(Fade_Co());
    }

    /// <summary>화면을 어둡게 한 상태에서 필로폰을 배치하고 다시 밝힌 뒤 대사를 시작한다.</summary>
    private IEnumerator Fade_Co()
    {
        GameManager.Instance.screenFader.FadeOut();
        yield return new WaitForSeconds(3.5f);

        philopon.SetActive(true);
        GameManager.Instance.bgmController.PlayBGM_Normal();

        GameManager.Instance.screenFader.FadeIn();
        StartCoroutine(TypingCo());
    }

    /// <summary>대사를 출력한 뒤 조작을 풀고 다음 미션을 안내한다.</summary>
    private IEnumerator TypingCo()
    {
        TextObj.SetActive(true);
        yield return TypingEffect.Type(_text, "필로폰을 제일 많이 하던데... 나도 한번....");

        yield return new WaitForSeconds(1.5f);
        CanvasManager.Instance.Player_SetActive(true);
        TextObj.SetActive(false);

        GameManager.Instance.Mission_Change("필로폰을 찾으세요");
    }
}
