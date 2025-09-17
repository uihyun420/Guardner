using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerItemUi : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Button selectButton;

    public void SetData(GuardnerData data, System.Action<int> onSelect)
    {
        if (infoText != null)
        {
            // 모든 정보를 하나의 텍스트에 표시
            infoText.text = $"{data.Name}\n공격력: {data.AttackPower}\n골드: {data.SummonGold}";
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect(data.Id));
        }
    }
}
