using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerEnhanceItemUi : MonoBehaviour
{
    [SerializeField] private Image guardnerImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Button EnhanceButton;
    //public TextMeshProUGUI levelText;
    //public TextMeshProUGUI statText;
    //public Button enhanceButton;

    private void Awake()
    {
        EnhanceButton.onClick.AddListener(OnClickEnhanceButton);
    }


    public void SetData(GuardnerEnhanceData data, Sprite sprite, System.Action onEnhance)
    {
        guardnerImage.sprite = sprite;
        nameText.text = data.Name;
        if (sprite == null)
            Debug.LogWarning($"Sprite�� null�Դϴ�! id: {data.Id}");
        //levelText.text = $"Lv.{data.Level}";
        //statText.text = $"���ݷ�: {data.AttackPower}\n���ݼӵ�: {data.APS}\nDPS: {data.DPS}";
        //enhanceButton.onClick.RemoveAllListeners();
        //enhanceButton.onClick.AddListener(() => onEnhance?.Invoke());
    }

    private void OnClickEnhanceButton()
    {

    }
}
