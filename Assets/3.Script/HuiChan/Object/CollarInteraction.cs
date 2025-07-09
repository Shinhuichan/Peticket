using UnityEngine;


public enum ObjectType
{
    Collar = 0,
    Muzzle
}
public class CollarInteraction : ObjectInteraction
{
    [SerializeField] ObjectType type;
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Dog")
        {
            // 필요한 Component들 정의
            Collider myCol = transform.GetComponentInChildren<Collider>();
            Rigidbody rb = transform.GetComponent<Rigidbody>();
            AnimalInteraction animal = col.gameObject.GetComponentInParent<AnimalInteraction>();

            // 부착됐을 때, 물리 작용 제거
            myCol.isTrigger = true;
            rb.isKinematic = true;

            // 어떤 target으로 부착되는 지 판단
            Transform targetTrans = type == ObjectType.Collar ? animal.collarTransform : animal.mouseTransform;
            myCol.gameObject.transform.position = targetTrans.position;
            myCol.gameObject.transform.rotation = targetTrans.rotation;
        }
    }
}