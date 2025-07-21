using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] int goToSceneIndex = 0;
    public void SceneMove()
    {
        SceneManager.LoadScene(goToSceneIndex);
    }
}
