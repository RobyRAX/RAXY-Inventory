using RAXY.Core.Addressable;
using RAXY.Utility.Localization;
using UnityEngine;

namespace RAXY.InventorySystem
{
    public interface IItemEntry
    {
        public string ItemId { get; }
        public bool IsStackable { get; }

        public string ItemName { get; }
        public string ItemDescription { get; }
        public string ItemAdditionalDescription { get; }
        public Sprite ItemIcon { get; }
    }
}
