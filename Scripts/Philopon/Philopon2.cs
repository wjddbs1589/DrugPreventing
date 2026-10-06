using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// 두 번째 필로폰 (금단을 견디지 못하고 다시 투약하는 장면).
/// 잡기 상호작용 이벤트에서 Use_Philopon2()를 호출한다.
/// 대사 후 화면이 어두워지고 강아지 장면으로 넘어간다.
/// </summary>
public class Philopon2 : MonoBehaviour
{
    [Tooltip("다음 장면에서 활성화할 강아지 오브젝트")]
    public GameObject Dog;

    // 필로폰 모델 (자식들)
    private GameObject[] _philoponBody;

    // 말풍선
    private GameObject _normalTextObj;
    private TextMeshProUGUI _normalText;

    // 중복 실행 방지
    private bool _used = false;

    /// <summary>말풍선 참조를 캐싱한다.</summary>
    private void Awake()
    {
        _normalTextObj = CanvasManager.Instance.TalkBoxUI;
        _normalText = _normalTextObj.GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>투약 후 제거할 필로폰 모델(자식)을 모두 저장한다.</summary>
    private void Start()
    {
        _philoponBody = new GameObject[transform.childCount];
        for (int i = 0; i < _philoponBody.Length; i++)
            _philoponBody[i] = transform.GetChild(i).gameObject;
    }

    /// <summary>필로폰을 다시 잡았을 때 호출된다. 한 번만 실행된다.</summary>
    public void Use_Philopon2()
    {
        if (_used) return;
        _used = true;
        StartCoroutine(Normal_TypingCo());
    }

    /// <summary>
    /// 투약 효과음 → 미션 완료 → 조작 잠금 → 필로폰 제거 → 대사 → 화면 어두워짐 → 강아지 장면 순서로 진행한다.
    /// </summary>
    public IEnumerator Normal_TypingCo()
    {
        AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, transform.position);

        GameManager.Instance.Mission_Complete();
        CanvasManager.Instance.Player_SetActive(false);
        GameManager.Instance.screenFader.FadeIn();

        Hide_Obj();

        _normalTextObj.SetActive(true);
        yield return TypingEffect.Type(_normalText, "지금 이 고통을 피하기 위해서는...");

        yield return new WaitForSeconds(2f);
        _normalTextObj.SetActive(false);

        GameManager.Instance.indicator.targetCount++;
        GameManager.Instance.screenFader.FadeOut();
        StartCoroutine(Spawn_Dog());
    }

    /// <summary>배경음을 멈추고 필로폰 모델을 제거한다.</summary>
    private void Hide_Obj()
    {
        GameManager.Instance.bgmController.audioSource.Stop();

        foreach (GameObject item in _philoponBody)
            Destroy(item);
    }

    /// <summary>화면이 어두워진 뒤 강아지를 등장시키고 금단 단계 오브젝트를 끈다.</summary>
    private IEnumerator Spawn_Dog()
    {
        yield return new WaitForSeconds(4);
        Dog.SetActive(true);
        FindObjectOfType<PhiloponTable2>().gameObject.SetActive(false);
    }
}
