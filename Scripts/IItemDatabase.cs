using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RAXY.InventorySystem
{
    public interface IItemDatabase
    {
        public List<IItemEntry> Items { get; }
        public IItemEntry GetItemEntry(string itemId);
        public UniTask Init();
    }
}
