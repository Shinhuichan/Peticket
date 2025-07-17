using System.Collections;
using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("Bowl Check Setting")]
    [SerializeField] LayerMask bowlLayer;
    [SerializeField] float destroyDelay = 3f;

    private bool hasReachedBowl = false;
    private Rigidbody rb;

    void Awake()
    {
        if (!TryGetComponent(out rb)) Debug.LogWarning($"FoodItem | RigidBody가 Null입니다.");
    }

    void Start()
    {
        StartCoroutine(CheckBowlContactAndDestroy());
    }

    void OnTriggerEnter(Collider other) // 그릇에 Collider가 Is Trigger로 되어있다면 사용
    {
        CheckForBowlContact(other.gameObject.layer);
    }

    // void OnCollisionEnter(Collision collision) // 그릇에 Collider가 Is Trigger가 아니라면 사용
    // {
    //     CheckForBowlContact(collision.gameObject.layer);
    // }

    private void CheckForBowlContact(int otherLayer)
    {
        if (((1 << otherLayer) & bowlLayer) != 0)
        {
            Debug.Log($"FoodItem: {gameObject.name}이(가) 그릇 레이어에 닿았다!");
            hasReachedBowl = true;
        }
    }

    IEnumerator CheckBowlContactAndDestroy()
    {
        // destroyDelay 초 만큼 기다려.
        yield return new WaitForSeconds(destroyDelay);

        // 기다린 후에 그릇에 닿았는지 확인
        if (!hasReachedBowl)
        {
            Debug.Log($"Food가 그릇에 닿지 않아서 자동으로 제거");
            Destroy(gameObject);
        }
        else Debug.Log($"Food가 그릇에 무사히 안착하여 제거되지 않습니다.");
    }
}