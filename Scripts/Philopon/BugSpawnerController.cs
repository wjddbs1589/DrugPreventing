using System.Collections;
using UnityEngine;

/// <summary>
/// 첫 번째 투약 후 벌레 환각 단계를 진행한다.
/// 화면이 밝아지면 플레이어가 움직일 수 있는 상태에서 벌레를 15초간 보여주고,
/// 끝나면 화면을 어둡게 하며 두 번째 필로폰 테이블(금단 단계)을 활성화한다.
/// </summary>
public class BugSpawnerController : MonoBehaviour
{
    [Tooltip("벌레 환각이 끝난 뒤 활성화할 두 번째 필로폰 테이블")]
    public GameObject philoponTable;

    // 벌레 생성 오브젝트 (자식)
    private GameObject _bugSpawner;

    // 벌레 환각 유지 시간 (초)
    private const float BUG_DURATION = 15f;

    /// <summary>벌레 생성 오브젝트를 캐싱하고 환각 단계를 시작한다.</summary>
    private void Start()
    {
        _bugSpawner = transform.GetChild(0).gameObject;
        StartCoroutine(Use_Philopon());
    }

    /// <summary>화면을 밝히고 조작을 푼 뒤 벌레를 표시하고, BUG_DURATION 뒤 환각을 끝낸다.</summary>
    private IEnumerator Use_Philopon()
    {
        yield return new WaitForSeconds(2f);
        GameManager.Instance.screenFader.FadeIn();
        CanvasManager.Instance.Player_SetActive(true);

        // 이동은 가능하지만 찾아갈 목표가 없으므로 방향 화살표는 숨긴다
        GameManager.Instance.indicator.moving = false;

        yield return new WaitForSeconds(2f);
        _bugSpawner.SetActive(true);

        yield return new WaitForSeconds(BUG_DURATION);
        Stop_Philopon();
    }

    /// <summary>화면을 어둡게 하고 조작을 잠근 뒤 두 번째 필로폰 테이블을 활성화한다.</summary>
    public void Stop_Philopon()
    {
        GameManager.Instance.screenFader.FadeOut();
        GameManager.Instance.indicator.moving = false;
        StartCoroutine(Remove_Bug());

        CanvasManager.Instance.Player_SetActive(false);
        philoponTable.SetActive(true);
    }

    /// <summary>화면이 어두워진 뒤 벌레를 숨긴다.</summary>
    private IEnumerator Remove_Bug()
    {
        yield return new WaitForSeconds(2.0f);
        _bugSpawner.SetActive(false);
    }
}
