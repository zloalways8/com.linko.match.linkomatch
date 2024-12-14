using UnityEngine;
using UnityEngine.UI;

public class CollectionManager : MonoBehaviour
{
    [Header("Collection Objects")]
    public Image mainObjectImage; // Основное изображение объекта
    public Image nameImage; // Изображение для названия объекта
    public Image descriptionImage; // Изображение для описания объекта

    [Header("Object Data")]
    public Sprite[] objectSprites; // Спрайты для каждого объекта
    public Sprite[] nameSprites; // Спрайты для названий объектов
    public Sprite[] descriptionSprites; // Спрайты для описаний объектов

    [Header("Navigation Buttons")]
    public Button nextButton; // Кнопка "Следующий"
    public Button previousButton; // Кнопка "Предыдущий"

    private int currentObjectIndex = 0; // Текущий индекс объекта
    private int unlockedObjects = 1; // Количество разблокированных объектов

    private const string UnlockedObjectsKey = "UnlockedObjects"; // Ключ для сохранения прогресса
    private const string HighestLevelKey = "HighestLevel"; // Ключ для сохранения уровня

    void Start()
    {
        OnLevelCompleted(PlayerPrefs.GetInt(AvatarPhaseSetup.PRESENT_STAGE, 0));
        int highestLevel = PlayerPrefs.GetInt(HighestLevelKey, 0);
        unlockedObjects = Mathf.Clamp((highestLevel / 5) + 1, 1, objectSprites.Length);

        // Обновляем сохраненные данные о разблокированных объектах
        PlayerPrefs.SetInt(UnlockedObjectsKey, unlockedObjects);
        PlayerPrefs.Save();

        UpdateCollectionDisplay();
        UpdateNavigationButtons();
    }

    public void OnNextButtonClicked()
    {
        if (currentObjectIndex < unlockedObjects - 1)
        {
            currentObjectIndex++;
            UpdateCollectionDisplay();
            UpdateNavigationButtons();
        }
    }

    public void OnPreviousButtonClicked()
    {
        if (currentObjectIndex > 0)
        {
            currentObjectIndex--;
            UpdateCollectionDisplay();
            UpdateNavigationButtons();
        }
    }

    public void OnLevelCompleted(int levelIndex)
    {
        PlayerPrefs.SetInt(HighestLevelKey, levelIndex);

        // Открываем новый объект каждые 5 уровней
        if (levelIndex >= unlockedObjects * 5 && unlockedObjects < objectSprites.Length)
        {
            unlockedObjects++;
            PlayerPrefs.SetInt(UnlockedObjectsKey, unlockedObjects);
        }

        PlayerPrefs.Save();
    }

    private void UpdateCollectionDisplay()
    {
        // Устанавливаем спрайты для текущего объекта, названия и описания
        mainObjectImage.sprite = objectSprites[currentObjectIndex];
        nameImage.sprite = nameSprites[currentObjectIndex];
        descriptionImage.sprite = descriptionSprites[currentObjectIndex];
    }

    private void UpdateNavigationButtons()
    {
        // Управляем доступностью кнопок навигации
        nextButton.interactable = currentObjectIndex < unlockedObjects - 1;
        previousButton.interactable = currentObjectIndex > 0;
    }
}
