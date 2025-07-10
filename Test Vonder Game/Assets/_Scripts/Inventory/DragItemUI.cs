using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image icon;
    public TextMeshProUGUI quantityText;
    public int slotIndex;

    private Vector3 originalPosition;
    private Transform originalParent; 
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = Inventory.Instance.canvas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        transform.SetParent(transform.root); 
        canvasGroup.blocksRaycasts = false; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        transform.localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        if (eventData.pointerEnter != null)
        {
            if (eventData.pointerEnter.TryGetComponent<InventorySlotUI>(out var targetSlot))
            {
                int fromIndex = slotIndex;
                int toIndex = Inventory.Instance.GetSlotIndexFromUI(targetSlot);

                if (toIndex == -1)
                {
                    ReturnToStart();
                    return;
                }

                var slots = Inventory.Instance.slots;
                bool isTargetEmpty = slots[toIndex].IsEmpty;

                if (isTargetEmpty)
                {
                    slots[toIndex] = slots[fromIndex];
                    slots[fromIndex] = new InventorySlot();

                    transform.SetParent(targetSlot.transform);
                    transform.position = targetSlot.transform.position;
                    slotIndex = toIndex;

                    Inventory.Instance.RefreshUI();
                    return;
                }
                else
                {
                    ReturnToStart();
                    return;
                }
            }
        }

        ReturnToStart();
    }

    void ReturnToStart()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
    }
}
