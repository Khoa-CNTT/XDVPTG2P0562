using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    public List<ItemStack> items = new List<ItemStack>();

    public void AddItem(ItemData item, int amount = 1)
    {
        if (item.isStackable)
        {
            var existing = items.Find(i => i.item.itemName == item.itemName);
            if (existing != null)
            {
                existing.quantity += amount;
                return;
            }
        }
        items.Add(new ItemStack(item, amount));
    }

    public void RemoveItem(ItemData item, int amount = 1)
    {
        var stack = items.Find(i => i.item.itemName == item.itemName);
        if (stack != null)
        {
            stack.quantity -= amount;
            if (stack.quantity <= 0) items.Remove(stack);
        }
    }
}
