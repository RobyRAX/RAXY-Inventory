using UnityEngine;

namespace RAXY.InventorySystem
{
    public abstract class InventoryObject : MonoBehaviour
    {
        protected InventoryManagerBase inventoryManager;

        public void Set_InventoryManager(InventoryManagerBase manager)
        {
            inventoryManager = manager;
        }

        public abstract void Refresh();
    }
}
