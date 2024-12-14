using UnityEngine;
using TMPro;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] highScoreTexts; // Массив для TextMeshProUGUI элементов для отображения рекордов
    private HighScoreManager highScoreManager;

    void Start()
    {
        highScoreManager = FindObjectOfType<HighScoreManager>();

        // Обновляем UI с максимальными рекордами
        UpdateHighScoreDisplay();
    }

    void UpdateHighScoreDisplay()
    {
        int[] highScores = highScoreManager.GetHighScores();
        string[] highScoreDates = highScoreManager.GetHighScoreDates();

        // Обновляем текст для каждого рекорда
        for (int i = 0; i < highScoreTexts.Length; i++)
        {
            highScoreTexts[i].text = $"{i + 1}. Score {highScores[i]}  {highScoreDates[i]}";
        }
    }
}