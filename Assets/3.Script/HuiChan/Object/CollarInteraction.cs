using CustomInspector;
using UnityEngine;

public enum ObjectType
{
    Collar = 0,
    Muzzle
}

public class CollarInteraction : ObjectInteraction
{
    [SerializeField] ObjectType type;

    AnimalInteraction animal;
    Collider myCol;
    Rigidbody rb; 

    void Start()
    {
        myCol = transform.GetComponentInChildren<Collider>();
    }

    void LateUpdate()
    {
        bool isEquipped = (type == ObjectType.Collar) ? GameManager.Instance.isCollarEquip : GameManager.Instance.isMuzzleEquip;

        if (isEquipped)
        {
            Transform targetTrans = (type == ObjectType.Collar) ? animal.collarTransform : animal.mouseTransform;
            myCol.transform.position = targetTrans.position;
            myCol.transform.rotation = targetTrans.rotation;

            if (rb != null) rb.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Dog"))
        {
            rb = GetComponent<Rigidbody>(); // 
            animal = col.GetComponentInParent<AnimalInteraction>();

            if (type == ObjectType.Collar)
                GameManager.Instance.isCollarEquip = true;
            else
                GameManager.Instance.isMuzzleEquip = true;
        }
    }
}
