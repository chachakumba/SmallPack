using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MinerPanel : MonoBehaviour
{
    public MoneyMiner attachedMiner;
    [SerializeField]
    TextMeshProUGUI minerName;
    [SerializeField]
    TextMeshProUGUI moneyPerSecondText;
    [SerializeField]
    TextMeshProUGUI costText;
    [SerializeField]
    Image image;
    [SerializeField]
    Button buyButton;

    public void SetMiner(MoneyMiner miner)
    {
        attachedMiner = miner;
        minerName.text = attachedMiner.minerName + ":" + attachedMiner.amountOfMiners;
        moneyPerSecondText.text = attachedMiner.moneyPerSecond + "$/s";
        costText.text = attachedMiner.cost + attachedMiner.costAddition * attachedMiner.amountOfMiners + "$";
        image.sprite = attachedMiner.sprite;
        buyButton.onClick.AddListener(delegate { ClickerManager.instance.BuyMiner(attachedMiner.minerName); });
        ClickerManager.instance.OnMinerBuy += UpdatePanel;
    }
    public void UpdatePanel(string minerIdName)
    {
        if (minerIdName == attachedMiner.minerName)
        {
            costText.text = attachedMiner.cost + attachedMiner.costAddition * attachedMiner.amountOfMiners + "$";
            minerName.text = attachedMiner.minerName + ":" + attachedMiner.amountOfMiners;
        }
    }
    public void ActivatePanel(string minerIdName, bool active)
    {
        if (minerIdName == attachedMiner.minerName)
        {
            gameObject.SetActive(active);
            UpdatePanel(minerIdName);
        }
    }
}
