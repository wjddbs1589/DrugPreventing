using System.Collections;
using UnityEngine;

/// <summary>
/// 벌레가 화면 곳곳에 나타나는 환각 연출.
/// 활성화되면 벌레 오브젝트(자식)를 12초 동안 표시한 뒤 숨긴다.
/// </summary>
public class PhiloponLight : MonoBehaviour
{
    // 벌레 오브젝트 묶음 (자식)
    private GameObject _bug;

    // 벌레 표시 시간 (초)
    private const float BUG_DURATION = 12f;

    /// <summary>벌레 오브젝트를 캐싱하고 연출을 시작한다.</summary>
    private void Start()
    {
        _bug = transform.GetChild(0).gameObject;
        StartCoroutine(Set_Bug());
    }

    /// <summary>벌레 등장 배경음과 함께 벌레를 표시하고 BUG_DURATION 뒤 숨긴다.</summary>
    private IEnumerator Set_Bug()
    {
        GameManager.Instance.bgmController.PlayBGM_Bug();

        _bug.SetActive(true);
        yield return new WaitForSeconds(BUG_DURATION);
        _bug.SetActive(false);
    }
}
