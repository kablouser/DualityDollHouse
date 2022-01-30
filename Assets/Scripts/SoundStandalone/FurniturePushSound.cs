using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurniturePushSound : MonoBehaviour
{
    [SerializeField] private AudioSource _pushSource;
    [SerializeField] private Rigidbody2D _rigidbody;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Approximately(_rigidbody.velocity.x, .0f) && _pushSource.isActiveAndEnabled && !_pushSource.isPlaying)
        {
            _pushSource.Play();
        }

    }
}
