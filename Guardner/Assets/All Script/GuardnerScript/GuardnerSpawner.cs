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
            Debug.Log($"소환된 가드너 이름: {behavior.name}, 아이디 {behavior.id}, 공격력 {behavior.attackPower}, 공격범위 {behavior.attackRange}, 공격속도 {behavior.aps}");
        }
        else
        {
            Debug.LogWarning($"아이디 {guardnerId}에 해당하는 프리팹이 없습니다.");
        }
    }

    public Vector2 GetSelectedAreaPosition()
    {
        int selectedIndex = screenTouch.GetSelectedAreaIndex();
        Debug.Log($"인덱스 : {selectedIndex}");
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