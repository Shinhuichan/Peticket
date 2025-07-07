using UnityEngine;

public class AutoInventorySaver : MonoBehaviour
{
    public InventorySaveManager saveManager;
    public float saveInterval = 30f;

    private float timer = 0f;

    private void Start()
    {
        saveManager.LoadInventory();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= saveInterval)
        {
            saveManager.SaveInventory();
            timer = 0f;
        }
    }

    private void OnApplicationQuit()
    {
        saveManager.SaveInventory();
    }
}
