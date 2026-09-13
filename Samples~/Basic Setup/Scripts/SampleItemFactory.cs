using RAXY.InventorySystem;
using UnityEngine;

public class SampleItemFactory : MonoBehaviour, IItemFactory
{
    public IItemInstance CreateItemInstance(IItemEntry itemEntry, int amount)
    {
        if (itemEntry == null)
        {
            Debug.LogWarning("[SampleItemFactory] CreateItemInstance failed: itemEntry is null.");
            return null;
        }

        if (itemEntry is not SampleCurrencySO)
        {
            Debug.LogWarning($"[SampleItemFactory] Unsupported item type '{itemEntry.GetType().Name}'.");
            return null;
        }

        return new SampleItemInstance_Currency
        {
            ItemEntry = itemEntry,
            ItemId = itemEntry.ItemId,
            ItemInstanceId = itemEntry.ItemId,
            Amount = amount
        };
    }

    public IItemInstance CloneInstance(IItemInstance otherItem)
    {
        if (otherItem == null)
        {
            Debug.LogWarning("[SampleItemFactory] CloneInstance failed: otherItem is null.");
            return null;
        }

        if (otherItem is not SampleItemInstance_Currency)
        {
            Debug.LogWarning($"[SampleItemFactory] Unsupported instance type '{otherItem.GetType().Name}'.");
            return null;
        }

        return new SampleItemInstance_Currency
        {
            ItemEntry = otherItem.ItemEntry,
            ItemId = otherItem.ItemId,
            ItemInstanceId = otherItem.ItemInstanceId,
            Amount = otherItem.Amount
        };
    }
}
