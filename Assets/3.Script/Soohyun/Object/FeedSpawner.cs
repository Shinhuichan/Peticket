using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedSpawner : MonoBehaviour
{
    [Header("Throw Settings")]
    public GameObject feedPrefab;
    public Transform handTransform;

    [Header("Dog Target")]
    public AnimalLogic dog;

    [Header("Input")] //temporary Setting
    public KeyCode feedSpawnKey = KeyCode.F1;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(feedSpawnKey))
        {
            SpawnFeed();
        }
    }

    void SpawnFeed()
    {
        GameObject feed = Instantiate(feedPrefab, handTransform.position, Quaternion.identity);
        dog.OnFeedSpawned(feed);
    }
}
