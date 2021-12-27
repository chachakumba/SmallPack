using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Miner", menuName = "Clicker/Miner")]
public class MoneyMiner : ScriptableObject
{
    public string minerName = "New Miner";
    public int moneyPerSecond = 1;
    public int cost = 5;
    public int costAddition = 1;
    public Sprite sprite;
    [HideInInspector]
    public int amountOfMiners = 0;
}
