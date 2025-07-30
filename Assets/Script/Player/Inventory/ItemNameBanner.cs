using UnityEngine;

public class ItemNameBanner : MonoBehaviour {
    [SerializeField] private UI_Inventory uiInventory;
    [SerializeField] private TMPro.TextMeshProUGUI itemNameText;

    private void Start() {
        uiInventory.OnItemSelected += UI_Inventory_OnItemSelected;
    }

    private void UI_Inventory_OnItemSelected(object sender, UI_Inventory.OnItemSelectedEventArgs e) {
        if (e.item != null && e.item.itemSO != null) {
            itemNameText.text = e.item.itemSO.itemName;
        } else {
            itemNameText.text = "Not Selected";
        }
    }
}
