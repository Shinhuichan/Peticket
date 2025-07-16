using System.Collections.Generic;
using UnityEngine;

public class PetUIManager : MonoBehaviour
{
    public GameObject petUIPrefab;
    public Transform uiParent;
    private PetAffinityUI selectedUI;
    public GameObject backButton;

    private List<string> petIds = new List<string> { "small", "middle", "large" };

    private void Start()
    {
        foreach (string id in petIds)
        {
            GameObject ui = Instantiate(petUIPrefab, uiParent);
            PetAffinityUI uiScript = ui.GetComponent<PetAffinityUI>();
            uiScript.Initialize(id, GetPetNameFromId(id), OnPetSelected);
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
    public void SelectPet(string petId)
    {
        foreach (Transform child in uiParent)
        {
            PetAffinityUI ui = child.GetComponent<PetAffinityUI>();
            if (ui == null) continue;

            bool isSelected = ui.GetPetId() == petId;
            ui.gameObject.SetActive(isSelected);  // 하나만 남기고 나머지 비활성화

            if (isSelected)
            {
                selectedUI = ui;
                Debug.Log($"선택된 펫: {petId}");
            }
        }
    }
    private void OnPetSelected(string petId)
{
    Debug.Log($"선택된 반려동물: {petId}");

    // ✅ 선택된 펫 ID 저장
    GameSaveManager.Instance.currentSaveData.selectedPetId = petId;

    foreach (var ui in GetComponentsInChildren<PetAffinityUI>())
    {
        bool isSelected = ui.GetPetId() == petId;
        ui.gameObject.SetActive(isSelected);

        if (isSelected) selectedUI = ui;
        if (backButton != null)
            backButton.SetActive(true);
    }

    // ✅ 선택 시 즉시 저장 (선택 기억)
    GameSaveManager.Instance.SaveGame(Vector3.zero); // 또는 현재 플레이어 위치
}
public void OnClick_BackToSelection()
{
    selectedUI = null;

    foreach (var ui in GetComponentsInChildren<PetAffinityUI>(true)) // ← true 중요!
    {
        ui.ResetSelection(); // SetActive(true) + 버튼도 다시 보이게
    }

    if (backButton != null)
        backButton.SetActive(false); // 뒤로가기 버튼 숨기기

    Debug.Log("🔙 선택 초기화 완료");
}

}
