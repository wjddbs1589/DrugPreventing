using UnityEngine;

/// <summary>
/// 현재 미션 대상을 가리키는 방향 화살표.
/// 각 단계를 완료한 스크립트가 targetCount를 1씩 올리면 다음 미션 대상을 가리킨다.
/// 대상에 minDistance 이내로 다가가거나 조작이 잠겨 있으면(moving = false) 화살표를 숨긴다.
/// </summary>
public class DirectionIndicator : MonoBehaviour
{
    /// <summary>현재 가리키는 미션 대상의 인덱스</summary>
    [HideInInspector] public int targetCount = 0;

    /// <summary>화살표 표시 여부 (대사·컷신 중에는 false)</summary>
    [HideInInspector] public bool moving = true;

    [Tooltip("진행 순서대로 나열한 미션 대상 오브젝트")]
    public GameObject[] MissionObject = new GameObject[5];

    // 이 거리 이내로 다가가면 화살표를 숨긴다
    private const float MIN_DISTANCE = 2f;

    // 화살표 모델 (자식 오브젝트)
    private GameObject _arrow;

    /// <summary>화살표 모델을 캐싱한다.</summary>
    private void Awake()
    {
        _arrow = transform.GetChild(0).gameObject;
    }

    /// <summary>시작 시에는 숨겨 두고, 진행에 맞춰 외부에서 활성화한다.</summary>
    private void Start()
    {
        gameObject.SetActive(false);
    }

    /// <summary>플레이어 이동이 반영된 뒤 미션 대상을 바라보고 화살표 표시 여부를 갱신한다.</summary>
    private void LateUpdate()
    {
        // 모든 미션을 마쳐 가리킬 대상이 없으면 숨긴다
        if (targetCount >= MissionObject.Length)
        {
            _arrow.SetActive(false);
            return;
        }

        Transform target = MissionObject[targetCount].transform;
        transform.LookAt(target);

        float distance = Vector3.Distance(transform.position, target.position);
        _arrow.SetActive(moving && distance > MIN_DISTANCE);
    }
}
