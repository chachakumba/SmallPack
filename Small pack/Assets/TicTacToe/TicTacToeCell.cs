using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[System.Serializable]
public class TicTacToeCell : MonoBehaviour
{
    public int num;
    public Image image;
    public TicTacToe state;
    AudioSource audioSource;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void Set()
    {
        audioSource.clip = TicTacToeManager.instance.clickSound;
        audioSource.Play();
        if (state == TicTacToe.none && !TicTacToeManager.instance.isGameStop)
            TicTacToeManager.instance.Set(num);
    }
    public void SetCross()
    {
        image.sprite = TicTacToeManager.instance.cross;
        state = TicTacToe.cross;
        TicTacToeManager.instance.emptyCells.Remove(this);
        TicTacToeManager.instance.crossCells.Add(this);
    }
    public void SetCircle()
    {
        image.sprite = TicTacToeManager.instance.circle;
        state = TicTacToe.circle;
        TicTacToeManager.instance.emptyCells.Remove(this);
        TicTacToeManager.instance.circleCells.Add(this);
    }
    public void Clear()
    {
        image.sprite = null;
        if (state == TicTacToe.cross)
        {
            state = TicTacToe.none;
            TicTacToeManager.instance.crossCells.Remove(this);
            TicTacToeManager.instance.emptyCells.Add(this);
        }
        else if(state == TicTacToe.circle)
        {
            state = TicTacToe.none;
            TicTacToeManager.instance.circleCells.Remove(this);
            TicTacToeManager.instance.emptyCells.Add(this);
        }
    }
}
public enum TicTacToe
{
    none, cross, circle
}
