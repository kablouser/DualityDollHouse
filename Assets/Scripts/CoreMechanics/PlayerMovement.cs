using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public bool IsFrozen { get; private set; }

    public bool IsCarryingKey
    {
        get => _isCarryingKey;
        set
        {
            _isCarryingKey = value;
            sceneSingletons.InvokeOnKeyChanged(value);
        }
    }

    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private SceneSingletons sceneSingletons;
    [SerializeField] private Animator _animator;

    [Header("Variables")]
    [SerializeField] private float _walkTopSpeed;

    [Tooltip("How fast player speeds up on the ground to _walkTopSpeed")]
    [SerializeField] private float _walkAcceleration;
    [Tooltip("How player grinds to a halt horizontally when on the ground")]
    [SerializeField] private float _walkDeceleration;

    [Tooltip("Horizontal acceleration whilst air-bourne")]
    [SerializeField] private float _midairAcceleration;

    [SerializeField] private float _jumpHeight;

    [Tooltip("The time needed with the player stopped to start freezing")]
    [SerializeField] private float _timeToFreeze = 2.0f;

    /// <summary>
    /// contacts on rigidbody2D, gotten through rigidbody2D.GetContacts
    /// </summary>
    private List<ContactPoint2D> _contacts;
    private bool _isGrounded;

    /// <summary>
    /// precalculated cache value
    /// </summary>
    private float _jumpVelocity;

    private bool isVerticalPressed;

    public bool _isCarryingKey;

    /// <summary>
    /// the beginning time at the latest period when stopped,
    /// if IsMoving() is true, this is negative
    /// </summary>
    private float _stoppedMovingTime;

    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _walkSound;
    [SerializeField] private AudioClip _jumpSound;

    public void SetGrabbed(bool isGrabbed)
    {
        _rigidbody2D.isKinematic = isGrabbed;
        _rigidbody2D.velocity = Vector2.zero;
        _collider2D.enabled = !isGrabbed;
        _animator.enabled = !isGrabbed;
        IsCarryingKey = false;
        IsFrozen = true;
        _stoppedMovingTime = Time.time;
    }

    public bool IsMoving()
    {
        Vector2 velocity = _rigidbody2D.velocity;
        return !Mathf.Approximately(velocity.x, 0.0f) || !Mathf.Approximately(velocity.y, 0.0f);
    }

    private void CalculateJumpVelocity()
    {
        float G = Physics2D.gravity.y;
        if (G < 0)
            _jumpVelocity = Mathf.Sqrt(2 * -G * _jumpHeight);
        else
        {
            Debug.LogError("CalculateJumpVelocity cannot use upwards gravity", this);
            _jumpVelocity = 0;
        }
    }

    private void UpdateIsGrounded()
    {
        _rigidbody2D.GetContacts(_contacts);
        _isGrounded = false;
        foreach (ContactPoint2D point in _contacts)
        {
            // normal.y is only positive when surface is under player
            // so that surface must be ground
            if (1e-5f < point.normal.y)
            {
                _isGrounded = true;
                break;
            }
        }
    }

    private void Awake()
    {
        _contacts = new List<ContactPoint2D>(4);
        _isGrounded = false;
        isVerticalPressed = false;
        CalculateJumpVelocity();

        sceneSingletons.MainCamera = Camera.main;
        sceneSingletons.PlayerMovement = this;
        IsCarryingKey = false;
        IsFrozen = true;
        _stoppedMovingTime = Time.time;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            CalculateJumpVelocity();
    }

    private void OnEnable()
    {
        sceneSingletons.LightSourceDetectedEvent += LightSourceDetectedEvent;
    }

    private void OnDisable()
    {
        sceneSingletons.LightSourceDetectedEvent -= LightSourceDetectedEvent;
        // OnDisable gets called when travelling to new level
        IsCarryingKey = false;
    }

    private void Update()
    {
        float verticalInput = Input.GetAxisRaw("Vertical");
        bool getVerticalDown = !isVerticalPressed && 0 < verticalInput;
        if (0 < verticalInput)
            isVerticalPressed = true;
        else
            isVerticalPressed = false;

        // is jump possible?
        if (_isGrounded)
        {
            // jump button has same effect as up direction
            if (Input.GetButtonDown("Jump") || getVerticalDown)
            {
                // stop further jump inputs, because Update could be faster than FixedUpdate
                _isGrounded = false;
                float velocityY = _rigidbody2D.velocity.y;
                float impulseY = _jumpVelocity - velocityY;
                _rigidbody2D.AddForce(new Vector2(0, impulseY), ForceMode2D.Impulse);

                //play jump sound
                _audioSource.clip = _jumpSound;
                _audioSource.Play();
            }
            else if (!Mathf.Approximately(_rigidbody2D.velocity.x, .0f) && !_audioSource.isPlaying)
            {
                _audioSource.clip = _walkSound;
                _audioSource.Play();
            }
        }
    }

    // TODO
    public bool IS_FROZEN;

    private void FixedUpdate()
    {
        UpdateIsGrounded();

        if (_isGrounded)
            MoveHorizontally(_walkAcceleration, _walkDeceleration);
        else
            MoveHorizontally(_midairAcceleration, 0);

        if (IsMoving())
        {
            IsFrozen = false;
            _stoppedMovingTime = -1;
        }
        else if (IsFrozen == false)
        {
            if (_stoppedMovingTime < 0)
            {
                // first frame of stopping
                _stoppedMovingTime = Time.time;
            }
            else if (_stoppedMovingTime + _timeToFreeze < Time.time)
            {
                // sufficient time has passed
                IsFrozen = true;
            }
        }

        IS_FROZEN = IsFrozen;
    }

    private void MoveHorizontally(float acceleration, float decceleration)
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float desiredVelocityX = horizontalInput * _walkTopSpeed;
        float velocityX = _rigidbody2D.velocity.x;

        // allow horizontal movement
        // are we already moving faster than our desiredVelocityX?
        bool isFasterThanDesired =
            // if signs of desiredVelocityX velocityX are same
            Mathf.Sign(desiredVelocityX) == Mathf.Sign(velocityX) &&
            Mathf.Abs(desiredVelocityX) < Mathf.Abs(velocityX);

        if (isFasterThanDesired || Mathf.Approximately(desiredVelocityX, 0))
        {
            // brake

            // calculation always moves velocityX towards the direction of 0
            float decceleratedVelocityX = velocityX - Mathf.Sign(velocityX) * decceleration * Time.fixedDeltaTime;
            // this could overshoot

            // check overshoot, if signs have changed
            if (Mathf.Sign(decceleratedVelocityX) != Mathf.Sign(velocityX))
                decceleratedVelocityX = 0;

            float impulseX = decceleratedVelocityX - velocityX;
            _rigidbody2D.AddForce(new Vector2(impulseX, 0), ForceMode2D.Impulse);
        }
        else
        {
            // walk

            // calculation always moves velocityX towards the direction of desiredVelocityX
            float acceleratedVelocityX = velocityX + Mathf.Sign(desiredVelocityX) * acceleration * Time.fixedDeltaTime;
            // this could overshoot

            // check if overshooting, if signs are the same
            if (Mathf.Sign(desiredVelocityX) == Mathf.Sign(acceleratedVelocityX) &&
                // absolute has gone beyond
                Mathf.Abs(desiredVelocityX) < Mathf.Abs(acceleratedVelocityX))
                acceleratedVelocityX = desiredVelocityX;

            float impulseX = acceleratedVelocityX - velocityX;
            _rigidbody2D.AddForce(new Vector2(impulseX, 0), ForceMode2D.Impulse);
        }
    }

    // drawing contact points and contact normals as rays
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    if (_contacts != null)
    //        foreach (var x in _contacts)
    //        {
    //            Gizmos.DrawRay(x.point, x.normal * 0.9f);
    //        }
    //}

    private void LightSourceDetectedEvent(GameObject _)
    {
        if (IsFrozen)
            // if in Freeze frame skip detection
            return;

        if (sceneSingletons.CurrentRoom)
            sceneSingletons.CurrentRoom.RestartRoom();
        else
            Debug.LogError("What have I detected a light when there's no current room?", this);
    }
}
