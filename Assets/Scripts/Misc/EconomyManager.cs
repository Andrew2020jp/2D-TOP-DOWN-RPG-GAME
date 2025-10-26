using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EconomyManager : Singleton<EconomyManager>
{
    private TMP_Text goldText;
    private int currentGold = 0;

    const string COIN_AMOUNT_TEXT = "Gold Amount Text";

    public void UpdateCurrentGold()
    {
        currentGold += 1;

        if (goldText == null)
        {
            goldText = GameObject.Find(COIN_AMOUNT_TEXT).GetComponent<TMP_Text>();
        }

        goldText.text = currentGold.ToString("D3");
    }

    public void ResetGold()
    {
        currentGold = 0;
    }

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
