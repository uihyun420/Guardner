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

    //[SerializeField] private TextMeshProUGUI 
    //[SerializeField] private TextMeshProUGUI enhanceButtonText;

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
        this.currentLevel = currentLevel;

        var currentData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, currentLevel);
        var nextData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, currentLevel + 1);

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

        if (nextData != null)
        {
            sb.Clear();
            sb.Append("강화 후 능력치\n")
              .Append("Lv : ").Append(nextData.Level).Append("\n")
              .Append("공격속도 : ").Append(nextData.APS).Append("\n")
              .Append("DPS : ").Append(nextData.DPS);
            enhancedStatus.text = sb.ToString();
            enhanceButton.interactable = true;
        }
        else
        {
            enhancedStatus.text = "최대 레벨";
            enhanceButton.interactable = false;
        }

        sb.Clear();
        sb.Append("공격력 : ").Append(currentData.AttackPower).Append("+").Append(nextData.AttackPower - currentData.AttackPower);
        attackText.text = sb.ToString();    

    }
    private void OnClickEnhanceButton()
    {
        var nextData = DataTableManager.GuardnerEnhanceTable.Get(currentGuardnerId, currentLevel + 1);

        // 1. 최대 레벨 체크
        if (nextData == null)
        {
            Debug.Log("최대 레벨입니다.");
            enhanceButton.interactable = false;
            return;
        }

        // 2. 골드 확인
        if (mainMenuUi.mainUiGold < nextData.NeedGold)
        {
            Debug.Log($"골드 부족! 필요: {nextData.NeedGold}, 보유: {mainMenuUi.mainUiGold}");
            // 필요시 경고 UI 표시
            return;
        }

        // 3. 아이템 확인 (현재는 아이템 시스템이 없으므로 생략)
        // if (!HasItem(nextData.NeedItemId, nextData.NeedItemQty))
        // {
        //     Debug.Log($"아이템 부족! ID: {nextData.NeedItemId}, 필요 수량: {nextData.NeedItemQty}");
        //     return;
        // }

        // 4. 재화 차감
        mainMenuUi.mainUiGold -= nextData.NeedGold;
        // UseItem(nextData.NeedItemId, nextData.NeedItemQty); // 아이템 시스템 구현 시 사용

        // 5. 강화 레벨 증가 (실제 데이터 저장 필요)
        // PlayerData.SetGuardnerLevel(currentGuardnerId, currentLevel + 1);

        // 6. UI 갱신
        SetEnhanceData(currentGuardnerId, currentLevel + 1);

        Debug.Log($"{nextData.Name} 강화 성공! Lv.{currentLevel} → Lv.{currentLevel + 1}");

        // 7. 필요시 강화 성공 이펙트, 사운드 등 추가

    }

    private void OnClickExitButton()
    {
        Close();
    }


}
