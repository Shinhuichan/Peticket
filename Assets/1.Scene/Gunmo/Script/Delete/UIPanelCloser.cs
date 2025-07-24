using UnityEngine;

public class UIPanelCloser : MonoBehaviour
{
    [Header("닫을 패널 목록")]
    public GameObject[] panelsToClose;

    /// <summary>
    /// 버튼 클릭 시 호출 → 열려 있는 패널들을 모두 비활성화
    /// </summary>
    public void OnClick_ClosePanels()
    {
        foreach (var panel in panelsToClose)
        {
            if (panel != null && panel.activeSelf)
            {
                panel.SetActive(false);
                Debug.Log($"🧹 패널 비활성화됨: {panel.name}");
            }
        }
    }
}
