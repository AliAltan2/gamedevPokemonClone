using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    
    [SerializeField] Sprite sprite;
    [SerializeField] string name;
    public Vector2 input;
    const float offSetY = 0.3f;
    private Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if(!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0 ) input.y = 0;

            if (input != Vector2.zero)
            {   
               StartCoroutine(character.Move(input,OnMoveOver));
            }   
        }

        character.HandleUpdate();
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }
    void Interact()
    {
        var faceingDirection = new Vector3(character.Animator.MoveX ,character.Animator.MoveY);
        var interactPos = transform.position+faceingDirection;

        //Debug.DrawLine(transform.position, interactPos, Color.green, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private  void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, offSetY), 0.2f, GameLayers.i.TriggerableLayers);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if(triggerable != null)
            {   
                character.Animator.IsMoving = false;
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }
    public string Name
    {
        get => name;
    }
    public Sprite Sprite
    {
        get=> sprite;
    }
    public Character Character
    {
        get=> character;
    }
}
