using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
    public static bool GameFinished;
    public GameObject gameOverMenu;
    public GameObject credits;
    public GameObject blackScreen;
    public Text creditsScoreText;
    public Text scoreText;
    private GameObject player;
    private Animator playerAnimator;
    private GameObject playerCamera;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCamera = GameObject.Find("Camera");
        playerAnimator = player.GetComponent<Animator>();
    }

    private void Update()
    {
        if (player.GetComponent<Character>().isDead && !GameFinished) StartCoroutine(GameLose());
    }

    public IEnumerator GameWin()
    {
        GameFinished = true;
        playerCamera.transform.localPosition = new Vector3(0f, 1.83f, 5.1f);
        playerCamera.transform.Rotate(0f, 180f, 0f);
        FindObjectOfType<Audio>().StopAmbiance();
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Win);
        yield return new WaitForSecondsRealtime(2f);
        playerAnimator.SetInteger("condition", 6); // Jump
        yield return new WaitForSecondsRealtime(1f);
        playerAnimator.SetInteger("condition", 0); // Idle
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(2f);
        creditsScoreText.text = "Your Score: " + player.GetComponent<Character>().score;
        blackScreen.GetComponent<Image>().canvasRenderer.SetAlpha(0f);
        blackScreen.SetActive(true);
        blackScreen.GetComponent<Image>().CrossFadeAlpha(1f, 4f, true);
        yield return new WaitForSecondsRealtime(4f);
        credits.SetActive(true);
        yield return new WaitForSecondsRealtime(20f);
        Time.timeScale = 1f;
        GameFinished = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(0);
    }

    private IEnumerator GameLose()
    {
        GameFinished = true;
        FindObjectOfType<Audio>().StopAmbiance();
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Lose);
        yield return new WaitForSeconds(4f);
        scoreText.text = "SCORE: " + player.GetComponent<Character>().score;
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MainMenu()
    {
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Button);
        player.GetComponent<Character>().isDead = false;
        Time.timeScale = 1f;
        GameFinished = false;
        SceneManager.LoadScene(0);
    }
}