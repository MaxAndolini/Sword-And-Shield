using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public GameObject main;
    public GameObject loading;
    public GameObject settings;
    public GameObject loadedText;
    public Slider progress;
    public Text loadingText;
    public AudioMixer audioMixer;
    public Dropdown resolution;
    public Toggle fullScreen;
    public Dropdown graphic;
    public Slider mouseSensitivity;
    public Slider audioVolume;
    public Text versionText;
    private Resolution[] resolutions;

    private void Awake()
    {
        var toggleScreen = PlayerPrefs.GetInt("FullScreen");

        if (toggleScreen == 1)
        {
            Screen.fullScreen = true;
            fullScreen.isOn = true;
        }
        else
        {
            Screen.fullScreen = false;
            fullScreen.isOn = false;
        }

        versionText.text = "v " + Application.version;

        main.SetActive(true);
        settings.SetActive(false);
        FindObjectOfType<Audio>().Play(Audio.Audios.Menu, true);
    }

    private void Start()
    {
        resolutions = Screen.resolutions;

        var res = new List<string>();
        var current = 0;

        for (var i = 0; i < resolutions.Length; i++)
        {
            var reso = resolutions[i].width + " X " + resolutions[i].height;
            res.Add(reso);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                current = i;
        }

        resolution.ClearOptions();
        resolution.AddOptions(res);
        resolution.value = PlayerPrefs.GetInt("Resolution", current);
        resolution.RefreshShownValue();

        graphic.value = PlayerPrefs.GetInt("Graphic", 5);
        graphic.RefreshShownValue();

        mouseSensitivity.value = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
        audioVolume.value = PlayerPrefs.GetFloat("Volume", 1f);
        FindObjectOfType<Audio>().ChangeMixerVolume(audioVolume.value);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void PlayGame()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        StartCoroutine(LoadGame(1));
    }

    private IEnumerator LoadGame(int scene)
    {
        var operation = SceneManager.LoadSceneAsync(scene);

        operation.allowSceneActivation = false;

        main.SetActive(false);
        loading.SetActive(true);

        while (!operation.isDone)
        {
            var loadingProgress = operation.progress;
            if (Math.Abs(loadingProgress - 0.9f) == 0) loadingProgress = 1f;
            loadingText.text = "LOADING PROGRESS: " + loadingProgress * 100 + "%";
            progress.value = loadingProgress;

            if (loadingProgress >= 0.9f)
            {
                loadedText.GetComponent<Text>().text =
                    "Press the " + KeyCode.Space.ToString().ToLower() + " bar to continue";
                loadedText.SetActive(true);

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    FindObjectOfType<Audio>().StopMenu();
                    operation.allowSceneActivation = true;
                }
            }

            yield return null;
        }
    }

    public void Settings()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        settings.SetActive(true);
        main.SetActive(false);
    }

    public void Menu()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        settings.SetActive(false);
        main.SetActive(true);
    }

    public void SetResolution(int index)
    {
        var res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolution.value);
        PlayerPrefs.Save();
    }

    public void ToggleFullScreen(bool toggle)
    {
        Screen.fullScreen = toggle;
        PlayerPrefs.SetInt("FullScreen", toggle ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Graphic", index);
        PlayerPrefs.Save();
    }

    public void SetMouseSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
        PlayerPrefs.Save();
    }

    public void SetVolume(float volume)
    {
        FindObjectOfType<Audio>().ChangeMixerVolume(audioVolume.value);
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        FindObjectOfType<Audio>().StopMenu();
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}