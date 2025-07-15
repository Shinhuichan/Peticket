using UnityEngine;

public class SelectedPetManager : MonoBehaviour
{
    public static SelectedPetManager Instance { get; private set; }

    public string selectedPetId;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ 씬 변경에도 살아남음
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }
}
