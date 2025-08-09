using System;
using System.Collections.Generic;
using UnityEngine;

public class ActiveItemUI : MonoBehaviour
{
    public event EventHandler OnItemEquipped;
    public event EventHandler OnPotionUsed;

    [SerializeField] private Player player;
    [SerializeField] private Transform activeItemContainer;
    [SerializeField] private Transform activeItemTemplate;

    private Dictionary<ItemType, Item> active;
    private struct PotionItem : IEquatable<PotionItem>
    {
        public Item Item;
        public float ExpireTime;

        public bool Equals(PotionItem other)
        {
            return Item.Equals(other.Item) && ExpireTime.Equals(other.ExpireTime);
        }

        public override bool Equals(object obj)
        {
            return obj is PotionItem other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Item, ExpireTime);
        }
    }

    private List<PotionItem> potionItems;

    private void Awake()
    {
        potionItems = new List<PotionItem>();
        active = new Dictionary<ItemType, Item>();
        foreach (ItemType s in System.Enum.GetValues(typeof(ItemType)))
        {
            active[s] = new Item(null, 0);
        }

        activeItemTemplate.gameObject.SetActive(false);
    }

    private void Update()
    {
        float now = Time.time;
        foreach(var potion in potionItems.ToArray())
        {
            if (potion.ExpireTime <= now)
            {
                potionItems.Remove(potion);
                RefreshActiveList();
            }
        }
    }

    public void TryEquipItem(Item item)
    {
        if (item == null || item.itemSO == null) return;
        if (item.itemSO is ConsumableItemSO consumableItemSO)
        {
            consumableItemSO.Consume(player, out float duration);
            potionItems.Add(new PotionItem { Item = item, ExpireTime = Time.time + duration });
            RefreshActiveList();
            OnPotionUsed?.Invoke(this, EventArgs.Empty);
        }
        if (item.itemSO is PassiveItemSO passive)
        {
            var slot = active[passive.itemType];
            if (slot.itemSO != null && slot.itemSO is PassiveItemSO)
            {
                ShowDialogBeforeEquipment(passive);
                return;
            }
            EquipPassiveItem(passive);
        }
    }

    public void EquipPassiveItem(PassiveItemSO passive)
    {
        var slot = active[passive.itemType];

        passive.RemoveEffect(player);

        // equip new
        slot.itemSO = passive;
        slot.quantity = 1;
        active[passive.itemType] = slot;
        passive.ApplyEffect(player);

        RefreshActiveList();
        OnItemEquipped?.Invoke(this, EventArgs.Empty);
    }

    public void DropItem(Item item)
    {
        if(item.itemSO is PassiveItemSO passiveItemSO)
        {
            passiveItemSO.RemoveEffect(player);
            active[passiveItemSO.itemType] = new Item(null, 0);
            RefreshActiveList();
        }
    }

    private void ShowDialogBeforeEquipment(PassiveItemSO passive)
    {
        var equipButton = new DialogButton
        {
            Label = "Equip!",
            ButtonType = DialogButtonType.Accept,
            Callback = () => {
                EquipPassiveItem(passive);
            }
        };

        var declineButton = new DialogButton
        {
            Label = "Cancel",
            ButtonType = DialogButtonType.Decline,
            Callback = () => { /* do nothing */ }
        };

        DialogManager.Instance.ShowDialog(new DialogData
        {
            Title = "Equip new item!",
            Message = "Your equipped item will be dropped!",
            Buttons = new List<DialogButton> { declineButton, equipButton },
        });
    }

    private void RefreshActiveList()
    {
        foreach(Transform child in activeItemContainer)
        {
            if(child == activeItemTemplate) continue; // skip the template
            Destroy(child.gameObject);
        }
        foreach (var kvp in active)
        {
            InitVisual(kvp.Value);
        }
        foreach (var potion in potionItems)
        {
            InitVisual(potion.Item);
        }
    }

    void InitVisual(Item item)
    {
        if (item.itemSO == null) return;
        RectTransform itemTransform = Instantiate(activeItemTemplate, activeItemContainer).GetComponent<RectTransform>();
        itemTransform.gameObject.SetActive(true);

        // the template is image, so we need to set the icon
        UnityEngine.UI.Image image = itemTransform.GetComponent<UnityEngine.UI.Image>();
        if (item.itemSO.icon != null)
        {
            image.sprite = item.itemSO.icon;
        }
    }

    public Item GetActiveItem(ItemType slot)
    {
        return active[slot];
    }

}
