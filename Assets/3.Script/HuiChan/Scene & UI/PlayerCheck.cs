using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCheck : MonoBehaviour
{
    [SerializeField] GameObject sceneChangeUI;
    [SerializeField] CheckItem checkItem;
    void Start()
    {
        sceneChangeUI = FindAnyObjectByType<SceneChange>().gameObject;
        sceneChangeUI.SetActive(false);
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
            bool isSceneIndex2 = SceneManager.GetActiveScene().buildIndex == 2;
            if (checkItem == null)
            {
                sceneChangeUI.SetActive(true);
                return;
            }
            bool hasAllItems = checkItem.HasAllitem();
            Debug.Log($"Scene Index : {SceneManager.GetActiveScene().buildIndex} && 현재 상황 : {checkItem.HasAllitem()}  / {!(isSceneIndex2 && !hasAllItems)}");
            sceneChangeUI.SetActive(!(isSceneIndex2 && !hasAllItems));
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
