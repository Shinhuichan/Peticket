using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PetUIManager : MonoBehaviour
{
    public GameObject petUIPrefab;
    public Transform uiParent;
    public GameObject backButton;
    public GameObject confirmButton;

    [Header("펫 프리팹 목록")]
    public GameObject smallDogPrefab;
    public GameObject middleDogPrefab;
    public GameObject largeDogPrefab;

    [Header("펫 생성 위치")]
    public Transform spawnPoint;
    [Header("확정 후 닫을 패널")]
    public GameObject panelToClose; // ✅ 이 패널을 비활성화할 예정

    private PetUI selectedUI;
    private static PetUIManager instance;

    private List<string> petIds = new List<string> { "small", "middle", "large" };

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        foreach (string id in petIds)
        {
            GameObject ui = Instantiate(petUIPrefab, uiParent);
            PetUI uiScript = ui.GetComponent<PetUI>();
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

    private void OnPetSelected(string petId)
    {
        GameSaveManager.Instance.SetSelectedPet(petId);

        foreach (var ui in GetComponentsInChildren<PetUI>(true))
        {
            bool isSelected = ui.GetPetId() == petId;
            ui.gameObject.SetActive(isSelected);

            if (isSelected)
                selectedUI = ui;
        }

        backButton?.SetActive(true);
        confirmButton?.SetActive(true);
    }

    public void OnClick_BackToSelection()
    {
        selectedUI = null;

        foreach (var ui in GetComponentsInChildren<PetUI>(true))
        {
            ui.ResetSelection();
        }

        backButton?.SetActive(false);
        confirmButton?.SetActive(false);
    }

    public void OnClick_ConfirmSelection()
    {
        string selectedPetId = GameSaveManager.Instance?.currentSaveData?.selectedPetId;

        if (string.IsNullOrEmpty(selectedPetId))
        {
            Debug.LogWarning("❌ 펫이 선택되지 않았습니다.");
            return;
        }

        GameObject prefabToSpawn = null;

        switch (selectedPetId)
        {
            case "small":
                prefabToSpawn = smallDogPrefab;
                break;
            case "middle":
                prefabToSpawn = middleDogPrefab;
                break;
            case "large":
                prefabToSpawn = largeDogPrefab;
                break;
        }

        if (prefabToSpawn != null && spawnPoint != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"🐶 펫 생성 완료: {selectedPetId}");
        }
        else
        {
            Debug.LogError("❌ 펫 생성 실패: 프리팹 또는 위치 누락");
        }

        // ✅ 버튼 비활성화
        backButton?.SetActive(false);
        confirmButton?.SetActive(false);
        // ✅ 지정된 패널 비활성화
        if (panelToClose != null)
        {
            panelToClose.SetActive(false);
            Debug.Log($"📦 패널 비활성화됨: {panelToClose.name}");
        }
    }
}

