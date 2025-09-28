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
            // 1. ���� ��ü ��Ȱ��ȭ
            //gameObject.SetActive(false);


            countText.color = Color.gray;
            if (iconImage != null)
                iconImage.color = Color.gray;
        }
        else
        {
            // 1. ���� Ȱ��ȭ
            //gameObject.SetActive(true);

            //2.���� ���� ����
            countText.color = Color.white;
            if (iconImage != null)
                iconImage.color = Color.white;
        }
    }
}