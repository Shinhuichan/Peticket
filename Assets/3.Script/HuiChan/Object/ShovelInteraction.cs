using System.Collections;
using UnityEngine;

public class ShovelInteraction : ObjectInteraction
{
    [SerializeField] Collider myCol;

    [Header("Shovel Settings")]
    [SerializeField] float activeDuration = 3.0f;

    private bool isShoveling = false; 
    private Coroutine shovelCoroutine;

    void Start()
    {
        if (myCol == null)
        {
            if (!TryGetComponent(out myCol))
            {
                Debug.LogWarning("ShovelInteraction | myCol이 Null입니다.");
                return;
            }
        }
        
        myCol.enabled = false;
    }
    
    public override void UseObject()
    {
        Debug.Log("ShovelInteraction: UseObject 호출");

        if (isShoveling)
        {
            Debug.Log("ShovelInteraction: 이미 삽질 중");
            return;
        }

        isShoveling = true;
        
        if (shovelCoroutine != null) StopCoroutine(shovelCoroutine);
        shovelCoroutine = StartCoroutine(ActivateColliderForDuration(activeDuration));
    }

    IEnumerator ActivateColliderForDuration(float duration)
    {
        myCol.enabled = true;
        Debug.Log($"ShovelInteraction: 콜라이더 활성화! ({duration}초 동안)");
        yield return new WaitForSeconds(duration);

        myCol.enabled = false;
        Debug.Log("ShovelInteraction: 콜라이더 비활성화! 삽질 완료. ");
        
        isShoveling = false;
        shovelCoroutine = null;
    }

    void OnTriggerEnter(Collider col)
    {
        if (myCol.enabled && col.gameObject.CompareTag("Poop"))
        {
            Debug.Log($"ShovelInteraction: {col.gameObject.name}");
            Destroy(col.gameObject);
        }
    }

    void OnDisable()
    {
        if (shovelCoroutine != null)
        {
            StopCoroutine(shovelCoroutine);
            shovelCoroutine = null;
        }
        isShoveling = false;
        
        if (myCol != null) myCol.enabled = false;
    }
}