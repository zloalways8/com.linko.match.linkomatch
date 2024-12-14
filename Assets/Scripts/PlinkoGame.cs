using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PlinkoGame : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] ballPrefabs; // Массив префабов шаров
    public Transform spawnPoint; // Точка спауна шаров
    public int totalBalls = 10; // Общее количество шаров
    public TextMeshProUGUI ballsText; // Текст для отображения очков

    [FormerlySerializedAs("scoreText")] [Header("Score Settings")]
    public TextMeshProUGUI ScoreText; // Текст для отображения очков
    public TextMeshProUGUI ScoreTextPlus;
    private int score = 0; // Текущее количество очков

    [Header("Zones")]
    public Collider2D[] zone10; // Массив триггеров для зоны +10
    public Collider2D[] zone20; // Массив триггеров для зоны +20
    public Collider2D[] zone30; // Массив триггеров для зоны +30

    [Header("Game End Settings")]
    public GameObject endGameObject; // Объект, который активируется в конце игры
    public GameObject GameObject; // Объект, который активируется в конце игры

    private int _ball = 0;
    private int spawnedBalls = 0; // Количество заспауниных шаров
    private int destroyedBalls = 0; // Количество удаленных шаров
    private int currentPrefabIndex = 0; // Индекс текущего шара в массиве
    public int Score = 0;
    
    public bool isPaused = false;  // Состояние паузы
    void Start()
    {
        _ball = PlayerPrefs.GetInt("Balls", 0);
        score = 0;
        Score = PlayerPrefs.GetInt("Score", score);
        UpdateScoreText();
        endGameObject.SetActive(false);
        ballsText.text = $"Balls: {PlayerPrefs.GetInt("Balls", 0)}";
        totalBalls = PlayerPrefs.GetInt("Balls", 0);
    }

    void Update()
    {
        // Спауним шары по одному по нажатию на экран
        if (Input.GetMouseButtonDown(0) && spawnedBalls < totalBalls)
        {
            // Начинаем спаунить шары по очереди
            StartCoroutine(SpawnBallsSequentially());
        }
    }

    // Коррутина для спауна шаров по одному с задержкой
    private IEnumerator SpawnBallsSequentially()
    {
        while (spawnedBalls < totalBalls)
        {
            SpawnBall();  // Спавним один шар
            _ball--;
            PlayerPrefs.SetInt("Balls", _ball);
            PlayerPrefs.Save();
            ballsText.text = $"Balls: {PlayerPrefs.GetInt("Balls", _ball)}";
            yield return new WaitForSeconds(0.5f);  // Задержка между спауном шаров
        }
    }

    // Метод для спауна одного шара
    void SpawnBall()
    {
        if (ballPrefabs.Length == 0 || spawnPoint == null)
        {
            Debug.LogError("Cannot spawn balls: Ball prefabs or spawn point is missing.");
            return;
        }

        float randomXOffset = Random.Range(-1f, 1f); // Случайное смещение по оси X
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x + randomXOffset, spawnPoint.position.y, spawnPoint.position.z);

        // Создаем шар из массива префабов
        GameObject newBall = Instantiate(ballPrefabs[currentPrefabIndex], spawnPosition, Quaternion.identity);

        PlinkoBall plinkoBallComponent = newBall.GetComponent<PlinkoBall>();
        if (plinkoBallComponent != null)
        {
            plinkoBallComponent.Initialize(this);
        }
        else
        {
            Debug.LogError("PlinkoBall component not found on the spawned ball!");
        }

        // Переходим к следующему префабу в массиве
        currentPrefabIndex = (currentPrefabIndex + 1) % ballPrefabs.Length;
        spawnedBalls++;
    }

    public void AddScore(int points)
    {
        score += points;
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.Save();
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        ScoreText.text = "Score: " + score;
        ScoreTextPlus.text = "Score: +" + score;

    }
    
    public void TogglePause()
    {
        isPaused = false;
        if (!isPaused)
        {
            Time.timeScale = 0f;
        }
    }
    
    public void TogglePlaye()
    {
        isPaused = true;
        if (isPaused)
        {
            Time.timeScale = 1f;
        }
    }

    public void OnBallDestroyed()
    {
        destroyedBalls++;
        if (destroyedBalls >= totalBalls)
        {
            GameObject.SetActive(false);
            endGameObject.SetActive(true);
        }
    }
}
