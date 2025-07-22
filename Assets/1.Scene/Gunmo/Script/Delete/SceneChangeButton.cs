using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeButton : MonoBehaviour
{
    [Header("이동할 씬 이름")]
    public string sceneToLoad;

    /// <summary>
    /// 버튼에서 호출되는 메서드. 지정한 씬으로 이동합니다.
    /// </summary>
    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
            Debug.Log($"➡ 씬 이동: {sceneToLoad}");
        }
        else
        {
            Debug.LogWarning("❌ 씬 이름이 비어 있습니다. 인스펙터에서 설정하세요.");
        }
    }
}
