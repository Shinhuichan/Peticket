using CustomInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField, ReadOnly] int goToSceneIndex = 0;
    void Start()
    {
        goToSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;   
    }
    public void SceneMove()
    {
        SceneManager.LoadScene(goToSceneIndex);

        // Scene이 바뀌면서 Camera 설정 및 SceneIndex 설정
        goToSceneIndex++;
        InputManager.Instance.canvas.worldCamera = Camera.current;
        InputManager.Instance.allDirects = null;
        Debug.Log($"Current Scene : {goToSceneIndex}");
    }
}