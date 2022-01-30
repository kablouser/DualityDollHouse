using UnityEngine;

public class LightPatterner : MonoBehaviour
{
    [System.Serializable]
    private struct PatternedLight
    {
        public GameObject light;
        public bool[] switchPattern;
    }

    [SerializeField] private SceneSingletons _sceneSingletons;
    [Tooltip("For room 2, left-most light has switchPattern [true,false,false,false]")]
    [SerializeField] private PatternedLight[] _patternedLights;

    /// <summary>
    /// Max pattern length of all _patternedLights's switchPattern
    /// </summary>
    private int _patternLength;

    private void Awake()
    {
        _patternLength = 0;
        foreach(PatternedLight patternedLight in _patternedLights)
            _patternLength = Mathf.Max(_patternLength, patternedLight.switchPattern.Length);
    }

    private void Update()
    {
#if UNITY_EDITOR
        if(_sceneSingletons.MusicBPM == 0)
        {
            Debug.LogWarning("Music BPM is set to 0 in sceneSingletons. That will not sync lights properly...");
        }
#endif

        // number of beats since music started
        int beatNumber = Mathf.FloorToInt(
            _sceneSingletons.MusicPlayer.time *
            // convert BPM to BPS (beats per second)
            _sceneSingletons.MusicBPM / 60.0f);

        // out of beat length
        int currentBeatID = beatNumber % _patternLength;
        foreach(PatternedLight patternedLight in _patternedLights)
        {
            if (currentBeatID < patternedLight.switchPattern.Length)
                // set to pattern
                patternedLight.light.SetActive(patternedLight.switchPattern[currentBeatID]);
            else
                // not enough data, default to off
                patternedLight.light.SetActive(false);
        }
    }
}
