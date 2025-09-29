using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryUi : GenericWindow
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject dictionarySlot;
    [SerializeField] private Transform contentParent;

    [SerializeField] private Button closeButton;

    private HashSet<int> ownedGuardnerIds = new HashSet<int>(); // �������ִ� ����� ���

    private void Start()
    {
        closeButton.onClick.AddListener(OnClickCloseButton);
    }

    public override void Open()
    {
        base.Open();
        InitializeOwnedGuardenr();
        DisPlayerOwnedGuardner();
    }

    public override void Close()
    {
        base.Close();
    }

    public void OnClickCloseButton()
    {
        Close();
    }

    private void InitializeOwnedGuardenr()
    {
        // ���߿��� ���̺� �����Ϳ��� �ε�
        ownedGuardnerIds.Add(11120);
        ownedGuardnerIds.Add(11125);
        ownedGuardnerIds.Add(11135);
        ownedGuardnerIds.Add(11235);
        ownedGuardnerIds.Add(11245);
        ownedGuardnerIds.Add(11250);
        ownedGuardnerIds.Add(11255);
        ownedGuardnerIds.Add(11260);
        ownedGuardnerIds.Add(11270);
        ownedGuardnerIds.Add(11310);   
        ownedGuardnerIds.Add(11315);
        ownedGuardnerIds.Add(11320);

    }

    private void DisPlayerOwnedGuardner()
    {
        foreach(Transform guardner in contentParent)
        {
            Destroy(guardner.gameObject);
        }

        foreach(var guardnerId in ownedGuardnerIds)
        {
            var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
            if(guardnerData != null)
            {
                CreationDictionarySlot(guardnerData);
            }
        }

        //��ũ�� ����
        if (scrollRect != null)
        {
            var contentHeight = scrollRect.content.rect.height;
            var viewportHeight = scrollRect.viewport.rect.height;
            scrollRect.vertical = contentHeight > viewportHeight;
        }
    }

    private void CreationDictionarySlot(GuardnerData data)
    {
        var slot = Instantiate(dictionarySlot, contentParent);
        var itemUi = slot.GetComponent<GuardnerItemUi>();

        if(itemUi != null)
        {
            itemUi.DictionarySetData(data, null);
        }
    }

    //public void UnlockGuardner(int guardnerId)
    //{
    //    if(ownedGuardnerIds.Add(guardnerId))
    //    {
    //        Debug.Log("����� ���� �߰�");
    //    }
    //}

    //����� ������ �ִ��� Ȯ��
    public bool HasGuardner(int guardnerId)
    {
        return ownedGuardnerIds.Contains(guardnerId);
    }

}
