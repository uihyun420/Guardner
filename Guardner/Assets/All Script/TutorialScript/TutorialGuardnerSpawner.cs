using UnityEngine;
using System.Collections.Generic;

public class TutorialGuardnerSpawner : MonoBehaviour
{
    [SerializeField]private GameObject[] spawnPos;
    public List<int> ownedGuardnerIds = new List<int> { 11120, 11235}; 
    [SerializeField] private ScreenTouch screenTouch;

    private List<GameObject> spawnedGuardners = new List<GameObject>();

    private void Awake()
    {
        if (screenTouch == null)
            screenTouch = FindObjectOfType<ScreenTouch>();
    }

    public bool SpawnGuardner(int guardnerId, Vector2 spawnPos)
    {
        var guardnerData = DataTableManager.GuardnerTable.Get(guardnerId);
        if (guardnerData == null) return false;

        // 이미 해당 위치에 가드너가 있는지 확인
        if (IsGuardnerAtPosition(spawnPos)) return false;

        // 가드너 스폰 로직 (실제 게임과 동일하게 구현)
        GameObject guardnerPrefab = Resources.Load<GameObject>($"GuardnerPrefabs/Guardner_{guardnerId}");
        if (guardnerPrefab != null)
        {
            GameObject newGuardner = Instantiate(guardnerPrefab, spawnPos, Quaternion.identity);
            var guardnerBehavior = newGuardner.GetComponent<GuardnerBehavior>();
            if (guardnerBehavior != null)
            {
                guardnerBehavior.Init(guardnerData);
            }
            spawnedGuardners.Add(newGuardner);
            return true;
        }
        return false;
    }

    public bool IsGuardnerAtPosition(Vector2 position)
    {
        foreach (var guardner in spawnedGuardners)
        {
            if (guardner != null && Vector2.Distance(guardner.transform.position, position) < 0.5f)
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
            if (guardner != null)
                Destroy(guardner);
        }
        spawnedGuardners.Clear();
    }
}