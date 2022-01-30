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

    [Tooltip("Speed for hand to move player when restarting room")]
    [Range(0.001f, 10.0f)]
    [SerializeField] private float _restartSpeed = 1;
    [SerializeField] private float _cameraFrameDuration = 1;

    [SerializeField] private HandAnimationHandler _handObject;

    [SerializeField] private Transform[] _resetPositionsOnRestart;
    [HideInInspector] [SerializeField] private Vector3[] _resetPositionsMemory;

    [SerializeField] private GameObject[] _setActivesOnRestart;

    [SerializeField] private GameObject[] _setActivesWhileInRoom;

    private void Start()
    {
        OnValidate();

        // set memory to start positions
        for (int i = 0; i < _resetPositionsOnRestart.Length; i++)
            _resetPositionsMemory[i] = _resetPositionsOnRestart[i].position;

        if (_isFirstRoom)
        {
            sceneSingletons.CurrentRoom = this;
            sceneSingletons.PlayerMovement.transform.position = (Vector2)transform.position + EntrancePosition;
            BeginFramingCamera(0);
        }
        else
            SetIsPlayerInRoom(false);
    }

    public void OnValidate()
    {
        if (_entranceRenderer != null)
            _entranceRenderer.transform.position = transform.position + (Vector3)EntrancePosition;
        // preallocate memory before start
        if (_resetPositionsOnRestart != null &&
            (_resetPositionsMemory == null ||
            _resetPositionsMemory.Length != _resetPositionsOnRestart.Length))
            _resetPositionsMemory = new Vector3[_resetPositionsOnRestart.Length];
    }

    public void BeginMovingTowardsEntrance(Transform target, System.Action onReached = null)
    {
        Vector2 endPosition = (Vector2)transform.position + EntrancePosition;
        // duration for moving animation part
        float duration = Vector2.Distance(target.position, endPosition) / _restartSpeed;

        StartCoroutine(SmoothedMoveRoutine(target, endPosition, duration, onReached));
    }

    public void BeginFramingCamera(float duration, System.Action onReached = null)
    {
        // change to this room
        sceneSingletons.CurrentRoom = this;

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
            CleanRoom();
        });
    }

    public void CleanRoom()
    {
        // reset positions
        for (int i = 0; i < _resetPositionsOnRestart.Length; i++)
            _resetPositionsOnRestart[i].position = _resetPositionsMemory[i];

        foreach (GameObject setActive in _setActivesOnRestart)
            setActive.SetActive(true);
    }

    public void SetIsPlayerInRoom(bool isPlayerInRoom)
    {
        foreach(GameObject target in _setActivesWhileInRoom)
            target.SetActive(isPlayerInRoom);
    }

    private IEnumerator SmoothedMoveRoutine(Transform moveTarget, Vector2 endPosition, float duration, System.Action onReached = null)
    {
        Vector3 startPosition = moveTarget.position;

        bool handAnimated = (moveTarget == sceneSingletons.PlayerMovement.transform);

        if (handAnimated)
        {
            _handObject.SetVisible(true);
            _handObject.transform.position = new Vector3(startPosition.x + 1.1f,
              startPosition.y - 1.2f, -.81f);

            _handObject.PlayAnimSynchronous("PickUp");
            _handObject.PlaySoundGrab();

            while (!_handObject.animFinished)
            {
                yield return null;
            }
        }

        // current time (at this line of code, its start of move to endPosition animation)
        float time = Time.time;
        // end of movement time
        float timeEnd = time + duration;
        AnimationCurve curve = AnimationCurve.EaseInOut(time, 0, timeEnd, 1);

        while (time < timeEnd)
        {
            // value goes from 0 to 1
            float value = curve.Evaluate(time);
            moveTarget.position = Vector3.Lerp(startPosition, endPosition, value);
            if (_handObject != null)
            {
                _handObject.transform.position = new Vector3(moveTarget.position.x + 1.1f,
                    moveTarget.position.y - 1.2f, _handObject.transform.position.z);
            }

            yield return null;
            time = Time.time;
        }

        _handObject.PlayAnimSynchronous("PutDown");
        _handObject.PlaySoundDrop();


        moveTarget.position = endPosition;
        onReached?.Invoke();

        while (!_handObject.animFinished)
        {
            yield return null;
        }
        _handObject.SetVisible(false);
    }
}
