using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraWeapon : Weapon
{
    protected Aura currentAura;

    public override void OnEquip()
    {
        if (currentStats.auraPrefab)
        {
            if(currentAura) Destroy(currentAura);
            currentAura = Instantiate(currentStats.auraPrefab,transform);
            currentAura.weapon = this;
            currentAura.owner = owner;

            float area = GetArea();
            currentAura.transform.localScale = new Vector3(area, area, area);
        }
    }
    public override void OnUnequip()
    {
        if (currentAura) Destroy(currentAura);
    }
    public override bool DoLevelUp()
    {
        if(!base.DoLevelUp()) return false;
        if (currentAura)
        {
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area);
        }
        return true;
    }
}
