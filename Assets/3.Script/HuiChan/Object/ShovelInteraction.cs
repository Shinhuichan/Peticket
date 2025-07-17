using System.Collections;
using UnityEngine;

public class ShovelInteraction : ObjectInteraction
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Poop"))
        {
            Debug.Log($"ShovelInteraction: {col.gameObject.name}");
            Destroy(col.gameObject);
        }
    }
}