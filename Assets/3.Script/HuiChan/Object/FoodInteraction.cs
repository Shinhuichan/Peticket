using System.Collections;
using UnityEngine;

public class FoodInteraction : ObjectInteraction
{
    [SerializeField] LayerMask targetLayer;
    [SerializeField] GameObject foodObj;
    [SerializeField] Transform spawnTrans;
    [Range(45f, 135f), SerializeField] float limitXZRotation = 90f;

    private bool isDroppingFoodRoutineRunning = false;

    IEnumerator DropFood()
    {
        isDroppingFoodRoutineRunning = true;
        float currentXRotation = transform.eulerAngles.x;
        float currentZRotation = transform.eulerAngles.z;
        if (currentXRotation > 180f) currentXRotation -= 360f;
        if (currentZRotation > 180f) currentZRotation -= 360f;

        if (Mathf.Abs(currentXRotation) >= limitXZRotation || Mathf.Abs(currentZRotation) >= limitXZRotation)
        {
            Debug.DrawRay(transform.position, Vector3.down, Color.black, 1f);
            RaycastHit hit;
            if (Physics.Raycast(spawnTrans.position, Vector3.down, out hit, 1f, targetLayer))
                // 밥이 나오는 내용
                Instantiate(foodObj, spawnTrans.position, Quaternion.identity);

            yield return new WaitForSeconds(0.25f);
        }
        isDroppingFoodRoutineRunning = false;
    }

    public override void UseObject()
    {
        Debug.Log("FoodInteraction 오브젝트 UseObject 호출됨.");

        // DropFood이 이미 실행 중이라면 더 이상 호출하지 않아.
        if (isDroppingFoodRoutineRunning) return;

        float currentXRotation = transform.eulerAngles.x;
        float currentZRotation = transform.eulerAngles.z;
        
        if (currentXRotation > 180f) currentXRotation -= 360f;
        if (currentZRotation > 180f) currentZRotation -= 360f;

        if (Mathf.Abs(currentXRotation) >= limitXZRotation || Mathf.Abs(currentZRotation) >= limitXZRotation)
            StartCoroutine(DropFood());
    }

    void OnDisable()
    {
        if (isDroppingFoodRoutineRunning)
        {
            StopAllCoroutines();
            isDroppingFoodRoutineRunning = false;
        }
    }
}