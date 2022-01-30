using UnityEngine;

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.attachedRigidbody.GetComponent<PlayerMovement>();
        // check not already carrying another key
        if (player && player.IsCarryingKey == false)
        {
            player.IsCarryingKey = true;
            gameObject.SetActive(false);
        }
    }
}
