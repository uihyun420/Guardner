using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SaveDataVC = SaveDataV1; // ���� �������� 

public class SaveLoadManager
{
    public static int SaveDataVersion { get; } = 1; // ���� ����

    static SaveLoadManager()
    {
        Load();
    }

    public static SaveDataVC Data { get; set; } = new SaveDataVC();

    private static readonly string[] SaveFileName =
    {
        "SaveAuto.json",
    };

    public static string SaveDirectory => $"{Application.persistentDataPath}/Save";

    private static JsonSerializerSettings settings = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.All,
        Converters = new List<JsonConverter>
        {
            new Vector2Converter(), // 2D �����̴� Vector2�� ����
            new GuardnerSaveDataConverter()
        }
    };


    public static bool Save(int slot = 0)
    {
        if (Data == null || slot < 0 || slot >= SaveFileName.Length)
            return false;

        try
        {
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }

            var path = Path.Combine(SaveDirectory, SaveFileName[slot]);
            var json = JsonConvert.SerializeObject(Data, settings);
            File.WriteAllText(path, json);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Save ���� �߻�: {ex.Message}");
            return false;
        }
    }

    public static bool Load(int slot = 0)
    {
        if (slot < 0 || slot >= SaveFileName.Length)
            return false;

        var path = Path.Combine(SaveDirectory, SaveFileName[slot]);
        if (!File.Exists(path))
            return false;

        try
        {
            var json = File.ReadAllText(path);
            var dataSave = JsonConvert.DeserializeObject<SaveData>(json, settings);

            while (dataSave.Version < SaveDataVersion)
            {
                dataSave = dataSave.VersionUp();
            }

            Data = dataSave as SaveDataVC;
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Load ���� �߻�: {ex.Message}");
            return false;
        }
    }

    // ����� ��ȭ ���� ���� ���� �޼���
    public static void SaveGuardnerEnhance(string guardnerId, GuardnerSaveData enhanceData)
    {
        Data.SetGuardnerEnhance(guardnerId, enhanceData);
        Save(); // �ڵ� ����
    }

    // �������� ���� ���� ������Ʈ ���� �޼���
    public static void UpdateStageProgress(int stageId)
    {
        if (stageId > Data.StageProgress)
        {
            Data.StageProgress = stageId;
            Save(); // �ڵ� ����
        }
    }

    // ����� ��� ���� �޼���
    public static void UnlockGuardner(string guardnerId)
    {
        Data.UnlockGuardner(guardnerId);
        Save(); // �ڵ� ����
    }

    // ��� �߰� ���� �޼���
    public static void AddGold(int amount)
    {
        Data.Gold += amount;
        Save(); // �ڵ� ����
    }





    // ������� ���� �ɷ�ġ ��ȸ (����� ��ȭ ���� �ݿ�)
    public static GuardnerSaveData GetGuardnerStats(string guardnerId)
    {
        var savedData = Data.GetGuardnerEnhance(guardnerId);
        if (savedData != null)
        {
            return savedData;
        }

        // ����� �����Ͱ� ���ٸ� �⺻ ���� 1 ������ ��ȯ
        var baseData = DataTableManager.GuardnerEnhanceTable.Get(int.Parse(guardnerId), 1);
        if (baseData != null)
        {
            return new GuardnerSaveData
            {
                Level = 1,
                AttackPower = baseData.AttackPower,
                Health = baseData.GateHP,
                AttackSpeed = baseData.APS,
            };
        }

        return null;
    }

    // ������� ���� ���� ��ȸ
    public static int GetGuardnerLevel(string guardnerId)
    {
        var savedData = Data.GetGuardnerEnhance(guardnerId);
        return savedData?.Level ?? 1; // ����� �����Ͱ� ������ ���� 1
    }

    // ����ʰ� ����Ǿ����� Ȯ��
    public static bool IsGuardnerUnlocked(string guardnerId)
    {
        return Data.IsGuardnerUnlocked(guardnerId);
    }


    //�������� 
    public static bool IsStageCleared(int stageId)
    {
        return Data.StageProgress >= stageId;
    }

    public static int GetMaxStage()
    {
        return Data.StageProgress;
    }

    public static int GetStartingStageId()
    {
        int stageProgress = Data.StageProgress;

        switch (stageProgress)
        {
            case 0: return 1630;
            case 1: return 2640;
            case 2: return 3650;
            case 3: return 4660;
            case 4: return 56100;
            case 5: return 65110;
            case 6: return 75120;
            case 7: return 85130;
            case 8: return 95140;
            case 9: return 105150;
            case 10: return 114160;
            case 11: return 124170;
            case 12: return 134180;
            case 13: return 144190;
            case 14: return 153200;
            default: return 1630; // �⺻���� �������� 1
        }
    }








}