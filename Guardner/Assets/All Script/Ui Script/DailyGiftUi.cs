using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyGiftUi : GenericWindow
{
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI timeText; // Inspector ����
    private TimeSpan remainTime;
    private bool isTimerActive = false;

    [SerializeField] private Button[] dailyGiftButtons = new Button[20]; // 20��ġ ��ư

    [SerializeField] private GameObject checkMarkGoldPrefab;
    [SerializeField] private GameObject checkMarkLotteryPrefab;
    [SerializeField] private GameObject checkMarkSkillPrefab;
    private GameObject[] checkMarkInstances = new GameObject[20];


    private int goldAmount = 1000; // 1~3�� ��� ����


    [SerializeField] private InventoryUi inventoryUi;
    [SerializeField] private Button exitButton;

    private const string lastDateKey = "LastClaimDate";
    private const string currentStreakKey = "CurrentStreak";

    private void Start()
    {
        InitializeButtons();
        exitButton?.onClick.AddListener(Close);
        PlayerPrefs.DeleteKey("LastClaimDate"); // �׽�Ʈ
        PlayerPrefs.DeleteKey("CurrentStreak");
        PlayerPrefs.Save();
    }

    public override void Open()
    {
        base.Open();
        if (dateText != null)
            dateText.text = DateTime.Now.ToString("HH:mm");
        UpdateDailyGiftUI();
        UpdateRemainTime();
        isTimerActive = true;
    }

    private void Update()
    {
        if (!isTimerActive) return;
        remainTime = remainTime.Subtract(TimeSpan.FromSeconds(Time.deltaTime));
        if (remainTime.TotalSeconds < 0)
        {
            remainTime = TimeSpan.Zero;
            isTimerActive = false;
            UpdateDailyGiftUI();
        }
        UpdateTimeText();
    }

    private void UpdateRemainTime()
    {
        DateTime now = DateTime.Now;
        DateTime tomorrow = now.Date.AddDays(1);
        remainTime = tomorrow - now;
        UpdateTimeText();
    }

    private void UpdateTimeText()
    {
        if (timeText != null)
            timeText.text = $"{(int)remainTime.TotalHours:D2}:{remainTime.Minutes:D2}";
    }

    private void InitializeButtons()
    {
        for (int i = 0; i < dailyGiftButtons.Length; i++)
        {
            int dayIndex = i; // Ŭ������ ���� ���� ����
            if (dailyGiftButtons[i] != null)
            {
                dailyGiftButtons[i].onClick.AddListener(() => OnDailyGiftButtonClicked(dayIndex + 1));
            }
        }
    }

    private void OnDailyGiftButtonClicked(int day)
    {
        if (!CanClaimToday())
        {
            Debug.Log("������ �̹� ������ �޾ҽ��ϴ�.");
            return;
        }

        int currentStreak = GetCurrentStreak();
        int expectedDay = currentStreak + 1; // 1�Ϻ��� 20�ϱ��� ������

        if (day != expectedDay)
        {
            Debug.Log($"������� �޾ƾ� �մϴ�. ���� ���� �� �ִ� ��: {expectedDay}��");
            return;
        }

        // 20�� �ʰ� �� 1�Ϻ��� �ٽ� ����
        if (expectedDay > 20)
        {
            expectedDay = 1;
            ResetStreak();
        }

        // ���� ����
        GiveReward(day);

        // �⼮ ��� ������Ʈ
        UpdateAttendanceRecord();

        // UI ������Ʈ
        UpdateDailyGiftUI();

        Debug.Log($"{day}���� �⼮üũ �Ϸ�!");
    }

    private void GiveReward(int day)
    {
        int dayInCycle = ((day - 1) % 5) + 1; // 1~5��

        switch (dayInCycle)
        {
            case 1:
            case 2:
            case 3:
                // ��� 1000 ���� ����
                AddGoldToSave(goldAmount);
                Debug.Log($"{goldAmount} ��带 �޾ҽ��ϴ�!");
                break;

            case 4:
                // ����� �̱�� (����Ŭ�� ���� ���� ����)
                int cycle = ((day - 1) / 5) + 1;
                int lotteryTickets = cycle;
                if (inventoryUi != null)
                {
                    inventoryUi.AddItem("LotteryTicket", lotteryTickets);
                    Debug.Log($"����� �̱�� {lotteryTickets}���� �޾ҽ��ϴ�! (����Ŭ {cycle})");
                }
                break;

            case 5:
                // ��ų �̱�� (����Ŭ�� ���� ���� ����)
                cycle = ((day - 1) / 5) + 1;
                int enhanceTickets = cycle;
                if (inventoryUi != null)
                {
                    inventoryUi.AddItem("EnhanceTicket", enhanceTickets);
                    Debug.Log($"��ų �̱�� {enhanceTickets}���� �޾ҽ��ϴ�! (����Ŭ {cycle})");
                }
                break;
        }
    }

    private void AddGoldToSave(int amount)
    {
        if (SaveLoadManager.Data != null)
        {
            SaveLoadManager.Data.Gold += amount;
            SaveLoadManager.Save();
        }
    }

    private bool CanClaimToday()
    {
        string lastClaimDate = PlayerPrefs.GetString(lastDateKey, "");
        string today = DateTime.Now.ToString("yyyy-MM-dd");

        return lastClaimDate != today;
    }

    private int GetCurrentStreak()
    {
        string lastClaimDate = PlayerPrefs.GetString(lastDateKey, "");
        int currentStreak = PlayerPrefs.GetInt(currentStreakKey, 0);

        if (string.IsNullOrEmpty(lastClaimDate))
        {
            return 0; // ù �⼮
        }

        DateTime lastClaim;
        if (DateTime.TryParse(lastClaimDate, out lastClaim))
        {
            DateTime today = DateTime.Now.Date;
            TimeSpan difference = today - lastClaim.Date;

            if (difference.Days == 1)
            {
                // ���� �⼮
                return currentStreak;
            }
            else if (difference.Days > 1)
            {
                // ���� �⼮ ���� - ����
                return 0;
            }
        }

        return currentStreak;
    }

    private void UpdateAttendanceRecord()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        int newStreak = GetCurrentStreak() + 1;

        // 20�� ���� �� ����
        if (newStreak > 20)
        {
            newStreak = 1;
        }

        PlayerPrefs.SetString(lastDateKey, today);
        PlayerPrefs.SetInt(currentStreakKey, newStreak);
        PlayerPrefs.Save();
    }

    private void ResetStreak()
    {
        PlayerPrefs.SetInt(currentStreakKey, 0);
        PlayerPrefs.Save();
    }

    private void UpdateDailyGiftUI()
    {
        int currentStreak = GetCurrentStreak();
        bool canClaimToday = CanClaimToday();
        int nextClaimableDay = canClaimToday ? currentStreak + 1 : -1;

        if (nextClaimableDay > 20)
            nextClaimableDay = 1;

        for (int i = 0; i < dailyGiftButtons.Length; i++)
        {
            int dayNumber = i + 1;
            bool isClaimable = canClaimToday && (dayNumber == nextClaimableDay);
            bool isAlreadyClaimed = (dayNumber <= currentStreak) || (!canClaimToday && dayNumber == currentStreak);

            if (dailyGiftButtons[i] != null)
            {
                dailyGiftButtons[i].interactable = isClaimable;
                UpdateButtonDisplay(i, dayNumber);

                // üũ��ũ �ν��Ͻ� ����
                if (isAlreadyClaimed)
                {
                    if (checkMarkInstances[i] == null)
                    {
                        GameObject prefab = GetCheckMarkPrefabForDay(dayNumber);
                        if (prefab != null)
                        {
                            checkMarkInstances[i] = Instantiate(prefab, dailyGiftButtons[i].transform);
                            var rect = checkMarkInstances[i].GetComponent<RectTransform>();
                            var btnRect = dailyGiftButtons[i].GetComponent<RectTransform>();
                            if (rect != null && btnRect != null)
                            {
                                rect.anchorMin = new Vector2(0, 0);
                                rect.anchorMax = new Vector2(1, 1);
                                rect.pivot = new Vector2(0.5f, 0.5f);
                                rect.anchoredPosition = Vector2.zero;
                                rect.sizeDelta = Vector2.zero;
                                rect.localScale = Vector3.one;
                            }
                            checkMarkInstances[i].SetActive(true);
                        }
                    }
                }
                else
                {
                    if (checkMarkInstances[i] != null)
                    {
                        Destroy(checkMarkInstances[i]);
                        checkMarkInstances[i] = null;
                    }
                }
            }
        }
    }

    private GameObject GetCheckMarkPrefabForDay(int dayNumber)
    {
        int dayInCycle = ((dayNumber - 1) % 5) + 1;
        switch (dayInCycle)
        {
            case 1:
            case 2:
            case 3:
                return checkMarkGoldPrefab;
            case 4:
                return checkMarkLotteryPrefab;
            case 5:
                return checkMarkSkillPrefab;
            default:
                return null;
        }
    }
    private void UpdateButtonDisplay(int buttonIndex, int dayNumber)
    {
        if (buttonIndex >= dailyGiftButtons.Length || dailyGiftButtons[buttonIndex] == null)
            return;

        int cycle = ((dayNumber - 1) / 5) + 1;
        int dayInCycle = ((dayNumber - 1) % 5) + 1;

        var buttonText = dailyGiftButtons[buttonIndex].GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            string rewardText = GetRewardText(cycle, dayInCycle);
            buttonText.text = rewardText; // ������ ǥ��
        }
    }

    private string GetRewardText(int cycle, int dayInCycle)
    {
        switch (dayInCycle)
        {
            case 1:
            case 2:
            case 3:
                return "1000";
            case 4:
            case 5:
                return cycle.ToString();
            default:
                return "";
        }
    }

    public override void Close()
    {
        base.Close();
    }
}