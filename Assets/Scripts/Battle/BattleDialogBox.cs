using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] Text dialogText;
    [SerializeField] Color HighlightedColor;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] int letterspeed;
    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;
    [SerializeField] GameObject choiceBox;

    [SerializeField] Text rpText;
    [SerializeField] Text typeText;

    [SerializeField] Text yesText;
    [SerializeField] Text noText;
    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;

    }
    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach(var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/letterspeed);
        }

        yield return new WaitForSeconds(1f);
    }
    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }
    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }
    public void UpdateChoiceBox(bool Selected)
    {
        if(Selected)
        {
            yesText.color = HighlightedColor;
            noText.color = Color.black;
        }else
        {
            noText.color = HighlightedColor;
            yesText.color = Color.black;            
        }
    }
    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }
    public void UpdateActionSelection(int selectedAction)
    {
        for(int i =0; i<actionTexts.Count; ++i)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = HighlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }
    public void SetMoveNames(List<Move> moves)
    {
        for (int i=0; i <moveTexts.Count; i++)
        {
            if( i< moves.Count) 
            {
                moveTexts[i].text = moves[i].Base.Name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for(int i =0; i<moveTexts.Count; ++i)
        {
            if(i == selectedMove)
            {
                moveTexts[i].color = HighlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
        rpText.text= $"RP {move.RP}/{move.Base.RP}";
        typeText.text = move.Base.Type.ToString();

        if(move.RP == 0)
        {
            rpText.color = Color.red;
        }
        else if(move.RP < move.Base.RP)
        {
            rpText.color = new Color(1f, 0.647f, 0f);
        }
        else
        {
            rpText.color = Color.black;
        }
    }

}

