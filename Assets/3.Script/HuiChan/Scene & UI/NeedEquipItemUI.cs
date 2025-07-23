using System.Collections.Generic;
using UnityEngine;

public class NeedEquipItemUI : MonoBehaviour
{
    [Header("Need Item")]
    [SerializeField] GameObject foodSlot;
    [SerializeField] GameObject ballSlot;
    [SerializeField] GameObject shovelSlot;
    [SerializeField] GameObject bowlSlot;

    [Header("Equip Item")]
    [SerializeField] GameObject muzzleSlot;
    [SerializeField] GameObject collarSlot;

    void Update()
    {
        NeedItemCheck();
        EquipItemCheck();
    }
    void NeedItemCheck()
    {
        if (foodSlot != null) foodSlot.SetActive(true);
        if (ballSlot != null) ballSlot.SetActive(true);
        if (shovelSlot != null) shovelSlot.SetActive(true);
        if (bowlSlot != null) bowlSlot.SetActive(true);

        List<string> itemNames = new List<string>();
        foreach (var item in GameManager.Instance.currentHasItem)
            itemNames.Add(item.ToString());

        foreach (string item in itemNames)
        {
            if (item.Contains("Food"))
                foodSlot.SetActive(false);
            else if (item.Contains("Ball"))
                ballSlot.SetActive(false);
            else if (item.Contains("Shovel"))
                shovelSlot.SetActive(false);
            else if (item.Contains("Bowl"))
                bowlSlot.SetActive(false);
        }
    }
    void EquipItemCheck()
    {
        muzzleSlot.SetActive(!GameManager.Instance.isMuzzleEquip);
        collarSlot.SetActive(!GameManager.Instance.isCollarEquip);
    }
}