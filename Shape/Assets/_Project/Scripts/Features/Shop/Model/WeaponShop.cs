using UnityEngine;

public class WeaponShop : MonoBehaviour
{
    public void UpgradeWeapon()
    {
        var weapon = PlayerManager.Instance.character.weaponInstance;

        if (weapon is RotateWeapon rotateWeapon)
        {
            // RotateWeapon 전용 강화 로직
            rotateWeapon.count += 1;
            rotateWeapon.InitWeapon(PlayerManager.Instance.player);
        }
    }

}
