
using UnityEngine;
using UnityEngine.UI;

public class StageClearUi : GenericWindow
{
    [SerializeField] private Button Button;
    public override void Open()
    {
        base.Open();
        Time.timeScale = 0;
    }
}
