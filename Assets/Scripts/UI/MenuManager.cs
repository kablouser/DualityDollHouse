using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public bool HasGameStarted { get; private set; }

    [SerializeField] private SceneSingletons sceneSingletons;
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject creditsMenu;
    [SerializeField] private AudioMixer volumeMixer;

    private void Start()
    {
        HasGameStarted = false;
        // disable controller
        if(sceneSingletons.PlayerMovement != null)
            sceneSingletons.PlayerMovement.enabled = false;
    }

    public void StartGame()
    {
        HasGameStarted = true;
        startMenu.SetActive(false);
        // allow play to move
        if(sceneSingletons.PlayerMovement != null)
            sceneSingletons.PlayerMovement.enabled = true;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        pauseMenu.SetActive(false);
    }

    public void BackFromCredits()
    {
        creditsMenu.SetActive(false);
        if(HasGameStarted)
            pauseMenu.SetActive(true);
        else
            startMenu.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameObject otherMenu = HasGameStarted ? pauseMenu : startMenu;
            if (creditsMenu.activeSelf)
            {
                creditsMenu.SetActive(false);
                otherMenu.SetActive(true);
            }
            else
            {
                otherMenu.SetActive(!otherMenu.activeSelf);
            }
        }
    }

    public void SetMasterVolume(float sliderValue)
    {
        SetVolume("MasterVolume", sliderValue);
    }

    public void SetSoundFXVolume(float sliderValue)
    {
        SetVolume("SoundFX_Volume", sliderValue);
    }

    public void SetMusicVolume(float sliderValue)
    {
        SetVolume("MusicVolume", sliderValue);
    }

    private void SetVolume(string parameterName, float sliderValue)
    {
        sliderValue = Mathf.Clamp(sliderValue, 0.00001f, 1.0f);
        volumeMixer.SetFloat(parameterName, Mathf.Log10(sliderValue) * 20);
    }

    [ContextMenu("Setup 2D Lighting")]
    private void Setup2DLighting()
    {
        Setup2DLightingRecursive(transform);
    }

    private void Setup2DLightingRecursive(Transform target)
    {
        Image image = target.GetComponent<Image>();
        if (image)
        {
            // null will resort to using the default UI material
            image.material = null;
            Color getColor = image.color;
            // transparency causes issues with 2D lighting
            getColor.a = 1;
            image.color = getColor;
        }

        for (int i = 0; i < target.childCount; ++i)
            Setup2DLightingRecursive(target.GetChild(i));
    }
}
