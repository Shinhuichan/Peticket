using UnityEngine;
using TMPro;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public TextMeshProUGUI warningText;

    private bool isOpen = false;

    public void ToggleInventory()
    {
        // 🔧 실시간 상태 동기화
        isOpen = inventoryPanel.activeSelf;

        // 🔁 상태 반전 후 처리
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        // 인벤토리 닫힐 때 warning 텍스트도 같이 끄기
        if (!isOpen && warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryPanel.SetActive(false);

        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }
}
