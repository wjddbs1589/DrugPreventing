using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 말풍선, 자막, 미션 안내 등에 공통으로 사용하는 타이핑 효과.
/// 문장을 한 글자씩 출력하며, 코루틴 안에서 yield return으로 호출하거나 StartCoroutine으로 직접 실행한다.
///
/// 사용 예:
///   yield return TypingEffect.Type(text, "대사");                        // 기본 속도
///   yield return TypingEffect.TypeOverDuration(text, "나레이션", 4.0f);  // 음성 길이에 맞춰 출력
/// </summary>
public static class TypingEffect
{
    // 기본 글자 출력 간격 (초)
    public const float DEFAULT_CHAR_INTERVAL = 0.05f;

    // 출력 시작 전 대기 시간 (초)
    public const float DEFAULT_START_DELAY = 0.3f;

    /// <summary>
    /// 텍스트를 비운 뒤 startDelay만큼 기다렸다가 charInterval 간격으로 한 글자씩 출력한다.
    /// </summary>
    public static IEnumerator Type(TMP_Text target, string message,
        float charInterval = DEFAULT_CHAR_INTERVAL, float startDelay = DEFAULT_START_DELAY)
    {
        target.text = "";
        yield return new WaitForSeconds(startDelay);

        for (int i = 0; i <= message.Length; i++)
        {
            target.text = message.Substring(0, i);
            yield return new WaitForSeconds(charInterval);
        }
    }

    /// <summary>
    /// 나레이션 음성 길이(duration)를 글자 수로 나눠 출력 간격을 계산한다.
    /// 문장마다 길이가 달라도 자막이 음성과 함께 끝나도록 맞춘다.
    /// </summary>
    public static IEnumerator TypeOverDuration(TMP_Text target, string message, float duration,
        float startDelay = DEFAULT_START_DELAY)
    {
        float charInterval = message.Length > 0 ? duration / message.Length : 0f;
        yield return Type(target, message, charInterval, startDelay);
    }
}
