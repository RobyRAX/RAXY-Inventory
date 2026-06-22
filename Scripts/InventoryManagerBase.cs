using Cysharp.Threading.Tasks;
using RAXY.Core;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RAXY.InventorySystem
{
    public abstract class InventoryManagerBase : MonoBehaviour
    {
        public const string PLAYER_INVENTORY_ID = "player_inventory";

        public InventoryInstance PlayerInventoryInstance
        {
            get
            {
                if (InventoryInstances.TryGetValue(PLAYER_INVENTORY_ID, out var instance))
                    return instance;

                return null;
            }
        }

        protected virtual void Awake()
        {
            if (ItemFactoryObj != null && ItemFactoryObj.TryGetComponent(out IItemFactory factory))
            {
                ItemFactory = factory;
            }

            if (setItemDbInitially)
            {
                if (ItemDatabaseSO is IItemDatabase itemDb)
                {
                    ItemDatabase = itemDb;
                    ItemDatabase.Init().Forget();
                }
            }

            SendInitialItems();
        }

        /// <summary>
        /// ItemAmountContainer is the item that will be added;<br/>
        /// string is inventoryId;<br/>
        /// </summary>
        public event Action<ItemAmountContainer, string> OnItemAdded;

        /// <summary>
        /// ItemAmountContainer is the item that will be used for subtract;<br/>
        /// string is inventoryId;<br/>
        /// </summary>
        public event Action<ItemAmountContainer, string> OnItemSubtracted;

        /// <summary>
        /// List<ItemAmountContainer> is the items that will be added;<br/>
        /// string is inventoryId;<br/>
        /// </summary>
        public event Action<List<ItemAmountContainer>, string> OnItemBatchAdded;

        public void Fire_ItemSubtractedEvent(ItemAmountContainer itemAmount, string inventoryId)
        {
            OnItemSubtracted?.Invoke(itemAmount, inventoryId);
        }

        public void Fire_ItemAddedEvent(ItemAmountContainer itemAmount, string inventoryId)
        {
            OnItemAdded?.Invoke(itemAmount, inventoryId);
        }

        public void Fire_ItemAddedBatchEvent(List<ItemAmountContainer> itemAmounts, string inventoryId)
        {
            OnItemBatchAdded?.Invoke(itemAmounts, inventoryId);
        }

        /// <summary>
        /// Subtract an item from a specific inventory and fire events consistently.
        /// </summary>
        public bool TrySubtractItem(string inventoryId, ItemAmountContainer input, bool notify = true)
        {
            if (input == null || input.amount <= 0)
                return false;

            if (string.IsNullOrEmpty(inventoryId))
                inventoryId = PLAYER_INVENTORY_ID;

            if (InventoryInstances == null ||
                !InventoryInstances.TryGetValue(inventoryId, out var inventoryInstance) ||
                inventoryInstance == null)
            {
                return false;
            }

            bool success = inventoryInstance.SubtractItem(input.itemId, input.amount);
            if (!success)
                return false;

            if (notify)
            {
                Fire_ItemSubtractedEvent(input, inventoryId);
            }

            return true;
        }

        /// <summary>
        /// Convert/craft items in a specific inventory by consuming inputs and granting outputs.
        /// </summary>
        public bool TryConvert(string inventoryId, List<ItemAmountContainer> inputs, 
                                List<ItemAmountContainer> outputs, 
                                bool notify, out string reason)
        {
            reason = "";

            if (string.IsNullOrEmpty(inventoryId))
                inventoryId = PLAYER_INVENTORY_ID;

            if (InventoryInstances == null ||
                !InventoryInstances.TryGetValue(inventoryId, out var inventoryInstance) ||
                inventoryInstance == null)
            {
                reason = "Inventory instance not found.";
                return false;
            }

            if (inputs == null || inputs.Count == 0)
            {
                reason = "Input list is empty.";
                return false;
            }

            // Aggregate required amounts per itemId to avoid duplicate inputs.
            Dictionary<string, int> requiredByItemId = new Dictionary<string, int>();
            foreach (var input in inputs)
            {
                if (input == null || string.IsNullOrEmpty(input.itemId) || input.amount <= 0)
                {
                    reason = "Invalid input item.";
                    return false;
                }

                if (requiredByItemId.ContainsKey(input.itemId))
                    requiredByItemId[input.itemId] += input.amount;
                else
                    requiredByItemId[input.itemId] = input.amount;
            }

            foreach (var kvp in requiredByItemId)
            {
                int ownedAmount = inventoryInstance.GetItemAmount(kvp.Key);
                if (ownedAmount < kvp.Value)
                {
                    reason = $"Insufficient amount for item '{kvp.Key}'.";
                    return false;
                }
            }

            // Consume inputs
            foreach (var kvp in requiredByItemId)
            {
                var input = new ItemAmountContainer(kvp.Key, kvp.Value);
                bool subtracted = TrySubtractItem(inventoryId, input, notify);
                if (!subtracted)
                {
                    reason = $"Failed to subtract item '{kvp.Key}'.";
                    return false;
                }
            }

            // Grant outputs
            if (outputs != null && outputs.Count > 0)
            {
                inventoryInstance.AddItem_Batch(outputs, notifyBatch: notify, notifySingle: false);
            }

            return true;
        }

        /// <summary>
        /// Convert/craft items in player inventory.
        /// </summary>
        [TitleGroup("Debug Function")]
        [Button]
        public bool TryConvert(List<ItemAmountContainer> inputs, List<ItemAmountContainer> outputs, bool notify, out string reason)
        {
            return TryConvert(PLAYER_INVENTORY_ID, inputs, outputs, notify, out reason);
        }

        public event Action OnInitialItemSent;

        public void SendInitialItems()
        {
            InventoryInstances ??= new Dictionary<string, InventoryInstance>();

            if (!InventoryInstances.TryGetValue(PLAYER_INVENTORY_ID, out var playerInventoryInstance) ||
                playerInventoryInstance == null)
            {
                playerInventoryInstance = new InventoryInstance(PLAYER_INVENTORY_ID, this);
                InventoryInstances[PLAYER_INVENTORY_ID] = playerInventoryInstance;
            }
            else
            {
                playerInventoryInstance.SetInventoryManager(this);
            }

            if (InitialItemSent == false)
            {
                if (InitialItems != null)
                {
                    foreach (var itemAmount in InitialItems)
                    {
                        playerInventoryInstance.AddItem(itemAmount, false);
                    }
                }

                InitialItemSent = true;
                OnInitialItemSent?.Invoke();
            }
        }

        [TitleGroup("Reference")]
        [SerializeField]
        bool setItemDbInitially = true;

        public IItemDatabase ItemDatabase { get; set; }

        [TitleGroup("Reference")]
        [ShowIf("@setItemDbInitially")]
        [InfoBox("This ScriptableObject doesn't use IItemDatabase", VisibleIf = "@ValidateItemDatabase", InfoMessageType = InfoMessageType.Error)]
        public ScriptableObject ItemDatabaseSO;
        bool ValidateItemDatabase
        {
            get
            {
                return ItemDatabaseSO != null && ItemDatabaseSO is not IItemDatabase;
            }
        }

        public IItemFactory ItemFactory { get; private set; }

        [TitleGroup("Reference")]
        [InfoBox("This object doesn't contain IItemFactory", VisibleIf = "@ValidateItemFactory", InfoMessageType = InfoMessageType.Error)]
        public GameObject ItemFactoryObj;
        bool ValidateItemFactory
        {
            get
            {
                return ItemFactoryObj != null && ItemFactoryObj.GetComponent<IItemFactory>() == null;
            }
        }

        [TitleGroup("Setting")]
        [ListDrawerSettings(ListElementLabelName = "Label")]
        public List<ItemAmountContainer> InitialItems;
        public bool InitialItemSent { get; set; }

        [TitleGroup("Inventory Instance")]
        [ShowInInspector]
        [DictionaryDrawerSettings(KeyLabel = "Inventory Id", ValueLabel = "Inventory Instance")]
        public Dictionary<string, InventoryInstance> InventoryInstances = new();
    }
}
