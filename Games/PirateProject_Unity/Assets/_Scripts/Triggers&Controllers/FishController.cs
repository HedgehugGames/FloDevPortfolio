using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishController : MonoBehaviour
{
    public GameObject[] fishPrefab;
    public int fishCount = 10;
    public float spawnRadius = 20f;
    public Transform[] spawn; // Set these in Inspector

    

    void Start()
    {
        int points = spawn.Length;
        int fishPerPoint = fishCount / points;
        int extraFish = fishCount % points; // To distribute leftovers

        for (int i = 0; i < points; i++)
        {
            int count = fishPerPoint + (i < extraFish ? 1 : 0); // Distribute evenly
            for (int j = 0; j < count; j++)
            {
                float y = Random.Range(-SeaControl.Instance.height / 2, SeaControl.Instance.height / 2);
                Vector3 spawnPos = spawn[i].position + new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    y,
                    Random.Range(-spawnRadius, spawnRadius)
                );

                GameObject fish = Instantiate(fishPrefab[Random.Range(0, fishPrefab.Length)], spawnPos, Quaternion.identity);
            }
        }
    }
}
