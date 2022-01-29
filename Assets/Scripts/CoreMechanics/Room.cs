using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour
{
    /// <summary>
    /// Local position to this room object
    /// </summary>
    public Vector2 EntrancePosition = Vector2.left;
    public Vector2 Size = Vector2.one;
    public Vector2 Center;

    [SerializeField] private Transform _entranceRenderer;
    [SerializeField] private SceneSingletons sceneSingletons;
    [SerializeField] private bool _isFirstRoom;

    [Tooltip("Time taken to restart room")]
    [SerializeField] private float _restartDuration = 1;
    [SerializeField] private float _cameraFrameDuration = 1;

    private void Start()
    {
        if (_isFirstRoom)
        {
            sceneSingletons.PlayerMovement.transform.position = (Vector2)transform.position + EntrancePosition;
            BeginFramingCamera(0);            
        }
    }

    public void OnValidate()
    {
        if(_entranceRenderer != null)
            _entranceRenderer.transform.position = transform.position + (Vector3)EntrancePosition;
    }

    public void BeginMovingTowardsEntrance(Transform target, System.Action onReached = null)
    {
        StartCoroutine(SmoothedMoveRoutine(target, (Vector2)transform.position + EntrancePosition, _restartDuration, onReached));
    }

    public void BeginFramingCamera(float duration, System.Action onReached = null)
    {
        Transform camera = sceneSingletons.MainCamera.transform;

        // frame camera around center
        Vector3 cameraNewPosition = transform.position + (Vector3)Center;        
        cameraNewPosition.z = camera.transform.position.z;
        StartCoroutine(SmoothedMoveRoutine(camera.transform, cameraNewPosition, duration, onReached));
    }

    [ContextMenu("Restart Room")]
    public void RestartRoom()
    {
        sceneSingletons.PlayerMovement.SetGrabbed(true);
        BeginMovingTowardsEntrance(sceneSingletons.PlayerMovement.transform,
        () =>
        {
            sceneSingletons.PlayerMovement.SetGrabbed(false);
        });
    }

    private IEnumerator SmoothedMoveRoutine(Transform moveTarget, Vector2 endPosition, float duration, System.Action onReached = null)
    {
        // current time
        float time = Time.time;
        // end of movement time
        float timeEnd = time + duration;
        AnimationCurve curve = AnimationCurve.EaseInOut(time, 0, timeEnd, 1);
        Vector3 startPosition = moveTarget.position;

        while (time < timeEnd)
        {
            // value goes from 0 to 1
            float value = curve.Evaluate(time);
            moveTarget.position = Vector3.Lerp(startPosition, endPosition, value);

            yield return null;
            time = Time.time;
        }

        moveTarget.position = endPosition;
        onReached?.Invoke();
    }
}