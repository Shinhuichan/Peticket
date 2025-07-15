using System.Collections.Generic;
using UnityEngine;

public class PetUIManager : MonoBehaviour
{
    public GameObject petUIPrefab;
    public Transform uiParent;

    private List<string> petIds = new List<string> { "small", "middle", "large" };

    private void Start()
    {
        foreach (string id in petIds)
        {
            GameObject ui = Instantiate(petUIPrefab, uiParent);
            PetAffinityUI uiScript = ui.GetComponent<PetAffinityUI>();
            uiScript.Initialize(id, GetPetNameFromId(id));
        }
    }

    private string GetPetNameFromId(string id)
    {
        return id switch
        {
            "small" => "소형견",
            "middle" => "중형견",
            "large" => "대형견",
            _ => "???"
        };
    }

    public void OnClick_SaveAffinity()
    {
        FindObjectOfType<PetAffinityManager>().SaveAffinity();
    }

    public void OnClick_LoadAffinity()
    {
        FindObjectOfType<PetAffinityManager>().LoadAffinity();
        RefreshAllPetUIs();
    }

    public void RefreshAllPetUIs()
    {
        foreach (var ui in GetComponentsInChildren<PetAffinityUI>())
        {
            ui.Refresh();
        }
    }
}
