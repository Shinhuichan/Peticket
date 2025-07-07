using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Setting")]
    [SerializeField] protected GameObject selectObj;

    [Header("Click Setting")]
    [SerializeField] private GameObject showObj;
    protected Button button;
    bool toggle = false;
    public virtual void OnEnable()
    {
        button = GetComponent<Button>();
        if (button == null) return;
        button.onClick.AddListener(ToggleObj);
    }

    public virtual void ToggleObj()
    {
        toggle = !toggle;
        showObj.SetActive(toggle);
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (selectObj != null) selectObj.SetActive(true);
        StartCoroutine(WaitInput());
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (selectObj != null) selectObj.SetActive(false);
    }

    public IEnumerator WaitInput()
    {
        yield return new WaitForSeconds(0.1f);
    }
}