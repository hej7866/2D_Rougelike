using UnityEngine;

public class SkillShopController : Shop
{
    [Header("Model")]
    [SerializeField] private SkillShop _skillShop;

    [Header("View")]
    [SerializeField] private ShopView _shopView;

    public override void Interact()
    {
        if(isInteract)
        {
            _shopView.skillShopUI.SetActive(true);
        }
    }
    
}
