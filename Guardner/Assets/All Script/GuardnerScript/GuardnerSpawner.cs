using UnityEngine;

public class GuardnerSpawner : MonoBehaviour
{
    public GameObject guardnerPrefab;
    private int guardnerId = 11235; // 테스트

    public void SpawnGuardner(int guardnerId, Vector2 spawnPos)
    {
        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
        if (guardnerData != null)
        {
            GameObject guardner = Instantiate(guardnerPrefab, spawnPos, Quaternion.identity);
            var behavior = guardner.GetComponent<GuardnerBehavior>();
            behavior.Init(guardnerData);
            Debug.Log($"소환된 몬스터 이름: {behavior.name}, 아이디 {behavior.id}, 공격력 {behavior.attackPower}, 공격범위 {behavior.attackRange}, 공격속도 {behavior.aps}");
        }
    }
}
