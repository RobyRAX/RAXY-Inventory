using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace RAXY.InventorySystem
{
    [DisallowMultipleComponent]
    public class CheckItemsInInventory : InventoryObject
    {
        public bool executeOnStart;

        [TitleGroup("Items")]
        [ListDrawerSettings(ListElementLabelName = "Label")]
        [SerializeField]
        List<ItemAmountContainer> items = new();

        [TitleGroup("Events")]
        [SerializeField]
        UnityEvent onItemsAvailable = new();

        [TitleGroup("Events")]
        [SerializeField]
        UnityEvent onItemsMissing = new();

        void Start()
        {
            if (executeOnStart)
                Execute();
        }

        [Button]
        public void Execute()
        {
            if (HasRequiredItems())
                onItemsAvailable?.Invoke();
            else
                onItemsMissing?.Invoke();
        }

        [TitleGroup("Debug Functions")]
        [Button]
        public override void Refresh()
        {
            Execute();
        }

        bool HasRequiredItems()
        {
            if (items == null || items.Count == 0)
                return false;

            if (inventoryManager == null)
                inventoryManager = FindFirstObjectByType<InventoryManagerBase>();

            var inventory = inventoryManager?.PlayerInventoryInstance;
            if (inventory == null)
                return false;

            var requiredAmounts = new Dictionary<string, long>();
            foreach (var item in items)
            {
                if (item == null || string.IsNullOrWhiteSpace(item.itemId) || item.amount <= 0)
                    return false;

                requiredAmounts.TryGetValue(item.itemId, out var currentAmount);
                requiredAmounts[item.itemId] = currentAmount + item.amount;
            }

            foreach (var requirement in requiredAmounts)
            {
                if (inventory.GetItemAmount(requirement.Key) < requirement.Value)
                    return false;
            }

            return true;
        }
    }
}
