using UnityEngine;
using UnityEngine.UI;

public class ActivateOnClick : MonoBehaviour
{
    [Header("연결할 버튼")]
    public Button targetButton;

    [Header("활성화할 오브젝트")]
    public GameObject objectToActivate;

    private void Start()
    {
        if (targetButton != null && objectToActivate != null)
        {
            targetButton.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            Debug.LogWarning("🔔 버튼 또는 오브젝트가 할당되지 않았습니다.");
        }
    }

    private void OnButtonClicked()
    {
        objectToActivate.SetActive(true);
        Debug.Log($"✅ {objectToActivate.name} 활성화됨");
    }
}
