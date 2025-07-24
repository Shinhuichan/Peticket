using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCheck : MonoBehaviour
{
    [SerializeField] GameObject sceneChangeUI;
    [SerializeField] CheckItem checkItem;
    void Start()
    {
        sceneChangeUI = FindAnyObjectByType<SceneChange>().gameObject;
        if (sceneChangeUI == null)
            Debug.Log($"PlayerCheck | sceneChangeUI가 Null입니다.");   
    }
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
            // Room Scene이면서 모든 Item
            if (SceneManager.GetActiveScene().buildIndex == 2 && !checkItem.HasAllitem())
                sceneChangeUI.SetActive(false);
            else sceneChangeUI.SetActive(true);
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
