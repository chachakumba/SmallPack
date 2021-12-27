using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class ClickerManager : MonoBehaviour
{
    public static ClickerManager instance;
    public double totalMoney = 0;
    public int moneyPerClick = 1;
    public int moneyPerSecond = 0;
    public int timeToMoneyPerSec = 1;
    public float maxTimeOfAquairingMoney = 0.15f;
    public DateTime lastDay;
    public DateTime today;
    [SerializeField]
    TextMeshProUGUI totalMoneyText;
    [SerializeField]
    TextMeshProUGUI moneyPerSecondText;
    public Image mainMoneyImage;
    public List<MoneyMiner> miners = new List<MoneyMiner>();
    public List<MoneyUpgrade> upgrades = new List<MoneyUpgrade>();
    public List<int> quantityOfMiners = new List<int>();
    public int maxUpgrade = 0;
    [SerializeField]
    RectTransform spawnOfMiners;
    [SerializeField]
    RectTransform spawnOfUpgrades;
    [SerializeField]
    GameObject minerPanelPrefab;
    [SerializeField]
    GameObject upgraderPanelPrefab;
    [SerializeField]
    Slider moneyPerSecSlider;
    [SerializeField]
    GameObject moneyWhenOffPanel;
    [SerializeField]
    TextMeshProUGUI moneyWhenOffText;
    public delegate void Buy(string objName);
    public delegate void UpgradeBuy(int buyedId);
    public event Buy OnMinerBuy;
    public event UpgradeBuy OnUpgraderBuy;
    [SerializeField]
    GameObject minerBuyVeil;
    [SerializeField]
    GameObject mainPanelVeil;
    [SerializeField]
    GameObject upgradeBuyVeil;
    float widthOfScreen = 608;
    public float speedOfScreenMove = 1;
    Coroutine savedMove;
    [SerializeField]
    Transform AllPanels;
    [Space]
    [SerializeField]
    GameObject audioSource;
    [SerializeField]
    AudioClip coinClick;
    [SerializeField]
    AudioClip coinPerSecondSound;
    [SerializeField]
    AudioClip swapPanel;
    [SerializeField]
    AudioClip buySound;
    [SerializeField]
    AudioClip allowSound;
    [SerializeField]
    AudioClip denySound;
    private void Awake()
    {
        Menu.LoadSave();
        instance = this;
        widthOfScreen = AllPanels.GetComponent<RectTransform>().rect.width;

        totalMoney = Menu.save.clickerMoney;
        totalMoneyText.text = totalMoney + "$";

        maxUpgrade = Menu.save.maxUpgrade;
        for (int i = 0; i < upgrades.Count; i++)
        {
            upgrades[i].id = i;
            upgrades[i].aquaired = false;
            if (maxUpgrade > i)
            {
                moneyPerClick = upgrades[i].newCostOfMoney;
                upgrades[i].aquaired = true;
                mainMoneyImage.sprite = upgrades[i].sprite;
            }
        }

        quantityOfMiners = Menu.save.quantityOfMiners;
        if (quantityOfMiners == null) quantityOfMiners = new List<int>();
        for (int i = 0; i < miners.Count; i++)
        {
            if (i >= quantityOfMiners.Count) quantityOfMiners.Add(0);
            ChangeMoneyPerSecond(miners[i].moneyPerSecond * quantityOfMiners[i]);
            miners[i].amountOfMiners = quantityOfMiners[i];
        }

        int contentHeigth = 0;
        foreach(MoneyMiner miner in miners)
        {
            contentHeigth++;
            Instantiate(minerPanelPrefab, spawnOfMiners).GetComponent<MinerPanel>().SetMiner(miner);
        }
        spawnOfMiners.sizeDelta = new Vector2(spawnOfMiners.localScale.x, 105 * contentHeigth + 5);

        contentHeigth = 0;
        foreach (MoneyUpgrade upgrader in upgrades)
        {
            contentHeigth++;
            Instantiate(upgraderPanelPrefab, spawnOfUpgrades).GetComponent<UpgradePanel>().SetUpgrader(upgrader);
        }
        spawnOfUpgrades.sizeDelta = new Vector2(spawnOfUpgrades.localScale.x, 105 * contentHeigth + 5);

        today = DateTime.UtcNow;
        lastDay = Menu.save.lastClickerDate;
        if (moneyPerSecond > 0)
        {
            if (today.Subtract(lastDay).Hours > maxTimeOfAquairingMoney)
            {
                ChangeMoney(Mathf.RoundToInt(moneyPerSecond * 60 * 60 * maxTimeOfAquairingMoney) / 2);
                moneyWhenOffText.text = Mathf.RoundToInt(moneyPerSecond * 60 * 60 * maxTimeOfAquairingMoney) / 2 + "$";
                Debug.Log("Added " + Mathf.RoundToInt(moneyPerSecond * 60 * 60 * maxTimeOfAquairingMoney) / 2 + "$");
            }
            else
            {
                ChangeMoney(Mathf.RoundToInt(moneyPerSecond * today.Subtract(lastDay).Seconds) / 2);
                moneyWhenOffText.text = Mathf.RoundToInt(moneyPerSecond * today.Subtract(lastDay).Seconds) / 2 + "$";
                Debug.Log("Added " + Mathf.RoundToInt(moneyPerSecond * today.Subtract(lastDay).Seconds) / 2 + "$");
            }
        }
        else
        {
            moneyWhenOffPanel.SetActive(false);
        }

        StartCoroutine(MoneyPerSecondCoroutine());
    }
    public void BuyMiner(string minerName)
    {
        foreach (MoneyMiner miner in miners)
        {
            if (miner.minerName == minerName)
            {
                if ((miner.cost + miner.costAddition * miner.amountOfMiners) <= totalMoney)
                {
                    PlaySound(buySound);
                    ChangeMoney(-(miner.cost + miner.costAddition * miner.amountOfMiners));
                    miner.amountOfMiners++;
                    ChangeMoneyPerSecond(miner.moneyPerSecond);
                    OnMinerBuy.Invoke(minerName);
                    Save();
                }
                else
                {
                    PlaySound(denySound);
                }
            }
        }
    }
    public void BuyUpgrader(int upgraderId)
    {
        int buyedId = 0;
        foreach(MoneyUpgrade upgrade in upgrades)
        {
            if(upgrade.id == upgraderId)
            {
                if (upgrade.cost <= totalMoney)
                {
                    PlaySound(buySound);
                    ChangeMoney(-upgrade.cost);
                    upgrade.aquaired = true;
                    moneyPerClick = upgrade.newCostOfMoney;
                    OnUpgraderBuy.Invoke(upgraderId);
                    buyedId = upgrade.id;
                    maxUpgrade = buyedId + 1;
                    Save();
                }
                else
                {
                    PlaySound(denySound);
                }
            }
        }
    }
    IEnumerator MoneyPerSecondCoroutine()
    {
        while (true)
        {
            StartCoroutine(ChangeMoneyPerSecSlider());
            yield return new WaitForSeconds(timeToMoneyPerSec);
            ChangeMoney(moneyPerSecond);
            if(moneyPerSecond > 0)PlaySound(coinPerSecondSound);
        }
    }
    IEnumerator ChangeMoneyPerSecSlider()
    {
        if (moneyPerSecond > 0)
        {
            moneyPerSecSlider.fillRect.gameObject.SetActive(true);
            float endTime = Time.time + timeToMoneyPerSec;
            bool repeat = true;
            while (repeat)
            {
                if (endTime > Time.time)
                {
                    yield return null;
                    moneyPerSecSlider.value = 1 - (endTime - Time.time);
                }
                else repeat = false;
            }
        }
        else
        {
            moneyPerSecSlider.value = 0;
            moneyPerSecSlider.fillRect.gameObject.SetActive(false);
            yield return null;
        }
    }
    //Moving througth panels
    #region
    public void MoveToMiners()
    {
        if (savedMove != null) StopCoroutine(savedMove);
        PlaySound(swapPanel);
        minerBuyVeil.SetActive(true);
        mainPanelVeil.SetActive(false);
        upgradeBuyVeil.SetActive(false);
        minerBuyVeil.GetComponentInParent<Button>().interactable = false;
        mainPanelVeil.GetComponentInParent<Button>().interactable = true;
        upgradeBuyVeil.GetComponentInParent<Button>().interactable = true;
        savedMove = StartCoroutine(MoveToMinersCour());
    }
    public void MoveToMain()
    {
        if (savedMove != null) StopCoroutine(savedMove);
        PlaySound(swapPanel);
        minerBuyVeil.SetActive(false);
        mainPanelVeil.SetActive(true);
        upgradeBuyVeil.SetActive(false);
        minerBuyVeil.GetComponentInParent<Button>().interactable = true;
        mainPanelVeil.GetComponentInParent<Button>().interactable = false;
        upgradeBuyVeil.GetComponentInParent<Button>().interactable = true;
        savedMove = StartCoroutine(MoveToCenterCour());
    }
    public void MoveToUpgraders()
    {
        if (savedMove != null) StopCoroutine(savedMove);
        PlaySound(swapPanel);
        minerBuyVeil.SetActive(false);
        mainPanelVeil.SetActive(false);
        upgradeBuyVeil.SetActive(true);
        minerBuyVeil.GetComponentInParent<Button>().interactable = true;
        mainPanelVeil.GetComponentInParent<Button>().interactable = true;
        upgradeBuyVeil.GetComponentInParent<Button>().interactable = false;
        savedMove = StartCoroutine(MoveToUpgradersCour());
    }
    IEnumerator MoveToMinersCour()
    {
        while (true)
        {
            if(AllPanels.localPosition.x > -widthOfScreen)
            {
                AllPanels.localPosition = new Vector3(Mathf.Lerp(AllPanels.localPosition.x, widthOfScreen, speedOfScreenMove), 0, 0);
            }
            yield return new WaitForSeconds(0.0001f);
        }
    }
    IEnumerator MoveToCenterCour()
    {
        while (true)
        {
            if (Mathf.Abs(AllPanels.localPosition.x) < 10)
            {
                break;
            }
            if (AllPanels.localPosition.x > -widthOfScreen)
            {
                AllPanels.localPosition = new Vector3(Mathf.Lerp(AllPanels.localPosition.x, 0, speedOfScreenMove), 0, 0);
            }
            yield return new WaitForSeconds(0.0001f);
        }
        AllPanels.localPosition = Vector3.zero;
    }
    IEnumerator MoveToUpgradersCour()
    {
        while (true)
        {
            if (AllPanels.localPosition.x > -widthOfScreen)
            {
                AllPanels.localPosition = new Vector3(Mathf.Lerp(AllPanels.localPosition.x, -widthOfScreen, speedOfScreenMove), 0, 0);
            }
            yield return new WaitForSeconds(0.0001f);
        }
    }
    #endregion
    public void MoneyWhenOffClick()
    {
        moneyWhenOffPanel.SetActive(false);
        PlaySound(allowSound);
    }
    public void ToMenuButton()
    {
        PlaySound(allowSound);
        StartCoroutine(WaitToMenu());
    }
    IEnumerator WaitToMenu()
    {
        yield return new WaitForSeconds(0.5f);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }
    public void Click()
    {
        ChangeMoney(moneyPerClick);
        PlaySound(coinClick);
    }

    public void ChangeMoney(int addition)
    {
        totalMoney += addition;
        totalMoneyText.text = totalMoney + "$";
        Save();
    }
    public void ChangeMoneyPerSecond(int addition)
    {
        moneyPerSecond += addition;
        moneyPerSecondText.text = moneyPerSecond + "$/sec";
    }
    public void BuyedUpgraderButton()
    {
        PlaySound(denySound);
    }

    public void PlaySound(AudioClip clip)
    {
        Instantiate(audioSource).GetComponent<AudioObject>().Play(clip);
    }
    public void Save()
    {
        for(int i = 0; i < miners.Count; i++)
        {
            quantityOfMiners[i] = miners[i].amountOfMiners;
        }
        Menu.SaveClicker(DateTime.UtcNow, quantityOfMiners, maxUpgrade, totalMoney);
    }
}
