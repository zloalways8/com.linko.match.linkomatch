using System;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AchievementSystem : MonoBehaviour
{
    public TextMeshProUGUI[] achievementTexts; // Тексты для отображения прогресса ачивок
    public Image[] achievementIcons; // Иконки для ачивок
    public Sprite[] achievementSprites; // Спрайты для ачивок
    public string[] tileTags; // Теги для различных тайлов
    public int[] requiredAchievements; // Количество нужных тайлов для ачивок
    private int[] currentAchievements; // Текущий прогресс по каждой ачивке
    [SerializeField] private AudioSource audioComponent;
    private string[] tileTag = new[] { "1", "2", "3" }; // Теги для различных тайлов
    public Match3Game match3Game; // Ссылка на класс Match3Game, чтобы использовать его методы
    public TextMeshProUGUI Score;
    public GameObject GamePanelObject;
    public GameObject victoryObject; // Объект победы
    public GameObject defeatObject; // Объект поражения

    public float timer = 120f; // Таймер на 2 минуты
    public TextMeshProUGUI timerText; // Текст для отображения таймера

    private int balls; // Переменная для подсчёта шариков

    void Start()
    {
        // Инициализируем значение переменной Balls из PlayerPrefs
        balls = PlayerPrefs.GetInt("Balls", 0);

        currentAchievements = new int[achievementIcons.Length];
        SetRandomAchievements();
        UpdateAchievementTexts();
    }

    void Update()
    {
        if (match3Game.isPaused)
        {
            timer = match3Game.Timer;
        }
        Score.text = $"Score: {match3Game.GetScore()}";

        // Обновляем таймер
        if (timer > 0)
        {
            timer -= Time.deltaTime; // Уменьшаем таймер с каждым кадром
            UpdateTimerText();
        }
        else
        {
            if (!victoryObject.activeSelf) // Если победа не активирована
            {
                ShowDefeat(); // Показываем объект поражения
            }
        }

        // Проверка на победу (если все ачивки собраны)
        if (currentAchievements.All(a => a >= requiredAchievements[Array.IndexOf(currentAchievements, a)]))
        {
            if (!victoryObject.activeSelf) // Если победа не активирована
            {
                ShowVictory(); // Показываем объект победы
            }
        }
    }

    void SetRandomAchievements()
    {
        // Рандомизация количества тайлов для каждой ачивки (10–30)
        for (int i = 0; i < requiredAchievements.Length; i++)
        {
            requiredAchievements[i] = Random.Range(10, 20);
        }

        // Перемешивание спрайтов (вместе с тегами, чтобы сохранялась связь)
        var shuffledIndices = Enumerable.Range(0, achievementSprites.Length).OrderBy(_ => Random.value).ToArray();

        for (int i = 0; i < achievementIcons.Length; i++)
        {
            int index = shuffledIndices[i];
            achievementIcons[i].sprite = achievementSprites[index];
            tileTag[i] = tileTags[index];
        }
    }

    public void CheckForAchievements()
    {
        // Получаем список совпавших тайлов из Match3Game
        var matchedTiles = match3Game.FindMatches();

        foreach (var tile in matchedTiles)
        {
            audioComponent.Play();
            string tileTag1 = tile.tag;

            // Проверяем, связан ли этот тег с ачивками
            for (int i = 0; i < tileTag.Length; i++)
            {
                if (tileTag1 == tileTag[i] && i < achievementIcons.Length && i < requiredAchievements.Length)
                {
                    currentAchievements[i]++;

                    // Если ачивка завершена
                    if (currentAchievements[i] >= requiredAchievements[i])
                    {
                        currentAchievements[i] = 0;
                        requiredAchievements[i] = 0;
                        achievementIcons[i].color = Color.red; // Отмечаем как завершённую
                    }

                    break;
                }
            }
        }

        // Обновляем текст прогресса
        UpdateAchievementTexts();
    }

    void UpdateAchievementTexts()
    {
        for (int i = 0; i < achievementTexts.Length; i++)
        {
            var number = requiredAchievements[i] - currentAchievements[i];
            achievementTexts[i].text = $"x{number}";
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timer / 60); // Получаем минуты
        int seconds = Mathf.FloorToInt(timer % 60); // Получаем секунды
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Обновляем текст таймера
    }

    void ShowVictory()
    {
        FindObjectOfType<HighScoreManager>().SaveNewHighScore(match3Game.score);
        GamePanelObject.SetActive(false);
        victoryObject.SetActive(true); // Показываем объект победы

        // Увеличиваем значение переменной Balls на 10
        balls += 10;

        // Сохраняем значение в PlayerPrefs
        PlayerPrefs.SetInt("Balls", balls);
        PlayerPrefs.Save();
    }

    void ShowDefeat()
    {
        GamePanelObject.SetActive(false);
        defeatObject.SetActive(true); // Показываем объект поражения
    }
}
