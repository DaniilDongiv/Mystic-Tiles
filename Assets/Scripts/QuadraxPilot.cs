using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuadraxPilot : MonoBehaviour
{
    [SerializeField] private GameObject[] _squarePrefabsArray;
    [SerializeField] private int _gridWidth = 13;
    [SerializeField] private int _gridHeight = 19;
    [SerializeField] private float _squareSpacingValue = 0.23f;
    [SerializeField] private AxisAligner[,] _squareGrid;
    private Vector3 _squareSizeVector = new Vector3(0.0733539388f, 0.0605170019f, 0.0733539388f);
    
    private float _stopwatchTime;
    private int _movesCounter;
    [SerializeField] private TextMeshProUGUI[] _stopwatchText;
    [SerializeField] private TextMeshProUGUI[] _movesText;
    
    [SerializeField] private AudioClip EchoStrand;
    [SerializeField] private AudioSource VibrationEmitter;
    [SerializeField] private GameObject _winMenu;
    [SerializeField] private GameObject _dimensionInterface;
    
    [SerializeField] private Image[] _achievementIcons;
    [SerializeField] private TextMeshProUGUI[] _achievementProgressTexts;
    [SerializeField] private Image[] _achievementIconsWinPanel;
    [SerializeField] private TextMeshProUGUI[] _achievementProgressTextsWinPanel;
    private Dictionary<string, int> _achievements = new Dictionary<string, int>();
    private Dictionary<string, int> _currentAchievementsProgress = new Dictionary<string, int>();
    
    private string[] _achievementTypes = {"cube1","cube2", "cube3", "cube4", "cube5", "cube6", "cube7", "cube"};
    [SerializeField] private Sprite[] _achievementSprites; // Сприты для ачивок
    
    private Dictionary<string, Image> _achievementIconMap = new Dictionary<string, Image>();
    private Dictionary<string, Image> _achievementIconMapWinPanel = new Dictionary<string, Image>();
    
    private void ShuffleList<T>(IList<T> list)
    {
        for (var i = list.Count - 1; i > 0; i--)
        {
            var j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    
    private void UpdateAchievementProgress(string squareTag, int squaresRemovedCount)
    {
        if (_achievements.ContainsKey(squareTag))
        {
            _currentAchievementsProgress[squareTag] += squaresRemovedCount;
            UpdateAchievementsUI();
            AssessConquestStatus();
            
        }
    }
    
    private void Start()
    {
        VibrationEmitter = gameObject.AddComponent<AudioSource>();
        VibrationEmitter.playOnAwake = false;
        VibrationEmitter.clip = EchoStrand;
        
        InitializeAchievements();
        
        TemporalFluxEnabler();

        ForgeMatrix();
        
        UpdateMovesUI();
        UpdateStopwatchUI();
        UpdateAchievementsUI();
    }

    private void Update()
    {
        _stopwatchTime += Time.deltaTime;
        UpdateStopwatchUI();
    }
    
    private void ForgeMatrix()
    {
        _squareGrid = new AxisAligner[_gridWidth, _gridHeight];

        for (int azimuthKey = 0; azimuthKey < _gridWidth; azimuthKey++)
        {
            for (int altitudeKey = 0; altitudeKey < _gridHeight; altitudeKey++)
            {
                GameObject newSquare = Instantiate(SelectRandomTileModel(), DetermineTileCoordinates(azimuthKey, altitudeKey), Quaternion.identity);
                newSquare.transform.SetParent(this.transform);
                newSquare.transform.localScale = _squareSizeVector;
                _squareGrid[azimuthKey, altitudeKey] = newSquare.GetComponent<AxisAligner>();
                _squareGrid[azimuthKey, altitudeKey].SetupTile(azimuthKey, altitudeKey);
            }
        }
    }
    private GameObject SelectRandomTileModel()
    {
        var randomIndex = Random.Range(0, _squarePrefabsArray.Length);
        return _squarePrefabsArray[randomIndex];
    }
    
    private Vector3 DetermineTileCoordinates(int x, int y)
    {
        var posX = x * (_squareSizeVector.x + _squareSpacingValue);
        var posY = y * (_squareSizeVector.y + _squareSpacingValue);
        return new Vector3(posX, posY, 0);
    }
    
    private void InitializeAchievements()
    {
        _achievements.Clear();
        _currentAchievementsProgress.Clear();
        _achievementIconMap.Clear();
        _achievementIconMapWinPanel.Clear();

        var indices = new List<int>();
        
        for (int i = 0; i < _achievementTypes.Length && i < _achievementSprites.Length; i++)
        {
            indices.Add(i);
        }

        ShuffleList(indices);

        var achievementCount = Mathf.Min(_achievementIcons.Length, indices.Count);

        for (var i = 0; i < achievementCount; i++)
        {
            var randomIndex = indices[i];
            var type = _achievementTypes[randomIndex];
            var sprite = _achievementSprites[randomIndex];

            var target = Random.Range(8, 60);
            _achievements[type] = target;
            _currentAchievementsProgress[type] = 0;
            
            _achievementIcons[i].sprite = sprite;
            _achievementIcons[i].gameObject.tag = type;
            _achievementIconMap[type] = _achievementIcons[i];
            _achievementIconsWinPanel[i].sprite = sprite;
            _achievementIconsWinPanel[i].gameObject.tag = type;
            _achievementIconMapWinPanel[type] = _achievementIconsWinPanel[i];
            
            _achievementProgressTexts[i].text = $"0/{target}";
            _achievementProgressTextsWinPanel[i].text = $"0/{target}";
        }
    }











    // Обновление отображения ачивок
    // Обновление отображения ачивок
    private void UpdateAchievementsUI()
    {
        foreach (var achievementType in _achievements.Keys)
        {
            if (_achievementIconMap.TryGetValue(achievementType, out Image icon))
            {
                int target = _achievements[achievementType];
                int current = _currentAchievementsProgress[achievementType];

                // Находим индекс текстового элемента, соответствующего иконке
                int iconIndex = System.Array.IndexOf(_achievementIcons, icon);
                if (iconIndex >= 0 && iconIndex < _achievementProgressTexts.Length)
                {
                    _achievementProgressTexts[iconIndex].text = $"{current}/{target}";
                    _achievementProgressTextsWinPanel[iconIndex].text = $"{current}/{target}";
                }
            }
        }
    }




    // Обработка клика на квадрат
    
    public void ProcessTileSelection(AxisAligner clickedAxisAligner)
    {
        List<AxisAligner> groupOfSquares = IdentifyCluster(clickedAxisAligner); // Поиск группы одинаковых квадратов
        
        if (groupOfSquares.Count > 1)
        {
            string squareTag = clickedAxisAligner.SquareTag; // Тип квадрата (его тег)

            
            foreach (AxisAligner square in groupOfSquares)
            {
                Destroy(square.gameObject); // Уничтожение каждого элемента группы
                _squareGrid[square.GridX, square.GridY] = null; // Очистка позиции в сетке
            }

            // Обновляем прогресс ачивок, если тег совпадает с тегом ачивки
            UpdateAchievementProgress(squareTag, groupOfSquares.Count);

            // Проигрывание звука клика
            VibrationEmitter.PlayOneShot(EchoStrand);

            // Увеличение счетчика ходов
            _movesCounter++;
            UpdateMovesUI();

            // Обработка сдвига и заполнения колонок
            CompactStacksRoutine();
            PopulateVacantColumns();
        }
    }


    // Проверка на победу
    private void AssessConquestStatus()
    {
        foreach (var achievement in _achievements)
        {
            if (_currentAchievementsProgress[achievement.Key] < achievement.Value)
            {
                return; // Если не все ачивки собраны, выход
            }
        }

        // Если все цели выполнены, показываем экран победы
        _winMenu.SetActive(true);
        _dimensionInterface.SetActive(false);
    }

    // Обновление отображения секундомера
    private void UpdateStopwatchUI()
    {
        int minutes = Mathf.FloorToInt(_stopwatchTime / 60); // Подсчет минут
        int seconds = Mathf.FloorToInt(_stopwatchTime % 60); // Подсчет секунд
        foreach (var _stopwatchTextWinPanel in _stopwatchText)
        {
            _stopwatchTextWinPanel.text = "TIME\n" + string.Format("{0:00}:{1:00}", minutes, seconds); // Отображение времени в формате 00:00
        }
        
    }

    // Обновление отображения количества ходов
    private void UpdateMovesUI()
    {
        
        foreach (var movesTextWinPanel in _movesText)
        {
            movesTextWinPanel.text = "MOVES\n" + _movesCounter; // Отображение количества ходов
        }
    }

    // Поиск группы одинаковых квадратов
    private List<AxisAligner> IdentifyCluster(AxisAligner startAxisAligner)
    {
        List<AxisAligner> squareGroup = new List<AxisAligner>();
        Queue<AxisAligner> squaresToCheck = new Queue<AxisAligner>();
        squaresToCheck.Enqueue(startAxisAligner);

        while (squaresToCheck.Count > 0)
        {
            AxisAligner axisAlignerToCheck = squaresToCheck.Dequeue();

            if (!squareGroup.Contains(axisAlignerToCheck) && axisAlignerToCheck.SquareTag == startAxisAligner.SquareTag)
            {
                squareGroup.Add(axisAlignerToCheck);
                foreach (AxisAligner neighborSquare in RetrieveAdjacentCells(axisAlignerToCheck))
                {
                    if (!squareGroup.Contains(neighborSquare) && neighborSquare != null && neighborSquare.SquareTag == startAxisAligner.SquareTag)
                    {
                        squaresToCheck.Enqueue(neighborSquare);
                    }
                }
            }
        }

        return squareGroup;
    }

    // Получение соседних квадратов
    private List<AxisAligner> RetrieveAdjacentCells(AxisAligner axisAlignerToCheck)
    {
        List<AxisAligner> neighborList = new List<AxisAligner>();

        if (axisAlignerToCheck.GridX > 0) neighborList.Add(_squareGrid[axisAlignerToCheck.GridX - 1, axisAlignerToCheck.GridY]);
        if (axisAlignerToCheck.GridX < _gridWidth - 1) neighborList.Add(_squareGrid[axisAlignerToCheck.GridX + 1, axisAlignerToCheck.GridY]);
        if (axisAlignerToCheck.GridY > 0) neighborList.Add(_squareGrid[axisAlignerToCheck.GridX, axisAlignerToCheck.GridY - 1]);
        if (axisAlignerToCheck.GridY < _gridHeight - 1) neighborList.Add(_squareGrid[axisAlignerToCheck.GridX, axisAlignerToCheck.GridY + 1]);

        return neighborList;
    }

    // Метод для сдвига колонок вниз при удалении элементов
    private void CompactStacksRoutine()
    {
        for (int colX = 0; colX < _gridWidth; colX++)
        {
            int emptySpaces = 0;

            for (int rowY = 0; rowY < _gridHeight; rowY++)
            {
                if (_squareGrid[colX, rowY] == null)
                {
                    emptySpaces++;
                }
                else if (emptySpaces > 0)
                {
                    _squareGrid[colX, rowY - emptySpaces] = _squareGrid[colX, rowY];
                    _squareGrid[colX, rowY] = null;
                    RecalibrateTilePlacement(_squareGrid[colX, rowY - emptySpaces], colX, rowY - emptySpaces);
                }
            }
        }
    }
    private void RecalibrateTilePlacement(AxisAligner axisAlignerToUpdate, int newX, int newY)
    {
        if (axisAlignerToUpdate != null)
        {
            axisAlignerToUpdate.ShiftToCoordinates(newX, newY);
        }
    }

    // Метод для заполнения пустых колонок новыми элементами
    private void PopulateVacantColumns()
    {
        for (int colX = 0; colX < _gridWidth; colX++)
        {
            for (int rowY = 0; rowY < _gridHeight; rowY++)
            {
                if (_squareGrid[colX, rowY] == null)
                {
                    // Создаем новый элемент на пустом месте
                    GameObject newSquare = Instantiate(SelectRandomTileModel(), DetermineTileCoordinates(colX, rowY), Quaternion.identity);
                    newSquare.transform.SetParent(this.transform);
                    newSquare.transform.localScale = _squareSizeVector;
                    _squareGrid[colX, rowY] = newSquare.GetComponent<AxisAligner>();
                    _squareGrid[colX, rowY].SetupTile(colX, rowY);
                }
            }
        }
    }

    // Активация игрового времени
    private void TemporalFluxEnabler()
    {
        Time.timeScale = 1f;
    }
}
