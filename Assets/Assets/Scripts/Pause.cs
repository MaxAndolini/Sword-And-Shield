using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public static bool GamePaused;
    public GameObject pauseMenu;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && GameStart.GameStarted && !GameOver.GameFinished)
        {
            if (GamePaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GamePaused = false;
        FindObjectOfType<Audio>().PlayAmbiance();
    }

    private void PauseGame()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GamePaused = true;
        FindObjectOfType<Audio>().StopAmbiance();
    }

    public void MainMenu()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        Time.timeScale = 1f;
        GamePaused = false;
        FindObjectOfType<Audio>().StopAmbiance();
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        FindObjectOfType<Audio>().StopAmbiance();
        Application.Quit();

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }
}