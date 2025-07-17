using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetUI : MonoBehaviour
{
    public TextMeshProUGUI petNameText;
    public Image petImage;
    public Button selectButton;

    private string petId;
    private System.Action<string> onSelectCallback;

    public void Initialize(string id, string name, System.Action<string> callback)
    {
        petId = id;
        petNameText.text = name;
        onSelectCallback = callback;

        if (selectButton != null)
        {
            selectButton.onClick.AddListener(() =>
            {
                onSelectCallback?.Invoke(petId);
                selectButton.gameObject.SetActive(false);
            });
        }
    }

    public void ResetSelection()
    {
        gameObject.SetActive(true);
        selectButton?.gameObject.SetActive(true);
    }

    public string GetPetId() => petId;
}
