using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightSourceDetection : MonoBehaviour
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private float angle = 60.0f;
    [SerializeField] private Light2D lightObject;
    [SerializeField] private SceneSingletons _sceneSingletons;

    private bool detected = false;
    // Start is called before the first frame update
    void Start()
    {
        lightObject.pointLightOuterAngle = angle;
    }

    // Update is called once per frame
    void Update()
    {
        Bounds playerBounds = playerCollider.bounds;
        Vector3[] bboxCorners = {
            new Vector3(playerBounds.extents.x, playerBounds.extents.y),
            new Vector3(playerBounds.extents.x, -playerBounds.extents.y),
            new Vector3(-playerBounds.extents.x, playerBounds.extents.y),
            new Vector3(-playerBounds.extents.x, -playerBounds.extents.y),
        };

        for (int i = 0; i < 4; i++)
        {
            Vector3 playerDirection = playerCollider.transform.position - transform.position - bboxCorners[i];
            if (Vector3.Angle(playerDirection, -transform.up) <= angle / 2)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection);
                if (hit.collider == playerCollider)
                {
                    // don't make red, red only for debug purposes
                    //hitReaction();
                    if (!detected)
                    {
                        //change status to detected and broadcast event
                        detected = true;
                        _sceneSingletons.InvokeLightSourceDetectedEvent(gameObject);
                    }
                    return;
                }
            }
        }
        if (detected)
        {
            //change status to undetected
            detected = false;
            _sceneSingletons.InvokeLightSourceUndetectedEvent(gameObject);
        }
        lightObject.color = Color.white;
    }

    private void hitReaction()
    {
        //change color to red for now
        lightObject.color = Color.red;
    }
}
