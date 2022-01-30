using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggieAnimStates : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private Rigidbody2D _rigidbody;

    private bool _isAirborne = false;
    private bool _isMoving = false;

    private bool _facingRight = true;

    // Start is called before the first frame update
    void Start()
    {

    }

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
        if (_rigidbody.velocity.x > distTolerance)
        {
            if (!_facingRight)
            {
                transform.RotateAround(transform.position, transform.up, 180.0f);
                _facingRight = true;
            }
        }
        else if (_rigidbody.velocity.x < -distTolerance)
        {
            if (_facingRight)
            {
                transform.RotateAround(transform.position, transform.up, 180.0f);
                _facingRight = false;
            }
        }


        //adjust animation parameters
        _isAirborne = !isGrounded;
        _isMoving = Mathf.Abs(_rigidbody.velocity.x) > speedTolerance;
    }
}
