using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeManager : MonoBehaviour
{
    [SerializeField] private GameObject slimePrefab; 
    [SerializeField] private Transform spawnPoint;   
    [SerializeField] private float respawnDelay = 5f;

    private GameObject currentSlime;

    private void Start()
    {
        SpawnSlime();
    }

    private void Update()
    {
        if (currentSlime == null)
        {
            StartCoroutine(RespawnAfterDelay());
            currentSlime = new GameObject();
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnSlime();
    }

    private void SpawnSlime()
    {
        currentSlime = Instantiate(slimePrefab, spawnPoint.position, Quaternion.identity);
    }
}
