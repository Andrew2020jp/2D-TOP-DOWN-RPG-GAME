using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBoxLogic : MonoBehaviour
{
    [TextArea(3, 6)]
    public string dialogText;
    
    public void TriggerDialog()
    {
        PopUpSystem pop = GameObject.FindGameObjectWithTag("DialogManager").GetComponent<PopUpSystem>();
        pop.PopUp(dialogText);
    }
}
