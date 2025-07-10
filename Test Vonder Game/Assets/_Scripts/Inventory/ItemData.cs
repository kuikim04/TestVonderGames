using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public enum ItemType
    {
        None,
        Healer,
        Tool,
        Seed
    }

    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public int maxStack = 10;

    public int healAmount = 10;
    public void Use()
    {
        switch (itemType)
        {
            case ItemType.Healer:
                UseHealer();
                break;
        }
    }

    private void UseHealer()
    {
        PlayerStat.RequestHeal(healAmount);
    }

}
