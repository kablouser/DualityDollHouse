using UnityEngine;

/// <summary>
/// Script of single instances of things inside a scene
/// </summary>
[CreateAssetMenu(fileName = "SceneSingletons", menuName = "ScriptableObjects/SceneSingletons")]
public class SceneSingletons : ScriptableObject
{
    public Camera MainCamera;
    public PlayerMovement PlayerMovement;
    public AudioSource musicPlayer;
    public float musicBPM;
}
