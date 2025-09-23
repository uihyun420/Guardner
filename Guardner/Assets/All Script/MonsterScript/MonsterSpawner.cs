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
    public MonsterPrefabInfo[] monsterPrefabs; // Inspector���� ����
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
            behavior.SetSortingOrder(sortingOrder); // ���� ���� ����
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
        return null; // �ش� ID�� �������� ������ null ��ȯ
    }

    public void ClearMonster()
    {
        foreach(var monster in spawnedMonsters)
        {
            Destroy(monster.gameObject);
        }
        spawnedMonsters.Clear();
    }
}
