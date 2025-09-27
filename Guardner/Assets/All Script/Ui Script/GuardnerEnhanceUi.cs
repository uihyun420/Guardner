using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardnerEnhanceUi : GenericWindow
{
    public ScrollRect scrollRect;
    public Transform contentParent;
    public GameObject guardnerEnhanceItemPrefab; // �� ����� �׸� ������

    [SerializeField] private Button BackButton;
    [SerializeField] private Button guardnerButton;

    [SerializeField] private GuardnerEnhanceResultUi guardnerEnhanceResultUi;   

    // ���� ��ȭ ���� ���� (����: �����δ� ���̺� ������ ��� �ҷ��;� ��)
    private Dictionary<int, int> guardnerLevelDict = new Dictionary<int, int>();

    private void OnEnable()
    {
        ResetList();
    }

    private void Awake()
    {
        BackButton.onClick.AddListener(OnClickBackButton);
    }
    public override void Open()
    {
        base.Open();
    }
    public override void Close()
    {
        base.Close();
    }

    public void ResetList()
    {
        // ���� �׸� ����
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ��� ��ȭ ������ �������� (Id+Level �������, Id���� ���� ������ ǥ��)
        foreach (var guardnerId in GetAllGuardnerIds())
        {
            int level = GetCurrentLevel(guardnerId);
            var enhanceData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, level);
            if (enhanceData != null)
            {
                CreateEnhanceItem(enhanceData, guardnerId, level);
            }
        }
    }

    private IEnumerable<int> GetAllGuardnerIds()
    {
        // ���� ���������� ������ ����� ��� ��� �����;� ��
        // ����: ��ȭ���̺� �ִ� ��� Id
        var ids = new HashSet<int>();
        foreach (var data in DataTableManager.GuardnerEnhanceTable.GetAll())
            ids.Add(data.Id);
        return ids;
    }
    private int GetCurrentLevel(int guardnerId)
    {
        // �����δ� �÷��̾� �����Ϳ��� ���� ��ȭ ������ �����;� ��
        if (guardnerLevelDict.TryGetValue(guardnerId, out int level))
            return level;
        return 1; // �⺻ 1����
    }

    private void CreateEnhanceItem(GuardnerEnhanceData data, int guardnerId, int level)
    {
        var go = Instantiate(guardnerEnhanceItemPrefab, contentParent);
        var itemUi = go.GetComponent<GuardnerEnhanceItemUi>();
        if (itemUi != null)
        {
            var sprite = GetGuardnerSprite(guardnerId);
            // �� �� ���� ȣ���ϰ�, ���� guardnerId ����
            itemUi.SetData(data, sprite, () => OnEnhanceButton(guardnerId));
        }
    }
    private Sprite GetGuardnerSprite(int guardnerId)
    {
        var sprite = Resources.Load<Sprite>($"GuardnerIcons/{guardnerId}");
        if (sprite == null)
            Debug.LogWarning($"�̹��� ����: GuardnerIcons/{guardnerId}");
        return sprite;
    }

    private void OnEnhanceButton(int guardnerId)
    {
        int level = GetCurrentLevel(guardnerId);
        //var nextData = DataTableManager.GuardnerEnhanceTable.Get(guardnerId, level + 1);
        //if (nextData != null)
        //{
        //    guardnerLevelDict[guardnerId] = level + 1;
        //    ResetList();
        //}
        //else
        //{
        //    Debug.Log("�ִ� �����Դϴ�.");
        //}

        guardnerEnhanceResultUi.SetEnhanceData(guardnerId, level);
        guardnerEnhanceResultUi.Open();
    }

    private void OnClickBackButton()
    {
        Close();
    }


}
