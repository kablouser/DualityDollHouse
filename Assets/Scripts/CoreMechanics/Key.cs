using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private SceneSingletons _sceneSingletons;


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
