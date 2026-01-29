using UnityEngine;
using TMPro;

public class EconomyManager : Singleton<EconomyManager>
{
    private TMP_Text goldText;
    private int currentGold = 0;

    const string COIN_AMOUNT_TEXT = "Gold Amount Text";

    // =========================
    // GOLD API
    // =========================

    public int CurrentGold => currentGold;

    // ✅ NEW (recommended)
    public void AddGold(int amount)
    {
        currentGold += amount;
        UpdateGoldUI();
    }

    // ✅ OLD (kept so PickUp.cs DOES NOT BREAK)
    public void UpdateCurrentGold()
    {
        AddGold(1);
    }

    public bool CanSpendGold(int amount)
    {
        return currentGold >= amount;
    }

    public bool SpendGold(int amount)
    {
        if (currentGold < amount)
            return false;

        currentGold -= amount;
        UpdateGoldUI();
        return true;
    }

    public void ResetGold()
    {
        currentGold = 0;
        UpdateGoldUI();
    }

    // =========================
    // UI
    // =========================

    public void UpdateGoldUI()
    {
        if (goldText == null)
        {
            GameObject goldTextObject = GameObject.Find(COIN_AMOUNT_TEXT);
            if (goldTextObject != null)
            {
                goldText = goldTextObject.GetComponent<TMP_Text>();
            }
        }

        if (goldText != null)
        {
            goldText.text = currentGold.ToString("D3");
        }
    }
}