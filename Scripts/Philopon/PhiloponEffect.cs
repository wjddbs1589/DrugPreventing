using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 필로폰 투약 후 화면이 점점 어두워지는 연출.
/// URP Volume의 Color Adjustments에서 Post Exposure 값을 animationDuration 동안 startValue에서 endValue로 낮춘다.
///
/// 재사용:
///   비활성화되면 다음 활성화 때 더 어둡고(-10) 길게(5초) 진행되도록 값을 바꿔,
///   두 번째 투약에서 더 강한 연출이 나오도록 한다.
/// </summary>
public class PhiloponEffect : MonoBehaviour
{
    [Tooltip("Color Adjustments가 포함된 URP Volume")]
    [SerializeField] private Volume vo;

    [Header("노출 변화")]
    [Tooltip("시작 노출값")]
    public float startValue = 0f;

    [Tooltip("목표 노출값 (낮을수록 어두움)")]
    public float endValue = -4f;

    [Tooltip("목표 노출값까지 걸리는 시간 (초)")]
    public float animationDuration = 3f;

    // 두 번째 연출에 적용할 값
    private const float SECOND_END_VALUE = -10f;
    private const float SECOND_DURATION = 5f;

    private ColorAdjustments _colorAdjustments;
    private float _currentTime = 0f;

    /// <summary>Volume의 런타임 프로필에서 Color Adjustments를 가져온다.</summary>
    private void Start()
    {
        vo.profile.TryGet(out _colorAdjustments);
    }

    /// <summary>animationDuration 동안 노출값을 선형으로 낮춘다.</summary>
    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > animationDuration) return;

        float t = _currentTime / animationDuration;
        _colorAdjustments.postExposure.value = Mathf.Lerp(startValue, endValue, t);
    }

    /// <summary>비활성화되면 다음 연출이 더 강하게 진행되도록 값을 바꾸고 경과 시간을 초기화한다.</summary>
    private void OnDisable()
    {
        startValue = 0f;
        endValue = SECOND_END_VALUE;
        animationDuration = SECOND_DURATION;
        _currentTime = 0f;
    }
}
