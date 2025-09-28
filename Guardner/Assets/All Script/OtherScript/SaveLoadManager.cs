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
}