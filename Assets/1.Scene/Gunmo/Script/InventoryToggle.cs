using UnityEngine;
using TMPro;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public TextMeshProUGUI warningText;

    private bool isOpen = false;

    public void ToggleInventory()
    {
        // 🔧 현재 상태 확인
        isOpen = inventoryPanel.activeSelf;

        // 🔁 상태 반전
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

    
        // 🔕 닫을 때 경고 텍스트 비활성화
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
