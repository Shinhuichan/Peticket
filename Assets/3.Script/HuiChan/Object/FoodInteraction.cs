using System.Collections;
using UnityEngine;

public class FoodInteraction : ObjectInteraction // ObjectInteraction 상속 유지
{
    [SerializeField] LayerMask targetLayer; // Raycast 대상 레이어 (그릇 레이어도 포함되어야 함!)
    [SerializeField] GameObject foodObj; // 떨어뜨릴 사료 덩어리 프리팹 (FoodItem 스크립트 붙어있어야 함)
    [SerializeField] Transform spawnTrans; // 사료가 생성될 위치
    [Range(45f, 135f), SerializeField] float limitXZRotation = 90f; // 기울기 임계값

    [Header("Bowl Filling Settings")]
    [SerializeField] public BowlInteraction targetBowl; // 굳건희! 여기에 그릇 오브젝트의 BowlInteraction 컴포넌트를 연결해줘!
    [SerializeField] float requiredContinuousPourTime = 2f; // 그릇을 채우기 위해 레이를 쏴야 하는 지속 시간
    [SerializeField] float rayCheckInterval = 0.1f; // Raycast를 체크하는 간격 (초)

    // === 사료 덩어리 생성 관련 변수 ===
    private Coroutine foodDispensingCoroutine; // 사료 덩어리 연속 생성 코루틴 참조
    [SerializeField] float foodDropParticleInterval = 0.5f; // 사료 덩어리 하나 생성될 때마다의 간격


    // === 그릇 채움 Ray 체크 관련 변수 ===
    private Coroutine bowlFillCheckCoroutine; // 그릇 채움 Ray 체크 코루틴 참조
    private float currentPourRayHitTime = 0f; // Ray가 그릇에 지속적으로 닿은 시간

    void Update()
    {
        float currentXRotation = transform.eulerAngles.x;
        float currentZRotation = transform.eulerAngles.z;
        if (currentXRotation > 180f) currentXRotation -= 360f;
        if (currentZRotation > 180f) currentZRotation -= 360f;

        bool isTiltedEnough = Mathf.Abs(currentXRotation) >= limitXZRotation || Mathf.Abs(currentZRotation) >= limitXZRotation;

        // 1. 사료 덩어리 연속 생성 코루틴 관리
        if (isTiltedEnough)
            if (foodDispensingCoroutine == null) // 아직 사료 덩어리 코루틴이 시작되지 않았다면
                foodDispensingCoroutine = StartCoroutine(ContinuousFoodDispensing());
        else // 기울기가 충분하지 않을 때
            if (foodDispensingCoroutine != null) // 사료 덩어리 코루틴이 실행 중이라면 중지
            {
                StopCoroutine(foodDispensingCoroutine);
                foodDispensingCoroutine = null;
            }

        // 2. 그릇 채움 Ray 체크 코루틴 관리 (기존 로직과 동일)
        if (isTiltedEnough)
            if (bowlFillCheckCoroutine == null) // 아직 Ray 체크 코루틴이 시작되지 않았다면
                bowlFillCheckCoroutine = StartCoroutine(ContinuousBowlRayCheck());
        else
            if (bowlFillCheckCoroutine != null) // 기울기가 충분하지 않은데 코루틴이 실행 중이라면
            {
                StopCoroutine(bowlFillCheckCoroutine);
                bowlFillCheckCoroutine = null;
                currentPourRayHitTime = 0f; // 타이머 초기화
            }
    }


    // 1. 사료 덩어리 연속 생성 코루틴 (오직 기울기가 충분할 때만 작동)
    IEnumerator ContinuousFoodDispensing()
    {
        while (true)
        {
            RaycastHit hit;
            if (Physics.Raycast(spawnTrans.position, Vector3.down, out hit, 1f, targetLayer))
            {
                if (hit.collider != null)
                {
                    Debug.DrawRay(spawnTrans.position, Vector3.down, Color.blue, 0.5f);
                    Instantiate(foodObj, spawnTrans.position, Quaternion.identity);
                }
            }

            yield return new WaitForSeconds(foodDropParticleInterval);
        }
    }


    IEnumerator ContinuousBowlRayCheck()
    {
        while (true)
        {
            Debug.DrawRay(spawnTrans.position, Vector3.down, Color.red, rayCheckInterval);

            RaycastHit hit;
            if (Physics.Raycast(spawnTrans.position, Vector3.down, out hit, 1f, targetLayer))
            {
                // ... (기존 로직)
                currentPourRayHitTime += rayCheckInterval;
                Debug.Log($"FoodInteraction: Ray 그릇에 닿음! 시간 누적: {currentPourRayHitTime:F2}초");

                if (currentPourRayHitTime >= requiredContinuousPourTime && targetBowl != null && targetBowl.foodObj != null && !targetBowl.foodObj.activeSelf)
                {
                    targetBowl.ActivateBowlFoodVisual();
                    currentPourRayHitTime = 0f;
                }
            }
            else currentPourRayHitTime = 0f;

            yield return new WaitForSeconds(rayCheckInterval);
        }
    }

    // 스크립트가 비활성화되거나 파괴될 때 모든 코루틴을 안전하게 중지
    void OnDisable()
    {
        // 사료 덩어리 생성 코루틴 중지
        if (foodDispensingCoroutine != null)
        {
            StopCoroutine(foodDispensingCoroutine);
            foodDispensingCoroutine = null;
        }

        // 그릇 채움 Ray 체크 코루틴 중지
        if (bowlFillCheckCoroutine != null)
        {
            StopCoroutine(bowlFillCheckCoroutine);
            bowlFillCheckCoroutine = null;
        }
        currentPourRayHitTime = 0f; // 타이머도 초기화
    }
}