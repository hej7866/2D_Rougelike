using UnityEngine;
using System;
using System.Collections;

public abstract class Weapon : ScriptableObject
{
    public abstract IEnumerator WeaponController(GameObject player);
    public abstract void InitWeapon(GameObject player);
}


