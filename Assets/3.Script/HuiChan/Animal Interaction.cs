using CustomInspector;
using UnityEngine;


public class AnimalInteraction : MonoBehaviour
{
    [SerializeField, ReadOnly] Rigidbody rb;
    [SerializeField, ReadOnly] Rigidbody otherRB;
    [SerializeField, ReadOnly] Vector3 relativeVelocity;

    void Start()
    {
        if (TryGetComponent(out rb) == false) Debug.LogWarning("RigidBody가 없음 | Animal Interaction");
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Hand"))
        {
            otherRB = other.GetComponent<Rigidbody>();
            Vector3 thisVelocity = (rb != null) ? rb.velocity : Vector3.zero;
            Vector3 otherVelocity = (otherRB != null) ? otherRB.velocity : Vector3.zero;
            relativeVelocity = otherVelocity - thisVelocity;

            Debug.Log($"{otherVelocity} - {thisVelocity} = {relativeVelocity}");

            if (relativeVelocity.magnitude > 1f) // 너무 빠른 속도로 들어왔을 경우
            {
                Debug.Log("당신은 너무 폭력적이에요!!!!!!");
                // 반려 동물의 슬픔
            }
            else if (relativeVelocity.magnitude >= 0.3f) // 적당한 속도로 들어왔을 경우
            {
                Debug.Log("반려 동물과의 상호작용 인지");
                // 반려 동물의 반응 시작 (추가 가능)
            }

            // 너무 느린 속도로 들어왔을 경우 아무런 반응이 없음.

        }
    }
    void OnTriggerStay(Collider other)
    {
        otherRB = other.GetComponent<Rigidbody>();
        Vector3 thisVelocity = (rb != null) ? rb.velocity : Vector3.zero;
        Vector3 otherVelocity = (otherRB != null) ? otherRB.velocity : Vector3.zero;
        relativeVelocity = otherVelocity - thisVelocity;
        Debug.Log($"{otherVelocity} - {thisVelocity} = {relativeVelocity}");

        if (relativeVelocity.magnitude > 0.3f && relativeVelocity.magnitude < 1f) // 쓰다듬기
        {
            Debug.Log("부드럽게 쓰다듬고 있어요!");

            // 쓰다듬는 애니메이션, 사운드 재생 등
        }

    }
}