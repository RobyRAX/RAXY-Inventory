using RAXY.InventorySystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class SampleInventoryManager : InventoryManagerBase
{
    [TitleGroup("Sample Wiring")]
    [SerializeField]
    SampleItemDatabaseSO sampleItemDatabase;

    [TitleGroup("Sample Wiring")]
    [SerializeField]
    SampleItemFactory sampleItemFactory;

    [TitleGroup("Debug Function")]
    [SerializeField]
    string debugItemId = "sample_coin";

    [TitleGroup("Debug Function")]
    [SerializeField]
    int debugAmount = 1;

    void Awake()
    {
        if (sampleItemFactory == null)
            sampleItemFactory = GetComponent<SampleItemFactory>();

        if (sampleItemDatabase != null)
            SetItemDatabase(sampleItemDatabase);

        if (sampleItemFactory != null)
            SetItemFactory(sampleItemFactory);

        SendInitialItems();
    }

    [TitleGroup("Debug Function")]
    [Button]
    public void Debug_AddItem()
    {
        if (PlayerInventoryInstance == null)
        {
            Debug.LogWarning("[SampleInventoryManager] Player inventory not found.");
            return;
        }

        PlayerInventoryInstance.AddItem(debugItemId, debugAmount, notify: true);
    }

    [TitleGroup("Debug Function")]
    [Button]
    public void Debug_SubtractItem()
    {
        TrySubtractItem(PLAYER_INVENTORY_ID, new ItemAmountContainer(debugItemId, debugAmount), notify: true);
    }
}
