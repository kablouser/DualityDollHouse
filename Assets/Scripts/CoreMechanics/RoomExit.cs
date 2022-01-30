using UnityEngine;

public class RoomExit : MonoBehaviour
{
    [SerializeField] private Room nextRoom;
    [SerializeField] private bool isKeyRequired;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _unlockedDoorSprite;
    [SerializeField] private Sprite _lockedDoorSprite;
    [SerializeField] private SceneSingletons _sceneSingletons;

    //audio stuff
    [SerializeField] private AudioClip _collectSound;
    [SerializeField] private AudioSource _audioSource;

#if UNITY_EDITOR
    private void Awake()
    {
        if (nextRoom == null)
            Debug.LogWarning("RoomExit's nextRoom is not setup!", this);
        SetLocked(isKeyRequired);
    }
#endif

    private void OnEnable()
    {
        _sceneSingletons.OnKeyChanged += OnKeyChanged;
    }

    private void OnDisable()
    {
        _sceneSingletons.OnKeyChanged -= OnKeyChanged;
    }

    private void OnKeyChanged(bool isCarryingKey)
    {
        SetLocked(isKeyRequired && isCarryingKey == false);

        if (isCarryingKey && _audioSource != null && _audioSource.enabled)
        {
            _audioSource.clip = _collectSound;
            _audioSource.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.attachedRigidbody.GetComponent<PlayerMovement>();
        if (player && nextRoom)
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
                nextRoom.CleanRoom();
            });
        }
    }

    public void SetLocked(bool isLocked)
    {
        _spriteRenderer.sprite = isLocked ? _lockedDoorSprite : _unlockedDoorSprite;
    }
}
