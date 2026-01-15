using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogBoxLogic : MonoBehaviour
{
    [TextArea(3, 6)]
    public string dialogText;

    private Action onConfirm;
    private Action onCancel;

    public void TriggerDialog()
    {
        PopUpSystem pop = GameObject.FindGameObjectWithTag("DialogManager").GetComponent<PopUpSystem>();
        pop.PopUp(dialogText);
    }
    public void Show(string text, Action confirmAction, Action cancelAction)
    {
        dialogText = text;
        onConfirm = confirmAction;
        onCancel = cancelAction;
    }

    public void OnConfirmButton()
    {
        onConfirm?.Invoke();
        Close();
    }

    public void OnCancelButton() 
    {
        onCancel?.Invoke();
        Close();
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
