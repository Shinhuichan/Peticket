using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    public GameObject panelToClose;

    [Header("해금 메시지 출력")]
    public TextMeshProUGUI unlockText;
    [TextArea] public string midDogMessage = "중형견 선택이 가능합니다!";
    [TextArea] public string largeDogMessage = "대형견 선택이 가능합니다!";
    public float messageDisplayDuration = 2f;

    private Coroutine unlockTextCoroutine;
    private PetUI selectedUI;
    private static PetUIManager instance;

    private List<string> petIds = new List<string> { "small", "middle", "large" };

    [ContextMenu("초기화: 텍스트 알림 리셋")]
    public void ResetUnlockTextFlags()
    {
        PlayerPrefs.DeleteKey("ShowedMidDogUnlockText");
        PlayerPrefs.DeleteKey("ShowedLargeDogUnlockText");
        Debug.Log("🔄 PlayerPrefs 텍스트 알림 플래그 초기화 완료");
    }

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
        unlockText?.gameObject.SetActive(false);



        foreach (string id in petIds)
        {
            GameObject ui = Instantiate(petUIPrefab, uiParent);
            ui.SetActive(false);

            PetUI uiScript = ui.GetComponent<PetUI>();
            uiScript.Initialize(id, GetPetNameFromId(id), OnPetSelected);
        }

        EnablePetUI("small");
        CheckAffinityUnlocks(); // 해금 조건 확인
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

        float affinity = GameSaveManager.Instance?.currentSaveData?.playerProgress ?? 0f;

        foreach (var ui in GetComponentsInChildren<PetUI>(true))
        {
            string petId = ui.GetPetId();

            bool canEnable = petId switch
            {
                "small" => true,
                "middle" => affinity >= 50f,
                "large" => affinity >= 100f,
                _ => false
            };

            ui.gameObject.SetActive(canEnable);

            if (canEnable)
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

        GameObject prefabToSpawn = selectedPetId switch
        {
            "small" => smallDogPrefab,
            "middle" => middleDogPrefab,
            "large" => largeDogPrefab,
            _ => null
        };

        if (prefabToSpawn != null && spawnPoint != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"🐶 펫 생성 완료: {selectedPetId}");
        }
        else
        {
            Debug.LogError("❌ 펫 생성 실패: 프리팹 또는 위치 누락");
        }

        backButton?.SetActive(false);
        confirmButton?.SetActive(false);
        panelToClose?.SetActive(false);
    }

    private void CheckAffinityUnlocks()
    {
        float affinity = GameSaveManager.Instance?.currentSaveData?.playerProgress ?? 0f;

        if (affinity >= 50f)
        {
            EnablePetUI("middle");

            if (!PlayerPrefs.HasKey("ShowedMidDogUnlockText"))
            {
                ShowUnlockMessage(midDogMessage);
                PlayerPrefs.SetInt("ShowedMidDogUnlockText", 1);
            }
        }

        if (affinity >= 100f)
        {
            EnablePetUI("large");

            if (!PlayerPrefs.HasKey("ShowedLargeDogUnlockText"))
            {
                ShowUnlockMessage(largeDogMessage);
                PlayerPrefs.SetInt("ShowedLargeDogUnlockText", 1);
            }
        }
    }

    private void EnablePetUI(string petId)
    {
        foreach (var ui in GetComponentsInChildren<PetUI>(true))
        {
            if (ui.GetPetId() == petId)
                ui.gameObject.SetActive(true);
        }
    }

    private void ShowUnlockMessage(string message)
    {
        if (unlockText == null) return;

        if (unlockTextCoroutine != null)
            StopCoroutine(unlockTextCoroutine);

        unlockTextCoroutine = StartCoroutine(ShowUnlockMessageRoutine(message));
    }

    private IEnumerator ShowUnlockMessageRoutine(string message)
    {
        unlockText.text = message;
        unlockText.gameObject.SetActive(true);
        yield return new WaitForSeconds(messageDisplayDuration);
        unlockText.gameObject.SetActive(false);
        unlockTextCoroutine = null;
    }
}
