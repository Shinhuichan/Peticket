using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PetUIManager : MonoBehaviour
{
    public GameObject petUIPrefab;    // 각 펫 카드 프리팹
    public Transform uiParent;        // UI가 배치될 부모
    public GameObject backButton;     // 뒤로가기 버튼
    public GameObject confirmButton; // ← 인스펙터 연결
    public string gameSceneName = "GameScene"; // 인게임 씬 이름 (인스펙터에서 설정)

    private PetUI selectedUI;
    private static PetUIManager instance;

    private List<string> petIds = new List<string> { "small", "middle", "large" };

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); // 중복 방지
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 변경 시 유지
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
        Debug.Log($"✅ 선택된 펫: {petId}");

        GameSaveManager.Instance.SetSelectedPet(petId); // 선택 저장

        foreach (var ui in GetComponentsInChildren<PetUI>(true))
        {
            bool isSelected = ui.GetPetId() == petId;
            ui.gameObject.SetActive(isSelected);

            if (isSelected)
                selectedUI = ui;
        }

        backButton?.SetActive(true);
        confirmButton?.SetActive(true); // 선택된 경우에만 확정 버튼 보이기
    }

    public void OnClick_BackToSelection()
    {
        selectedUI = null;

        foreach (var ui in GetComponentsInChildren<PetUI>(true))
        {
            ui.ResetSelection(); // 다시 보이게
        }

        backButton?.SetActive(false);
        confirmButton?.SetActive(false);
        Debug.Log("🔙 선택 초기화 완료");
    }

    public void OnClick_ConfirmSelection()
{
    string selectedPetId = GameSaveManager.Instance?.currentSaveData?.selectedPetId;

    if (!string.IsNullOrEmpty(selectedPetId))
    {
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            Debug.Log($"🚀 선택된 펫({selectedPetId})으로 게임 씬 이동");
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("⚠ 게임 씬 이름이 설정되어 있지 않습니다. 인스펙터에서 확인하세요.");
        }
    }
    else
    {
        Debug.LogWarning("❌ 펫이 선택되지 않았습니다. 선택 후 계속 진행할 수 있습니다.");
    }
}
}
