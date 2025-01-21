using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if(item is Weapon)
            {
                Weapon w = item as Weapon;
            }
            else
            {
                Passive p = item as Passive;
            }
        }
        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty() { return item == null; }
    }
    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);
    public UiInventoryIconDisplay weaponUi, passiveUi;

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();
    public List<PassiveData> availablePassives = new List<PassiveData>();

    public UiUpgradeWindown upgradeWindown;

    PlayerStats player;

    private void Start()
    {
        player = GetComponent<PlayerStats>();
    }
    public bool Has(ItemData type) { return Get(type); }
    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }
    public Passive Get(PassiveData type)
    {
        foreach(Slot s in passiveSlots)
        {
            Passive p = s. item as Passive;
            if(p && p.data == type)
                return p;
        }
        return null;
    }
    public Weapon Get(WeaponData type)
    {
        foreach(Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if(w && w.data == type) return w;
        }
        return null;
    }
    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        if( removeUpgradeAvailability)availableWeapons.Remove(data);
        for(int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if(w.data = data)
            {
                //weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }
        return false;
    }
    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        if(removeUpgradeAvailability) availablePassives.Remove(data);
        for (int i = 0; i < passiveSlots.Count; i++)
        {
            Passive p = passiveSlots[i].item as Passive;
            if (p.data = data)
            {
                //weaponSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }
        return false;
    }
    public bool Remove(ItemData data,bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData)  return Remove(data as PassiveData,removeUpgradeAvailability);
        if (data is WeaponData) return Remove(data as WeaponData,removeUpgradeAvailability);
        return false;
    }
    public int Add(WeaponData data)
    {
        int slotNum = -1;
        for(int i = 0;i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }
        if(slotNum < 0) return slotNum;
        Type weaponType = Type.GetType(data.behaviour);

        if(weaponType != null)
        {
            GameObject go = new GameObject(data.baseStats.name + "Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.transform.SetParent(transform);
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.Initialise(data);
            spawnedWeapon.OnEquip();

            weaponSlots[slotNum].Assign(spawnedWeapon);
            weaponUi.Refresh();

            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
                GameManager.instance.EndLevelUp();
            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format("Invalid weapon type specified for {0}.", data.name));
        }
        return -1;
    }
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        for(int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }
        if(slotNum < 0)return slotNum;
        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform);
        p.transform.localPosition = Vector2.zero;

        passiveSlots[slotNum].Assign(p);
        passiveUi.Refresh();

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();
        return slotNum;
    }
    public int Add(ItemData data)
    {
        if(data is WeaponData) return Add(data as WeaponData);
        else if(data is PassiveData) return Add(data as PassiveData);
        return -1;
    }
    public bool LevelUp(ItemData data)
    {
        Item item = Get(data);
        if(item) return LevelUp(item);
        return false;
    }
    public bool LevelUp(Item item)
    {
        if (!item.DoLevelUp())
        {
            Debug.LogWarning(string.Format("Failed to level up {0}", item.name));
            return false;
        }
        weaponUi.Refresh();
        passiveUi.Refresh();
        if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        if(item is Weapon) player.RecalculateStats();
        if(item is Passive) player.RecalculateStats();
        return true;
    }
    int GetSlotsLeft(List<Slot> slots)
    {
        int count = 0;
        foreach(Slot s in slots)
        {
            if(s.IsEmpty()) count++;
        }
        return count;
    }

    void ApplyUpgradeOptions()
    {
        List<ItemData> availabUpgrades = new List<ItemData>();
        List<ItemData> allUpgrades = new List<ItemData>(availabUpgrades);
        allUpgrades.AddRange(availableWeapons);
        allUpgrades.AddRange(availablePassives);

        int weaponSlotsLeft = GetSlotsLeft(weaponSlots);
        int passiveSlotsLeft = GetSlotsLeft(passiveSlots);

        foreach(ItemData data in allUpgrades)
        {
            Item obj = Get(data);

            if (obj)
            {
                if(obj.currentLevel < data.maxLevel) availabUpgrades.Add(data);
            }
            else
            {
                if (data is WeaponData && weaponSlotsLeft > 0) availabUpgrades.Add(data);
                else if(data is PassiveData && passiveSlotsLeft > 0) availabUpgrades.Add(data);
            }
        }
        int availUpgradeCount = availabUpgrades.Count;
        if(availUpgradeCount > 0)
        {
            bool getEctraItem = 1f - 1f /player.Stats.luck > UnityEngine.Random.value;
            if (getEctraItem || availabUpgrades.Count < 4) upgradeWindown.SetUpgrades(this, availabUpgrades, 4);
            else upgradeWindown.SetUpgrades(this, availabUpgrades, 3, "Increase your Luck Stats for a chace to get 4 items!");
        }
        else if(GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        
    }

    public void RemoveAndApplyUpgrades()
    {
        ApplyUpgradeOptions();
    }
}
