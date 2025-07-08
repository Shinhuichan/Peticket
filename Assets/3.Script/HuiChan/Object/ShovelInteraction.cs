using UnityEngine;

public class ShovelInteraction : ObjectInteraction
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Poop") Destroy(col.gameObject);
    }
}