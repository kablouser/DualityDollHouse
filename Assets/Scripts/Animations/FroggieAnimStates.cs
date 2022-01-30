using System.Collections.Generic;
using UnityEngine;

public class FroggieAnimStates : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private PlayerMovement _playerMovement;

    [Tooltip("Base multiplier for how fast you want the animation to play, gets faster as player moves faster")]
    [SerializeField] private float _movingBaseMultiplier = 1f;

    [Tooltip("Base multiplier angle to rotate the jump animation by, its scales with velocity.x")]
    [SerializeField] private float _jumpAngleBaseMultiplier = 6;

    private bool _isAirborne = false;
    private bool _isMoving = false;

    private bool _facingRight = true;

    // Update is called once per frame
    void Update()
    {
        _anim.SetBool("Airborne", _isAirborne);
        _anim.SetBool("Moving", _isMoving);
    }

    void FixedUpdate()
    {
        const float distTolerance = 1e-5f;
        const float speedTolerance = .3f;
        List<ContactPoint2D> contacts = new List<ContactPoint2D>();
        _rigidbody.GetContacts(contacts);

        //check ground
        bool isGrounded = false;
        foreach (ContactPoint2D point in contacts)
        {
            if (point.normal.y > distTolerance)
            {
                isGrounded = true;
                break;
            }
        }

        //set orientation
        float velocityX = _rigidbody.velocity.x;
        if (velocityX > distTolerance)
        {
            if (!_facingRight)
            {
                transform.RotateAround(transform.position, transform.up, 180.0f);
                _facingRight = true;
            }
        }
        else if (velocityX < -distTolerance)
        {
            if (_facingRight)
            {
                transform.RotateAround(transform.position, transform.up, 180.0f);
                _facingRight = false;
            }
        }

        //adjust animation parameters
        _isAirborne = !isGrounded;
        _isMoving = _playerMovement.IsFrozen == false;
        _anim.SetFloat("MovingMultiplier",
            // 0 multiplier causes transition issues
            Mathf.Max(0.1f, Mathf.Abs(velocityX)) *
            _movingBaseMultiplier);

        if (_isAirborne)
        {
            const float MAX_ANGLE = -30.0f;
            float clampedAngle = Mathf.Max(MAX_ANGLE, -Mathf.Abs(velocityX) * _jumpAngleBaseMultiplier);
            _anim.transform.localRotation = Quaternion.Euler(0, 0, clampedAngle);
        }
        else
            _anim.transform.localRotation = Quaternion.identity;
    }
}
