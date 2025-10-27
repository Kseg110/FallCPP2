using UnityEngine;

public class Pickups : MonoBehaviour
{
    public enum PickupType
    {
        Life = 0,
        Score = 1,
        Powerup = 2
    }

    public PickupType pickupType = PickupType.Life; // Type of the pickup

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {


            switch (pickupType)
            {
                case PickupType.Life:
                    GameManager.Instance.lives++;
                    //Debug.Log("Life collected! Current lives: " + pc.lives);
                    break;
                case PickupType.Score:
                    GameManager.Instance.score++;
                    Debug.Log("Score collected! Current score: " + GameManager.Instance.score);
                    break;
                case PickupType.Powerup:
                    PlayerController pc = collision.GetComponent<PlayerController>();
                    //pc.ActivateJumpForceChange();
                    break;
            }

            Destroy(gameObject); // Destroy the pickup after collection
        }
    }
}