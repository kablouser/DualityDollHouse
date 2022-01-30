using UnityEngine;

public class RoomExit : MonoBehaviour
{
    [SerializeField] private Room nextRoom;
    [SerializeField] private bool isKeyRequired;

#if UNITY_EDITOR
    private void Awake()
    {
        if(nextRoom == null)
            Debug.LogWarning("RoomExit's nextRoom is not setup!", this);
    }
    #endif

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.attachedRigidbody.GetComponent<PlayerMovement>();
        if(player && nextRoom)
        {
            // check key if needed
            if (isKeyRequired && player.IsCarryingKey == false)
                return;

            // make player "disappear"
            player.gameObject.SetActive(false);
            nextRoom.BeginFramingCamera(1.0f,
            // onReached
            () =>
            {
                player.transform.position = (Vector2)nextRoom.transform.position + nextRoom.EntrancePosition;
                player.gameObject.SetActive(true);
            });
        }
    }
}
