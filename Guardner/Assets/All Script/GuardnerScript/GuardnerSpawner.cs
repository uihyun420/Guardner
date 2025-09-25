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
    public GuardnerPrefabInfo[] guardnerPrefabs;
    public List<GuardnerBehavior> spawnedGuardners = new List<GuardnerBehavior>();

    public GuardnerSpawnUi guardnerSpawnUi;
    public ScreenTouch screenTouch;
    public GameObject[] spawnPos;

    [SerializeField] private BattleUi battleUi;

    public void SpawnGuardner(int guardnerId, Vector2 spawnPos)
    {
        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);

        int summonGold = guardnerData.SummonGold;
        if(battleUi.gold < summonGold)
        {

            return;
        }

        battleUi.gold -= summonGold;
        battleUi.SetGoldText();

        var prefabInfo = guardnerPrefabs.FirstOrDefault(p => p.guardnerId == guardnerId);
        if (prefabInfo.prefab != null)
        {
            GameObject guardner = Instantiate(prefabInfo.prefab, spawnPos, Quaternion.identity);
            var behavior = guardner.GetComponent<GuardnerBehavior>();
            behavior.Init(guardnerData);
            spawnedGuardners.Add(behavior);
            Debug.Log($"��ȯ�� ����� �̸�: {behavior.name}, ���̵� {behavior.id}, ���ݷ� {behavior.attackPower}, ���ݹ��� {behavior.attackRange}, ���ݼӵ� {behavior.aps}");
        }
        else
        {
            Debug.LogWarning($"���̵� {guardnerId}�� �ش��ϴ� �������� �����ϴ�.");
        }
    }

    public Vector2 GetSelectedAreaPosition()
    {
        int selectedIndex = screenTouch.GetSelectedAreaIndex();
        Debug.Log($"�ε��� : {selectedIndex}");
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