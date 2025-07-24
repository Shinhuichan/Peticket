using UnityEngine;
using UnityEngine.UI;

public class UIButtonSFXInjector : MonoBehaviour
{
    public string sfxKey = "Button Pop"; 

    private void Awake()
    {
        Button[] buttons = FindObjectsOfType<Button>(true); // 비활성 포함
        foreach (Button btn in buttons)
        {
            Button currentButton = btn; 
            currentButton.onClick.AddListener(() =>
            {
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFXByKey(sfxKey);
                }
                else
                {
                    Debug.LogWarning("[UIButtonSFXInjector] AudioManager.Instance is null!");
                }
            });
        }
    }
}
