using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetAffinityUI : MonoBehaviour
{
    public TextMeshProUGUI petNameText;
    public Image petImage;
    public Button selectButton;

    private string petId;
    private System.Action<string> onSelectedCallback;

    public void Initialize(string id, string petName, Sprite petSprite, System.Action<string> onSelected = null)
    {
        petId = id;
        petNameText.text = petName;
        petImage.sprite = petSprite;
        onSelectedCallback = onSelected;

        if (selectButton != null)
        {
            selectButton.onClick.AddListener(() =>
            {
                onSelectedCallback?.Invoke(petId);
                selectButton.gameObject.SetActive(false);
            });
        }
    }

    public void ResetSelection()
    {
        gameObject.SetActive(true);
        if (selectButton != null)
            selectButton.gameObject.SetActive(true);
    }

    public string GetPetId() => petId;
}
