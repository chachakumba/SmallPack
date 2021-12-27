using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TetrisCell : MonoBehaviour
{
    public int row;
    public int col;
    public bool ocupied = false;
    public bool readyTofall = false;
    public Image image;
    private void Awake()
    {
        foreach(Image img in GetComponentsInChildren<Image>())
        {
            if (img != GetComponent<Image>()) image = img;
        }
    }
    private void Start()
    {
        Clear();
    }
    public void Paint(Color newColor)
    {
        ocupied = true;
        if (image != null) image.color = newColor;
        TetrisManager.instance.oqupiedCells.Add(this);
    }
    public void Clear()
    {
        ocupied = false;
        if (image != null) image.color = TetrisManager.instance.nullColor;
        TetrisManager.instance.oqupiedCells.Remove(this);
    }
}
