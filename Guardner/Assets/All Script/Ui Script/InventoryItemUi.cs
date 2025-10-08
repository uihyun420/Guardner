using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class InventoryItemUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private Image iconImage;

    public void SetCount(int count)
    {
        countText.text = count.ToString();
        if (count <= 0)
        {
            countText.color = Color.gray;
            if (iconImage != null)
                iconImage.color = Color.gray;
        }
        else
        {
            //2.원래 색상 복원
            countText.color = Color.white;
            if (iconImage != null)
                iconImage.color = Color.white;
        }
    }
}