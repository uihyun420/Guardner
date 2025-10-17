using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GenericWindow : MonoBehaviour
{
    public GameObject firstSelected;
    protected WindowManager manager;

    [SerializeField] protected bool isOverlayWindow = false; // �������� â���� ����  
    [SerializeField] protected ScreenTouch screenTouch;
    [SerializeField] protected BattleUi battleUi;

    public void Init(WindowManager mgr)
    {
        manager = mgr;
    }

    public void OnFocus()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
    public virtual void Open()
    {
        gameObject.SetActive(true);
        OnFocus();
        
        if(isOverlayWindow)
        {
            if(screenTouch != null)
            {
                screenTouch.SetUiBlocking(true);
            }
            BlockBattleUI(true); // �������� â�� ���� �� ��ų ��ư ����
        }        
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);

        if (isOverlayWindow)
        {
            if (screenTouch != null)
                screenTouch.SetUiBlocking(false);

            BlockBattleUI(false);
        }        
    }

    public void BlockBattleUI(bool block)
    {
        if(battleUi != null)
        {
            if (battleUi.skill1 != null) battleUi.skill1.interactable = !block;
            if (battleUi.skill2 != null) battleUi.skill2.interactable = !block;
            if (battleUi.skill3 != null) battleUi.skill3.interactable = !block;
        }
    }
}
