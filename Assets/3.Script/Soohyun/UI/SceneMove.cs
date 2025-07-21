using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneMove : MonoBehaviour
{
    public Button StartButton;
    void Start()
    {
        StartButton.onClick.AddListener(StartScene);
    }

    public void StartScene()
    {
        SceneManager.LoadScene("0. Tutoriual");
    }
}
