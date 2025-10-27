using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
    //Defined in child classes, this method is called when the player picks up the item
    abstract public void OnPickup();

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            OnPickup();
            Destroy(gameObject); // Destroy the pickup after it has been collected
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnPickup();
            Destroy(gameObject); // Destroy the pickup after it has been collected
        }
    }
}