using System;
using UnityEngine;

public class HighScoreManager : MonoBehaviour
{
    private const string ScoreKeyPrefix = "HighScore_";
    private const string DateKeyPrefix = "HighScoreDate_";

    private int[] highScores = new int[3];
    private string[] highScoreDates = new string[3];

    void Start()
    {
        LoadHighScores();
    }

    // Загружаем высокие рекорды из PlayerPrefs
    public void LoadHighScores()
    {
        for (int i = 0; i < 3; i++)
        {
            highScores[i] = PlayerPrefs.GetInt(ScoreKeyPrefix + i, 0);
            highScoreDates[i] = PlayerPrefs.GetString(DateKeyPrefix + i, "No Date");
        }
    }

    // Сохраняем новый рекорд с датой
    public void SaveNewHighScore(int newScore)
    {
        DateTime currentDate = DateTime.Now;
        string formattedDate = currentDate.ToString("dd.MM.yyyy"); // Форматируем дату как "14.12.2024"

        // Проходим по всем рекордам и обновляем их, если новый рекорд больше
        for (int i = 0; i < 3; i++)
        {
            if (newScore > highScores[i])
            {
                // Перемещаем старые рекорды вниз
                for (int j = 2; j > i; j--)
                {
                    highScores[j] = highScores[j - 1];
                    highScoreDates[j] = highScoreDates[j - 1];
                }

                // Сохраняем новый рекорд
                highScores[i] = newScore;
                highScoreDates[i] = formattedDate;

                // Сохраняем в PlayerPrefs
                PlayerPrefs.SetInt(ScoreKeyPrefix + i, highScores[i]);
                PlayerPrefs.SetString(DateKeyPrefix + i, highScoreDates[i]);
                PlayerPrefs.Save();
                break;
            }
        }
    }

    // Получаем все высокие рекорды
    public int[] GetHighScores()
    {
        return highScores;
    }

    public string[] GetHighScoreDates()
    {
        return highScoreDates;
    }
}