namespace RAXY.InventorySystem
{
    public interface IItemFactory
    {
        public IItemInstance CloneInstance(IItemInstance otherItem);
        public IItemInstance CreateItemInstance(IItemEntry itemEntry, int amount);
    }
}
