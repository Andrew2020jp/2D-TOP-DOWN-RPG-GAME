using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBoxLogic : MonoBehaviour
{
    public FreeRoamObjectBehavior objectBehavior;
    public Unit white_npc;
    public SignalItem contextOn;
    public SignalItem contextOff;
    public GameObject dialogueBox;
    public TMP_Text dialogText;
    public string dialog;
    public bool dialogActive;
    public string popUp;
    bool playerInRange = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && playerInRange)
        {
            PopUpSystem pop = GameObject.FindGameObjectWithTag("DialogManager").GetComponent<PopUpSystem>();
            pop.PopUp(popUp);
        }
    }

}
