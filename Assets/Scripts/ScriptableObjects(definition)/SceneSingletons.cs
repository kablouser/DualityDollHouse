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
    [Tooltip("Usually 4")]
    public int MusicBars;

    public Room CurrentRoom
    { 
        get => _currentRoom;
        set
        {            
            if(_currentRoom != null)
                _currentRoom.SetIsPlayerInRoom(false);
            value.SetIsPlayerInRoom(true);
            _currentRoom = value;
        }
    }

    public delegate void LightSourceEvent(GameObject lightSource);
    public event LightSourceEvent LightSourceDetectedEvent;
    public event LightSourceEvent LightSourceUndetectedEvent;

    public delegate void KeyEvent(bool isCarryingKey);
    public event KeyEvent OnKeyChanged;

    private Room _currentRoom;

    public void InvokeLightSourceDetectedEvent(GameObject lightSource)
    {
        LightSourceDetectedEvent?.Invoke(lightSource);
    }

    public void InvokeLightSourceUndetectedEvent(GameObject lightSource)
    {
        LightSourceUndetectedEvent?.Invoke(lightSource);
    }

    public void InvokeOnKeyChanged(bool isCarryingKey)
    {
        OnKeyChanged?.Invoke(isCarryingKey);
    }

    /// <summary>
    /// Get bar number of the music playing.
    /// So 4 bar music would return [0,1,2,3] and repeat.
    /// </summary>
    /// <returns>range [0,MusicBars-1]</returns>
    public int GetMusicBar()
    {
        #if UNITY_EDITOR
        if(MusicBPM == 0)
        {
            Debug.LogWarning("MusicBPM is 0 in sceneSingletons. That will not sync properly...");
        }
        if(MusicBars == 0)
        {
            Debug.LogWarning("MusicBars is 0 in sceneSingletons. That will not sync properly...");
        }
        #endif

        int beat = Mathf.FloorToInt(
            MusicPlayer.time *
            // convert BPM to BPS (beats per second)
            MusicBPM / 60.0f);
        return beat % Mathf.Max(1,MusicBars);
    }
}
