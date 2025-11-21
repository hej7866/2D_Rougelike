using UnityEngine;

public class AAShopController : Shop
{
    [Header("Model")]
    [SerializeField] private AAShop _aaShop;

    [Header("View")]
    [SerializeField] private ShopView _shopView;

    public override void Interact()
    {
        if(isInteract)
        {
            _shopView.aaShopUI.SetActive(true);
        }
    }
    
}
