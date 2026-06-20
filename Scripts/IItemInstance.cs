namespace RAXY.InventorySystem
{
    public interface IItemInstance
    {
        public string ItemId { get; set; }
        public string ItemInstanceId { get; set; }
        public int Amount { get; set; }
        public IItemEntry ItemEntry { get; set; }

#if UNITY_EDITOR
        string Label => $"{ItemInstanceId} - {Amount}x";
#endif
    }
}
