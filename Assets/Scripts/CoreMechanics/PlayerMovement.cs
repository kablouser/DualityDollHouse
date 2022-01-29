using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D _rigidbody2D;

    [Header("Variables")]
    [SerializeField] private float _walkTopSpeed;

    [Tooltip("How fast player grinds to a halt horizontally when on the ground")]
    [SerializeField] private float _walkAcceleration;
    [Tooltip("How fast player grinds to a halt horizontally when on the ground")]
    [SerializeField] private float _walkDeceleration;

    [SerializeField] private float _jumpHeight;

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
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
            CalculateJumpVelocity();
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
            }
        }
    }

    private void FixedUpdate()
    {
        UpdateIsGrounded();

        if (_isGrounded)
        {
            // allow horizontal movement

            float horizontalInput = Input.GetAxis("Horizontal");
            float desiredVelocityX = horizontalInput * _walkTopSpeed;
            float velocityX = _rigidbody2D.velocity.x;

            // are we already moving faster than our desiredVelocityX?
            bool isFasterThanDesired =
                // if signs of desiredVelocityX velocityX are same
                Mathf.Sign(desiredVelocityX) == Mathf.Sign(velocityX) &&
                Mathf.Abs(desiredVelocityX) < Mathf.Abs(velocityX);

            if (isFasterThanDesired || Mathf.Approximately(desiredVelocityX, 0))
            {
                // brake

                // calculation always moves velocityX towards the direction of 0
                float decceleratedVelocityX = velocityX - Mathf.Sign(velocityX) * _walkDeceleration * Time.fixedDeltaTime;
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
                float acceleratedVelocityX = velocityX + Mathf.Sign(desiredVelocityX) * _walkAcceleration * Time.fixedDeltaTime;
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
}
