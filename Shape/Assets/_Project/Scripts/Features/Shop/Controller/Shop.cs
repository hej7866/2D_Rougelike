using UnityEngine;

public abstract class Shop : MonoBehaviour
{
    public bool isInteract = false;
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            isInteract = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isInteract = false;
    }
    
    public abstract void Interact();
}
