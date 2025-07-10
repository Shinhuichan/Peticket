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
    [SerializeField, ReadOnly] bool isEquip = false;
    AnimalInteraction animal;
    Collider myCol;
    void Start()
    {
        myCol = transform.GetComponentInChildren<Collider>();
    }
    void LateUpdate()
    {
        if (isEquip)
        {
            // 어떤 target으로 부착되는 지 판단
            Transform targetTrans = type == ObjectType.Collar ? animal.collarTransform : animal.mouseTransform;
            myCol.gameObject.transform.position = targetTrans.position;
            myCol.gameObject.transform.rotation = targetTrans.rotation;
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Dog")
        {
            // 필요한 Component들 정의
            
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            animal = col.gameObject.GetComponentInParent<AnimalInteraction>();

            isEquip = true;
        }
    }
}