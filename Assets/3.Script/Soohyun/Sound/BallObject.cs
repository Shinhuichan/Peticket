using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class BallObject : MonoBehaviour
{
    public bool isFromInventory = true;
    public AudioClip impactSound;

    private AudioSource audioSource;
    private bool hasPlayedSound = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // 3D 사운드
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Ground")) return;

        if (!hasPlayedSound && collision.relativeVelocity.magnitude > 1f)
        {
            hasPlayedSound = true;
            Debug.Log($"[Ball] 충돌 감지됨! → 소리 발생 + 탐지 시작");

            PlaySound();

            // 시각화용 디버그
            Debug.DrawRay(transform.position, Vector3.up * 3f, Color.yellow, 2f);
            Collider[] nearby = Physics.OverlapSphere(transform.position, 5f);

            Debug.Log($"[Ball] 반경 내 탐지된 Collider 수: {nearby.Length}");

            foreach (var col in nearby)
            {
                Debug.Log($"[Ball] 충돌된 오브젝트: {col.name}");

                if (col.TryGetComponent<AnimalLogic>(out var dog))
                {
                    Debug.Log($"[Ball] 반경 내 강아지 감지됨 → {dog.name}");
                    dog.OnBallSoundDetected(this.gameObject);
                }
            }
        }
    }

    void Update()
    {
        // 공이 일정 높이 이상으로 이동하거나, 속도가 커졌을 때 다시 활성화
        if (hasPlayedSound && GetComponent<Rigidbody>().velocity.magnitude < 0.1f)
        {
            // 정지 상태에서 초기화
            hasPlayedSound = false;
        }
    }


    void PlaySound()
    {
        if (impactSound != null)
        {
            audioSource.clip = impactSound;
            audioSource.Play();
        }
    }
}
