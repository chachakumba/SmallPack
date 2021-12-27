using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public enum TicTacToeResult
{
    win, lose, tie
}
public class TicTacToeManager : MonoBehaviour
{
    public static TicTacToeManager instance;
    public TicTacToeCell[,] cells = new TicTacToeCell[3,3];
    public List<TicTacToeCell> crossCells = new List<TicTacToeCell>();
    public List<TicTacToeCell> circleCells = new List<TicTacToeCell>();
    public List<TicTacToeCell> emptyCells = new List<TicTacToeCell>();
    public GameObject[] cellsGameObjects1stRow = new GameObject[3];
    public GameObject[] cellsGameObjects2ndRow = new GameObject[3];
    public GameObject[] cellsGameObjects3rdRow = new GameObject[3];
    public Sprite cross;
    public Sprite circle;
    public bool isCrossesTurn = true;
    public TicTacToe aiTeam = TicTacToe.circle;
    public int numOfRows = 3;
    public int numOfCols = 3;
    public bool aiMovesOnCircles = true;

    [SerializeField]
    AudioClip loseSound;
    [SerializeField]
    AudioClip winSound;
    [SerializeField]
    AudioClip tieSound;
    [SerializeField]
    public AudioClip clickSound;
    AudioSource audioSource;
    public bool isGameStop = false;
    [SerializeField]
    TextMeshProUGUI resultText;
    [SerializeField]
    TextMeshProUGUI winsText;
    [SerializeField]
    TextMeshProUGUI loseText;
    [SerializeField]
    TextMeshProUGUI totalGamesText;
    [SerializeField]
    GameObject resultPanel;
    [SerializeField]
    AudioSource musicSource;
    public AudioClip backgroundMusic;
    private void Awake()
    {
        instance = this;
        for(int i = 0; i < 3; i++)
        {
            cells[0, i] = cellsGameObjects1stRow[i].GetComponent<TicTacToeCell>();
        }
        for (int i = 0; i < 3; i++)
        {
            cells[1, i] = cellsGameObjects2ndRow[i].GetComponent<TicTacToeCell>();
        }
        for (int i = 0; i < 3; i++)
        {
            cells[2, i] = cellsGameObjects3rdRow[i].GetComponent<TicTacToeCell>();
        }
        foreach(TicTacToeCell cell in cells)
        {
            emptyCells.Add(cell);
        }
        audioSource = GetComponent<AudioSource>();
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

    public void Restart()
    {
        foreach(TicTacToeCell cell in cells)
        {
            cell.Clear();
        }
        resultPanel.SetActive(false);
        isCrossesTurn = true;
        isGameStop = false;
    }
    public void ToMainMenu()
    {
        StartCoroutine(WaitYoReturnToMenu());
    }
    IEnumerator WaitYoReturnToMenu()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }
    public void Set(int num)
    {
        int row = num / 3;
        int col = num % 3;
        //Debug.Log("row: " + row + " col: " + col);
        if (isCrossesTurn)
        {
            cells[row, col].SetCross();
            isCrossesTurn = !isCrossesTurn;
        }
        else
        {
            cells[row, col].SetCircle();
            isCrossesTurn = !isCrossesTurn;
        }
        if (CheckWin(num))
        {
            if (!isCrossesTurn)
            {
                if (aiMovesOnCircles)
                {
                    Win();
                }
                else
                {
                    Lose();
                }
                Debug.LogWarning("Crosses win");
            }
            else
            {
                if (!aiMovesOnCircles)
                {
                    Win();
                }
                else
                {
                    Lose();
                }
                Debug.LogWarning("Circles win");

            }
        }
        else
        {
            if (emptyCells.Count == 0)
            {
                Tie();
            }
            else if (aiMovesOnCircles && !isCrossesTurn || !aiMovesOnCircles && isCrossesTurn)
            {
                MoveAI();
            }
        }
    }
    void Win()
    {
        isGameStop = true;
        Menu.SaveTicTacToe(TicTacToeResult.win);
        StartCoroutine(playMusicWithDelay(winSound));

        resultText.text = "You win!";
        winsText.text = "Wins: " + Menu.save.ticTacToeWins;
        loseText.text = "Loses: " + Menu.save.ticTacToeLoses;
        totalGamesText.text = "Total played: " + Menu.save.ticTacToeTotalPlayed;
        resultPanel.SetActive(true);
    }
    void Lose()
    {
        isGameStop = true;
        Menu.SaveTicTacToe(TicTacToeResult.lose);
        StartCoroutine(playMusicWithDelay(loseSound));

        resultText.text = "You lose";
        winsText.text = "Wins: " + Menu.save.ticTacToeWins;
        loseText.text = "Loses: " + Menu.save.ticTacToeLoses;
        totalGamesText.text = "Total played: " + Menu.save.ticTacToeTotalPlayed;
        resultPanel.SetActive(true);
    }
    void Tie()
    {
        isGameStop = true;
        Menu.SaveTicTacToe(TicTacToeResult.tie);
        StartCoroutine(playMusicWithDelay(tieSound));

        resultText.text = "Tie";
        winsText.text = "Wins: " + Menu.save.ticTacToeWins;
        loseText.text = "Loses: " + Menu.save.ticTacToeLoses;
        totalGamesText.text = "Total played: " + Menu.save.ticTacToeTotalPlayed;
        resultPanel.SetActive(true);
    }
    public void OnButtonPress()
    {
        audioSource.clip = Menu.instance.buttonPress;
        audioSource.Play();
    }
    public void OnButtonAllow()
    {
        audioSource.clip = Menu.instance.buttonAllow;
        audioSource.Play();
    }
    public void OnButtonDeny()
    {
        audioSource.clip = Menu.instance.buttonDeny;
        audioSource.Play();
    }
    IEnumerator playMusicWithDelay(AudioClip clip)
    {
        yield return new WaitForSeconds(0.5f);
        audioSource.clip = clip;
        audioSource.Play();
    }
    void MoveAI()
    {
        bool moved = false;
        if (isCrossesTurn)
        {
            moved = CheckWinCross();
            if (!moved)
            {
                moved = CheckWinCircles();
                if (!moved)
                {
                    if (emptyCells.Count > 0)
                    {
                        emptyCells[Random.Range(0, emptyCells.Count)].Set();
                    }
                    else
                    {
                        Tie();
                        Debug.Log("Can't move");
                    }
                }
            }
        }
        else
        {
            moved = CheckWinCircles();
            if (!moved)
            {
                moved = CheckWinCross();
                if (!moved)
                {
                    if (emptyCells.Count > 0)
                    {
                        emptyCells[Random.Range(0, emptyCells.Count)].Set();
                    }
                    else
                    {
                        Tie();
                        Debug.Log("Can't move");
                    }
                }
            }
        }
    }
    bool CheckWinCircles()
    {
        TicTacToe aiTeam = TicTacToe.circle;
        foreach (TicTacToeCell circle in circleCells)
        {
            int row = circle.num / 3;
            int col = circle.num % 3;
            if (row + 1 < numOfRows && row - 1 >= 0)
            {
                if (cells[row + 1, col].state == aiTeam && cells[row - 1, col].state == TicTacToe.none)
                {
                    cells[row - 1, col].Set();
                    return true;
                }
                if (cells[row + 1, col].state == TicTacToe.none && cells[row - 1, col].state == aiTeam)
                {
                    cells[row + 1, col].Set();
                    return true;
                }
            }
            if (col + 1 < numOfCols && col - 1 >= 0)
            {
                if (cells[row, col + 1].state == aiTeam && cells[row, col - 1].state == TicTacToe.none)
                {
                    cells[row, col - 1].Set();
                    return true;
                }
                if (cells[row, col + 1].state == TicTacToe.none && cells[row, col - 1].state == aiTeam)
                {
                    cells[row, col + 1].Set();
                    return true;
                }
            }
            if (row + 1 < numOfRows && col + 1 < numOfCols && row - 1 >= 0 && col - 1 >= 0)
            {
                if (cells[row + 1, col + 1].state == aiTeam && cells[row - 1, col - 1].state == TicTacToe.none)
                {
                    cells[row - 1, col - 1].Set();
                    return true;
                }

                if (cells[row + 1, col + 1].state == TicTacToe.none && cells[row - 1, col - 1].state == aiTeam)
                {
                    cells[row + 1, col + 1].Set();
                    return true;
                }

                if (cells[row - 1, col + 1].state == aiTeam && cells[row + 1, col - 1].state == TicTacToe.none)
                {
                    cells[row + 1, col - 1].Set();
                    return true;
                }

                if (cells[row - 1, col + 1].state == TicTacToe.none && cells[row + 1, col - 1].state == aiTeam)
                {
                    cells[row - 1, col + 1].Set();
                    return true;
                }
            }


            if (row + 2 < numOfRows)
            {
                if (cells[row + 1, col].state == aiTeam && cells[row + 2, col].state == TicTacToe.none)
                {
                    cells[row + 2, col].Set();
                    return true;
                }

                if (cells[row + 1, col].state == TicTacToe.none && cells[row + 2, col].state == aiTeam)
                {
                    cells[row + 1, col].Set();
                    return true;
                }
            }
            if (col + 2 < numOfCols)
            {
                if (cells[row, col + 1].state == aiTeam && cells[row, col + 2].state == TicTacToe.none)
                {
                    cells[row, col + 2].Set();
                    return true;
                }

                if (cells[row, col + 1].state == TicTacToe.none && cells[row, col + 2].state == aiTeam)
                {
                    cells[row, col + 1].Set();
                    return true;
                }
            }
            if (row - 2 >= 0)
            {
                if (cells[row - 1, col].state == aiTeam && cells[row - 2, col].state == TicTacToe.none)
                {
                    cells[row - 2, col].Set();
                    return true;
                }

                if (cells[row - 1, col].state == TicTacToe.none && cells[row - 2, col].state == aiTeam)
                {
                    cells[row - 1, col].Set();
                    return true;
                }
            }
            if (col - 2 >= 0)
            {
                if (cells[row, col - 1].state == aiTeam && cells[row, col - 2].state == TicTacToe.none)
                {
                    cells[row, col - 2].Set();
                    return true;
                }

                if (cells[row, col - 1].state == TicTacToe.none && cells[row, col - 2].state == aiTeam)
                {
                    cells[row, col - 1].Set();
                    return true;
                }
            }


            if (row + 2 < numOfRows && col + 2 < numOfCols)
            {
                if (cells[row + 1, col + 1].state == aiTeam && cells[row + 2, col + 2].state == TicTacToe.none)
                {
                    cells[row + 2, col + 2].Set();
                    return true;
                }


                if (cells[row + 1, col + 1].state == TicTacToe.none && cells[row + 2, col + 2].state == aiTeam)
                {
                    cells[row + 1, col + 1].Set();
                    return true;
                }
            }
            if (row - 2 >= 0 && col + 2 < numOfCols)
            {
                if (cells[row - 1, col + 1].state == aiTeam && cells[row - 2, col + 2].state == TicTacToe.none)
                {
                    cells[row - 2, col + 2].Set();
                    return true;
                }


                if (cells[row - 1, col + 1].state == TicTacToe.none && cells[row - 2, col + 2].state == aiTeam)
                {
                    cells[row - 1, col + 1].Set();
                    return true;
                }
            }
            if (row + 2 < numOfRows && col - 2 >= 0)
            {
                if (cells[row + 1, col - 1].state == aiTeam && cells[row + 2, col - 2].state == TicTacToe.none)
                {
                    cells[row + 2, col - 2].Set();
                    return true;
                }

                if (cells[row + 1, col - 1].state == TicTacToe.none && cells[row + 2, col - 2].state == aiTeam)
                {
                    cells[row + 1, col - 1].Set();
                    return true;
                }
            }
            if (row - 2 >= 0 && col - 2 >= 0)
            {
                if (cells[row - 1, col - 1].state == aiTeam && cells[row - 2, col - 2].state == TicTacToe.none)
                {
                    cells[row - 2, col - 2].Set();
                    return true;
                }

                if (cells[row - 1, col - 1].state == TicTacToe.none && cells[row - 2, col - 2].state == aiTeam)
                {
                    cells[row - 1, col - 1].Set();
                    return true;
                }
            }
        }
        return false;
    }
    bool CheckWinCross()
    {
        TicTacToe aiTeam = TicTacToe.cross;
        foreach (TicTacToeCell cross in crossCells)
        {
            int row = cross.num / 3;
            int col = cross.num % 3;
            if (row + 1 < numOfRows && row - 1 >= 0)
            {
                if (cells[row + 1, col].state == aiTeam && cells[row - 1, col].state == TicTacToe.none)
                {
                    cells[row - 1, col].Set();
                    return true;
                }
                if (cells[row + 1, col].state == TicTacToe.none && cells[row - 1, col].state == aiTeam)
                {
                    cells[row + 1, col].Set();
                    return true;
                }
            }
            if (col + 1 < numOfCols && col - 1 >= 0)
            {
                if (cells[row, col + 1].state == aiTeam && cells[row, col - 1].state == TicTacToe.none)
                {
                    cells[row, col - 1].Set();
                    return true;
                }
                if (cells[row, col + 1].state == TicTacToe.none && cells[row, col - 1].state == aiTeam)
                {
                    cells[row, col + 1].Set();
                    return true;
                }
            }
            if (row + 1 < numOfRows && col + 1 < numOfCols && row - 1 >= 0 && col - 1 >= 0)
            {
                if (cells[row + 1, col + 1].state == aiTeam && cells[row - 1, col - 1].state == TicTacToe.none)
                {
                    cells[row - 1, col - 1].Set();
                    return true;
                }

                if (cells[row + 1, col + 1].state == TicTacToe.none && cells[row - 1, col - 1].state == aiTeam)
                {
                    cells[row + 1, col + 1].Set();
                    return true;
                }

                if (cells[row - 1, col + 1].state == aiTeam && cells[row + 1, col - 1].state == TicTacToe.none)
                {
                    cells[row + 1, col - 1].Set();
                    return true;
                }

                if (cells[row - 1, col + 1].state == TicTacToe.none && cells[row + 1, col - 1].state == aiTeam)
                {
                    cells[row - 1, col + 1].Set();
                    return true;
                }
            }


            if (row + 2 < numOfRows)
            {
                if (cells[row + 1, col].state == aiTeam && cells[row + 2, col].state == TicTacToe.none)
                {
                    cells[row + 2, col].Set();
                    return true;
                }

                if (cells[row + 1, col].state == TicTacToe.none && cells[row + 2, col].state == aiTeam)
                {
                    cells[row + 1, col].Set();
                    return true;
                }
            }
            if (col + 2 < numOfCols)
            {
                if (cells[row, col + 1].state == aiTeam && cells[row, col + 2].state == TicTacToe.none)
                {
                    cells[row, col + 2].Set();
                    return true;
                }

                if (cells[row, col + 1].state == TicTacToe.none && cells[row, col + 2].state == aiTeam)
                {
                    cells[row, col + 1].Set();
                    return true;
                }
            }
            if (row - 2 >= 0)
            {
                if (cells[row - 1, col].state == aiTeam && cells[row - 2, col].state == TicTacToe.none)
                {
                    cells[row - 2, col].Set();
                    return true;
                }

                if (cells[row - 1, col].state == TicTacToe.none && cells[row - 2, col].state == aiTeam)
                {
                    cells[row - 1, col].Set();
                    return true;
                }
            }
            if (col - 2 >= 0)
            {
                if (cells[row, col - 1].state == aiTeam && cells[row, col - 2].state == TicTacToe.none)
                {
                    cells[row, col - 2].Set();
                    return true;
                }

                if (cells[row, col - 1].state == TicTacToe.none && cells[row, col - 2].state == aiTeam)
                {
                    cells[row, col - 1].Set();
                    return true;
                }
            }


            if (row + 2 < numOfRows && col + 2 < numOfCols)
            {
                if (cells[row + 1, col + 1].state == aiTeam && cells[row + 2, col + 2].state == TicTacToe.none)
                {
                    cells[row + 2, col + 2].Set();
                    return true;
                }


                if (cells[row + 1, col + 1].state == TicTacToe.none && cells[row + 2, col + 2].state == aiTeam)
                {
                    cells[row + 1, col + 1].Set();
                    return true;
                }
            }
            if (row - 2 >= 0 && col + 2 < numOfCols)
            {
                if (cells[row - 1, col + 1].state == aiTeam && cells[row - 2, col + 2].state == TicTacToe.none)
                {
                    cells[row - 2, col + 2].Set();
                    return true;
                }


                if (cells[row - 1, col + 1].state == TicTacToe.none && cells[row - 2, col + 2].state == aiTeam)
                {
                    cells[row - 1, col + 1].Set();
                    return true;
                }
            }
            if (row + 2 < numOfRows && col - 2 >= 0)
            {
                if (cells[row + 1, col - 1].state == aiTeam && cells[row + 2, col - 2].state == TicTacToe.none)
                {
                    cells[row + 2, col - 2].Set();
                    return true;
                }

                if (cells[row + 1, col - 1].state == TicTacToe.none && cells[row + 2, col - 2].state == aiTeam)
                {
                    cells[row + 1, col - 1].Set();
                    return true;
                }
            }
            if (row - 2 >= 0 && col - 2 >= 0)
            {
                if (cells[row - 1, col - 1].state == aiTeam && cells[row - 2, col - 2].state == TicTacToe.none)
                {
                    cells[row - 2, col - 2].Set();
                    return true;
                }

                if (cells[row - 1, col - 1].state == TicTacToe.none && cells[row - 2, col - 2].state == aiTeam)
                {
                    cells[row - 1, col - 1].Set();
                    return true;
                }
            }
        }
        return false;
    }
    public bool CheckWin(int num)
    {
        bool win = false;

        int row = num / 3;
        int col = num % 3;
        
        if (row + 1 < numOfRows && row - 1 >= 0)
        {
            if (cells[row + 1, col].state == cells[row, col].state && cells[row - 1, col].state == cells[row, col].state)
                win = true;
        }
        if (col + 1 < numOfCols && col - 1 >= 0)
        {
            if (cells[row, col + 1].state == cells[row, col].state && cells[row, col - 1].state == cells[row, col].state)
                win = true;
        }
        if (row + 1 < numOfRows && col + 1 < numOfCols && row - 1 >= 0 && col - 1 >= 0)
        {
            if (cells[row + 1, col + 1].state == cells[row, col].state && cells[row - 1, col - 1].state == cells[row, col].state)
                win = true;

            if (cells[row - 1, col + 1].state == cells[row, col].state && cells[row + 1, col - 1].state == cells[row, col].state)
                win = true;
        }

        if (row + 2 < numOfRows)
        {
            if (cells[row + 1, col].state == cells[row, col].state && cells[row + 2, col].state == cells[row, col].state)
                win = true;
        }
        if (col + 2 < numOfCols)
        {
            if (cells[row, col + 1].state == cells[row, col].state && cells[row, col + 2].state == cells[row, col].state)
                win = true;
        }
        if(row - 2 >= 0)
        {
            if (cells[row - 1, col].state == cells[row, col].state && cells[row - 2, col].state == cells[row, col].state)
                win = true;
        }
        if (col - 2 >= 0)
        {
            if (cells[row, col - 1].state == cells[row, col].state && cells[row, col - 2].state == cells[row, col].state)
                win = true;
        }

        if(row + 2 < numOfRows && col + 2 < numOfCols)
        {
            if (cells[row + 1, col + 1].state == cells[row, col].state && cells[row + 2, col + 2].state == cells[row, col].state)
                win = true;
        }
        if (row - 2 >= 0 && col + 2 < numOfCols)
        {
            if (cells[row - 1, col + 1].state == cells[row, col].state && cells[row - 2, col + 2].state == cells[row, col].state)
                win = true;
        }
        if (row + 2 < numOfRows && col - 2 >= 0)
        {
            if (cells[row + 1, col - 1].state == cells[row, col].state && cells[row + 2, col - 2].state == cells[row, col].state)
                win = true;
        }
        if (row - 2 >= 0 && col - 2 >= 0)
        {
            if (cells[row - 1, col - 1].state == cells[row, col].state && cells[row - 2, col - 2].state == cells[row, col].state)
                win = true;
        }
        return win;
    }
}
