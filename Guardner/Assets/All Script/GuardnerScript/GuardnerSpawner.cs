using UnityEngine;
using System.Linq;

[System.Serializable]
public struct GuardnerPrefabInfo
{
    public int guardnerId;
    public GameObject prefab;
}

public class GuardnerSpawner : MonoBehaviour
{
    public GuardnerPrefabInfo[] guardnerPrefabs;

    public void SpawnGuardner(int guardnerId, Vector2 spawnPos)
    {
        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);

        var prefabInfo = guardnerPrefabs.FirstOrDefault(p => p.guardnerId == guardnerId);
        if (prefabInfo.prefab != null)
        {
            GameObject guardner = Instantiate(prefabInfo.prefab, spawnPos, Quaternion.identity);
            var behavior = guardner.GetComponent<GuardnerBehavior>();
            behavior.Init(guardnerData);
            Debug.Log($"��ȯ�� ����� �̸�: {behavior.name}, ���̵� {behavior.id}, ���ݷ� {behavior.attackPower}, ���ݹ��� {behavior.attackRange}, ���ݼӵ� {behavior.aps}");
        }
        else
        {
            Debug.LogWarning($"���̵� {guardnerId}�� �ش��ϴ� �������� �����ϴ�.");
        }
    }
}