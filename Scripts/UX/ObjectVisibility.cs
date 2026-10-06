using UnityEngine;

/// <summary>
/// 플레이어(카메라)가 가까이 올수록 상호작용 대상의 아웃라인을 두껍게 표시한다.
/// VR을 처음 써보는 사람도 잡을 수 있는 물체를 쉽게 알아볼 수 있도록 한다.
///
/// 거리별 표시:
///   MAX_DISTANCE 이상   → 아웃라인 없음
///   MIN_DISTANCE 이하   → 최대 두께
///   그 사이             → 거리에 비례해 두께가 늘어남
/// </summary>
public class ObjectVisibility : MonoBehaviour
{
    [Tooltip("두께를 조절할 대상의 아웃라인 컴포넌트")]
    public Outline ObjOutline;

    // 아웃라인이 보이기 시작하는 거리 (m)
    private const float MAX_DISTANCE = 2f;

    // 아웃라인이 최대 두께가 되는 거리 (m)
    private const float MIN_DISTANCE = 1f;

    // 최대 아웃라인 두께
    private const float MAX_OUTLINE_WIDTH = 4f;

    // 거리를 잴 기준 (메인 카메라)
    private Transform _viewer;

    /// <summary>메인 카메라를 거리 기준으로 저장한다.</summary>
    private void Start()
    {
        _viewer = Camera.main.transform;
    }

    /// <summary>카메라와의 거리로 0~1 비율을 계산해 아웃라인 두께에 반영한다.</summary>
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, _viewer.position);

        // 가까울수록 1, 멀수록 0
        float ratio = 1f - Mathf.InverseLerp(MIN_DISTANCE, MAX_DISTANCE, distance);
        SetOutlineWidth(ratio);
    }

    /// <summary>비율(0~1)에 맞춰 아웃라인 두께를 설정한다.</summary>
    private void SetOutlineWidth(float ratio)
    {
        if (ObjOutline != null)
            ObjOutline.OutlineWidth = MAX_OUTLINE_WIDTH * ratio;
    }
}
