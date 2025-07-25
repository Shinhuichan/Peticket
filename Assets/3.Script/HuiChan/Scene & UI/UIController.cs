using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct UI
{
    public string name;
    public GameObject uiObj;
}
[System.Serializable]
public struct SceneUI
{
    public string sceneName;
    public List<UI> ui;
}
public class UIController : SingletonBehaviour<UIController>
{
    protected override bool IsDontDestroy() => true;

    [SerializeField] List<SceneUI> UIs;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += UISetting;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= UISetting;
    }

    private void UISetting(Scene scene, LoadSceneMode mode)
    {
        SceneSetting(scene.name);
    }

    private void SceneSetting(string currentScene)
    {
        foreach (SceneUI sceneUI in UIs)
        {
            bool isCurrentScene = sceneUI.sceneName.Equals(currentScene);

            foreach (UI ui in sceneUI.ui)
            {
                if (ui.uiObj != null)
                    ui.uiObj.SetActive(isCurrentScene);
            }
        }
    }
}
