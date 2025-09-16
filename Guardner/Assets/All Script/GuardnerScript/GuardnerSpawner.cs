using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public struct GuardnerPrefabInfo
{
    public int guardnerId;
    public GameObject prefab;
}

public class GuardnerSpawner : MonoBehaviour
{
    public GuardnerPrefabInfo[] guardnerPrefabs;
    public List<GuardnerBehavior> spawnedGuardners = new List<GuardnerBehavior>();

    public void SpawnGuardner(int guardnerId, Vector2 spawnPos)
    {
        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);

        var prefabInfo = guardnerPrefabs.FirstOrDefault(p => p.guardnerId == guardnerId);
        if (prefabInfo.prefab != null)
        {
            GameObject guardner = Instantiate(prefabInfo.prefab, spawnPos, Quaternion.identity);
            var behavior = guardner.GetComponent<GuardnerBehavior>();
            behavior.Init(guardnerData);
            spawnedGuardners.Add(behavior);
            Debug.Log($"소환된 가드너 이름: {behavior.name}, 아이디 {behavior.id}, 공격력 {behavior.attackPower}, 공격범위 {behavior.attackRange}, 공격속도 {behavior.aps}");
        }
        else
        {
            Debug.LogWarning($"아이디 {guardnerId}에 해당하는 프리팹이 없습니다.");
        }
    }
}