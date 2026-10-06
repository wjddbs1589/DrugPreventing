using System.Collections;
using UnityEngine;

/// <summary>
/// 강아지 등장 장면. 활성화되면 짖는 소리와 함께 화면이 밝아지고 문이 열린다.
/// </summary>
public class DogSpawn : MonoBehaviour
{
    [Tooltip("열릴 문 오브젝트")]
    public GameObject Door;

    // 문이 열리는 데 걸리는 시간 (초)
    private const float DOOR_OPEN_DURATION = 1f;

    // 문 회전 각도 (닫힘 → 열림)
    private const float DOOR_CLOSED_ANGLE = 90f;
    private const float DOOR_OPEN_ANGLE = 240f;

    // 강아지 짖는 소리
    private AudioSource _dogSound;

    /// <summary>짖는 소리를 캐싱하고 등장 연출을 시작한다.</summary>
    private void Awake()
    {
        _dogSound = GetComponent<AudioSource>();
        StartCoroutine(OpenDoor());
    }

    /// <summary>짖는 소리 재생, 조작 해제, 화면 밝아짐과 함께 문을 연다.</summary>
    private IEnumerator OpenDoor()
    {
        _dogSound.Play();
        CanvasManager.Instance.Player_SetActive(true);
        GameManager.Instance.screenFader.FadeIn();

        float elapsed = 0f;
        while (elapsed < DOOR_OPEN_DURATION)
        {
            elapsed += Time.deltaTime;
            float angle = Mathf.Lerp(DOOR_CLOSED_ANGLE, DOOR_OPEN_ANGLE, elapsed / DOOR_OPEN_DURATION);
            Door.transform.rotation = Quaternion.Euler(0, angle, 0);
            yield return null;
        }
        Door.transform.rotation = Quaternion.Euler(0, DOOR_OPEN_ANGLE, 0);
    }
}
