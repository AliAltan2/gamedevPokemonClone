using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] List<Vector2> movePattern;
    [SerializeField] float timeBetweenPattern;
    float idleTimer = 0f;
    NPCState state;
    int currentMovement = 0;

    Character character;
    private void Awake() {
        character = GetComponent<Character>();
    }
    
    public void Interact(Transform initiator)
    {   
        if(state == NPCState.idle)
        {
            state = NPCState.Dialog;
            character.LookToMe(initiator.position);
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog, ()=>{
                idleTimer = 0;
                state = NPCState.idle;
            }));

        }
        //StartCoroutine(character.Move(new Vector2(-2,0)));
    }
    private void Update() {

        if(state == NPCState.idle)
        {
            idleTimer += Time.deltaTime;
            if(idleTimer >timeBetweenPattern)
            {
                idleTimer = 0;
                if(movePattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
            }
        }

        character.HandleUpdate();
    }
    IEnumerator Walk()
    {
        state = NPCState.walking;

        var oldPos = transform.position;

        yield return character.Move(movePattern[currentMovement]);

        if(transform.position != oldPos)
        {
            currentMovement = (currentMovement + 1) % movePattern.Count ;
        }
        state = NPCState.idle;
    }
}
public enum NPCState{ idle, walking, Dialog}
