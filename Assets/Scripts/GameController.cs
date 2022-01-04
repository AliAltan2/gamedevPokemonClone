using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    FreeRoam,
    Battle,
    Talking,
    Cutscene
}
public class GameController : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    PoliticianController rival;
    GameState state;
    public static GameController Instance {get; private set;}
    private void Awake(){
        Instance = this;
        ConditionsDB.Init();
    }
    private void Start() 
    {
        battleSystem.OnBattleOver += EndBattle;

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Talking;
        };  
        DialogManager.Instance.OnClosedDialog += () =>
        {   
            if(state == GameState.Talking)
            {
                state = GameState.FreeRoam;
            }
        };
    }
    void EndBattle(bool win)
    {
        if(rival != null && win == true)
        {
            rival.BattleLost();
            rival = null;
        }
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }
    public void StartBattle()
    {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PartySystem>();
        var partylessPolitician = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomPolitician();
        var partylessPoliticianCopy = new Politician(partylessPolitician.Base , partylessPolitician.Level);
        battleSystem.StartBattle(playerParty, partylessPoliticianCopy);
    }

    public void StartRivalBattle(PoliticianController rival)
    {   
        this.rival = rival;
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerController.GetComponent<PartySystem>();
        var rivalParty = rival.GetComponent<PartySystem>();
        battleSystem.StartRivalBattle(playerParty,rivalParty);
    }
    public void OnEnterRivalView(PoliticianController politician)
    {
        state = GameState.Cutscene;
        StartCoroutine(politician.triggerPoliticalBattle(playerController));        
    }
    private void Update() {
        if(state == GameState.FreeRoam)
        {
            playerController.HandleUpdate();
        }
        else if(state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if(state == GameState.Talking)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}
