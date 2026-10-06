using UnityEngine;

/// <summary>
/// 플레이어(카메라)가 가까이 올수록 파티클을 선명하게 표시한다.
/// 상호작용 대상 주변의 파티클이 가까워질 때만 보이도록 해서 시선을 자연스럽게 유도한다.
///
/// 거리별 표시:
///   maxDistance 이상 → 투명 (알파 0)
///   minDistance 이하 → 불투명 (알파 1)
///
/// 최적화:
///   Gradient와 키 배열을 Awake에서 한 번만 만들고 매 프레임 알파값만 바꿔, Quest 스탠드얼론 환경에서 GC 할당을 줄인다.
/// </summary>
public class ParticleAlphaController : MonoBehaviour
{
    [Tooltip("알파값을 조절할 파티클 시스템")]
    public ParticleSystem particle;

    [Tooltip("거리를 잴 기준. 비워두면 메인 카메라를 사용한다.")]
    public Transform mainCamera;

    [Tooltip("알파값이 1이 되는 거리 (m)")]
    public float minDistance = 1.0f;

    [Tooltip("알파값이 0이 되는 거리 (m)")]
    public float maxDistance = 3.0f;

    // 매 프레임 재사용하는 그라디언트와 키 배열
    private Gradient _gradient;
    private GradientColorKey[] _colorKeys;
    private GradientAlphaKey[] _alphaKeys;

    /// <summary>기준 카메라를 정하고 재사용할 그라디언트를 만든다.</summary>
    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main.transform;

        _gradient = new Gradient();
        _colorKeys = new[] { new GradientColorKey(Color.white, 0f) };
        _alphaKeys = new[] { new GradientAlphaKey(1f, 0f) };
    }

    /// <summary>카메라와의 거리로 알파값을 계산해 Color over Lifetime에 적용한다.</summary>
    private void Update()
    {
        float distance = Vector3.Distance(transform.position, mainCamera.position);

        // 가까울수록 1, 멀수록 0
        float alpha = 1f - Mathf.InverseLerp(minDistance, maxDistance, distance);

        _alphaKeys[0].alpha = alpha;
        _gradient.SetKeys(_colorKeys, _alphaKeys);

        var colorOverLifetime = particle.colorOverLifetime;
        colorOverLifetime.color = _gradient;
    }
}
