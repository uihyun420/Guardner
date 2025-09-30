using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReCellUi : GenericWindow
{
    [SerializeField] private TextMeshProUGUI guardnerInfoText;
    [SerializeField] private TextMeshProUGUI reCellText;

    [SerializeField] private Button reCellButton;
    [SerializeField] private GuardnerSpawner guardnerSpawner;
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private GameObject spawnRect;
    

    private GuardnerBehavior targetGuardner; // 추가: 현재 선택된 가드너 참조


    private void Awake()
    {
        reCellButton.onClick.AddListener(OnClickReCellButton);
    }

    public void Open(GuardnerBehavior guardner)
    {
        targetGuardner = guardner;
        // 가드너 월드 위치를 스크린 좌표로 변환
        Vector3 worldPos = guardner.transform.position;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos + Vector3.up * 1.2f); // 위로 약간 띄움
        transform.position = screenPos;

        var sb = new StringBuilder();
        sb.Clear();
        sb.Append(guardner.guardnerData.Name);
        guardnerInfoText.text = sb.ToString();

        int sellGold = Mathf.RoundToInt(targetGuardner.guardnerData.SellingGold * 0.7f);
        sb.Clear();
        sb.Append("판매 골드 ").Append("\n").Append(sellGold).Append("G");
        reCellText.text = sb.ToString();

        SpawnRectUnEnable();
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        targetGuardner = null;
        SpawnRectEnable();
    }

    private void SpawnRectUnEnable()
    {
        var collider = spawnRect.GetComponent<Collider2D>();
        if(collider != null)
        {
            collider.enabled = false;
        }
    }
    private void SpawnRectEnable()
    {
        var collider = spawnRect.GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    private void OnClickReCellButton()
    {
        if (targetGuardner == null) return;

        int sellGold = Mathf.RoundToInt(targetGuardner.guardnerData.SellingGold * 0.7f);

        battleUi.AddGold(sellGold);

        battleUi.UpdateGuardnerPlusCount();

        battleUi.SetGoldText();
        Destroy(targetGuardner.gameObject);

        // UI 창 닫기
        Close();
    }
}
