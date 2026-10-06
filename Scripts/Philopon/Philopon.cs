using UnityEngine;

/// <summary>
/// 첫 번째 필로폰. 잡기 상호작용 이벤트에서 Use_Philopon()을 호출한다.
/// 투약 효과음과 함께 화면을 어둡게 하고, 벌레 환각 단계(BugSpawner)를 시작한다.
/// </summary>
public class Philopon : MonoBehaviour
{
    [Tooltip("투약 후 활성화할 벌레 환각 연출")]
    public GameObject BugSpawner;

    // 중복 실행 방지
    private bool _used = false;

    /// <summary>
    /// 필로폰을 잡았을 때 호출된다. 한 번만 실행된다.
    /// 효과음 → 미션 완료 → 조작 잠금 → 화면 어두워짐 → 벌레 환각 시작 순서로 처리한다.
    /// </summary>
    public void Use_Philopon()
    {
        if (_used) return;
        _used = true;

        // 오브젝트가 곧 비활성화되므로 위치 기반 일회성 재생으로 효과음을 끝까지 들려준다
        AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, transform.position);

        GameManager.Instance.Mission_Complete();
        GameManager.Instance.indicator.targetCount++;
        GameManager.Instance.bgmController.audioSource.Stop();

        CanvasManager.Instance.Player_SetActive(false);
        GameManager.Instance.screenFader.FadeOut();

        GameManager.Instance.bgmController.PlayBGM_Bug();

        BugSpawner.SetActive(true);
        transform.parent.gameObject.SetActive(false);
    }
}
