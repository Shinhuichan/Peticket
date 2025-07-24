using CustomInspector;
using UnityEngine;

public class PlayerCheck : MonoBehaviour
{
    [SerializeField] GameObject sceneChangeUI;
    [SerializeField] CheckItem checkItem;

    void OnTriggerEnter(Collider other)
    {
        if (sceneChangeUI == null)
        {
            Debug.Log("PlayerCheck | SceneChangeUI가 Null입니다.");
            return;
        }
        if (other.tag.Equals("Player"))
        {
            Debug.Log("Player 진입");
            if (checkItem == null)
                sceneChangeUI.SetActive(true);
            if (checkItem != null && checkItem.HasAllitem())
                sceneChangeUI.SetActive(true);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (sceneChangeUI == null)
        {
            Debug.Log("PlayerCheck | SceneChangeUI가 Null입니다.");
            return;
        }
        if (other.tag.Equals("Player"))
        {
            Debug.Log("Player 이탈");
            sceneChangeUI.SetActive(false);
        }
    }
}
