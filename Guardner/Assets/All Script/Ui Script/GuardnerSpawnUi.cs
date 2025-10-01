using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class GuardnerSpawnUi : GenericWindow
{
    public ScrollRect scrollRect;

    public GameObject guardnerItemPrefab; // ����� ������ ui ������ 
    public Transform contentParent; // ��ũ�ѷ�Ʈ�� content

    [SerializeField] private GameObject blockScreenPanel;
    [SerializeField] private GuardnerSpawner guardnerSpawner; // inspector���� ����
    [SerializeField] private BattleUi battleUi;
    [SerializeField] private Button ExitButton;
    [SerializeField] private ScreenTouch screenTouch; // ScreenTouch ���� �߰�
    [SerializeField] private ReCellUi reCellUi;
    

    private int selectedGuardnerId; // ���õ� ����� ID ����
    private int selectedAreaIndex;

    private void Awake()
    {
        isOverlayWindow = true;
        ExitButton.onClick.AddListener(OnClickExitButton);
    }

    public override void Open()
    {
        reCellUi.gameObject.SetActive(false);
        reCellUi.enabled = false;

        selectedAreaIndex = guardnerSpawner.screenTouch.GetSelectedAreaIndex();
        if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
        {
            Vector2 expectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;
            if (guardnerSpawner.IsGuardnerAtPosition(expectedSpawnPos))
            {
                Debug.Log("�̹� ����ʰ� ��ȯ�� ��ġ�Դϴ�.");
                return;
            }
        }

        base.Open();

        // ScreenTouch�� ������ ��Ȱ��ȭ
        if (screenTouch != null)
        {
            screenTouch.enabled = false; // ������Ʈ ��ü�� ��Ȱ��ȭ
        }

        // ��ȭ ������ ���ŵ� �� �����Ƿ� �Ź� ���� ǥ��
        DisplayAvailableGuardner();

        // ScrollRect ������ DisplayAvailableGuardner �Ŀ� ���� (PlayerSkillSetUi�� �����ϰ�)
        if (scrollRect != null)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.enabled = true;
            scrollRect.gameObject.SetActive(true);
        }

        // ���� �� ScrollRect �ʱ�ȭ (PlayerSkillSetUi�� ������ �ڷ�ƾ ���)
        StartCoroutine(InitializeScrollRect());
    }


    private IEnumerator InitializeScrollRect()
    {
        // 2������ ��� (UI�� ������ ������ ������)
        yield return null;
        yield return null;

        if (scrollRect != null)
        {
            // Canvas ���� ������Ʈ
            Canvas.ForceUpdateCanvases();

            // ScrollRect �缳��
            scrollRect.enabled = false;
            yield return null;
            scrollRect.enabled = true;

            // ��ũ�� ��ġ�� �� ���� �ʱ�ȭ
            scrollRect.verticalNormalizedPosition = 1f;

            // EventSystem Ŭ���� �� �ٽ� ����
            EventSystem.current.SetSelectedGameObject(null);
            yield return null;
            EventSystem.current.SetSelectedGameObject(scrollRect.gameObject);
        }
    }

    public override void Close()
    {
        // ScreenTouch �ٽ� Ȱ��ȭ
        if (screenTouch != null)
        {
            screenTouch.enabled = true;
        }

        base.Close();

    }

    private void DisplayAvailableGuardner()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // ������ ����ʸ� ǥ���ϵ��� ����
        if (guardnerSpawner != null)
        {
            foreach (var guardnerId in guardnerSpawner.ownedGuardnerIds)
            {
                var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
                if (guardnerData != null)
                {
                    CreateGuardnerItem(guardnerData, guardnerId);
                }
            }
        }

        // ������ ũ��� ����Ʈ ũ�� ���ؼ� ��ũ�� Ȱ��/��Ȱ��
        if (scrollRect != null)
        {
            var contentHeight = scrollRect.content.rect.height;
            var viewportHeight = scrollRect.viewport.rect.height;
            scrollRect.vertical = contentHeight > viewportHeight;
        }
    }

    private void CreateGuardnerItem(GuardnerData data, int guardnerId)
    {
        var item = Instantiate(guardnerItemPrefab, contentParent);
        var itemUi = item.GetComponent<GuardnerItemUi>();

        if (itemUi != null)
        {
            itemUi.SetData(data, OnSelectGuardner);

            if (battleUi.gold < data.SummonGold)
            {
                itemUi.SetTextColor(Color.red);
                itemUi.SetNameTextColor(Color.red);
            }
            else
            {
                itemUi.SetTextColor(Color.white);
                itemUi.SetNameTextColor(Color.white);
            }
        }
    }

    private void OnSelectGuardner(int guardnerId)
    {
        selectedGuardnerId = guardnerId;

        if (selectedAreaIndex >= 0 && selectedAreaIndex < guardnerSpawner.spawnPos.Length)
        {
            Vector2 selectedSpawnPos = guardnerSpawner.spawnPos[selectedAreaIndex].transform.position;

            if (guardnerSpawner.IsGuardnerAtPosition(selectedSpawnPos))
            {
                Debug.Log("����ʰ� �̹� �����մϴ�.");
                return;
            }
           if(guardnerSpawner.SpawnGuardner(guardnerId, selectedSpawnPos))
            {
                battleUi.UpdateGuardnerCount();
            }
        }
        Close();
    }

    private void OnClickExitButton()
    {
        Close();
    }
}
