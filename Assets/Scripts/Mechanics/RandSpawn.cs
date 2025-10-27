using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using System.Collections.Generic;

public class itemSpawn : MonoBehaviour
{
    [SerializeField] private List<GameObject> pickups; // Assign in unity inspector
    [SerializeField] private List<Transform> spawnPoints; // Assign in unity inspector

    public GameObject GetRandomObject()
    {
        if (pickups == null || pickups.Count == 0)
        {
            Debug.LogWarning("Collecrible list is empty!");
            return null;
        }
        int index = Random.Range(0, pickups.Count);
        return pickups[index];
    }


    public Vector2 GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            Debug.LogWarning("Spawn point list is empty");
            return Vector2.zero;
        }
        int index = Random.Range(0, spawnPoints.Count);
        return spawnPoints[index].position;
    }


    public void SpawnRandomObject()
    {
        GameObject pickups = GetRandomObject();
        Vector2 spawnPos = GetRandomSpawnPoint();

        if (pickups != null)
        {
            Instantiate(pickups, spawnPos, Quaternion.identity);
        }
    }



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnRandomObject();
    }
}