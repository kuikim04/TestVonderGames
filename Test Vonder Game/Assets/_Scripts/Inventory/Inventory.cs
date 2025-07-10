using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemData;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    [Header("UI")]
    public Canvas canvas;
    [SerializeField] private InventorySlotUI[] slotUIs;
    [SerializeField] private GameObject dragItemPrefab;

    [Header("Inventory")]
    public List<InventorySlot> slots = new();
    [SerializeField] private int maxSlots;

    [Header("Craft")]
    [SerializeField] private GameObject craftObject;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (slotUIs == null || slotUIs.Length == 0)
        {
            Debug.Log("slotUIs not assigned");
            return;
        }

        maxSlots = slotUIs.Length;

        for (int i = 0; i < maxSlots; i++)
            slots.Add(new InventorySlot());

        RefreshUI();
    }

    public bool AddItem(ItemData item, int amount)
    {
        int remaining = amount;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty && slot.item.itemName == item.itemName && slot.quantity < item.maxStack)
            {
                int toAdd = Mathf.Min(item.maxStack - slot.quantity, remaining);
                slot.quantity += toAdd;
                remaining -= toAdd;
                CreateDragItemUI(i, item, slot.quantity);

                if (remaining <= 0) return true;
            }
        }

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.IsEmpty)
            {
                int toAdd = Mathf.Min(item.maxStack, remaining);
                slot.item = item;
                slot.quantity = toAdd;
                remaining -= toAdd;
                CreateDragItemUI(i, item, slot.quantity);

                if (remaining <= 0) return true;
            }
        }

        Debug.Log("Inventory Full");
        return remaining <= 0;
    }

    private void CreateDragItemUI(int slotIndex, ItemData item, int amount)
    {
        foreach (Transform child in slotUIs[slotIndex].transform)
        {
            if (child.GetComponent<DragItemUI>())
            {
                Destroy(child.gameObject);
                break;
            }
        }

        var go = Instantiate(dragItemPrefab, slotUIs[slotIndex].transform);
        var drag = go.GetComponent<DragItemUI>();

        drag.slotIndex = slotIndex;
        drag.icon.sprite = item.icon;
        drag.icon.enabled = true;
        drag.quantityText.text = amount > 1 ? $"{amount}/{item.maxStack}" : "";
    }

    public void UseItem(int index)
    {
        if (index >= slots.Count || slots[index].IsEmpty) return;

        var slot = slots[index];
        if (slot.item.itemType == ItemData.ItemType.Healer)
        {
            slot.item.Use();
            slot.quantity--;
            if (slot.quantity <= 0) slot.Clear();

            RefreshUI();
        }
    }

    public void SortInventory()
    {
        slots.Sort((a, b) =>
        {
            if (a.IsEmpty && b.IsEmpty) return 0;
            if (a.IsEmpty) return 1;
            if (b.IsEmpty) return -1;

            int typeOrder = GetItemTypePriority(a.item.itemType).CompareTo(GetItemTypePriority(b.item.itemType));
            if (typeOrder != 0)
                return typeOrder;

            int nameCompare = string.Compare(a.item.itemName, b.item.itemName, System.StringComparison.OrdinalIgnoreCase);
            if (nameCompare == 0)
            {
                return b.quantity.CompareTo(a.quantity);
            }
            else
            {
                return nameCompare;
            }
        });

        RefreshUI();
    }


    private int GetItemTypePriority(ItemData.ItemType type) => type switch
    {
        ItemData.ItemType.Healer => 0,
        ItemData.ItemType.Tool => 1,
        ItemData.ItemType.Seed => 2,
        _ => 3
    };

    public void RefreshUI()
    {
        for (int i = 0; i < slotUIs.Length && i < slots.Count; i++)
        {
            var slot = slots[i];

            foreach (Transform child in slotUIs[i].transform)
            {
                if (child.GetComponent<DragItemUI>())
                    Destroy(child.gameObject);
            }

            if (!slot.IsEmpty)
                CreateDragItemUI(i, slot.item, slot.quantity);
        }
    }

    public int GetSlotIndexFromUI(InventorySlotUI ui)
    {
        for (int i = 0; i < slotUIs.Length; i++)
            if (slotUIs[i] == ui) return i;
        return -1;
    }

    #region CRAFT

    public void OpenCloseCraftPage()
    {
        craftObject.SetActive(!craftObject.activeSelf);
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        int remaining = amount;

        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty && slot.item.itemName == item.itemName)
            {
                if (slot.quantity >= remaining)
                {
                    slot.quantity -= remaining;
                    if (slot.quantity <= 0)
                        slot.Clear();

                    RefreshUI();

                    return true;
                }
                else
                {
                    remaining -= slot.quantity;
                    slot.Clear();
                }
            }
        }

        RefreshUI();

        return remaining <= 0;
    }

    public int GetTotalQuantity(ItemData item)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.item.itemName == item.itemName)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    #endregion


    public void HackItem(ItemData itemData)
    {
        AddItem(itemData, 10);
    }

}

