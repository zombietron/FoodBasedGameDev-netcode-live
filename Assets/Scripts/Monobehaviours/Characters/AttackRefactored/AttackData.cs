using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackData : MonoBehaviour
{
    [SerializeField]
    List<ProjectileInfo> projectiles;
    
    public List<ProjectileInfo> PROJECTILES
    {
        get { return projectiles; }
    }

}

[System.Serializable]
public class ProjectileInfo
{
    public GameObject projectilePrefab;
    public int ammoAmount;
    public int maxAmmoAmount;
    public Sprite icon;
    public void RefillAmmo()
    {
        ammoAmount = maxAmmoAmount;
    }

}
