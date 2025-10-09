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

        // �̹� �ش� ��ġ�� ����ʰ� �ִ��� Ȯ��
        if (IsGuardnerAtPosition(spawnPos)) return false;

        // ����� ���� ���� (���� ���Ӱ� �����ϰ� ����)
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