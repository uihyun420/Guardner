using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerEnhanceItemUi : MonoBehaviour
{
    [SerializeField] private Image guardnerImage;
    [SerializeField] private TextMeshProUGUI nameText;
    //public TextMeshProUGUI levelText;
    //public TextMeshProUGUI statText;
    //public Button enhanceButton;

    public void SetData(GuardnerEnhanceData data, Sprite sprite, System.Action onEnhance)
    {
        guardnerImage.sprite = sprite;
        nameText.text = data.Name;
        //levelText.text = $"Lv.{data.Level}";
        //statText.text = $"���ݷ�: {data.AttackPower}\n���ݼӵ�: {data.APS}\nDPS: {data.DPS}";
        //enhanceButton.onClick.RemoveAllListeners();
        //enhanceButton.onClick.AddListener(() => onEnhance?.Invoke());
    }
}
