using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GenericWindow : MonoBehaviour
{
    public GameObject firstSelected;
    protected WindowManager manager;

    [SerializeField] protected bool isOverlayWindow = false; // 오버레이 창인지 구분


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
            var screenTouch = FindObjectOfType<ScreenTouch>();
            if(screenTouch != null)
            {
                screenTouch.SetUiBlocking(true);
            }
            BlockBattleUI(true); // 오버레이 창이 열릴 때 스킬 버튼 차단
        }
        SoundManager.soundManager.PlaySFX("UiClickSfx");
    }
    public virtual void Close()
    {
        gameObject.SetActive(false);

        if (isOverlayWindow)
        {
            var screenTouch = FindObjectOfType<ScreenTouch>();
            if (screenTouch != null)
                screenTouch.SetUiBlocking(false);

            BlockBattleUI(false);
        }
        SoundManager.soundManager.PlaySFX("UiClickSfx");
    }


    public void BlockBattleUI(bool block)
    {
        BattleUi battleUi = FindObjectOfType<BattleUi>();
        if(battleUi != null)
        {
            if (battleUi.skill1 != null) battleUi.skill1.interactable = !block;
            if (battleUi.skill2 != null) battleUi.skill2.interactable = !block;
            if (battleUi.skill3 != null) battleUi.skill3.interactable = !block;
        }
    }
}
