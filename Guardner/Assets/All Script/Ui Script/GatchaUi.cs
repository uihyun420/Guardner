using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GatchaUi : GenericWindow
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI guardnerDicriptText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image image;
    [SerializeField] private Button exitButton;



    private void Awake()
    {
        exitButton.onClick.AddListener(OnClickExitButton);
    }

    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
    }


    private void OnClickExitButton()
    {
        Close();
    }

    public void SetGuardnerInfo(GuardnerEnhanceData data, Sprite sprite)
    {
        nameText.text = data.Name;
        guardnerDicriptText.text = $"���ݷ� : {data.AttackPower}\n" +
            $"���ݼӵ� : {data.APS}\n" +
            $"���� ���� : {data.AttackRange}\n" +
            $"{data.Reference}";

        attackText.text = $"{data.AttackPower}";
        levelText.text = $"{data.Level}";
        image.sprite = sprite;
    }

}
