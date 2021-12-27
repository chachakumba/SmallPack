using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//col = x
//row = y
public class TetrisManager : MonoBehaviour
{
    public Color boxColor;
    public Color lineColor;
    public Color tColor;
    public Color zColor;
    public Color backZColor;
    public Color lColor;
    public Color backLColor;
    public Color nullColor;
    [Space]
    public Sprite boxSprite;
    public Sprite lineSprite;
    public Sprite tSprite;
    public Sprite zSprite;
    public Sprite backZSprite;
    public Sprite lSprite;
    public Sprite backLSprite;
    public static TetrisManager instance;
    public List<TetrisCell> cells = new List<TetrisCell>();
    public List<TetrisCell> oqupiedCells = new List<TetrisCell>();
    public List<TetrisCell> readyToFall = new List<TetrisCell>();
    public TetrisCell[,] grid = new TetrisCell[10, 20];

    float timeToFall = 0.5f;
    float savedTimeToFall = 0.5f;
    public float initialTimeToFall = 0.5f;

    public TetrisFigure currentFigure;
    [SerializeField]
    GameObject cellsParent;
    public Vector2Int startingPos = new Vector2Int(5, 18);
    bool started = false;
    [Space]
    public int nextFigure;
    [Header("Score")]
    public int score = 0;
    public int countOfFigures = 0;
    public int difficultyLevel = 1;
    public int figuresToNewLevel = 10;
    public float difficultyTimeAdder = 0.02f;
    public int pointsPerFigure = 100;
    public int pointsPerRow = 500;
    [SerializeField]
    TextMeshProUGUI scoreText;
    [SerializeField]
    TextMeshProUGUI maxScoreText;
    [SerializeField]
    Image nextFigureImage;
    [Space]
    [SerializeField]
    GameObject startPanel;
    [SerializeField]
    GameObject losePanel;
    [SerializeField]
    TextMeshProUGUI maxScoreLoseText;
    [SerializeField]
    TextMeshProUGUI scoreLoseText;
    [Space]
    [Header("Sounds")]
    AudioSource audioSource;
    [SerializeField]
    AudioSource musicSource;
    [SerializeField]
    AudioClip backgroundMusic;
    [SerializeField]
    AudioClip loseSound;
    [SerializeField]
    AudioClip rotateSound;
    [SerializeField]
    AudioClip hitSound;
    [SerializeField]
    AudioClip lineSound;
    [SerializeField]
    AudioClip startSound;
    [SerializeField]
    AudioClip moveSound;

