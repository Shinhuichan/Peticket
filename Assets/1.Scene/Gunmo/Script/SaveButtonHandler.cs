using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveButtonHandler : MonoBehaviour
{
    public Transform movingTargetTransform; // XR Origin 또는 움직이는 루트

    [Header("패널 연결")]
    public GameObject settingsPanel;
    public GameObject helpPanel;

    [Header("홈 버튼 이동 설정")]
    public string homeSceneName = "StartScene";

    public void OnClick_SaveManually()
    {
        if (GameSaveManager.Instance == null || movingTargetTransform == null)
        {
            Debug.LogError("❌ 저장 실패: 대상이 없습니다.");
            return;
        }

        Vector3 pos = movingTargetTransform.position;
        GameSaveManager.Instance.SaveGame(pos);
        Debug.Log($"💾 저장됨: {pos}");
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    public void OpenHelp()
    {
        if (helpPanel != null)
            helpPanel.SetActive(true);
    }

    public void ReturnToHome()
    {
        if (!string.IsNullOrEmpty(homeSceneName))
        {
            SceneManager.LoadScene(homeSceneName);
            Debug.Log($"🏠 홈 씬으로 이동: {homeSceneName}");
        }
        else
        {
            Debug.LogError("❌ 홈 씬 이름이 비어 있습니다.");
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
