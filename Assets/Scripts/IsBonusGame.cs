using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IsBonusGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _ballsText;
    public Button yourButton; // Перетащите сюда вашу кнопку в инспекторе

    void Start()
    {
        int balls = PlayerPrefs.GetInt("Balls", 0);
        
        _ballsText.text = $"Balls available:\n{balls}";

        // Если balls больше 0, то активируем кнопку, иначе деактивируем
        if (balls > 0)
        {
            yourButton.interactable = true; // Кнопка доступна для нажатия
        }
        else
        {
            yourButton.interactable = false; // Кнопка недоступна
        }
    }
}
