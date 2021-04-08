using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpawnEnemy : MonoBehaviour
{
    public Slider waveSlider;
    public GameObject enemyPrefab;
    public GameObject waveText;
    public int waveCurrent;

    public int enemyCount;
    public int wave0EnemyLimit;
    public float wave0EnemySize;
    public int wave1EnemyLimit;
    public float wave1EnemySize;
    public int wave2EnemyLimit;
    public float wave2EnemySize;

    private readonly float[,] spawnPoints =
    {
        {733.858f, 290.641f}, {721.26f, 282.139f}, {723.788f, 304.973f}, {717.1f, 318.458f}, {717.1f, 318.458f},
        {724.588f, 346.714f}, {767.196f, 360.41f}, {774.038f, 360.41f}, {817.896f, 353.907f}, {816.457f, 337.033f},
        {823.389f, 308.805f},
        {814.074f, 286.949f}, {794.522f, 289.05f}
    };

    private void Start()
    {
        EnemyDrop(0);
        StartCoroutine(FirstText());
    }

    public void Update()
    {
        switch (waveCurrent)
        {
            case 0 when enemyCount == 0:
                waveCurrent++;
                StartCoroutine(ChangeText());
                EnemyDrop(1);
                break;
            case 1 when enemyCount == 0:
                waveCurrent++;
                StartCoroutine(ChangeText());
                EnemyDrop(2);
                break;
            case 2 when enemyCount == 0:
            {
                waveCurrent++;
                StartCoroutine(FinalText());
                var gameOver = FindObjectOfType<GameOver>();
                StartCoroutine(gameOver.GameWin());
                break;
            }
        }
    }

    private IEnumerator FirstText()
    {
        yield return new WaitForSeconds(3);
        waveText.GetComponent<Text>().text = "WAVE 1";
        yield return new WaitForSeconds(2);
        waveText.SetActive(false);
    }

    private IEnumerator ChangeText()
    {
        waveText.GetComponent<Text>().text = "WAVE " + (waveCurrent + 1);
        waveText.SetActive(true);
        yield return new WaitForSeconds(3);
        waveText.SetActive(false);
    }

    private IEnumerator FinalText()
    {
        waveText.GetComponent<Text>().text = "YOU WIN!";
        waveText.SetActive(true);
        yield return new WaitForSecondsRealtime(7f);
        waveText.SetActive(false);
    }

    private void EnemyDrop(int wave)
    {
        var enemyLimit = 0;
        float enemySize = 1;
        switch (wave)
        {
            case 0:
                enemyLimit = wave0EnemyLimit;
                enemySize = wave0EnemySize;
                break;
            case 1:
                enemyLimit = wave1EnemyLimit;
                enemySize = wave1EnemySize;
                break;
            case 2:
                enemyLimit = wave2EnemyLimit;
                enemySize = wave2EnemySize;
                break;
        }

        while (enemyCount < enemyLimit)
        {
            var pos = Random.Range(0, spawnPoints.Length / 2);
            var enemy = Instantiate(enemyPrefab, new Vector3(spawnPoints[pos, 0], 0.008574724f, spawnPoints[pos, 1]),
                Quaternion.identity);
            enemyCount += 1;
            enemy.tag = "Enemy";
            enemy.transform.localScale = new Vector3(enemySize, enemySize, enemySize);
        }

        CalculateWave();
    }

    public void CalculateWave()
    {
        var enemyLimit = 0;
        switch (waveCurrent)
        {
            case 0:
                enemyLimit = wave0EnemyLimit;
                break;
            case 1:
                enemyLimit = wave1EnemyLimit;
                break;
            case 2:
                enemyLimit = wave2EnemyLimit;
                break;
        }

        waveSlider.value = (float) ((enemyLimit - enemyCount) * 100) / enemyLimit;
    }

    public void ReduceEnemy()
    {
        enemyCount--;
        CalculateWave();
    }
}