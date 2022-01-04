using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliticianController : MonoBehaviour, Interactable
{    
    [SerializeField] Dialog dialog;
    [SerializeField] Dialog dialogAfterLoss;
    [SerializeField] GameObject exclamation;
    [SerializeField] GameObject fov;
    [SerializeField] Sprite sprite;
    [SerializeField] string name;
    Character character;
    bool battleLost = false;


    private void Awake() {
        character = GetComponent<Character>();
    }
    private void Start() {
        SetFovRotation(character.Animator.DefaultDirection);
    }
    private void Update() {
        character.HandleUpdate();
    }
    public void Interact(Transform initiator)
    {
        character.LookToMe(initiator.position);
        if(!battleLost)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog,()=> 
            {
                GameController.Instance.StartRivalBattle(this);
            }));
        }else
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialogAfterLoss));
        }

    }
    public void BattleLost()
    {
        battleLost = true;
        fov.gameObject.SetActive(false);
    }
    public IEnumerator triggerPoliticalBattle(PlayerController player)
    {   
        // Challange will be served 
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // chad walk up 
        var difference = player.transform.position - transform.position;

        var moveVector = difference - difference.normalized;

        moveVector = new Vector2(Mathf.Round(moveVector.x),Mathf.Round(moveVector.y));
        yield return character.Move(moveVector);

        // talk 
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog,()=> {
            GameController.Instance.StartRivalBattle(this);
        }));
    }
    public void SetFovRotation(FacingDirections direction)
    {
        float angle = 0f;
        if(direction == FacingDirections.Right)
        {
            angle = 90f;
        }else if(direction == FacingDirections.Up)
        {
            angle = 180f;
        }else if(direction == FacingDirections.Left)
        {
            angle = 270f;
        }
        fov.transform.eulerAngles = new Vector3(0f,0f,angle);
    }


    public string Name
    {
        get => name;
    }
    public Sprite Sprite
    {
        get => sprite;
    }
}
