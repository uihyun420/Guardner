using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerEnhanceResultUi : GenericWindow
{
    [SerializeField] private Button enhanceButton;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI currentStatus;
    [SerializeField] private TextMeshProUGUI enhancedStatus;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private Button exitButton;
    [SerializeField] private Image guardnerImage; // Inspector���� ����

    //[SerializeField] private TextMeshProUGUI 
    //[SerializeField] private TextMeshProUGUI enhanceButtonText;

    [SerializeField] private MainMenuUi mainMenuUi; // Inspector���� ����

    private int currentGuardnerId;
    private int currentLevel;

    private void OnEnable()
    {
        enhanceButton.onClick.RemoveAllListeners();
        enhanceButton.onClick.AddListener(OnClickEnhanceButton);
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

    public void SetEnhanceData(int guardnerId, int currentLevel)
    {
        var sb = new StringBuilder();

        this.currentGuardnerId = guardnerId;
        this.currentLevel = currentLevel;

        var currentData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, currentLevel);
        var nextData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, currentLevel + 1);

        var sprite = Resources.Load<Sprite>($"GuardnerIcons/{guardnerId}");
        if (guardnerImage != null)
            guardnerImage.sprite = sprite;

        if (currentData == null)
        {
            nameText.text = "������ ����";
            currentStatus.text = "-";
            enhancedStatus.text = "-";
            attackText.text = "-";
            enhanceButton.interactable = false;
            return;
        }
        sb.Clear();
        sb.Append(currentData.Name);
        nameText.text = sb.ToString();

        sb.Clear();
        sb.Append("���� �ɷ�ġ\n").Append("Lv : ").Append(currentData.Level).Append("\n").Append("���ݼӵ� : ").Append(currentData.APS).Append("\n").Append("DPS : ").Append(currentData.DPS);
        currentStatus.text = sb.ToString();

        if (nextData != null)
        {
            sb.Clear();
            sb.Append("��ȭ �� �ɷ�ġ\n")
              .Append("Lv : ").Append(nextData.Level).Append("\n")
              .Append("���ݼӵ� : ").Append(nextData.APS).Append("\n")
              .Append("DPS : ").Append(nextData.DPS);
            enhancedStatus.text = sb.ToString();
            enhanceButton.interactable = true;
        }
        else
        {
            enhancedStatus.text = "�ִ� ����";
            enhanceButton.interactable = false;
        }

        sb.Clear();
        sb.Append("���ݷ� : ").Append(currentData.AttackPower).Append("+").Append(nextData.AttackPower - currentData.AttackPower);
        attackText.text = sb.ToString();    

    }
    private void OnClickEnhanceButton()
    {
        var nextData = DataTableManager.GuardnerEnhanceTable.Get(currentGuardnerId, currentLevel + 1);

        // 1. �ִ� ���� üũ
        if (nextData == null)
        {
            Debug.Log("�ִ� �����Դϴ�.");
            enhanceButton.interactable = false;
            return;
        }

        // 2. ��� Ȯ��
        if (mainMenuUi.mainUiGold < nextData.NeedGold)
        {
            Debug.Log($"��� ����! �ʿ�: {nextData.NeedGold}, ����: {mainMenuUi.mainUiGold}");
            // �ʿ�� ��� UI ǥ��
            return;
        }

        // 3. ������ Ȯ�� (����� ������ �ý����� �����Ƿ� ����)
        // if (!HasItem(nextData.NeedItemId, nextData.NeedItemQty))
        // {
        //     Debug.Log($"������ ����! ID: {nextData.NeedItemId}, �ʿ� ����: {nextData.NeedItemQty}");
        //     return;
        // }

        // 4. ��ȭ ����
        mainMenuUi.mainUiGold -= nextData.NeedGold;
        // UseItem(nextData.NeedItemId, nextData.NeedItemQty); // ������ �ý��� ���� �� ���

        // 5. ��ȭ ���� ���� (���� ������ ���� �ʿ�)
        // PlayerData.SetGuardnerLevel(currentGuardnerId, currentLevel + 1);

        // 6. UI ����
        SetEnhanceData(currentGuardnerId, currentLevel + 1);

        Debug.Log($"{nextData.Name} ��ȭ ����! Lv.{currentLevel} �� Lv.{currentLevel + 1}");

        // 7. �ʿ�� ��ȭ ���� ����Ʈ, ���� �� �߰�

    }

    private void OnClickExitButton()
    {
        Close();
    }


}
