using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
public enum BattleState
{
    Start,
    ActionSelection,
    MoveSelection,
    RunningTurn,
    Busy,
    PartyScreen,
    BattleOver,
    AboutToUse
}
public enum BattleAction
{
    Move,
    Switch,
    UseItem,
    Run
}
public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image rivalImage;
    [SerializeField] GameObject catcherSprite;

    public event Action <bool> OnBattleOver;
    bool aboutToUseBool = true;
    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;
    int escapeTrys;

    PartySystem playerParty;
    PartySystem rivalParty;
    Politician partylessPolitician;

    bool isRivalBattle = false;
    PlayerController player;
    PoliticianController rival;

    public void StartBattle(PartySystem playerParty, Politician partylessPolitician) 
    {
        isRivalBattle = false;
        this.playerParty = playerParty;
        this.partylessPolitician = partylessPolitician;
        player = playerParty.GetComponent<PlayerController>();
        StartCoroutine(SetUpBattle());
    }

    public void StartRivalBattle(PartySystem playerParty, PartySystem rivalParty) 
    {
        this.playerParty = playerParty;
        this.rivalParty = rivalParty;

        isRivalBattle = true;
        player = playerParty.GetComponent<PlayerController>();
        rival = rivalParty.GetComponent<PoliticianController>();
        StartCoroutine(SetUpBattle());
    }
    public IEnumerator SetUpBattle()
    {   
        playerUnit.CleanUp();
        enemyUnit.CleanUp();
        if(!isRivalBattle){
            playerUnit.Setup(playerParty.GetResonablePolitician());
            enemyUnit.Setup(partylessPolitician);

            dialogBox.SetMoveNames(playerUnit.Politician.Moves);
            yield return dialogBox.TypeDialog($" A {enemyUnit.Politician.Base.Name} showed up and decided to \"debate\" you to the death !");
            yield return new WaitForSeconds(1.5f);
        }
        else
        {
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            rivalImage.gameObject.SetActive(true);
            
            playerImage.sprite = player.Sprite;
            rivalImage.sprite = rival.Sprite;

            yield return dialogBox.TypeDialog($"{rival.Name} wants to challange you for reigonal hegemony !");

            rivalImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPolitician = rivalParty.GetResonablePolitician();
            enemyUnit.Setup(enemyPolitician);
            
            yield return dialogBox.TypeDialog($"{rival.Name}'s {enemyPolitician.Base.Name} decided to start a discussion !");



            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPolitician = playerParty.GetResonablePolitician();    
            playerUnit.Setup(playerPolitician);

            yield return dialogBox.TypeDialog($"{playerPolitician.Base.Name} got this !");
            dialogBox.SetMoveNames(playerUnit.Politician.Moves);
        }

        escapeTrys = 0;
        partyScreen.Init();

        ActionSelection();
    }

    void BattleOver(bool winner)
    {
        state = BattleState.BattleOver;
        playerParty.Politicians.ForEach(p=>p.OnBattleOver());
        OnBattleOver(winner);

    }
    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        dialogBox.SetDialog("Will you debate, or run away in fear ?");
        dialogBox.EnableActionSelector(true);
    }

    void OpenPartyScreen()
    {
        state  = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Politicians);
        partyScreen.gameObject.SetActive(true);
    }
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    IEnumerator AboutToUse(Politician newPolitician)
    {
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{rival.Name} is about to put {newPolitician.Base.Name} into the debate. Do you want to change your representative ?");
        state = BattleState.AboutToUse;
        dialogBox.EnableChoiceBox(true);
    }
    public void HandleUpdate()
    {
        if(state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
    }


    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if(playerAction == BattleAction.Move)
        {
            playerUnit.Politician.CurrentMove = playerUnit.Politician.Moves[currentMove];
            enemyUnit.Politician.CurrentMove = enemyUnit.Politician.GetRandomMove();

            int playerMovePriority = playerUnit.Politician.CurrentMove.Base.Priority;
            int enemyMovePriority = enemyUnit.Politician.CurrentMove.Base.Priority;


            bool playerGoesFirst = true;
            if(enemyMovePriority > playerMovePriority)
            {
                playerGoesFirst = false;
            }
            else if (enemyMovePriority == playerMovePriority)
            {
                playerGoesFirst = playerUnit.Politician.Speed >= enemyUnit.Politician.Speed;
            }

            var firstUnit =(playerGoesFirst)? playerUnit : enemyUnit ;
            var secondUnit =(playerGoesFirst)? enemyUnit : playerUnit ;

            var secondPolitician = secondUnit.Politician;
            //1st
            yield return RunMove(firstUnit, secondUnit, firstUnit.Politician.CurrentMove);
            yield return RunAfterTurn(firstUnit);
            if(state == BattleState.BattleOver)
            {
                yield break;
            }

            //2nd
            if(secondPolitician.HP > 0)
            {
                yield return RunMove(secondUnit, firstUnit, secondUnit.Politician.CurrentMove);
                yield return RunAfterTurn(secondUnit);
                if(state == BattleState.BattleOver)
                {
                    yield break;
                }
            }
        }
        else
        {
            if(playerAction == BattleAction.Switch)
            {
                var selectedPolitician = playerParty.Politicians[currentMember];
                state = BattleState.Busy;
                yield return SwitchPolitician(selectedPolitician);
            }else if(playerAction == BattleAction.UseItem)
            {   
                dialogBox.EnableActionSelector(false);
                yield return ThrowCatcher();
            }else if(playerAction == BattleAction.Run)
            {
                yield return tryToRun();
            }

            var enemyMove = enemyUnit.Politician.GetRandomMove();
            yield return RunMove(enemyUnit,playerUnit,enemyMove);
            yield return RunAfterTurn(enemyUnit);
            if(state == BattleState.BattleOver)
            {
                yield break;
            }
        }
        if(state != BattleState.BattleOver)
        {
            ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move )
    {   
        bool canRunMove = sourceUnit.Politician.OnBeforeMove();

        if(!canRunMove)
        {   
            yield return ShowStatusChanges(sourceUnit.Politician);
            yield return sourceUnit.Hud.UpdateHP();
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Politician);
        
        move.RP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Politician.Base.Name} used {move.Base.Name}");
        
        if(CheckIfMoveHits(move,sourceUnit.Politician,targetUnit.Politician))
        {
            sourceUnit.AttackAnimation();
            yield return new WaitForSeconds(1f);
            targetUnit.HitAnimation();

            if(move.Base.Category == MoveCategory.Status)
            {   
                yield return RunMoveEffects(move.Base.Effects,sourceUnit.Politician,targetUnit.Politician, move.Base.Target);
            }
            else
            {

                var damageDetails  = targetUnit.Politician.TakeDammage(move, sourceUnit.Politician);

                yield return targetUnit.Hud.UpdateHP();
                yield return ShowDamageReport(damageDetails);
            }

            if(move.Base.Secondaries != null && move.Base.Secondaries.Count > 0 && targetUnit.Politician.HP > 0)
            {
                foreach(var secondary in move.Base.Secondaries)
                {
                    var random = UnityEngine.Random.Range(1,101);
                    if(random <= secondary.Chance)
                    {
                        yield return RunMoveEffects(secondary,sourceUnit.Politician,targetUnit.Politician, secondary.Target);
                    }
                }
            }
            if(targetUnit.Politician.HP <= 0)
            {
                yield return HandlePoliticanFainted(targetUnit);
            }  
        }else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Politician.Base.Name}'s argument didn't hit any valid points !");
        }
    }  
    IEnumerator RunAfterTurn(BattleUnit sourceUnit)
    {   
        if(state == BattleState.BattleOver)
        {
            yield break;
        }
        yield return new WaitUntil(() => state == BattleState.RunningTurn);
        // Looks for statuses after the end of the turn
        sourceUnit.Politician.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Politician);
        yield return sourceUnit.Hud.UpdateHP();
        if(sourceUnit.Politician.HP <= 0)
        {
            yield return HandlePoliticanFainted(sourceUnit);
            yield return new WaitUntil(() => state == BattleState.RunningTurn);
        }
    }

    IEnumerator RunMoveEffects(MoveEffects effects, Politician source, Politician target , MoveTarget moveTarget)
    {
        if(effects.Boosts != null)
        {
            if(moveTarget == MoveTarget.Self)
            {
                source.ApplyBoosts(effects.Boosts);
            }
            else
            {
                target.ApplyBoosts(effects.Boosts);
            }
        }
        if(effects.Status != ConditionID.none)
        {
            target.SetStatus(effects.Status);
        }

        if(effects.VolatileStatus != ConditionID.none)
        {
            target.SetVolatileStatus(effects.VolatileStatus);
        }
        yield return ShowStatusChanges(source);
        yield return ShowStatusChanges(target);        
    }
    IEnumerator ShowStatusChanges(Politician politician)
    {
        while (politician.StatusChanges.Count > 0)
        {
            var message = politician.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }
    }
    bool CheckIfMoveHits(Move move, Politician source, Politician target)
    {
        if(move.Base.AlwaysHits)
        {
            return true;
        }
        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.StatBoosts[Stat.Accuracy];
        int evasion = target.StatBoosts[Stat.Evasion];

        var boostValues = new float[] {1f, 1.5f , 2f , 2.5f , 3f , 3.5f , 4f};

        if(accuracy > 0 )
        {
            moveAccuracy *= boostValues[accuracy];
        }
        else
        {
            moveAccuracy /= boostValues[-accuracy];
        }
        if(evasion > 0 )
        {
            moveAccuracy /= boostValues[evasion];
        }
        else
        {
            moveAccuracy *= boostValues[-evasion];
        }

        return UnityEngine.Random.Range(1,101) <= moveAccuracy;
    }
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            var nextPolitician = playerParty.GetResonablePolitician();
            if (nextPolitician != null)
            {
                OpenPartyScreen();
            }
            else
            {
                BattleOver(false);
            }   
        }
        else
        {   
            if(!isRivalBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextPolitician =  rivalParty.GetResonablePolitician();
                if(nextPolitician != null)
                {
                    StartCoroutine(AboutToUse(nextPolitician));
                }else
                {
                    BattleOver(true);
                }
            }
        }
    }     
    IEnumerator ShowDamageReport(DamageReport damageDetails)
    {
        if(damageDetails.Critical > 1f)
        {
            yield return dialogBox.TypeDialog("A Critical hit !!!");

        }
        if(damageDetails.TypeEffectiveness > 1f)
        {
            yield return dialogBox.TypeDialog("Those words bit harder than expected !");
        }
        else if(damageDetails.TypeEffectiveness < 1f)
        {
            yield return dialogBox.TypeDialog("Those words didn't even scratch !"); 
        }
    }

    void HandleActionSelection()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentAction += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentAction -= 2;
        }

        currentAction = Mathf.Clamp(currentAction, 0,3);
        
        dialogBox.UpdateActionSelection(currentAction);
        
        if(Input.GetKeyDown(KeyCode.Z))
        {
            if(currentAction == 0)
            {
                MoveSelection();
            }
            else if(currentAction == 1)
            {
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if(currentAction == 2)
            {
                //PoliticianChange
                prevState = state;
                OpenPartyScreen();
            }
            else if(currentAction == 3)
            {
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }
    void HandleMoveSelection()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMove -= 2;
        }       

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Politician.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Politician.Moves[currentMove]);

        if(Input.GetKeyDown(KeyCode.Z))
        {   
            var move = playerUnit.Politician.Moves[currentMove];
            if(move.RP == 0)
            {
                return;
            }
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true); 
            ActionSelection();
        }
    }
    void HandlePartySelection()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            ++currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            --currentMember;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentMember += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentMember -= 2;
        }       
        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Politicians.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            var SelectedMember = playerParty.Politicians[currentMember];
            if(SelectedMember.HP <= 0 )
            {
                partyScreen.SetMessageText("Your politician can't debatate right now !");
                return;
            }
            if(SelectedMember == playerUnit.Politician)
            {
                partyScreen.SetMessageText("You already are debating with that politician !");
                return;
            }

            partyScreen.gameObject.SetActive(false);
            if(prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.Switch));   
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPolitician(SelectedMember));
            }
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {   
            if(playerUnit.Politician.HP <= 0)
            {   
                partyScreen.SetMessageText("Chose a politician to continue the debate !");
                return;
            }
            partyScreen.gameObject.SetActive(false);
            if(prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextRivalPolitician());
            }else
            {
                ActionSelection();
            }
        

        }
    }
    void HandleAboutToUse()
    {
        if(Input.GetKeyDown(KeyCode.UpArrow)||Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUseBool = !aboutToUseBool;
        }
        dialogBox.UpdateChoiceBox(aboutToUseBool);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableChoiceBox(false);
            if(aboutToUseBool == true)
            {   
                prevState = BattleState.AboutToUse;
                OpenPartyScreen();
            }else
            {
               StartCoroutine(SendNextRivalPolitician());
            }
        }
        else if(Input.GetKeyDown(KeyCode.X))
        {
           dialogBox.EnableChoiceBox(false); 
           StartCoroutine(SendNextRivalPolitician());
        }
    }
    IEnumerator HandlePoliticanFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Politician.Base.Name} lost their will to debate and tactically retreated !");
        faintedUnit.FaintAnimation();

        yield return new WaitForSeconds(2f);
        if(!faintedUnit.IsPlayerUnit)
        {
            int expYield = faintedUnit.Politician.Base.ExpYield;
            int enemyLevel = faintedUnit.Politician.Level;
            float duelBonus = (isRivalBattle)? 1.5f : 1f;

            int expGain = Mathf.FloorToInt((expYield * enemyLevel * duelBonus) /7)  ;
            playerUnit.Politician.Exp += expGain;
            yield return dialogBox.TypeDialog($"{playerUnit.Politician.Base.Name} gained {expGain} respect !");
            yield return playerUnit.Hud.SetExpSmoothly();

            yield return new WaitForSeconds(1f);

            while (playerUnit.Politician.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Politician.Base.Name} grew to the respect level of {playerUnit.Politician.Level} !");
                yield return playerUnit.Hud.SetExpSmoothly(true);
            }


        }
        CheckForBattleOver(faintedUnit);
    }
    IEnumerator SwitchPolitician(Politician newPolitician)
    {   
        if(playerUnit.Politician.HP > 0 )
        {
            yield return dialogBox.TypeDialog($"That's enough {playerUnit.Politician.Base.Name}");
            playerUnit.FaintAnimation();
            yield return new WaitForSeconds(2f);            
        }

        playerUnit.Setup(newPolitician);

        dialogBox.SetMoveNames(newPolitician.Moves);
        yield return dialogBox.TypeDialog($"Time for {newPolitician.Base.Name} to shine !");

        if(prevState == null)
        {
            state = BattleState.RunningTurn;
        }
        else if(prevState == BattleState.AboutToUse)
        {   
            prevState = null;
            StartCoroutine(SendNextRivalPolitician());

        }
    }
    IEnumerator SendNextRivalPolitician()
    {
        state = BattleState.Busy;

        var nextPolitician = rivalParty.GetResonablePolitician();
        enemyUnit.Setup(nextPolitician);
        yield return dialogBox.TypeDialog($"{nextPolitician.Base.Name} gets the attention of the auidiance !");
        state = BattleState.RunningTurn;

    }
    IEnumerator ThrowCatcher()
    {
        if(isRivalBattle)
        {
            yield return dialogBox.TypeDialog("You can't buy that kind of loyalty with money ");
            state = BattleState.RunningTurn;
            yield break;
        }
        state = BattleState.Busy;
        yield return dialogBox.TypeDialog($"{player.Name} sent a party deal to the partyless politician !");

        var catcherObj = Instantiate(catcherSprite, playerUnit.transform.position -new Vector3(-2,0), Quaternion.identity);

        var catcher = catcherObj.GetComponent<SpriteRenderer>();

        yield return catcher.transform.DOJump(enemyUnit.transform.position + new Vector3(0,1), 3f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return catcher.transform.DOMoveY(enemyUnit.transform.position.y - 1 , 0.5f).WaitForCompletion();

        int shakeCount = AttemtToCatch(enemyUnit.Politician);
        for (int i = 0; i< Mathf.Min(shakeCount,3); i++)
        {
            yield return new WaitForSeconds(0.5f);
            yield return catcher.transform.DOPunchRotation(new Vector3(0,0,15f),1f).WaitForCompletion();
        }
        if(shakeCount == 4)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Politician.Base.Name} agreed to join your political party !");
            yield return catcher.DOFade(0,1.5f).WaitForCompletion();

            playerParty.AddPolitician(enemyUnit.Politician);

            yield return dialogBox.TypeDialog($"{enemyUnit.Politician.Base.Name} is now in your party !");
            Destroy(catcher);
            BattleOver(true);
        }else
        {
            yield return new WaitForSeconds(0.5f);
            catcher.DOFade(0,0.2f);
            yield return enemyUnit.NoCaptureAnimation();

            if(shakeCount < 2 )
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Politician.Base.Name} noded away to your deal !");
            }
            if(shakeCount < 3)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Politician.Base.Name} almost accepted but ultimately refused !");
            }
            Destroy(catcher);
            state = BattleState.RunningTurn;
        }
    }
    int AttemtToCatch(Politician politician)
    {
        float a = (3* politician.MaxHp -2 * politician.HP) * politician.Base.CatchRate * ConditionsDB.GetStatusBonus(politician.Status) /(3* politician.MaxHp); // Actaul catch rate of pokeomn games 
        if(a > 255)
        {
            return 4;
        }
        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while(shakeCount < 4)
        {
            if(UnityEngine.Random.Range(0,65535) >= b){
                break;
            }
            ++shakeCount;
        }
        return shakeCount;
    }
    IEnumerator tryToRun()
    {
        state = BattleState.Busy;
        if(isRivalBattle)
        {
            yield return dialogBox.TypeDialog("You can't run away from political rival duels, you must face him to the end !");
            state = BattleState.RunningTurn;
            yield break;
        }
        ++escapeTrys;
        int playerSpeed = playerUnit.Politician.Speed;
        int enemySpeed = enemyUnit.Politician.Speed;

        if(enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog("Your secretary bailed you out !");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed*128)/enemySpeed + 30 * escapeTrys;
            f = f% 256;
            if(UnityEngine.Random.Range(0,256) < f)
            {
                yield return dialogBox.TypeDialog("Your secretary bailed you out !");
                BattleOver(true);
            }else
            {
                dialogBox.TypeDialog("Your Secretary couldn't bail you out, sorry ");
                state = BattleState.RunningTurn;
            }
        }

    }

}
