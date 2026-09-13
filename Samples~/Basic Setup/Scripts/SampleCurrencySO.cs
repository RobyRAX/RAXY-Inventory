using RAXY.InventorySystem;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sample Currency", menuName = "RAXY/Inventory System/Samples/Currency")]
public class SampleCurrencySO : ScriptableObject, IItemEntry
{
    [SerializeField]
    string itemId;

    [SerializeField]
    string itemName;

    [SerializeField]
    [TextArea]
    string itemDescription;

    [SerializeField]
    Sprite itemIcon;

    public string ItemId => string.IsNullOrEmpty(itemId) ? name : itemId;

    public bool IsStackable => true;

    public string ItemName => string.IsNullOrEmpty(itemName) ? ItemId : itemName;

    public string ItemDescription => itemDescription;

    public string ItemAdditionalDescription => null;

    public Sprite ItemIcon => itemIcon;
}
