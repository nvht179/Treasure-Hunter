public abstract class ConsumableItemSO : ItemSO, IConsumable
{
    public int usesPerItem = 1;
    public abstract void Consume(Player player);
}