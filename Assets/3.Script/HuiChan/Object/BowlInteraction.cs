using CustomInspector;
using UnityEngine;

public class BowlInteraction : ObjectInteraction
{
    [Header("Bowl Setting")]
    public GameObject foodObj; // 그릇 안에 채워질 사료 모델 (렌더링 켜고 끌용)

    void Awake()
    {
        if (foodObj != null) foodObj.SetActive(false);
    }
    
    // FoodInteraction이 그릇의 사료 모델을 활성화할 때 호출할 public 메서드
    public void ActivateBowlFoodVisual()
    {
        if (foodObj != null)
        {
            foodObj.SetActive(true);
            Debug.Log($"BowlInteraction: 그릇이 사료로 채워졌습니다!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // "Food" 태그를 가진 오브젝트가 그릇의 트리거에 들어오면 파괴.
        // 이 로직은 FoodItem.cs에서 이미 foodObj가 "Bowl" 태그에 닿으면 스스로 파괴되도록 했으니
        if (other.CompareTag("Food"))
        {
            Debug.Log($"BowlInteraction: 'Food' 태그를 가진 오브젝트 ({other.name})가 그릇에 들어와 파괴됨.");
            Destroy(other.gameObject);
        }
    }
}