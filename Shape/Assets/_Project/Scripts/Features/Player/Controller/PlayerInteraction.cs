using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Shop _shop;
    void Update()
    {
        if(_shop != null && _shop.isInteract)
        {
            if(Input.GetKeyDown(KeyCode.E)) _shop.Interact();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Shop"))
        {
            _shop = collision.collider.GetComponent<Shop>();
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Shop"))
        {
            _shop = null;
        }
    }
}
