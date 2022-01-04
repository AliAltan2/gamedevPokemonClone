using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PartyScreen : MonoBehaviour
{   
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    List<Politician> politicians;
    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    public void SetPartyData(List<Politician> politicians)
    {
        this.politicians = politicians;
        for(int i = 0; i < memberSlots.Length;i++)
        {
            if(i < politicians.Count) 
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(politicians[i]);

            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        messageText.text = "Choose your Politician !";
    }
    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < politicians.Count;i++)
        {
            if( i == selectedMember)
            {
                memberSlots[i].SetSelected(true);
            }
            else
            {
                memberSlots[i].SetSelected(false);
            }

        }
    }
    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
