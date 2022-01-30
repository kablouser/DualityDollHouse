using UnityEngine;

/// <summary>
/// Script of single instances of things inside a scene
/// </summary>
[CreateAssetMenu(fileName = "SceneSingletons", menuName = "ScriptableObjects/SceneSingletons")]
public class SceneSingletons : ScriptableObject
{
    public Camera MainCamera;
    public PlayerMovement PlayerMovement;
    public AudioSource MusicPlayer;
    public float MusicBPM;
    public Room CurrentRoom;

    public delegate void LightSourceEvent(GameObject lightSource);
    public event LightSourceEvent LightSourceDetectedEvent;
    public event LightSourceEvent LightSourceUndetectedEvent;

    public void InvokeLightSourceDetectedEvent(GameObject lightSource)
    {
        LightSourceDetectedEvent?.Invoke(lightSource);
    }

    public void InvokeLightSourceUndetectedEvent(GameObject lightSource)
    {
        LightSourceUndetectedEvent?.Invoke(lightSource);
    }
}
