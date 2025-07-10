using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryQuickSlotsBarUI : MonoBehaviour
{
    public QuickSlotUI[] slotUIs;

    void Update()
    {
        for (int i = 0; i < slotUIs.Length; i++)
        {
            var slot = Inventory.Instance.slots[i];
            slotUIs[i].Set(slot);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) Inventory.Instance.UseItem(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) Inventory.Instance.UseItem(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) Inventory.Instance.UseItem(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) Inventory.Instance.UseItem(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) Inventory.Instance.UseItem(4);
    }
}
