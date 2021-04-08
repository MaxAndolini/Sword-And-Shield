using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    public static bool GameStarted;
    public GameObject gameStartBrief;
    public Text dashText;
    public Text powerText;
    public GameObject blackScreen;

    private void Start()
    {
        dashText.text = "Dash (" + KeyCode.E + ")";
        powerText.text = "Power (" + KeyCode.F + ")";

        blackScreen.GetComponent<Image>().canvasRenderer.SetAlpha(1f);
        blackScreen.SetActive(true);
        gameStartBrief.SetActive(true);
        blackScreen.GetComponent<Image>().CrossFadeAlpha(0f, 7f, true);
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Begin);
        Time.timeScale = 0f;
        GameStarted = false;
        StartCoroutine(GameBegin());
    }

    private IEnumerator GameBegin()
    {
        yield return new WaitForSecondsRealtime(4f);
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Trumpet);
        blackScreen.SetActive(false);
        Time.timeScale = 1f;
        GameStarted = true;
        FindObjectOfType<Audio>().PlayAmbiance();
        StartCoroutine(BriefInactive());
    }

    private IEnumerator BriefInactive()
    {
        yield return new WaitForSecondsRealtime(8f);
        gameStartBrief.SetActive(false);
    }
}