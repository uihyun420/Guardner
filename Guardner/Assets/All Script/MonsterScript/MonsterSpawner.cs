using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class MonsterPrefabInfo
    {
        public int monsterId;
        public GameObject prefab;
    }
    public MonsterPrefabInfo[] monsterPrefabs; // Inspector에서 설정
    public List<MonsterBehavior> spawnedMonsters = new List<MonsterBehavior>();

    private Vector2 spawnPos = new Vector2(-3, 4);
    [SerializeField] private BattleUi battleUi;


    public void SpawnMonster(int monsterId, Vector2 spawnPos, int sortingOrder = 0)
    {
        var monsterData = DataTableManager.MonsterTable.Get(monsterId);
        if (monsterData != null)
        {
            GameObject prefabToUse = GetMonsterPrefab(monsterId);
            if (prefabToUse == null) return;

            var monster = Instantiate(prefabToUse, spawnPos, Quaternion.identity);
            var behavior = monster.GetComponent<MonsterBehavior>();
            behavior.Init(monsterData);
            behavior.SetBattleUi(battleUi);
            behavior.SetSortingOrder(sortingOrder); // 정렬 순서 설정
            spawnedMonsters.Add(behavior);
        }
    }
    private GameObject GetMonsterPrefab(int monsterId)
    {
        foreach (var info in monsterPrefabs)
        {
            if (info.monsterId == monsterId)
                return info.prefab;
        }
        return null; // 해당 ID의 프리팹이 없으면 null 반환
    }

    public void ClearMonster()
    {
        foreach(var monster in spawnedMonsters)
        {
            if(monster != null && monster.gameObject != null)
            {
                Destroy(monster.gameObject);
            }
        }
        spawnedMonsters.Clear();
    }


    public void RemoveMonster(MonsterBehavior monster)
    {
        if(spawnedMonsters.Contains(monster))
        {
            spawnedMonsters.Remove(monster);
        }
    }


    public void CleanMonster()
    {
        for(int i = spawnedMonsters.Count -1; i>=0; i--)
        {
            if (spawnedMonsters[i] != null && spawnedMonsters[i].gameObject != null)
            {
                Destroy(spawnedMonsters[i].gameObject);
            }
        }
        spawnedMonsters.Clear();
    }

}
