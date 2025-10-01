using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryInfoUi : GenericWindow
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI attackPowerText;
    [SerializeField] private TextMeshProUGUI referenceText;
    [SerializeField] private Image guardnerImage;

    [SerializeField] private Button closeButton;

    public void SetGuardnerInfo(GuardnerData data)
    {
        if (nameText != null)
            nameText.text = data.Name;

        if (attackPowerText != null)
            attackPowerText.text = $"{data.AttackPower}";

        if (referenceText != null)
            referenceText.text = data.Reference;

        if (guardnerImage != null)
        {
            string imagePath = $"GuardnerIcons/guardner_{data.GuardenerDrawId}";
            var sprite = Resources.Load<Sprite>(imagePath);

            if (sprite != null)
            {
                guardnerImage.sprite = sprite;
                guardnerImage.gameObject.SetActive(true);
            }
            else
            {
                guardnerImage.gameObject.SetActive(false);
            }
        }
    }

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);

    }
    public override void Open()
    {
        base.Open();
    }

    public override void Close()
    {
        base.Close();
        SoundManager.soundManager.PlaySFX("UiClickSfx");
    }
}
