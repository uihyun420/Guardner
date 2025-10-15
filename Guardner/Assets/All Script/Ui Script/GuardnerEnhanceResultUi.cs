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
    [SerializeField] private TextMeshProUGUI enhancePriceText;
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

        // ����� ����� ���� Ȯ�� (����� �����Ͱ� �ִٸ� ���)
        var savedData = SaveLoadManager.Data.GetGuardnerEnhance(guardnerId.ToString());
        if (savedData != null)
        {
            this.currentLevel = savedData.Level;
        }
        else
        {
            this.currentLevel = currentLevel;
        }

        var currentData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, this.currentLevel);
        var nextData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, this.currentLevel + 1);

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

        sb.Clear();
        sb.Append("��ȭ ��� : ").Append(nextData.NeedGold);
        enhancePriceText.text = sb.ToString();

        if (nextData != null)
        {
            sb.Clear();
            sb.Append("��ȭ \n")
              .Append("Lv : ").Append(nextData.Level).Append("\n")
              .Append("���ݼӵ� : ").Append(nextData.APS).Append("\n")
              .Append("DPS : ").Append(nextData.DPS);
            
            enhancedStatus.text = sb.ToString();
            enhanceButton.interactable = true;

            sb.Clear();
            sb.Append("���ݷ� : ").Append(currentData.AttackPower).Append("+").Append(nextData.AttackPower - currentData.AttackPower);
            attackText.text = sb.ToString();            
        }
        else
        {
            enhancedStatus.text = "�ִ� ����";
            enhanceButton.interactable = false;
            attackText.text = $"���ݷ� : {currentData.AttackPower} (�ִ�)";
        }
    }

    private void OnClickEnhanceButton()
    {
        var nextData = DataTableManager.GuardnerEnhanceTable.Get(currentGuardnerId, currentLevel + 1);

        if (nextData == null)
        {
            enhanceButton.interactable = false;
            return;
        }

        if (SaveLoadManager.Data.Gold < nextData.NeedGold)
        {
            return;
        }

        // 4. ��ȭ ����
        SaveLoadManager.Data.Gold -= nextData.NeedGold;

        // 5. ��ȭ�� ����� ������ ���� �� ����
        var enhancedGuardner = new GuardnerSaveData
        {
            Level = nextData.Level,
            AttackPower = nextData.AttackPower,
            Health = nextData.GateHP, // GateHP�� Health�� ���
            AttackSpeed = nextData.APS,
            MovementSpeed = 1.0f, // �⺻�� (���̺� MovementSpeed�� ���ٸ�)
        };

        SaveLoadManager.SaveGuardnerEnhance(currentGuardnerId.ToString(), enhancedGuardner);

        SaveLoadManager.UnlockGuardner(currentGuardnerId.ToString());

        currentLevel = nextData.Level;
        SetEnhanceData(currentGuardnerId, currentLevel);

        // ���� UI�� ��� ����
        if (mainMenuUi != null)
        {
            mainMenuUi.mainUiGold = SaveLoadManager.Data.Gold;
        }
        SoundManager.soundManager.PlaySFX("LevelUpSfx");
    }    

    private void OnClickExitButton()
    {
        Close();
    }
}