
using System.Collections;
using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("Bowl Check Setting")]
    [SerializeField] LayerMask bowlLayer; // 그릇의 레이어 (인스펙터에서 설정)
    [SerializeField] float destroyDelay = 3f; // 그릇에 닿지 않았을 때 제거될 시간

    private bool hasReachedBowl = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        // 3초 타이머 시작 (그릇에 닿지 않으면 제거)
        StartCoroutine(CheckBowlContactAndDestroy());
    }

    // 콜라이더 또는 트리거 충돌 감지 (음식이 그릇에 떨어졌을 때)
    void OnTriggerEnter(Collider other) // 그릇에 Collider가 Is Trigger로 되어있다면 사용
    {
        CheckForBowlContact(other.gameObject); // GameObject 자체를 넘겨 태그 체크
    }

    void OnCollisionEnter(Collision collision) // 그릇에 Collider가 Is Trigger가 아니라면 사용
    {
        CheckForBowlContact(collision.gameObject); // GameObject 자체를 넘겨 태그 체크
    }

    private void CheckForBowlContact(GameObject otherObject)
    {
        if (otherObject.CompareTag("Bowl")) // "Bowl" 태그 체크
        {
            Debug.Log($"FoodItem: {gameObject.name}이(가) 'Bowl' 태그에 닿았다! 즉시 제거!");
            // 즉시 제거!
            Destroy(gameObject);
        }
    }

    IEnumerator CheckBowlContactAndDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);

        if (!hasReachedBowl)
        {
            Debug.Log($"FoodItem: {gameObject.name}이(가) 그릇에 닿지 않아서 자동으로 제거됩니다.");
            Destroy(gameObject);
        }
    }
}