using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public List<MonsterBehavior> spawnedMonsters = new List<MonsterBehavior>();
    private int monsterId = 401100; // Å×½ºÆ®
    private Vector2 spawnPos = new Vector2(-3, 4);
    [SerializeField] private BattleUi battleUi;
    private void Start()
    {
        StartCoroutine(CoSpawnMonster());
    }

    public void SpawnMonster(int monsterId)
    {
        var monsterData = DataTableManager.MonsterTable.Get(monsterId);
        if (monsterData != null)
        {
            var monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            var behavior = monster.GetComponent<MonsterBehavior>();
            behavior.Init(monsterData);
            behavior.SetBattleUi(battleUi);
            spawnedMonsters.Add(behavior);
        }
    }

    private IEnumerator CoSpawnMonster()
    {
        while (true)
        {
            SpawnMonster(monsterId);
            yield return new WaitForSeconds(5f);
        }
    }
}
