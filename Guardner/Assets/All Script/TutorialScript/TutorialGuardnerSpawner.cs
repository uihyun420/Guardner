using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TutorialGuardnerSpawner : GuardnerSpawner
{
    private void Awake()
    {
        // 튜토리얼용 가드너 설정
        ownedGuardnerIds = new HashSet<int> { 11120, 11235 };

        if (screenTouch == null)
            screenTouch = FindObjectOfType<ScreenTouch>();
    }

    private new void Start()
    {
    }

    // 튜토리얼 전용 스폰 메서드 (골드 체크 없이)
    public bool SpawnGuardnerForTutorial(int guardnerId, Vector2 spawnPos)
    {
        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
        if (guardnerData == null)
        {
            return false;
        }

        if (IsGuardnerAtPosition(spawnPos)) return false;

        var prefabInfo = guardnerPrefabs.FirstOrDefault(p => p.guardnerId == guardnerId);
        if (prefabInfo.prefab != null)
        {
            GameObject guardner = Instantiate(prefabInfo.prefab, spawnPos, Quaternion.identity);
            var behavior = guardner.GetComponent<GuardnerBehavior>();
            if (behavior != null)
            {
                behavior.Init(guardnerData);
                spawnedGuardners.Add(behavior);
                return true;
            }
        }

        return false;
    }
}