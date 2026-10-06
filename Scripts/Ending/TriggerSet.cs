using UnityEngine;

/// <summary>
/// 강아지의 대기 행동 애니메이션을 무작위로 선택한다.
/// 직전에 재생한 행동은 다시 고르지 않아 같은 동작이 연속으로 반복되지 않도록 한다.
///
/// 애니메이션 이벤트:
///   기본 상태로 돌아오는 시점에 Anim_End()를 호출해 다음 행동을 고를 수 있게 한다.
/// </summary>
public class TriggerSet : MonoBehaviour
{
    // 선택 가능한 행동 트리거 목록
    private static readonly string[] ActionTriggers =
    {
        "Idle3", "Idle4", "Idle5", "Pissing", "Crouch", "Dig", "Lie", "Sit"
    };

    /// <summary>행동 애니메이션 재생 중 여부</summary>
    public bool AnimPlaying = false;

    private Animator _dogAnim;

    // 직전에 선택한 행동 인덱스 (-1 = 선택 전)
    private int _prevIndex = -1;

    /// <summary>Animator를 캐싱한다.</summary>
    private void Awake()
    {
        _dogAnim = GetComponent<Animator>();
    }

    /// <summary>직전과 다른 행동을 무작위로 골라 재생한다.</summary>
    public void Set_Trigger()
    {
        AnimPlaying = true;

        int index;
        do
        {
            index = Random.Range(0, ActionTriggers.Length);
        }
        while (index == _prevIndex);

        _dogAnim.SetTrigger(ActionTriggers[index]);
        _prevIndex = index;
    }

    /// <summary>애니메이션 이벤트. 기본 상태로 돌아오면 재생 상태를 초기화한다.</summary>
    private void Anim_End()
    {
        AnimPlaying = false;
    }
}
