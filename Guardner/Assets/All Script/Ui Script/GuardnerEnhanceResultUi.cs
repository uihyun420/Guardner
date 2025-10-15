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
    [SerializeField] private Image guardnerImage; // Inspector에서 연결
    [SerializeField] private TextMeshProUGUI enhancePriceText;
    [SerializeField] private MainMenuUi mainMenuUi; // Inspector에서 연결

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

        // 저장된 가드너 레벨 확인 (저장된 데이터가 있다면 사용)
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
            nameText.text = "데이터 없음";
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
        sb.Append("현재 능력치\n").Append("Lv : ").Append(currentData.Level).Append("\n").Append("공격속도 : ").Append(currentData.APS).Append("\n").Append("DPS : ").Append(currentData.DPS);
        currentStatus.text = sb.ToString();

        sb.Clear();
        sb.Append("강화 비용 : ").Append(nextData.NeedGold);
        enhancePriceText.text = sb.ToString();

        if (nextData != null)
        {
            sb.Clear();
            sb.Append("강화 \n")
              .Append("Lv : ").Append(nextData.Level).Append("\n")
              .Append("공격속도 : ").Append(nextData.APS).Append("\n")
              .Append("DPS : ").Append(nextData.DPS);
            
            enhancedStatus.text = sb.ToString();
            enhanceButton.interactable = true;

            sb.Clear();
            sb.Append("공격력 : ").Append(currentData.AttackPower).Append("+").Append(nextData.AttackPower - currentData.AttackPower);
            attackText.text = sb.ToString();            
        }
        else
        {
            enhancedStatus.text = "최대 레벨";
            enhanceButton.interactable = false;
            attackText.text = $"공격력 : {currentData.AttackPower} (최대)";
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

        // 4. 재화 차감
        SaveLoadManager.Data.Gold -= nextData.NeedGold;

        // 5. 강화된 가드너 데이터 생성 및 저장
        var enhancedGuardner = new GuardnerSaveData
        {
            Level = nextData.Level,
            AttackPower = nextData.AttackPower,
            Health = nextData.GateHP, // GateHP를 Health로 사용
            AttackSpeed = nextData.APS,
            MovementSpeed = 1.0f, // 기본값 (테이블에 MovementSpeed가 없다면)
        };

        SaveLoadManager.SaveGuardnerEnhance(currentGuardnerId.ToString(), enhancedGuardner);

        SaveLoadManager.UnlockGuardner(currentGuardnerId.ToString());

        currentLevel = nextData.Level;
        SetEnhanceData(currentGuardnerId, currentLevel);

        // 메인 UI의 골드 갱신
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