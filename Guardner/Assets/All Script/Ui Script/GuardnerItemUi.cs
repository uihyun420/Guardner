using System.Collections;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerItemUi : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Image guardnerImage; // 스킬 아이콘 이미지
    public Button selectButton;


    [SerializeField] private TextMeshProUGUI guardnerSpwawnDiscriptText;
    [SerializeField] private TextMeshProUGUI nameText;

    [SerializeField] private TextMeshProUGUI dictionaryName;
    [SerializeField] private TextMeshProUGUI dictionaryDiscription;


    private int guardnerId;

    public void SetNameTextColor(Color color)
    {
        nameText.color = color;
    }

    public void SetData(GuardnerData data, System.Action<int> onSelect)
    {
        guardnerId = data.Id;

        // 1. 이름 표시
        if (nameText != null)
            nameText.text = data.Name;

        // 2. 강화 정보 가져오기
        var enhancedData = SaveLoadManager.GetGuardnerStats(data.Id.ToString());
        int attackPower = enhancedData?.AttackPower ?? data.AttackPower;

        // 3. 스킬명 가져오기 (SkillID가 0이 아니면)
        //string skillName = "-";
        //if (data.SkillID != 0)
        //{
        //    var skillData = DataTableManager.GuardnerSkillTable.Get(data.SkillID).Name;
        //    if (skillData != null)
        //        skillName = skillData;
        //}

        // 4. 설명 텍스트 구성
        if (guardnerSpwawnDiscriptText != null)
        {
            guardnerSpwawnDiscriptText.text =
                $"공격력: {attackPower}\n" +
                $"공격속도: {data.APS}\n" +
                $"소환 골드: {data.SummonGold}";
        }

        // 5. 기존 infoText도 강화 정보 반영(원한다면)
        if (infoText != null)
        {
            if (enhancedData != null)
                infoText.text = $"{data.Name} Lv.{enhancedData.Level}\n공격력: {attackPower} (+{attackPower - data.AttackPower})\n골드: {data.SummonGold}";
            else
                infoText.text = $"{data.Name}\n공격력: {data.AttackPower}\n골드: {data.SummonGold}";
        }

        // 6. 이미지
        if (guardnerImage != null)
        {
            string imagePath = $"GuardnerIcons/guardner_{data.GuardenerDrawId}";
            var sprite = Resources.Load<Sprite>(imagePath);
            guardnerImage.sprite = sprite;
            guardnerImage.gameObject.SetActive(sprite != null);
        }

        // 7. 버튼
        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect(data.Id));
        }
    }


    public void SetTextColor(Color color)
    {
        if(infoText != null)
        {
            infoText.color = color;
        }
    }

    public void DictionarySetData(GuardnerData data, System.Action<int> onSelect)
    {
        guardnerId = data.Id;

        if (dictionaryName != null)
        {
            var nameBuilder = new StringBuilder();
            nameBuilder.Append(data.Name);
            dictionaryName.text = nameBuilder.ToString();
        }

        if (dictionaryDiscription != null)
        {
            var descBuilder = new StringBuilder();
            descBuilder.Append(data.Reference).Append("\n").Append("공격력: ").Append(data.AttackPower);
            dictionaryDiscription.text = descBuilder.ToString();
        }

        if (guardnerImage != null)
        {
            string imagePath = $"GuardnerIcons/guardner_{data.GuardenerDrawId}";
            var sprite = Resources.Load<Sprite>(imagePath);

            if (sprite != null)
            {
                guardnerImage.sprite = sprite;
                guardnerImage.gameObject.SetActive(true);
            }
            else
            {
                guardnerImage.gameObject.SetActive(false);
            }
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect(data.Id));
        }
    }
}
