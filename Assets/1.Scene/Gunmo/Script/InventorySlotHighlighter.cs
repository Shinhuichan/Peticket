using UnityEngine;

public class InventorySlotHighlighter : MonoBehaviour
{
    public InventorySlot[] slots;
    public int currentIndex = 0;
    public GameObject highlightEffect;

    private void Start()
    {
        UpdateHighlight();
    }

    public void MoveHighlight(int direction)
    {
        currentIndex += direction;
        currentIndex = Mathf.Clamp(currentIndex, 0, slots.Length - 1);
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetHighlight(i == currentIndex);
        }

        if (highlightEffect != null)
        {
            highlightEffect.transform.SetParent(slots[currentIndex].transform, false);
            highlightEffect.transform.localPosition = Vector3.zero;
        }
    }

    public void SelectCurrentSlot()
    {
        if (slots[currentIndex] != null)
        {
            slots[currentIndex].RemoveItemToHand();
        }
    }
}
