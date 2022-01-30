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

    [SerializeField] private AudioSource audioCueSource;
    [SerializeField] private bool[] audioCueSwitchPattern;

    private void Update()
    {
        int currentBarNumber = _sceneSingletons.GetMusicBar();

        foreach(PatternedLight patternedLight in _patternedLights)
        {
            if (currentBarNumber < patternedLight.switchPattern.Length)
                // set to pattern
                patternedLight.light.SetActive(patternedLight.switchPattern[currentBarNumber]);
            else
                // not enough data, default to off
                patternedLight.light.SetActive(false);
        }

        // audio cue (for full room lit)
        if (currentBarNumber < audioCueSwitchPattern.Length &&
            // if switch pattern wants to play audio cue
            audioCueSwitchPattern[currentBarNumber])
        {
            if(audioCueSource.isPlaying == false)
                audioCueSource.Play();
        }
        else
            audioCueSource.Stop();
    }
}
