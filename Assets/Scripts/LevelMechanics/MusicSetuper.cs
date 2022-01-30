using UnityEngine;

public class MusicSetuper : MonoBehaviour
{
    [SerializeField] private AudioSource _musicPlayer;
    [SerializeField] private SceneSingletons _sceneSingletons;
    private void Awake()
    {
        _sceneSingletons.MusicPlayer = _musicPlayer;
    }
}
