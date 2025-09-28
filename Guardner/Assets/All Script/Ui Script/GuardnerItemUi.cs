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

    [SerializeField] private TextMeshProUGUI dictionaryName;
    [SerializeField] private TextMeshProUGUI dictionaryDiscription;

    private int guardnerId;

    public void SetData(GuardnerData data, System.Action<int> onSelect)
    {
        guardnerId = data.Id;

        if (infoText != null)
        {
            // 강화된 가드너 정보 확인
            var enhancedData = SaveLoadManager.GetGuardnerStats(data.Id.ToString());

            if (enhancedData != null)
            {
                // 강화된 데이터가 있으면 강화된 수치 표시
                infoText.text = $"{data.Name} Lv.{enhancedData.Level}\n공격력: {enhancedData.AttackPower} (+{enhancedData.AttackPower - data.AttackPower})\n골드: {data.SummonGold}";
            }
            else
            {
                // 기본 데이터 표시
                infoText.text = $"{data.Name}\n공격력: {data.AttackPower}\n골드: {data.SummonGold}";
            }
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
