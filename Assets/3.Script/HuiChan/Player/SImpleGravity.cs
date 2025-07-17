using UnityEngine;

public class SimpleGravity : MonoBehaviour
{
    public float gravity = -9.81f;
    public CharacterController controller;
    private float verticalVelocity = 0f;

    void Update()
    {
        if (controller.isGrounded)
        {
            // 땅에 닿아 있으면 Y축 속도를 0으로 초기화
            verticalVelocity = -1f;  // 살짝 눌러 붙는 느낌
        }
        else
        {
            // 중력 적용
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = new Vector3(0, verticalVelocity, 0);
        controller.Move(move * Time.deltaTime);
    }
}
