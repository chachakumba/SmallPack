using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade", menuName = "Clicker/Upgrader")]
public class MoneyUpgrade : ScriptableObject
{
    public string upgradeName = "New Upgrade";
    public int cost = 10;
    public int newCostOfMoney = 2;
    public Sprite sprite;
    [HideInInspector]
    public bool aquaired = false;
    [HideInInspector]
    public int id;
}
