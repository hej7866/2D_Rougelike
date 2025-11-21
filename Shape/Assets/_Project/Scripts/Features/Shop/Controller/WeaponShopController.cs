using UnityEngine;

public class WeaponShopController : Shop
{
    [Header("Model")]
    [SerializeField] private WeaponShop _weaponShop;

    [Header("View")]
    [SerializeField] private ShopView _shopView;

    public override void Interact()
    {
        if(isInteract)
        {
            if(_shopView.weaponShopUI.activeSelf)
            {
                _shopView.weaponShopUI.SetActive(false);
            }
            else
            {
                _shopView.weaponShopUI.SetActive(true);
            }
        }
    }
    
}
