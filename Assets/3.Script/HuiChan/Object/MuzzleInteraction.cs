using UnityEngine;

public class MuzzleInteraction : ObjectInteraction
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Dog")
        {
            Collider myCol = transform.GetComponentInChildren<Collider>();
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            AnimalInteraction animal = col.gameObject.GetComponentInParent<AnimalInteraction>();

            myCol.isTrigger = true;
            rb.isKinematic = true;
            myCol.gameObject.transform.position = animal.collarTransform.position;
            myCol.gameObject.transform.rotation = animal.collarTransform.rotation;
        }
    }
}