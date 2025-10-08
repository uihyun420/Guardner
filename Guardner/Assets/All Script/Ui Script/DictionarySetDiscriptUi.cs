using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DictionarySetDiscriptUi : MonoBehaviour
{
    [Header("Set 1")]
    [SerializeField] private TextMeshProUGUI set1NameText;
    [SerializeField] private TextMeshProUGUI set1RewardText;

    [Header("Set 2")]
    [SerializeField] private TextMeshProUGUI set2NameText;
    [SerializeField] private TextMeshProUGUI set2RewardText;

    [Header("Set 3")]
    [SerializeField] private TextMeshProUGUI set3NameText;
    [SerializeField] private TextMeshProUGUI set3RewardText;

    private void Start()
    {
        DisplayAllSets();
    }

    public void DisplayAllSets()
    {
        var allSets = DataTableManager.DictionarySetTable.GetAll();
        var setArray = new DictionarySetData[3];

        int index = 0;
        foreach (var setData in allSets)
        {
            if (index < 3)
            {
                setArray[index] = setData;
                index++;
            }
        }

        // Set 1
        if (setArray[0] != null)
        {
            set1NameText.text = setArray[0].SetName;
            set1RewardText.text = $"보상: {setArray[0].RewardAmount}";
        }

        // Set 2
        if (setArray[1] != null)
        {
            set2NameText.text = setArray[1].SetName;
            set2RewardText.text = $"보상: {setArray[1].RewardAmount}";
        }

        // Set 3
        if (setArray[2] != null)
        {
            set3NameText.text = setArray[2].SetName;
            set3RewardText.text = $"보상: {setArray[2].RewardAmount}";
        }
    }
}