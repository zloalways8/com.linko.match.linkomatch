using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class Match3Game : MonoBehaviour
{
    [SerializeField] private AudioSource _componentAudio;
    [SerializeField] private AudioSource _componentSmena;
    public int gridWidth = 7;
    public int gridHeight = 9;
    public float tileSpacing = 1.1f;
    public GameObject[] tilePrefabs;
    public TextMeshProUGUI scoreText;
    public AchievementSystem _AchievementSystem;
    private GameObject[,] grid;
    public int score;
    public float Timer = 0f;
    public bool isPaused = false;  // Состояние паузы
    private Coroutine currentCoroutine = null;  // Текущая корутина

    void Start()
    {
        if (PlayerPrefs.GetInt("Score",0 )> 0)
        {
            score += PlayerPrefs.GetInt("Score", 0);
            PlayerPrefs.SetInt("Score", 0);
            PlayerPrefs.Save();
        }
        GenerateGrid();
        UpdateScoreText();
    }

    void GenerateGrid()
    {
        grid = new GameObject[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                SpawnTile(x, y);
            }
        }

        // Удаляем совпадения при старте игры
        StartCoroutine(RemoveMatchesAtStart());
    }

    void SpawnTile(int x, int y)
    {
        Vector2 spawnPosition = new Vector2(x * tileSpacing, y * tileSpacing);
        int randomIndex = Random.Range(0, tilePrefabs.Length);
        GameObject tile = Instantiate(tilePrefabs[randomIndex], spawnPosition, Quaternion.identity);
        tile.transform.parent = transform;
        tile.name = $"Tile_{x}_{y}";
        grid[x, y] = tile;

        Tile tileComponent = tile.AddComponent<Tile>();
        tileComponent.Initialize(x, y, this);
    }

    public void SwapTiles(Tile tile1, Tile tile2)
    {
        // Меняем тайлы в массиве
        _componentSmena.Play();
        int x1 = tile1.X;
        int y1 = tile1.Y;
        int x2 = tile2.X;
        int y2 = tile2.Y;

        grid[x1, y1] = tile2.gameObject;
        grid[x2, y2] = tile1.gameObject;

        // Обновляем координаты в Tile
        tile1.Initialize(x2, y2, this);
        tile2.Initialize(x1, y1, this);

        // Анимация перемещения тайлов
        DOTween.Sequence()
            .Append(tile1.transform.DOMove(new Vector2(x2 * tileSpacing, y2 * tileSpacing), 0.3f))
            .Append(tile2.transform.DOMove(new Vector2(x1 * tileSpacing, y1 * tileSpacing), 0.3f));

        // Проверяем совпадения
        if (!isPaused)
            StartCoroutine(CheckMatchesAndRefill());
    }
    
    IEnumerator CheckMatchesAndRefill()
    {
        yield return new WaitForSeconds(0.2f);

        // Проверяем совпадения
        List<GameObject> matchedTiles = FindMatches();
        if (matchedTiles.Count > 0)
        {
            _componentAudio.Play();
            _AchievementSystem.CheckForAchievements();
            // Удаление совпавших тайлов
            RemoveMatches(matchedTiles);

            // Ждем завершения удаления
            yield return new WaitForSeconds(0.5f);

            // Опускание тайлов
            DropTiles();

            // Ждем завершения анимации падения
            yield return new WaitForSeconds(0.5f);

            // Заполнение пустых мест новыми тайлами
            FillEmptySpaces();

            // Возвращаемся к проверке совпадений только если не на паузе
            if (!isPaused)
            {
                currentCoroutine = StartCoroutine(CheckMatchesAndRefill());
            }
            else
            {
                currentCoroutine = null; // Останавливаем текущую проверку
            }
        }
        else
        {
            currentCoroutine = null; // Нет совпадений — завершаем корутину
        }
    }

    public List<GameObject> FindMatches()
    {
        List<GameObject> matchedTiles = new List<GameObject>();

        // Горизонтальные совпадения
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth - 2; x++)
            {
                if (grid[x, y] != null && grid[x + 1, y] != null && grid[x + 2, y] != null &&
                    grid[x, y].tag == grid[x + 1, y].tag && grid[x, y].tag == grid[x + 2, y].tag)
                {
                    if (!matchedTiles.Contains(grid[x, y])) matchedTiles.Add(grid[x, y]);
                    if (!matchedTiles.Contains(grid[x + 1, y])) matchedTiles.Add(grid[x + 1, y]);
                    if (!matchedTiles.Contains(grid[x + 2, y])) matchedTiles.Add(grid[x + 2, y]);
                }
            }
        }

        // Вертикальные совпадения
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight - 2; y++)
            {
                if (grid[x, y] != null && grid[x, y + 1] != null && grid[x, y + 2] != null &&
                    grid[x, y].tag == grid[x, y + 1].tag && grid[x, y].tag == grid[x, y + 2].tag)
                {
                    if (!matchedTiles.Contains(grid[x, y])) matchedTiles.Add(grid[x, y]);
                    if (!matchedTiles.Contains(grid[x, y + 1])) matchedTiles.Add(grid[x, y + 1]);
                    if (!matchedTiles.Contains(grid[x, y + 2])) matchedTiles.Add(grid[x, y + 2]);
                }
            }
        }

        return matchedTiles;
    }

    void RemoveMatches(List<GameObject> matchedTiles)
    {
        foreach (GameObject tile in matchedTiles)
        {
            int x = Mathf.RoundToInt(tile.transform.position.x / tileSpacing);
            int y = Mathf.RoundToInt(tile.transform.position.y / tileSpacing);
            if (grid[x, y] == tile)
            {
                grid[x, y] = null;
            }

            // Добавляем анимацию исчезновения
            tile.GetComponent<SpriteRenderer>().DOFade(0f, 0.5f).OnKill(() => Destroy(tile));

        }

        score += 25;
        UpdateScoreText();
    }

    void DropTiles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    for (int yAbove = y + 1; yAbove < gridHeight; yAbove++)
                    {
                        if (grid[x, yAbove] != null)
                        {
                            grid[x, y] = grid[x, yAbove];
                            grid[x, yAbove] = null;

                            grid[x, y].transform.DOMove(new Vector2(x * tileSpacing, y * tileSpacing), 0.3f);

                            Tile tileComponent = grid[x, y].GetComponent<Tile>();
                            tileComponent.Initialize(x, y, this);

                            break;
                        }
                    }
                }
            }
        }
    }

    void FillEmptySpaces()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                if (grid[x, y] == null)
                {
                    SpawnTile(x, y);
                }
            }
        }

        // После заполнения поля проверяем совпадения
        if (!isPaused)
        {
            currentCoroutine = StartCoroutine(CheckMatchesAndRefill());
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }

    public int GetScore()
    {
        return score;
    }

    IEnumerator RemoveMatchesAtStart()
    {
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            if (isPaused) yield break;

            List<GameObject> matchedTiles = FindMatches();
            if (matchedTiles.Count == 0) break;

            RemoveMatches(matchedTiles);
            yield return new WaitForSeconds(0.5f);
            DropTiles();
            yield return new WaitForSeconds(0.5f);
            FillEmptySpaces();
        }
    }

    // Метод для паузы
    public void TogglePause()
    {
        isPaused = !isPaused;
        Timer = _AchievementSystem.timer;
        if (isPaused)
        {
            
            FillEmptySpaces();
            StartCoroutine(CheckMatchesAndRefill());
        }
        else
        {
            _AchievementSystem.timer = Timer;
            FillEmptySpaces();
            StartCoroutine(CheckMatchesAndRefill());
            
        }
    }
}
