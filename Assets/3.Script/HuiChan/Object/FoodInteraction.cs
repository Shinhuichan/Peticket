using System.Collections;
using UnityEngine;

public class FoodInteraction : ObjectInteraction
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject foodObj;
    [SerializeField] Transform spawnTrans;
    [Range(45f, 135f), SerializeField] float limitXZRotation = 90f;

    void Update()
    {
        StartCoroutine(DropFood());
    }
    IEnumerator DropFood()
    {
        if (transform.eulerAngles.x > limitXZRotation || transform.eulerAngles.z > limitXZRotation)
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.black);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 500f, targetLayer))
            {
                // 밥이 나오는 부분
                Instantiate(foodObj, spawnTrans.position, Quaternion.identity);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}