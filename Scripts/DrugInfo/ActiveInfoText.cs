using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 마약 선택 버튼에 붙는 약물 정보 컴포넌트.
/// 버튼을 누르면 해당 약물의 이름, 분류, 부작용 설명과 이미지를 설명창에 채우고,
/// 영상을 시청한 뒤에는 버튼을 어둡게 표시한다.
/// </summary>
public class ActiveInfoText : MonoBehaviour
{
    [Header("정보를 표시할 텍스트 UI")]
    public TextMeshProUGUI BigName_Text;
    public TextMeshProUGUI name_Text;
    public TextMeshProUGUI type_Text;
    public TextMeshProUGUI info_Text;

    [Header("약물 이미지를 표시할 UI")]
    public Image drugImageUI;
    public Image drugElementImageUI;

    [Header("표시할 이미지")]
    public Sprite drugImage;
    public Sprite drugElementImage;

    [Header("활성화할 설명창")]
    public GameObject infoBoard;

    [Tooltip("약물 인덱스 (0 = LSD, 1 = 나비약, 2 = 펜타닐, 3 = 합성대마)")]
    public int targetIndex;

    // 버튼 시청 완료 시 적용할 어두운 색
    private static readonly Color UsedColor = new Color(0.5f, 0.5f, 0.5f);

    /// <summary>설명창을 열고 이 버튼의 약물 정보와 이미지를 채운다.</summary>
    public void Set_explainBoard()
    {
        infoBoard.SetActive(true);

        SetDrugInfo(targetIndex);

        drugImageUI.sprite = drugImage;
        drugElementImageUI.sprite = drugElementImage;
    }

    /// <summary>약물 인덱스에 맞는 이름, 분류, 설명을 설명창 텍스트에 적용한다.</summary>
    private void SetDrugInfo(int index)
    {
        string nameBig = "";
        string nameSmall = "";
        string type = "";
        string description = "";

        switch (index)
        {
            case 0:
                nameBig = "LSD";
                nameSmall = "명칭 : 리세르그산 디에틸아미드 (LSD)";
                type = "분류 : 마약 (환각제)";
                description = "뇌와 염색체에 손상을 일으키며 심박동과 혈압이 빨라지고 수전증이나 오한 등을 일으킴.\n환각에 사로잡혀 다른 사람을 죽이거나 자살하는 등 2차 사건사고의 위험성 존재.";
                break;
            case 1:
                nameBig = "나비약";
                nameSmall = "명칭 : 암페타민 계통의 (디에타민 정)";
                type = "분류 : 향정신성 의약품";
                description = "중추 신경계를 지치게 자극해 불면증이나 불안감, 두통 등에 시달림. 과량투여로 심하게 중독된다면\n호흡이 빨라지거나 환각, 공황, 정신분열 증상이 나타나거나 혼수상태에 이르렀다가 사망할 수 있음.";
                break;
            case 2:
                nameBig = "펜타닐";
                nameSmall = "명칭 : 펜타닐";
                type = "분류 : 마약 (오피오이드계의 마약성 진통제)";
                description = "모르핀보다 100배 이상 강하며 2mg의 극소량만으로 사망에 이를 수 있음.\n미국에서 가장 많은 과다복용 사망자를 낸 약물.";
                break;
            case 3:
                nameBig = "합성대마";
                nameSmall = "명칭 : 합성대마";
                type = "분류 : 향정신성 의약품";
                description = "대마초의 THC 성분만 추출해 농축하거나 다른 화학성분과 합성한 것으로 대마와는 다르게 분류되고 환각성이 강하며 부정맥 등 심장질환의 위험을 증가시키고 불안증, 공황, 발작 등 신경학적 부작용을 일으킴.";
                break;
        }

        BigName_Text.text = nameBig;
        name_Text.text = nameSmall;
        type_Text.text = type;
        info_Text.text = description;
    }

    /// <summary>영상을 시청한 버튼의 강조 애니메이션을 멈추고 이미지와 글자를 어둡게 한다.</summary>
    public void Btn_Used()
    {
        GetComponent<Animator>().SetTrigger("Off");

        Image image = transform.GetChild(0).GetComponentInChildren<Image>();
        image.color = UsedColor;

        TextMeshProUGUI label = GetComponentInChildren<TextMeshProUGUI>();
        label.color = UsedColor;
    }
}
