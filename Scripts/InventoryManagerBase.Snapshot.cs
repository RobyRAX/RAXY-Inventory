using System.Collections.Generic;
using UnityEngine;

namespace RAXY.InventorySystem
{
    public abstract partial class InventoryManagerBase
    {
        public InventorySnapshot CaptureSnapshot()
        {
            var snapshot = new InventorySnapshot();
            if (InventoryInstances == null)
                return snapshot;

            foreach (var pair in InventoryInstances)
            {
                if (string.IsNullOrEmpty(pair.Key) || pair.Value == null)
                    continue;

                var inventory = new InventoryRecordSnapshot { inventoryId = pair.Key };
                if (pair.Value.storedItems != null)
                {
                    foreach (var item in pair.Value.storedItems.Values)
                    {
                        if (item == null || string.IsNullOrEmpty(item.ItemId) ||
                            string.IsNullOrEmpty(item.ItemInstanceId) || item.Amount <= 0)
                            continue;

                        var itemSnapshot = CaptureItemSnapshot(item);
                        if (itemSnapshot != null)
                            inventory.items.Add(itemSnapshot);
                    }
                }

                snapshot.inventories.Add(inventory);
            }

            return snapshot;
        }

        protected virtual InventoryItemSnapshot CaptureItemSnapshot(IItemInstance item)
        {
            return new InventoryItemSnapshot
            {
                itemId = item.ItemId,
                itemInstanceId = item.ItemInstanceId,
                amount = item.Amount
            };
        }

        protected virtual bool RestoreItemSnapshot(InventoryItemSnapshot snapshot, IItemInstance instance)
        {
            return true;
        }

        public bool RestoreSnapshot(InventorySnapshot snapshot)
        {
            if (snapshot == null || snapshot.version != 1 || snapshot.inventories == null)
            {
                Debug.LogWarning("[InventoryManager] Invalid or unsupported inventory snapshot.");
                return false;
            }

            if (ItemDatabase == null || ItemFactory == null)
            {
                Debug.LogError("[InventoryManager] Cannot restore inventory without an item database and factory.");
                return false;
            }

            if (ItemDatabase.Items == null)
            {
                Debug.LogError("[InventoryManager] Item database is unavailable; saved items were not restored.");
                return false;
            }

            if (ItemDatabase.Items.Count == 0)
            {
                foreach (var inventory in snapshot.inventories)
                {
                    if (inventory == null || inventory.items == null || inventory.items.Count == 0)
                        continue;

                    Debug.LogError("[InventoryManager] Item database is empty; saved items were not restored.");
                    return false;
                }
            }

            var replacement = new Dictionary<string, InventoryInstance>();
            foreach (var savedInventory in snapshot.inventories)
            {
                if (savedInventory == null || string.IsNullOrWhiteSpace(savedInventory.inventoryId))
                {
                    Debug.LogWarning("[InventoryManager] Skipping saved inventory with no ID.");
                    continue;
                }

                if (replacement.ContainsKey(savedInventory.inventoryId))
                {
                    Debug.LogWarning($"[InventoryManager] Skipping duplicate inventory ID '{savedInventory.inventoryId}'.");
                    continue;
                }

                var inventory = new InventoryInstance(savedInventory.inventoryId, this);
                if (savedInventory.items != null)
                {
                    foreach (var savedItem in savedInventory.items)
                    {
                        if (savedItem == null || string.IsNullOrWhiteSpace(savedItem.itemId) ||
                            string.IsNullOrWhiteSpace(savedItem.itemInstanceId) || savedItem.amount <= 0)
                        {
                            Debug.LogWarning($"[InventoryManager] Skipping invalid item in '{savedInventory.inventoryId}'.");
                            continue;
                        }

                        if (inventory.storedItems.ContainsKey(savedItem.itemInstanceId))
                        {
                            Debug.LogWarning($"[InventoryManager] Skipping duplicate instance ID '{savedItem.itemInstanceId}'.");
                            continue;
                        }

                        var itemEntry = ItemDatabase.GetItemEntry(savedItem.itemId);
                        if (itemEntry == null)
                        {
                            Debug.LogWarning($"[InventoryManager] Skipping unknown item '{savedItem.itemId}'.");
                            continue;
                        }

                        var instance = ItemFactory.CreateItemInstance(itemEntry, savedItem.amount);
                        if (instance == null)
                        {
                            Debug.LogWarning($"[InventoryManager] Skipping item '{savedItem.itemId}' because its instance could not be created.");
                            continue;
                        }

                        instance.ItemInstanceId = savedItem.itemInstanceId;
                        if (!RestoreItemSnapshot(savedItem, instance))
                        {
                            Debug.LogWarning($"[InventoryManager] Skipping item '{savedItem.itemId}' because its snapshot could not be restored.");
                            continue;
                        }

                        inventory.storedItems.Add(savedItem.itemInstanceId, instance);
                    }
                }

                replacement.Add(savedInventory.inventoryId, inventory);
            }

            if (!replacement.ContainsKey(PLAYER_INVENTORY_ID))
                replacement.Add(PLAYER_INVENTORY_ID, new InventoryInstance(PLAYER_INVENTORY_ID, this));

            InventoryInstances = replacement;
            InitialItemSent = true;
            return true;
        }
    }
}
