using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMemberFov : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController player)
    {
        GameController.Instance.OnEnterRivalView(GetComponentInParent<PoliticianController>());
    }
}
