using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public float PositionX;
    public float PositionY;
    public int CurrentHP;
    public int PotionCount;
}


[Serializable]
public class InventoryItemData
{
    public string ItemID;
    public int Quantity;
}
