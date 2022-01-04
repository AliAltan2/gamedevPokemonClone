using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int letterspeed;

    public event Action OnShowDialog;
    public event Action OnClosedDialog;
    public static DialogManager Instance {get; private set;}

    private void Awake() {
        Instance = this;
    }

    int currentLine = 0;
    Action onDialogFinished;
    Dialog dialog;
    bool isTyping;
    public bool isShowing {get; private set;} 
    public IEnumerator ShowDialog(Dialog dialog, Action onFinished=null)
    {   
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        isShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(dialog.Lines[0]));
    }
    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Z) && !isTyping)
        {
            ++currentLine;
            if(currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {   
                currentLine = 0;
                dialogBox.SetActive(false);
                onDialogFinished.Invoke();
                isShowing = false;
                OnClosedDialog?.Invoke();  
            }
        }
    }
    public IEnumerator TypeDialog(string line)
    {   
        isTyping = true;
        dialogText.text = "";
        foreach(var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/letterspeed);
        }
        isTyping = false;
    }
}    
