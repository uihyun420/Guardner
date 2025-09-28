using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SaveDataVC = SaveDataV1; // 현재 버전으로 

public class SaveLoadManager
{
    public static int SaveDataVersion { get; } = 1; // 현재 버전

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
            new Vector2Converter(), // 2D 게임이니 Vector2로 변경
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
            Debug.LogError($"Save 예외 발생: {ex.Message}");
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
            Debug.LogError($"Load 예외 발생: {ex.Message}");
            return false;
        }
    }

    // 가드너 강화 정보 저장 헬퍼 메서드
    public static void SaveGuardnerEnhance(string guardnerId, GuardnerSaveData enhanceData)
    {
        Data.SetGuardnerEnhance(guardnerId, enhanceData);
        Save(); // 자동 저장
    }

    // 스테이지 진행 상태 업데이트 헬퍼 메서드
    public static void UpdateStageProgress(int stageId)
    {
        if (stageId > Data.StageProgress)
        {
            Data.StageProgress = stageId;
            Save(); // 자동 저장
        }
    }

    // 가드너 언락 헬퍼 메서드
    public static void UnlockGuardner(string guardnerId)
    {
        Data.UnlockGuardner(guardnerId);
        Save(); // 자동 저장
    }

    // 골드 추가 헬퍼 메서드
    public static void AddGold(int amount)
    {
        Data.Gold += amount;
        Save(); // 자동 저장
    }
}