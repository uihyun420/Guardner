using UnityEngine;

public class GuardnerSpawner : MonoBehaviour
{
    public GameObject guardnerPrefab;
    private int guardnerId = 11235; // �׽�Ʈ

    public void SpawnGuardner(int guardnerId, Vector2 spawnPos)
    {
        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
        if (guardnerData != null)
        {
            GameObject guardner = Instantiate(guardnerPrefab, spawnPos, Quaternion.identity);
            var behavior = guardner.GetComponent<GuardnerBehavior>();
            behavior.Init(guardnerData);
            Debug.Log($"��ȯ�� ���� �̸�: {behavior.name}, ���̵� {behavior.id}, ���ݷ� {behavior.attackPower}, ���ݹ��� {behavior.attackRange}, ���ݼӵ� {behavior.aps}");
        }
    }
}
