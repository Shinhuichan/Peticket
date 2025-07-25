using CustomInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField, ReadOnly] int goToSceneIndex = 0;
    public void SceneMove()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"SceneMove | 현재: {currentSceneIndex}, 다음: {nextSceneIndex}");
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("SceneMove | 다음 씬이 Build Settings에 없습니다!");
        }
        
        GameManager.Instance.isCollarEquip = false;
        GameManager.Instance.isMuzzleEquip = false;
        GameManager.Instance.origin.transform.position = new Vector3(0f, 0.55f, 0f);
        GameManager.Instance.mainCam.transform.position = Vector3.zero;
        InputManager.Instance.canvas.worldCamera = Camera.main;
        InputManager.Instance.allDirects = null;
    }
}