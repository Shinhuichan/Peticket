using UnityEngine;

public class ItemClickManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 좌클릭
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var pickup = hit.collider.GetComponent<ItemMousePickup>();
                if (pickup != null)
                {
                    pickup.OnPointerClick(null); // 클릭 이벤트 강제 실행
                }
            }
        }
    }
}
