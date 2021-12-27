using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    public MoneyUpgrade attachedUpgrader;
    [SerializeField]
    TextMeshProUGUI upgraderName;
    [SerializeField]
    TextMeshProUGUI newMoneyCost;
    [SerializeField]
    TextMeshProUGUI costText;
    [SerializeField]
    Image image;
    [SerializeField]
    GameObject boughtVeil;
    [SerializeField]
    Button buyButton;
    public void SetUpgrader(MoneyUpgrade upgrader)
    {
        attachedUpgrader = upgrader;
        upgraderName.text = attachedUpgrader.upgradeName;
        newMoneyCost.text = attachedUpgrader.newCostOfMoney + "$/tap";
        costText.text = upgrader.cost + "$";
        image.sprite = attachedUpgrader.sprite;
        boughtVeil.SetActive(attachedUpgrader.aquaired);
        buyButton.onClick.AddListener(delegate { ClickerManager.instance.BuyUpgrader(attachedUpgrader.id); });
        ClickerManager.instance.OnUpgraderBuy += UpdatePanel;
    }
    public void UpdatePanel(int id)
    {
        if(id == attachedUpgrader.id)
            ClickerManager.instance.mainMoneyImage.sprite = attachedUpgrader.sprite;

        if (id >= attachedUpgrader.id)
        {
            attachedUpgrader.aquaired = true;
            boughtVeil.SetActive(true);
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(delegate { ClickerManager.instance.BuyedUpgraderButton(); });
        }
    }
    public void ActivePanel(int id, bool active)
    {
        if (id == attachedUpgrader.id)
            gameObject.SetActive(active);
    }
}
