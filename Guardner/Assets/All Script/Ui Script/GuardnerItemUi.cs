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
            // ��� ������ �ϳ��� �ؽ�Ʈ�� ǥ��
            infoText.text = $"{data.Name}\n���ݷ�: {data.AttackPower}\n���: {data.SummonGold}";
        }

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect(data.Id));
        }
    }
}
