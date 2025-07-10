using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlotUI : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var dragItem = eventData.pointerDrag?.GetComponent<DragItemUI>();

        if (dragItem != null)
        {
            Inventory.Instance.slots[dragItem.slotIndex].Clear();
            Inventory.Instance.RefreshUI();
            Destroy(dragItem.gameObject);
        }
    }
}
