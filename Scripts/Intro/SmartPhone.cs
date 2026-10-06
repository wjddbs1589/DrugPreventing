using UnityEngine;

/// <summary>
/// 첫 미션 대상인 스마트폰. 잡기 상호작용 이벤트에서 Active()를 호출한다.
/// 미션을 완료 처리하고 DM 화면(SmartPhoneCanvas)을 연 뒤 자신은 제거된다.
/// </summary>
public class SmartPhone : MonoBehaviour
{
    // DM 화면을 표시하는 UI
    private SmartPhoneCanvas _smartPhoneCanvas;

    /// <summary>씬의 DM 화면 UI를 찾아 저장한다.</summary>
    private void Awake()
    {
        _smartPhoneCanvas = FindObjectOfType<SmartPhoneCanvas>();
    }

    /// <summary>
    /// 스마트폰을 잡았을 때 호출된다.
    /// 배경음 전환 → 미션 완료 → 다음 미션 대상으로 화살표 전환 → DM 화면 표시 순서로 처리한다.
    /// </summary>
    public void Active()
    {
        GameManager.Instance.bgmController.PlayBGM_Drug();
        GameManager.Instance.Mission_Complete();
        GameManager.Instance.indicator.targetCount++;

        _smartPhoneCanvas.On_Screen();
        Destroy(gameObject);
    }
}
