using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    public int monsterId = 401100; // Å×½ºÆ®
    public Vector2 spawnPos = Vector2.zero;


    private void Start()
    {
        StartCoroutine(CoSpawnMonster());
    }


    public void SpawnMonster(int monsterId)
    {
        var monsterData = DataTableManager.MonsterTable.Get(monsterId);
        if (monsterData != null)
        {
            GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);
            var behavior = monster.GetComponent<MonsterBehavior>();
            behavior.Init(monsterData);
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
