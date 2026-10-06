using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// LSD 체험 연출(Effect_LSD 오브젝트)의 유지 시간을 관리한다.
/// 화면 왜곡과 색상 변환은 같은 오브젝트의 URP Volume에서 처리하며,
/// 이 컴포넌트는 활성화된 뒤 effectDuration이 지나면 체험을 종료하고 스스로 비활성화한다.
/// </summary>
public class LSDeffect : MonoBehaviour
{
    [Tooltip("LSD 연출 유지 시간 (초)")]
    [FormerlySerializedAs("rotationDuration")]
    public float effectDuration = 20.0f;

    /// <summary>활성화되면 종료 타이머를 시작한다.</summary>
    private void Start()
    {
        StartCoroutine(End_Lsd());
    }

    /// <summary>effectDuration이 지나면 약물 선택 화면으로 돌아가도록 알리고 연출을 끈다.</summary>
    private IEnumerator End_Lsd()
    {
        yield return new WaitForSeconds(effectDuration);
        CanvasManager.Instance.DrugCanvas.End_LSD();
        gameObject.SetActive(false);
    }
}
