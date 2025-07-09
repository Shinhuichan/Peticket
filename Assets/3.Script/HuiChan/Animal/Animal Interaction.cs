using CustomInspector;
using UnityEngine;

public class AnimalInteraction : MonoBehaviour
{
    Rigidbody rb;
    [AsRange(0f, 20f)] public Vector2 interactionrange;
    [SerializeField, ReadOnly] Vector3 relativeVelocity;

    public Transform collarTransform;
    public Transform mouseTransform;

    HandController hand;

    void Start()
    {
        if (TryGetComponent(out rb) == false) Debug.LogWarning("RigidBody가 없음 | Animal Interaction");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Hand"))
        {
            hand = other.GetComponent<HandController>();
            if (hand == null) return;

            Vector3 thisVelocity = (rb != null) ? rb.velocity : Vector3.zero;
            Vector3 otherVelocity = hand.currentVelocity;
            relativeVelocity = otherVelocity - thisVelocity;
            Debug.Log($"{otherVelocity} - {thisVelocity}");

            if (relativeVelocity.magnitude > interactionrange.y) // 너무 빠른 속도로 들어왔을 경우
            {
                Debug.Log("당신은 너무 폭력적이에요!!!!!!");
                // 반려 동물의 슬픔
            }
            else if (relativeVelocity.magnitude >= interactionrange.x) // 적당한 속도로 들어왔을 경우
            {
                Debug.Log("반려 동물과의 상호작용 인지");
                // 반려 동물의 반응 시작 (추가 가능)
            }

            // 너무 느린 속도로 들어왔을 경우 아무런 반응이 없음.

        }
    }
    void OnTriggerStay(Collider other)
    {
        hand = other.GetComponent<HandController>();
        if (hand == null) return;

        Vector3 thisVelocity = (rb != null) ? rb.velocity : Vector3.zero;
        Vector3 otherVelocity = hand.currentVelocity;
        relativeVelocity = otherVelocity - thisVelocity;

        if (relativeVelocity.magnitude > interactionrange.y) // 너무 빠른 속도로 들어왔을 경우
        {
            Debug.Log("당신은 너무 폭력적이에요!!!!!!");
            // 반려 동물의 슬픔
        }
        else
        if (relativeVelocity.magnitude > interactionrange.x && relativeVelocity.magnitude < interactionrange.y) // 쓰다듬기
        {
            Debug.Log("부드럽게 쓰다듬고 있어요!");

            // 쓰다듬는 애니메이션, 사운드 재생 등
        }

    }
}