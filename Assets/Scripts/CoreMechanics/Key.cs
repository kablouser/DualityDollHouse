using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private SceneSingletons _sceneSingletons;

    //audio stuff
    [SerializeField] private AudioClip _collectSound;
    [SerializeField] private AudioSource _audioSource;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement player = collision.attachedRigidbody.GetComponent<PlayerMovement>();
        // check not already carrying another key
        if (player && player.IsCarryingKey == false)
        {
            player.IsCarryingKey = true;

            _audioSource.clip = _collectSound;
            _audioSource.Play();

            gameObject.SetActive(false);
        }
    }
}
