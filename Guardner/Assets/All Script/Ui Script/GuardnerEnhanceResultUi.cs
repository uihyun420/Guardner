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
        this.currentGuardnerId = guardnerId;
        this.currentLevel = currentLevel;

        var currentData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, currentLevel);
        var nextData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, currentLevel + 1);

        if (currentData == null)
        {
            nameText.text = "������ ����";
            currentStatus.text = "-";
            enhancedStatus.text = "-";
            attackText.text = "-";
            enhanceButton.interactable = false;
            return;
        }

        nameText.text = currentData.Name;
        currentStatus.text = $"Lv.{currentData.Level}\n���ݷ�: {currentData.AttackPower}\n���ݼӵ�: {currentData.APS}\nDPS: {currentData.DPS}";
        attackText.text = $"����: {currentData.AttackPower}";

        if (nextData != null)
        {
            enhancedStatus.text = $"Lv.{nextData.Level}\n���ݷ�: {nextData.AttackPower}\n���ݼӵ�: {nextData.APS}\nDPS: {nextData.DPS}";
            enhanceButton.interactable = true;
        }
        else
        {
            enhancedStatus.text = "�ִ� ����";
            enhanceButton.interactable = false;
        }
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
