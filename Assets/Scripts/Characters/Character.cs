using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Character : MonoBehaviour
{
    public float movespeed;
    public bool IsMoving {get; private set;}

    public float OffsetY {get;private set;} = 0.3f;
    CharacterAnimator animator;
    private void Awake() {
        animator = GetComponent<CharacterAnimator>();
        SetPositionAndSnap(transform.position);
    }
    public IEnumerator Move(Vector2 moveVector , Action OnMoveOver = null)
    {
        animator.MoveX = Mathf.Clamp(moveVector.x, -1f, 1f);
        animator.MoveY  = Mathf.Clamp(moveVector.y,-1f,1f);


        var targetPos = transform.position;
        targetPos.x += moveVector.x;
        targetPos.y += moveVector.y;

        if(!isPathClear(targetPos))
        {
            yield break;
        }

        IsMoving = true;

        while((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, movespeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        IsMoving = false;

        OnMoveOver?.Invoke();
    }
    private bool isPathClear(Vector3 targetPos)
    {
        var differene = targetPos - transform.position;
        var dir = differene.normalized;

        if (Physics2D.BoxCast(transform.position + dir, new Vector2(0.5f,0.5f),0f, dir, differene.magnitude -1,GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer | GameLayers.i.PlayerLayer)== true) 
        {
            return false;
        }
        return true;

    }
    public void HandleUpdate()
    {
        animator.IsMoving = IsMoving;
    }
    private bool IsWalkable(Vector3 targetPos) //checks if stuff you are trying to walk is walkable 
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, GameLayers.i.SolidLayer | GameLayers.i.InteractableLayer) != null)
        {
            return false;
        } 
        return true;

    }
    public void LookToMe(Vector3 targetPos)
    {
        var xdifference = Mathf.Floor(targetPos.x) - Mathf.Floor(transform.position.x);
        var ydifference = Mathf.Floor(targetPos.y) - Mathf.Floor(transform.position.y);

        if(xdifference == 0 || ydifference == 0)
        {
            animator.MoveX = Mathf.Clamp(xdifference, -1f, 1f);
            animator.MoveY  = Mathf.Clamp(ydifference,-1f,1f);
            
        }
        else
        {
            Debug.LogError("Look to me properly if you want me to look to you !!! (Error in lookTOME) ");
        }
    }
    public CharacterAnimator Animator
    {
        get => animator;
    }
    public void SetPositionAndSnap(Vector2 pos)
    {
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f + OffsetY;

        transform.position = pos;
    }
}

