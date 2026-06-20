using RAXY.Core.Addressable;
using RAXY.Utility.Localization;
using UnityEngine;

namespace RAXY.InventorySystem
{
    public interface IItemEntry
    {
        public string ItemName { get; }
        public string ItemId { get; }
        public bool IsStackable { get; }

        public AddressableAssetProviderSprite ItemIconProvider { get; }
        public LocalizationCacher ItemNameLoc { get; }
        public LocalizationCacher ItemDescriptionLoc { get; }
        public LocalizationCacher ItemAdditionalDescriptionLoc { get; }

    }
}
