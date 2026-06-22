using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RAXY.InventorySystem
{
    public class ItemInstance_Base : IItemInstance
    {
        [SerializeField]
        string itemId;
        public string ItemId
        {
            get => itemId;
            set => itemId = value;
        }

        [JsonIgnore]
        [ShowInInspector]
        [ReadOnly]
        IItemEntry _cachedItemEntry;

        IItemDatabase _itemDb;
        
        [JsonIgnore]
        public IItemEntry ItemEntry
        {
            get
            {
                if (_cachedItemEntry == null)
                {
                    if (_itemDb == null)
                        return null;

                    _cachedItemEntry = _itemDb.GetItemEntry(ItemId);
                    return _cachedItemEntry;
                }
                else
                {
                    return _cachedItemEntry;
                }
            }
            set
            {
                _cachedItemEntry = value;
            }
        }

        [SerializeField]
        string itemInstanceId;
        public string ItemInstanceId
        {
            get => itemInstanceId;
            set => itemInstanceId = value;
        }

        [SerializeField]
        int amount;
        public int Amount
        {
            get => amount;
            set => amount = value;
        }

        public ItemInstance_Base() { }
        public ItemInstance_Base(IItemDatabase itemDb)
        {
            _itemDb = itemDb;
        }
    }
}