    List<int> figuresToSpawn = new List<int>();
    private void Awake()
    {
        instance = this;
        savedTimeToFall = initialTimeToFall;
        timeToFall = savedTimeToFall;
        int num = 0;
        cells = new List<TetrisCell>(cellsParent.GetComponentsInChildren<TetrisCell>());
        foreach (TetrisCell cell in cells)
        {
            cell.col = num % 10;
            cell.row = num / 10;
            grid[cell.col, cell.row] = cell;
            num++;
        }
        for(int i = 0; i < 10; i++)
        {
            for(int j = 0; j < 20; j++)
            {
                if (grid[i,j] == null) Debug.LogError("Null cell " + i + ", " + j);
            }
        }
        audioSource = GetComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.Play();
        Menu.LoadSave();
    }
    public void Restart()
    {
        StartCoroutine(WaitToRestart());
    }
    IEnumerator WaitToRestart()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene("Tetris");
    }
    public void ToMainMenu()
    {
        StartCoroutine(WaitToReturnToMenu());
    }
    IEnumerator WaitToReturnToMenu()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.LoadScene(0);
    }
    private void Start()
    {
        maxScoreText.text = "Max score: " + Menu.save?.tetrisMaxScore;
    }
    private void Update()
    {
        if (Input.anyKeyDown && ! started) StartGame();
        if (Input.GetKeyDown(KeyCode.RightArrow) && currentFigure != null) RightButton();
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentFigure != null) LeftButton();
        if (Input.GetKeyDown(KeyCode.DownArrow)) timeToFall = 0.01f;
        if (Input.GetKeyUp(KeyCode.DownArrow)) timeToFall = savedTimeToFall;
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentFigure != null) RoteteRightButton();
    }
    public void StartGame()
    {
        StartCoroutine(FallCour());
        audioSource.clip = startSound;
        audioSource.Play();
    }
    public void RightButton()
    {
        MoveRight();
        audioSource.clip = moveSound;
        audioSource.Play();
    }
    public void LeftButton()
    {
        MoveLeft();
        audioSource.clip = moveSound;
        audioSource.Play();
    }
    public void RotateLeftButton()
    {
        RotateFigureLeft();
        audioSource.clip = rotateSound;
        audioSource.Play();
    }
    public void RoteteRightButton()
    {
        RotateFigureRight();
        audioSource.clip = rotateSound;
        audioSource.Play();
    }
    public void SpeedUp()
    {
        StartCoroutine(SpeedCoroutine());
    }
    IEnumerator SpeedCoroutine()
    {
        timeToFall = 0.01f;
        yield return new WaitUntil(() => currentFigure == null);
        timeToFall = savedTimeToFall;
    }
    IEnumerator FallCour()
    {
        started = true;
        startPanel.SetActive(false);
        yield return null;
        int rand = Random.Range(0, figuresToSpawn.Count);
        if (figuresToSpawn.Count < 1)
        {
            for (int i = 0; i < 7; i++) figuresToSpawn.Add(i);
        }
        nextFigure = figuresToSpawn[rand];
        figuresToSpawn.RemoveAt(rand);
        CreateNewFigure(GetNewFigure());
        while (true)
        {
            if (currentFigure != null)
            {
                FallReady(currentFigure);
                Fall(currentFigure);
            }
            readyToFall.Clear();
            yield return new WaitForSeconds(timeToFall);

            if (currentFigure == null)
            {
                audioSource.clip = hitSound;
                audioSource.Play();
            }
            bool lined = true;
            while (lined)
            {
                lined = CheckLines();
                if (lined)
                {
                    audioSource.clip = lineSound;
                    audioSource.Play();
                    AddScore(pointsPerRow);
                    yield return new WaitForSeconds(0.7f);
                }
            }

            if (currentFigure == null)
            {
                AddScore(pointsPerFigure);
                countOfFigures++;
                difficultyLevel = (countOfFigures / figuresToNewLevel) + 1;
                timeToFall = initialTimeToFall - difficultyTimeAdder * difficultyLevel;
                savedTimeToFall = timeToFall;
                CreateNewFigure(GetNewFigure());
            }
        }
    }
    public int GetNewFigure()
    {
        int savedFigure = nextFigure;
        if(figuresToSpawn.Count < 1)
        {
            for (int i = 0; i < 7; i++) figuresToSpawn.Add(i);
        }
        int rand = Random.Range(0, figuresToSpawn.Count);
        nextFigure = figuresToSpawn[rand];
        figuresToSpawn.RemoveAt(rand);
        switch (nextFigure)
        {
            case 0:
                nextFigureImage.sprite = boxSprite;
                nextFigureImage.rectTransform.sizeDelta = new Vector2Int(120, 120);
                break;
            case 1:
                nextFigureImage.sprite = lineSprite;
                nextFigureImage.rectTransform.sizeDelta = new Vector2Int(240, 60);
                break;
            case 2:
                nextFigureImage.sprite = tSprite;
                nextFigureImage.rectTransform.sizeDelta = new Vector2Int(180, 120);
                break;
            case 3:
                nextFigureImage.sprite = lSprite;
                nextFigureImage.rectTransform.sizeDelta = new Vector2Int(180, 120);
                break;
            case 4:
                nextFigureImage.sprite = backLSprite;
                nextFigureImage.rectTransform.sizeDelta = new Vector2Int(180, 120);
                break;
            case 5:
                nextFigureImage.sprite = zSprite;
                nextFigureImage.rectTransform.sizeDelta = new Vector2Int(180, 120);
                break;
            case 6:
                nextFigureImage.sprite = backZSprite;
                nextFigureImage.rectTransform.sizeDelta = new Vector2Int(180, 120);
                break;
        }
        return savedFigure;
    }
    public void Lose()
    {
        StopAllCoroutines();
        losePanel.SetActive(true);
        audioSource.clip = loseSound;
        audioSource.Play();
        musicSource.Stop();
        if (score > Menu.save.tetrisMaxScore)
        {
            Menu.SaveTetris(score);
            maxScoreLoseText.text = "New record!";
            scoreLoseText.text = "Score: " + score;
        }
        else
        {
            maxScoreLoseText.text = "Max score: " + Menu.save.tetrisMaxScore;
            scoreLoseText.text = "Score: " + score;
        }
    }
    public void AddScore(int scoreAdd)
    {
        score += scoreAdd * difficultyLevel;
        scoreText.text = "Score: " + score;
    }
    public void CreateNewFigure(int num)
    {
        switch (num)
        {
            case 0:
                currentFigure = CreateSquare(startingPos.x, startingPos.y);
                break;
            case 1:
                currentFigure = CreateLine(startingPos.x, startingPos.y);
                break;
            case 2:
                currentFigure = CreateT(startingPos.x, startingPos.y);
                break;
            case 3:
                currentFigure = CreateL(startingPos.x, startingPos.y);
                break;
            case 4:
                currentFigure = CreateBackL(startingPos.x, startingPos.y);
                break;
            case 5:
                currentFigure = CreateZ(startingPos.x, startingPos.y);
                break;
            case 6:
                currentFigure = CreateBackZ(startingPos.x, startingPos.y);
                break;
        }
    }
    public void RotateFigureRight()
    {
        Vector2Int[] newVectors = new Vector2Int[4];

        for(int  i = 0; i < currentFigure.posOfAddPixels.Length; i++)
        {
            newVectors[i] = new Vector2Int(currentFigure.posOfAddPixels[i].y, -currentFigure.posOfAddPixels[i].x);
        }

        bool canRotate = true;
        foreach (Vector2Int vec in newVectors)
        {
            if (vec.x + currentFigure.colOfCenter < 10 && vec.x + currentFigure.colOfCenter >= 0 && vec.y + currentFigure.rowOfCenter >= 0 && vec.y + currentFigure.rowOfCenter < 20)
            {
                if (grid[vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter].ocupied)
                {
                    if ((new Vector2Int(vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.posOfAddPixels[0].x + currentFigure.colOfCenter, currentFigure.posOfAddPixels[0].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.posOfAddPixels[1].x + currentFigure.colOfCenter, currentFigure.posOfAddPixels[1].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.posOfAddPixels[2].x + currentFigure.colOfCenter, currentFigure.posOfAddPixels[2].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.colOfCenter, currentFigure.rowOfCenter)))
                        canRotate = true;
                    else
                    {
                        canRotate = false;
                        break;
                    }
                }
            }
            else
            {
                canRotate = false;
                break;
            }
        }
        
        if (canRotate)
        {
            foreach (Vector2Int vec in currentFigure.posOfAddPixels)
            {
                grid[vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter].Clear();
            }
            currentFigure.posOfAddPixels = newVectors;
            foreach (Vector2Int vec in currentFigure.posOfAddPixels)
            {
                grid[vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter].Paint(currentFigure.color);
            }
        }

    }
    public void RotateFigureLeft()
    {
        Vector2Int[] newVectors = new Vector2Int[4];

        for (int i = 0; i < currentFigure.posOfAddPixels.Length; i++)
        {
            newVectors[i] = new Vector2Int(-currentFigure.posOfAddPixels[i].y, currentFigure.posOfAddPixels[i].x);
        }

        bool canRotate = true;
        foreach (Vector2Int vec in newVectors)
        {
            if (vec.x + currentFigure.colOfCenter < 10 && vec.x + currentFigure.colOfCenter >= 0 && vec.y + currentFigure.rowOfCenter >= 0 && vec.y + currentFigure.rowOfCenter < 20)
            {
                if (grid[vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter].ocupied)
                {
                    if ((new Vector2Int(vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.posOfAddPixels[0].x + currentFigure.colOfCenter, currentFigure.posOfAddPixels[0].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.posOfAddPixels[1].x + currentFigure.colOfCenter, currentFigure.posOfAddPixels[1].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.posOfAddPixels[2].x + currentFigure.colOfCenter, currentFigure.posOfAddPixels[2].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.colOfCenter, currentFigure.rowOfCenter)))
                        canRotate = true;
                    else
                    {
                        canRotate = false;
                        break;
                    }
                }
            }
            else
            {
                canRotate = false;
                break;
            }
        }

        if (canRotate)
        {
            foreach (Vector2Int vec in currentFigure.posOfAddPixels)
            {
                grid[vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter].Clear();
            }
            currentFigure.posOfAddPixels = newVectors;
            foreach (Vector2Int vec in currentFigure.posOfAddPixels)
            {
                grid[vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter].Paint(currentFigure.color);
            }
        }

    }
    public void MoveRight()
    {
        List<Vector2Int> rightestCells = new List<Vector2Int>(currentFigure.posOfAddPixels);
        List<Vector2Int> sortedRightestCells = new List<Vector2Int>();
        for (int i = 0; i < currentFigure.posOfAddPixels.Length; i++)
        {
            int rightestCel = -10;
            foreach (Vector2Int vec in rightestCells)
            {
                if (vec.x > rightestCel)
                {
                    rightestCel = vec.x;
                }
            }
            if (rightestCel + currentFigure.colOfCenter >= 9) break;
            foreach (Vector2Int vec in rightestCells)
            {
                if (vec.x == rightestCel)
                {
                    sortedRightestCells.Add(vec);
                    rightestCells.Remove(vec);
                    break;
                }
            }
        }
        if (sortedRightestCells.Count == 4)
        {
            bool canMove = true;
            foreach (Vector2Int vec in sortedRightestCells)
            {
                if (grid[vec.x + currentFigure.colOfCenter + 1, vec.y + currentFigure.rowOfCenter].ocupied)
                {
                    if ((new Vector2Int(vec.x + currentFigure.colOfCenter + 1, vec.y + currentFigure.rowOfCenter) == new Vector2Int(sortedRightestCells[0].x + currentFigure.colOfCenter, sortedRightestCells[0].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter + 1, vec.y + currentFigure.rowOfCenter) == new Vector2Int(sortedRightestCells[1].x + currentFigure.colOfCenter, sortedRightestCells[1].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter + 1, vec.y + currentFigure.rowOfCenter) == new Vector2Int(sortedRightestCells[2].x + currentFigure.colOfCenter, sortedRightestCells[2].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter + 1, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.colOfCenter, currentFigure.rowOfCenter)))
                        canMove = true;
                    else
                    {
                        canMove = false;
                        break;
                    }
                }
            }
            if (canMove)
            {
                foreach (Vector2Int vec in sortedRightestCells)
                {
                    MoveCellRight(grid[vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter]);
                }
                currentFigure.colOfCenter++;
            }
        }
    }
    public void MoveCellRight(TetrisCell cell)
    {
        if (cell.col < 9 && cell.ocupied)
        {
            if (grid[cell.col + 1, cell.row] != null && !grid[cell.col + 1, cell.row].ocupied)
            {
                grid[cell.col + 1, cell.row].Paint(currentFigure.color);
                cell.Clear();
            }
        }
        cell.readyTofall = false;
    }
    public void MoveLeft()
    {
        List<Vector2Int> leftestCells = new List<Vector2Int>(currentFigure.posOfAddPixels);
        List<Vector2Int> sortedLeftestCells = new List<Vector2Int>();
        for (int i = 0; i < currentFigure.posOfAddPixels.Length; i++)
        {
            int leftestCel = 11;
            foreach (Vector2Int vec in leftestCells)
            {
                if (vec.x < leftestCel)
                {
                    leftestCel = vec.x;
                }
            }
            if (leftestCel + currentFigure.colOfCenter <= 0) break;
            foreach (Vector2Int vec in leftestCells)
            {
                if (vec.x == leftestCel)
                {
                    sortedLeftestCells.Add(vec);
                    leftestCells.Remove(vec);
                    break;
                }
            }
        }
        if (sortedLeftestCells.Count == 4)
        {
            bool canMove = true;
            foreach (Vector2Int vec in sortedLeftestCells)
            {
                if (grid[vec.x + currentFigure.colOfCenter - 1, vec.y + currentFigure.rowOfCenter].ocupied)
                    if ((new Vector2Int(vec.x + currentFigure.colOfCenter - 1, vec.y + currentFigure.rowOfCenter) == new Vector2Int(sortedLeftestCells[0].x + currentFigure.colOfCenter, sortedLeftestCells[0].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter - 1, vec.y + currentFigure.rowOfCenter) == new Vector2Int(sortedLeftestCells[1].x + currentFigure.colOfCenter, sortedLeftestCells[1].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter - 1, vec.y + currentFigure.rowOfCenter) == new Vector2Int(sortedLeftestCells[2].x + currentFigure.colOfCenter, sortedLeftestCells[2].y + currentFigure.rowOfCenter) ||
                    new Vector2Int(vec.x + currentFigure.colOfCenter - 1, vec.y + currentFigure.rowOfCenter) == new Vector2Int(currentFigure.colOfCenter, currentFigure.rowOfCenter)))
                    {
                        canMove = true;
                    }
                    else
                    {
                        canMove = false;
                        break;
                    }
            }
            if (canMove)
            {
                foreach (Vector2Int vec in sortedLeftestCells)
                {
                    MoveCellLeft(grid[vec.x + currentFigure.colOfCenter, vec.y + currentFigure.rowOfCenter]);
                }
                currentFigure.colOfCenter--;
            }
        }
    }
    public void MoveCellLeft(TetrisCell cell)
    {
        if (cell.col > 0 && cell.ocupied)
        {
            if (grid[cell.col - 1, cell.row] != null && !grid[cell.col - 1, cell.row].ocupied)
            {
                grid[cell.col - 1, cell.row].Paint(currentFigure.color);
                cell.Clear();
            }
        }
        cell.readyTofall = false;
    }
    public bool CheckLines()
    {
        if (currentFigure == null)
            for (int i = 0; i < 20; i++)
            {
                bool full = true;
                for (int j = 0; j < 10; j++)
                {
                    if (!grid[j, i].ocupied) { full = false; break; }
                }
                if (full)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        grid[j, i].Clear();
                    }
                    for(int j = i; j < 20; j++)
                    {
                        for(int l = 0; l < 10; l++)
                        {
                            Fall(grid[l, j]);
                        }
                    }
                    return true;
                }
            }
        return false;
    }
    public void FallReady(TetrisFigure figure)
    {
        foreach (Vector2Int vec in figure.posOfAddPixels)
            FallReady(grid[figure.colOfCenter + vec.x, figure.rowOfCenter + vec.y]);
    }
    public void Fall(TetrisFigure figure)
    {
        bool canFall = true;
        if (figure.rowOfCenter - 1 < 0 || figure.rowOfCenter + figure.posOfAddPixels[0].y - 1 < 0 || figure.rowOfCenter + figure.posOfAddPixels[1].y - 1 < 0 || figure.rowOfCenter + figure.posOfAddPixels[2].y - 1 < 0)
        {
            canFall = false;
            currentFigure = null;
            grid[figure.colOfCenter + figure.posOfAddPixels[0].x, figure.rowOfCenter + figure.posOfAddPixels[0].y].readyTofall = false;
            grid[figure.colOfCenter + figure.posOfAddPixels[1].x, figure.rowOfCenter + figure.posOfAddPixels[1].y].readyTofall = false;
            grid[figure.colOfCenter + figure.posOfAddPixels[2].x, figure.rowOfCenter + figure.posOfAddPixels[2].y].readyTofall = false;
            grid[figure.colOfCenter + figure.posOfAddPixels[3].x, figure.rowOfCenter + figure.posOfAddPixels[3].y].readyTofall = false;
        }
        else
        {
            if (grid[figure.colOfCenter, figure.rowOfCenter - 1].ocupied)
                if (!grid[figure.colOfCenter, figure.rowOfCenter - 1].readyTofall)
                    canFall = false;
            if (grid[figure.colOfCenter + figure.posOfAddPixels[0].x, figure.rowOfCenter + figure.posOfAddPixels[0].y - 1].ocupied)
                if (!grid[figure.colOfCenter + figure.posOfAddPixels[0].x, figure.rowOfCenter + figure.posOfAddPixels[0].y - 1].readyTofall)
                    canFall = false;
            if (grid[figure.colOfCenter + figure.posOfAddPixels[1].x, figure.rowOfCenter + figure.posOfAddPixels[1].y - 1].ocupied)
                if (!grid[figure.colOfCenter + figure.posOfAddPixels[1].x, figure.rowOfCenter + figure.posOfAddPixels[1].y - 1].readyTofall)
                    canFall = false;
            if (grid[figure.colOfCenter + figure.posOfAddPixels[2].x, figure.rowOfCenter + figure.posOfAddPixels[2].y - 1].ocupied)
                if (!grid[figure.colOfCenter + figure.posOfAddPixels[2].x, figure.rowOfCenter + figure.posOfAddPixels[2].y - 1].readyTofall)
                    canFall = false;
        }
        if (canFall)
        {
            List<Vector2Int> fallingCells = new List<Vector2Int>(currentFigure.posOfAddPixels);
            List<Vector2Int> sortedFallingCells = new List<Vector2Int>();
            for (int i = 0; i < currentFigure.posOfAddPixels.Length; i++)
            {
                int lowestCell = 20;
                foreach (Vector2Int vec in fallingCells)
                {
                    if (vec.y < lowestCell)
                    {
                        lowestCell = vec.y;
                    }
                }
                foreach (Vector2Int vec in fallingCells)
                {
                    if (vec.y == lowestCell)
                    {
                        sortedFallingCells.Add(vec);
                        fallingCells.Remove(vec);
                        break;
                    }
                }
            }
            for (int i = 0; i < sortedFallingCells.Count; i++)
            {
                Fall(grid[figure.colOfCenter + sortedFallingCells[i].x, figure.rowOfCenter + sortedFallingCells[i].y]);
            }
            figure.rowOfCenter -= 1;
        }
        else
        {
            canFall = false;
            currentFigure = null;
            grid[figure.colOfCenter + figure.posOfAddPixels[0].x, figure.rowOfCenter + figure.posOfAddPixels[0].y].readyTofall = false;
            grid[figure.colOfCenter + figure.posOfAddPixels[1].x, figure.rowOfCenter + figure.posOfAddPixels[1].y].readyTofall = false;
            grid[figure.colOfCenter + figure.posOfAddPixels[2].x, figure.rowOfCenter + figure.posOfAddPixels[2].y].readyTofall = false;
            grid[figure.colOfCenter + figure.posOfAddPixels[3].x, figure.rowOfCenter + figure.posOfAddPixels[3].y].readyTofall = false;
        }
    }
    //Creation of figures
    #region 
    public TetrisFigure CreateSquare(int colOfCenter, int rowOfCenter)
    {
        TetrisFigure newFigure = new TetrisFigure(colOfCenter, rowOfCenter);
        newFigure.color = boxColor;

        newFigure.posOfAddPixels[0] = new Vector2Int(1, 0);
        newFigure.posOfAddPixels[1] = new Vector2Int(0, 1);
        newFigure.posOfAddPixels[2] = new Vector2Int(1, 1);

        foreach(Vector2Int cell in newFigure.posOfAddPixels)
        {
            if (grid[newFigure.colOfCenter + cell.x, newFigure.rowOfCenter + cell.y].ocupied)
            {
                Lose();
                return null;
            }
        }

        grid[newFigure.colOfCenter, newFigure.rowOfCenter].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[0].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[0].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[1].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[1].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[2].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[2].y].Paint(newFigure.color);

        return newFigure;
    }
    public TetrisFigure CreateLine(int colOfCenter, int rowOfCenter)
    {
        TetrisFigure newFigure = new TetrisFigure(colOfCenter, rowOfCenter);
        newFigure.color = lineColor;

        newFigure.posOfAddPixels[0] = new Vector2Int(1, 0);
        newFigure.posOfAddPixels[1] = new Vector2Int(2, 0);
        newFigure.posOfAddPixels[2] = new Vector2Int(-1, 0);

        foreach (Vector2Int cell in newFigure.posOfAddPixels)
        {
            if (grid[newFigure.colOfCenter + cell.x, newFigure.rowOfCenter + cell.y].ocupied)
            {
                Lose();
                return null;
            }
        }

        grid[newFigure.colOfCenter, newFigure.rowOfCenter].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[0].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[0].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[1].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[1].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[2].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[2].y].Paint(newFigure.color);

        return newFigure;
    }
    public TetrisFigure CreateT(int colOfCenter, int rowOfCenter)
    {
        TetrisFigure newFigure = new TetrisFigure(colOfCenter, rowOfCenter);
        newFigure.color = tColor;

        newFigure.posOfAddPixels[0] = new Vector2Int(1, 0);
        newFigure.posOfAddPixels[1] = new Vector2Int(-1, 0);
        newFigure.posOfAddPixels[2] = new Vector2Int(0, 1);

        foreach (Vector2Int cell in newFigure.posOfAddPixels)
        {
            if (grid[newFigure.colOfCenter + cell.x, newFigure.rowOfCenter + cell.y].ocupied)
            {
                Lose();
                return null;
            }
        }

        grid[newFigure.colOfCenter, newFigure.rowOfCenter].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[0].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[0].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[1].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[1].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[2].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[2].y].Paint(newFigure.color);

        return newFigure;
    }
    public TetrisFigure CreateL(int colOfCenter, int rowOfCenter)
    {
        TetrisFigure newFigure = new TetrisFigure(colOfCenter, rowOfCenter);
        newFigure.color = lColor;

        newFigure.posOfAddPixels[0] = new Vector2Int(1, 0);
        newFigure.posOfAddPixels[1] = new Vector2Int(-1, 0);
        newFigure.posOfAddPixels[2] = new Vector2Int(1, 1);

        foreach (Vector2Int cell in newFigure.posOfAddPixels)
        {
            if (grid[newFigure.colOfCenter + cell.x, newFigure.rowOfCenter + cell.y].ocupied)
            {
                Lose();
                return null;
            }
        }

        grid[newFigure.colOfCenter, newFigure.rowOfCenter].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[0].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[0].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[1].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[1].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[2].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[2].y].Paint(newFigure.color);

        return newFigure;
    }
    public TetrisFigure CreateBackL(int colOfCenter, int rowOfCenter)
    {
        TetrisFigure newFigure = new TetrisFigure(colOfCenter, rowOfCenter);
        newFigure.color = backLColor;

        newFigure.posOfAddPixels[0] = new Vector2Int(1, 0);
        newFigure.posOfAddPixels[1] = new Vector2Int(-1, 0);
        newFigure.posOfAddPixels[2] = new Vector2Int(-1, 1);

        foreach (Vector2Int cell in newFigure.posOfAddPixels)
        {
            if (grid[newFigure.colOfCenter + cell.x, newFigure.rowOfCenter + cell.y].ocupied)
            {
                Lose();
                return null;
            }
        }

        grid[newFigure.colOfCenter, newFigure.rowOfCenter].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[0].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[0].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[1].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[1].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[2].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[2].y].Paint(newFigure.color);

        return newFigure;
    }
    public TetrisFigure CreateZ(int colOfCenter, int rowOfCenter)
    {
        TetrisFigure newFigure = new TetrisFigure(colOfCenter, rowOfCenter);
        newFigure.color = zColor;

        newFigure.posOfAddPixels[0] = new Vector2Int(-1, 0);
        newFigure.posOfAddPixels[1] = new Vector2Int(0, 1);
        newFigure.posOfAddPixels[2] = new Vector2Int(1, 1);

        foreach (Vector2Int cell in newFigure.posOfAddPixels)
        {
            if (grid[newFigure.colOfCenter + cell.x, newFigure.rowOfCenter + cell.y].ocupied)
            {
                Lose();
                return null;
            }
        }

        grid[newFigure.colOfCenter, newFigure.rowOfCenter].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[0].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[0].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[1].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[1].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[2].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[2].y].Paint(newFigure.color);

        return newFigure;
    }
    public TetrisFigure CreateBackZ(int colOfCenter, int rowOfCenter)
    {
        TetrisFigure newFigure = new TetrisFigure(colOfCenter, rowOfCenter);
        newFigure.color = backZColor;

        newFigure.posOfAddPixels[0] = new Vector2Int(1, 0);
        newFigure.posOfAddPixels[1] = new Vector2Int(0, 1);
        newFigure.posOfAddPixels[2] = new Vector2Int(-1, 1);

        foreach (Vector2Int cell in newFigure.posOfAddPixels)
        {
            if (grid[newFigure.colOfCenter + cell.x, newFigure.rowOfCenter + cell.y].ocupied)
            {
                Lose();
                return null;
            }
        }

        grid[newFigure.colOfCenter, newFigure.rowOfCenter].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[0].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[0].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[1].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[1].y].Paint(newFigure.color);
        grid[newFigure.colOfCenter + newFigure.posOfAddPixels[2].x, newFigure.rowOfCenter + newFigure.posOfAddPixels[2].y].Paint(newFigure.color);

        return newFigure;
    }

    #endregion
    public void CreateCell(int col, int row, Color color)
    {
        if(grid[col, row] == null) Debug.LogError("Null cell");
        grid[col, row].Paint(color);
    }
    public void FallReady(TetrisCell cell)
    {
        cell.readyTofall = true;
        readyToFall.Add(cell);
    }
    public void Fall(TetrisCell cell)
    {
        if (cell.row > 0 && cell.ocupied)
        {
            if (grid[cell.col, cell.row - 1] != null && (!grid[cell.col, cell.row - 1].ocupied || grid[cell.col, cell.row - 1].readyTofall))
            {
                grid[cell.col, cell.row - 1].Paint(cell.image.color);
                cell.Clear();
            }
        }
        cell.readyTofall = false;
    }
}

public class TetrisFigure
{
    public int colOfCenter;
    public int rowOfCenter;
    public Vector2Int[] posOfAddPixels = new Vector2Int[4];
    public Color color;
    public TetrisFigure(int newColOfCenter, int newRowOfCenter) { colOfCenter = newColOfCenter; rowOfCenter = newRowOfCenter; posOfAddPixels[3] = Vector2Int.zero; }
}