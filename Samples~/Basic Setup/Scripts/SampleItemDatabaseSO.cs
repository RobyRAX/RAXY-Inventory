using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RAXY.InventorySystem;
using UnityEngine;

[CreateAssetMenu(fileName = "Sample Item Database", menuName = "RAXY/Inventory System/Samples/Item Database")]
public class SampleItemDatabaseSO : ScriptableObject, IItemDatabase
{
    [SerializeField]
    List<SampleCurrencySO> items = new();

    public List<IItemEntry> Items
    {
        get
        {
            var result = new List<IItemEntry>();
            if (items == null)
                return result;

            foreach (var item in items)
            {
                if (item != null)
                    result.Add(item);
            }

            return result;
        }
    }

    public IItemEntry GetItemEntry(string itemId)
    {
        if (string.IsNullOrEmpty(itemId) || items == null)
            return null;

        foreach (var item in items)
        {
            if (item != null && item.ItemId == itemId)
                return item;
        }

        return null;
    }

    public UniTask Init()
    {
        return UniTask.CompletedTask;
    }
}
