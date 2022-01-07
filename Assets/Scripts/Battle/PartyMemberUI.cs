using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hPBar;
    [SerializeField] Color highlightedColor;
    Politician _politician;
    public void SetData(Politician politician)
    {
        _politician = politician;
        nameText.text = politician.Base.Name;
        levelText.text = "Lvl " + politician.Level;
        hPBar.SetHP((float)politician.HP / politician.MaxHp);
    }
    public void SetSelected(bool selected)
    {
        if(selected)
        {
            nameText.color = highlightedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
