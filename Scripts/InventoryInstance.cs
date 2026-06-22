using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.InventorySystem
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class InventoryInstance
    {
        public string inventoryId;

        [DictionaryDrawerSettings(
            KeyLabel = "Instance Id",
            ValueLabel = "Label",
            DisplayMode = DictionaryDisplayOptions.Foldout)]
        [HideReferenceObjectPicker]
        public Dictionary<string, IItemInstance> storedItems = new();

        public InventoryInstance() { }
        public InventoryInstance(string inventoryId, InventoryManagerBase inventoryManager)
        {
            this.inventoryId = inventoryId;
            _inventoryManager = inventoryManager;
            storedItems = new Dictionary<string, IItemInstance>();
        }

        InventoryManagerBase _inventoryManager;

        public void SetInventoryManager(InventoryManagerBase inventoryManager)
        {
            _inventoryManager = inventoryManager;
        }

        public void Clone(InventoryInstance other, bool deepClone = false)
        {
            // If deepClone, start by tracking all matching IDs
            HashSet<string> matchedInstanceIds = deepClone ? new HashSet<string>() : null;

            foreach (var otherItem in other.storedItems.Values)
            {
                if (storedItems.TryGetValue(otherItem.ItemInstanceId, out var existedItem))
                {
                    existedItem.Amount = otherItem.Amount;

                    // Lanjutin ke tipe2 lain nantinya
                }
                else
                {
                    var clonedItem = _inventoryManager.ItemFactory.CloneInstance(otherItem);
                    if (clonedItem != null)
                        storedItems[clonedItem.ItemInstanceId] = clonedItem;
                }

                // Track matched items if deepClone
                if (deepClone)
                {
                    matchedInstanceIds.Add(otherItem.ItemInstanceId);
                }
            }

            // If deepClone, remove items not in the incoming inventory
            if (deepClone)
            {
                var idsToRemove = storedItems.Keys.Where(id => !matchedInstanceIds.Contains(id)).ToList();
                foreach (var id in idsToRemove)
                {
                    storedItems.Remove(id);
                }
            }
        }

        public IItemInstance GetItemInstance(string instanceId)
        {
            return !string.IsNullOrEmpty(instanceId) && storedItems.TryGetValue(instanceId, out var item)
                ? item
                : null;
        }

        public List<IItemInstance> GetItemInstances(string itemId)
        {
            return storedItems.Values.Where(item => item.ItemId == itemId).ToList();
        }

        public int GetItemAmount(string itemId)
        {
            var itemInstances = GetItemInstances(itemId);
            int amount = 0;
            foreach (var itemInstance in itemInstances)
            {
                amount += itemInstance.Amount;
            }

            return amount;
        }

        #region Operation
        public bool HasItem(string itemId)
        {
            return storedItems.Values.Any(item => item.ItemId == itemId && item.Amount > 0);
        }

        public void AddItem_Batch(List<ItemAmountContainer> itemBatch, bool notifyBatch = true, bool notifySingle = false)
        {
            foreach (var itemAmountCont in itemBatch)
            {
                AddItem(itemAmountCont, notifySingle);
            }

            if (notifyBatch)
            {
                _inventoryManager.Fire_ItemAddedBatchEvent(itemBatch, inventoryId);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemAmountContainer"></param>
        /// <param name="notify">Broadcast event based on this bool for outside system</param>
        public IItemInstance AddItem(ItemAmountContainer itemAmountContainer, bool notify = true)
        {
            if (itemAmountContainer == null)
                return null;

            if (storedItems == null)
                storedItems = new Dictionary<string, IItemInstance>();

            if (itemAmountContainer.amount <= 0)
                return null;

            if (_inventoryManager == null)
                return null;

            if (_inventoryManager.ItemDatabase == null)
                return null;

            var itemSO = _inventoryManager.ItemDatabase.GetItemEntry(itemAmountContainer.itemId);
            if (itemSO == null)
                return null;

            IItemInstance itemInstance = null;
            itemInstance = storedItems.Values.FirstOrDefault(item => item.ItemId == itemAmountContainer.itemId);

            if (itemInstance != null && itemSO.IsStackable)
            {
                itemInstance.Amount += itemAmountContainer.amount;
            }
            else
            {
                var factory = _inventoryManager.ItemFactory;
                if (factory == null)
                    return null;

                itemInstance = factory.CreateItemInstance(itemSO, itemAmountContainer.amount);
                if (itemInstance == null)
                    return null;

                storedItems[itemInstance.ItemInstanceId] = itemInstance;
            }

            if (notify)
            {
                _inventoryManager.Fire_ItemAddedEvent(itemAmountContainer, inventoryId);
            }

            return itemInstance;
        }

        public IItemInstance AddItem(string itemId, int amount, bool showNotif = true)
        {
            ItemAmountContainer itemAmountContainer = new ItemAmountContainer
            {
                itemId = itemId,
                amount = amount
            };
            return AddItem(itemAmountContainer, showNotif);
        }

        public IItemInstance AddItem(IItemEntry itemSO, int amount, bool showNotif = true)
        {
            ItemAmountContainer itemAmountContainer = new ItemAmountContainer
            {
                itemId = itemSO.ItemId,
                amount = amount
            };
            return AddItem(itemAmountContainer, showNotif);
        }

        public bool SubtractItem(string itemId, int amount)
        {
            if (amount <= 0)
                return false;

            var existingItemPair = storedItems.FirstOrDefault(pair => pair.Value.ItemId == itemId);
            var existingItem = existingItemPair.Value;
            if (existingItem == null || existingItem.Amount < amount)
                return false;

            existingItem.Amount -= amount;

            if (existingItem.Amount <= 0)
                storedItems.Remove(existingItemPair.Key);

            return true;
        }

        public bool RemoveItem(string instanceId)
        {
            return !string.IsNullOrEmpty(instanceId) && storedItems.Remove(instanceId);
        }

        public void ClearInventory()
        {
            storedItems.Clear();
        }
        #endregion
    }
}
