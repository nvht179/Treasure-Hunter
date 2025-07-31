using UnityEngine;

public class DescriptionBoard : MonoBehaviour {
    [SerializeField] private GameObject selectItemInventoryGameObject;
    [SerializeField] private Transform itemVisualTemplate;
    [SerializeField] private TMPro.TextMeshProUGUI descriptionText;
    [SerializeField] private TMPro.TextMeshProUGUI itemNameText;
    [SerializeField] private TMPro.TextMeshProUGUI priceText;

    private ISelectItem selectItemInventory;

    private void Awake() {
        selectItemInventory = selectItemInventoryGameObject.GetComponent<ISelectItem>();
        HideInfo();
    }

    private void Start() {
        selectItemInventory.OnItemSelected += UI_Inventory_OnItemSelected;
    }

    private void UI_Inventory_OnItemSelected(object sender, ISelectItem.OnItemSelectedEventArgs e) {
        if (e.item == null || e.item.itemSO == null) {
            descriptionText.text = string.Empty;
            itemNameText.text = string.Empty;
            return;
        }

        // visual
        foreach(Transform child in itemVisualTemplate) {
            Destroy(child.gameObject);
        }
        Transform itemPrefab = Instantiate(e.item.itemSO.prefab, itemVisualTemplate);
        itemPrefab.localPosition = new Vector3(0, 0, -1);

        // name and description
        itemNameText.text = e.item.itemSO.itemName;
        descriptionText.text = e.item.itemSO.description;

        if(priceText != null) {
            priceText.text = e.item.itemSO.buyPrice.ToString();
        }
    }

    private void HideInfo() {
        foreach(Transform child in itemVisualTemplate) {
            Destroy(child.gameObject);
        }

        descriptionText.text = string.Empty;
        itemNameText.text = string.Empty;
        if(priceText != null) {
            priceText.text = string.Empty;
        }
    }
}
