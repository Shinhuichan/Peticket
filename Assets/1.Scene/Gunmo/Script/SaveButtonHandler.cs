using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveButtonHandler : MonoBehaviour
{
    public Transform movingTargetTransform; // XR Origin 또는 움직이는 루트

    [Header("패널 연결")]
    public GameObject settingsPanel;
    public GameObject helpPanel;
    [Header("인벤토리 연결")]
    public GameObject inventoryPanel;

    [Header("홈 버튼 이동 설정")]
    public string homeSceneName = "StartScene";
    [Header("파괴할 오브젝트 이름들")]
    public string[] objectsToDestroyOnReturnHome;


    private void Awake()
{
    if (Player.Instance != null && Player.Instance.playerPosition != null)
    {
        movingTargetTransform = Player.Instance.playerPosition;
        Debug.Log($"✅ movingTargetTransform 자동 연결됨: {movingTargetTransform.name}");
    }
    else
    {
        Debug.LogError("❌ Player.Instance 또는 playerPosition이 존재하지 않습니다.");
    }
}
    void Start()
    {
        movingTargetTransform = GameManager.Instance.player.playerPosition.transform;
    }
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

             if (inventoryPanel != null)
            inventoryPanel.SetActive(false); // ✅ 인벤토리 닫기
    }

    public void OpenHelp()
    {
        if (helpPanel != null)
            helpPanel.SetActive(true);

            if (inventoryPanel != null)
            inventoryPanel.SetActive(false); // ✅ 인벤토리 닫기
    }
    public void CloseHelp()
    {
        if (helpPanel != null)
            helpPanel.SetActive(false);
    }

    public void ReturnToHome()
    {
        // ✅ 입력한 이름을 가진 오브젝트 모두 제거
        foreach (string name in objectsToDestroyOnReturnHome)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null)
            {
                Destroy(obj);
                Debug.Log($"🧹 오브젝트 제거됨: {name}");
            }
            else
            {
                Debug.LogWarning($"⚠ 제거할 오브젝트를 찾을 수 없음: {name}");
            }
        }

        // ✅ 씬 이동
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
