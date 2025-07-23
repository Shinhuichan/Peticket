using UnityEngine;

public class EventUIManager : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject ballUI;
    public GameObject leashUI;
    public GameObject bottleUI;
    public GameObject muzzleUI;
    public GameObject dogTagUI;
    public GameObject wasteBagUI;
    public GameObject wateringPotUI;
    public GameObject garbageTongsUI;
    public GameObject snackUI;

    [Header("설정")]
    public bool autoHide = true;
    public float hideDelay = 3f;

    private void Start()
    {
        HideAll();
    }

    private void HideAll()
    {
        ballUI?.SetActive(false);
        leashUI?.SetActive(false);
        bottleUI?.SetActive(false);
        muzzleUI?.SetActive(false);
        dogTagUI?.SetActive(false);
        wasteBagUI?.SetActive(false);
        wateringPotUI?.SetActive(false);
        garbageTongsUI?.SetActive(false);
        snackUI?.SetActive(false);
    }

    private void ShowUI(GameObject ui)
    {
        if (ui == null) return;

        HideAll();
        ui.SetActive(true);

        if (autoHide)
            Invoke(nameof(HideAll), hideDelay);
    }

    // 아래는 외부에서 호출할 함수들
    public void ShowBallUI() => ShowUI(ballUI);
    public void ShowLeashUI() => ShowUI(leashUI);
    public void ShowBottleUI() => ShowUI(bottleUI);
    public void ShowMuzzleUI() => ShowUI(muzzleUI);
    public void ShowDogTagUI() => ShowUI(dogTagUI);
    public void ShowWasteBagUI() => ShowUI(wasteBagUI);
    public void ShowWateringPotUI() => ShowUI(wateringPotUI);
    public void ShowGarbageTongsUI() => ShowUI(garbageTongsUI);
    public void ShowSnackUI() => ShowUI(snackUI);
}
