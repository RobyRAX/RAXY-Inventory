
using System;
using Sirenix.OdinInspector;

namespace RAXY.InventorySystem
{
    [HideReferenceObjectPicker]
    [Serializable]
    public class ItemAmountContainer
    {
        string Label => $"{itemId} - {amount}x";

        public string itemId;
        public int amount = 1;

        [Title("Helper")]
        [OnValueChanged("OnItemSoChange")]
        [NonSerialized]
        [ShowInInspector]
        IItemEntry itemSO_Helper;

        void OnItemSoChange()
        {
            if (itemSO_Helper == null)
            {
                itemId = "";
                return;
            }

            itemId = itemSO_Helper.ItemId;
        }

        public ItemAmountContainer() { }
        public ItemAmountContainer(string itemId, int amount)
        {
            this.itemId = itemId;
            this.amount = amount;
        }
    }
}