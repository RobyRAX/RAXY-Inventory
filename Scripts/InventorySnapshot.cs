using System;
using System.Collections.Generic;

namespace RAXY.InventorySystem
{
    [Serializable]
    public sealed class InventorySnapshot
    {
        public int version = 1;
        public List<InventoryRecordSnapshot> inventories = new();
    }

    [Serializable]
    public sealed class InventoryRecordSnapshot
    {
        public string inventoryId;
        public List<InventoryItemSnapshot> items = new();
    }

    [Serializable]
    public sealed class InventoryItemSnapshot
    {
        public string itemId;
        public string itemInstanceId;
        public int amount;
    }
}
