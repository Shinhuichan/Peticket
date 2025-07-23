using CustomInspector;
using TMPro;
using UnityEngine;

public class BowlInteraction : ObjectInteraction
{
    [Header("Bowl Setting")]
    [SerializeField] BoxCollider col;
    [SerializeField] GameObject foodObj;
    [SerializeField] int foodCount = 0;
    [SerializeField] int limitCount = 10;
    public bool isFull = false;

    void Update()
    {
        if (foodCount >= limitCount)
        {
            isFull = true;
            foodObj.SetActive(true);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Food"))
        {
            foodCount++;
            Destroy(other.gameObject);
        }
    }
}