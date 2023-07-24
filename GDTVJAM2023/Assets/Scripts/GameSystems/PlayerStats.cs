using System.Collections;
using System.Collections.Generic;
using System;

public enum Slot
{
    Head,
    Chest,
    Legs,
    Feet,
    Hands,
    Neck,
    Wrist,
    Ammo,
    WeaponLeft,
    Shield,
    TwoHandedWeapon
}

[System.Serializable]
public class Equipment : Item
{
    public Slot EquippedSlot;
}

[System.Serializable]
public class Item
{
    public string Name = "Cool Rock";
    public bool IsConsumable = false;
    public int Value = 10;
    public int Quantity = 1;
}

[System.Serializable]
public class Armor : Equipment
{
    public int Defense = 3;
    public int FireResist = 0;
    public int PoisonResist = 0;
    public int ElectricResist = 0;
    public int WaterResist = 0;
}


[Serializable]
public class PlayerStats
{
    public int Health = 100;
    public string Name = "player";
    public float BaseAttackSpeed = 1;

    public Dictionary<Slot, Equipment> Equipment = new Dictionary<Slot, Equipment>()
    {
        {   Slot.Head, new Armor()
            {
                Name = "Tiara",
                Value = 5,
                Defense = 1
        } }
    };

    public List<Item> Inventory = new List<Item>()
    {
        new Item()
        {
            Value = 1,
            IsConsumable = false,
            Name = "Gold Pieces",
            Quantity = 10
        }
    };
}
