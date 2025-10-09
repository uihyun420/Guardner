using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

[System.Serializable]
public struct GuardnerPrefabInfo
{
    public int guardnerId;
    public GameObject prefab;
}

public class GuardnerSpawner : MonoBehaviour
{
    public HashSet<int> ownedGuardnerIds = new HashSet<int>(); // 내가 가지고 있는 정원사

    public GuardnerPrefabInfo[] guardnerPrefabs;
    public List<GuardnerBehavior> spawnedGuardners = new List<GuardnerBehavior>();

    public GuardnerSpawnUi guardnerSpawnUi;
    public ScreenTouch screenTouch;
    public GameObject[] spawnPos;

    [SerializeField] private BattleUi battleUi;

    private void Start()
    {
        InitializeOwnedGuardners();
    }
    private void InitializeOwnedGuardners()
    {
        var loaded = SaveLoadManager.Data.OwnedGuardnerIds;
        if (loaded != null && loaded.Count > 0)
        {
            ownedGuardnerIds = new HashSet<int>(loaded);
        }
        else
        {
            ownedGuardnerIds.Add(11120);
            ownedGuardnerIds.Add(11235);
            // 헬퍼 메서드로 저장
            SaveLoadManager.SaveOwnedGuardners(ownedGuardnerIds);
        }
    }

    // 가드너 획득 메서드
    public bool AcquireGuardner(int guardnerId)
    {
        if (ownedGuardnerIds.Add(guardnerId))
        {
            SaveLoadManager.SaveOwnedGuardners(ownedGuardnerIds);
            UpdateAllUIs(guardnerId);
            return true;
        }
        return false;
    }

    // 보유 여부 확인
    public bool HasGuardner(int guardnerId)
    {
        return ownedGuardnerIds.Contains(guardnerId);
    }
    // 모든 UI 업데이트
    private void UpdateAllUIs(int newGuardnerId)
    {
        // DictionaryUi 업데이트
        var dictionaryUi = FindObjectOfType<DictionaryUi>();
        if (dictionaryUi != null)
        {
            dictionaryUi.AddGuardnerToCollection(newGuardnerId);
        }
    }

    public bool SpawnGuardner(int guardnerId, Vector2 spawnPos)
    {
        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);

        int summonGold = guardnerData.SummonGold;
        if(battleUi.gold < summonGold)
        {
            return false;
        }

        battleUi.gold -= summonGold;
        battleUi.SetGoldText();

        var prefabInfo = guardnerPrefabs.FirstOrDefault(p => p.guardnerId == guardnerId);
        if (prefabInfo.prefab != null)
        {
            GameObject guardner = Instantiate(prefabInfo.prefab, spawnPos, Quaternion.identity);
            var behavior = guardner.GetComponent<GuardnerBehavior>();

            // 강화된 가드너 정보 적용
            var enhancedStats = SaveLoadManager.GetGuardnerStats(guardnerId.ToString());
            if (enhancedStats != null)
            {
                // 기본 데이터 초기화
                behavior.Init(guardnerData);

                // 강화된 능력치로 덮어쓰기
                behavior.attackPower = enhancedStats.AttackPower;
                behavior.aps = enhancedStats.AttackSpeed;
            }
            else
            {
                // 기본 데이터
                behavior.Init(guardnerData);
            }
            
            spawnedGuardners.Add(behavior);
            return true; // 소환 성공
        }
        else
        {
            return false;
        }
    }

    public Vector2 GetSelectedAreaPosition()
    {
        int selectedIndex = screenTouch.GetSelectedAreaIndex();
        return spawnPos[selectedIndex].transform.position;
    }

    public bool IsGuardnerAtPosition(Vector2 position, float checkRadius = 0.5f)
    {
        foreach(var guardner in spawnedGuardners)
        {
            if(guardner != null && Vector2.Distance(guardner.transform.position, position) < checkRadius)
            {
                return true;
            }
        }
        return false;
    }

    public void ClearGuardner()
    {
        foreach (var guardner in spawnedGuardners)
        {
            if (guardner != null && guardner.gameObject != null)
            {
                Destroy(guardner.gameObject);
            }
        }
        spawnedGuardners.Clear();
    }
}