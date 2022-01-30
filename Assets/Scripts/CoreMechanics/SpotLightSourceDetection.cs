using UnityEngine;
using PathCreation;

public class SpotLightSourceDetection : MonoBehaviour
{
    [SerializeField] private SceneSingletons _sceneSingletons;
    [SerializeField] private PathCreator _pathCreator;

    [SerializeField] private float _pathingSpeed;
    [SerializeField] private float _pathBeginOffset;

    private void OnValidate()
    {
        transform.position = _pathCreator.path.GetPointAtDistance(_pathBeginOffset);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.attachedRigidbody.GetComponent<PlayerMovement>())
        {
            _sceneSingletons.InvokeLightSourceDetectedEvent(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.GetComponent<PlayerMovement>())
        {
            _sceneSingletons.InvokeLightSourceUndetectedEvent(gameObject);
        }
    }

    private void Update()
    {
        transform.position = _pathCreator.path.GetPointAtDistance(_pathBeginOffset + Time.time * _pathingSpeed);
    }
}
