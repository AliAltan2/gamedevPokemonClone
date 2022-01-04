using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{   
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] FacingDirections defaultDirection = FacingDirections.Down;
    public float MoveX {get; set;}
    public float MoveY {get; set;}

    public bool IsMoving {get; set;}
//---------------------------------------------------------------------------

    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;
    bool wasMoving;
    SpriteAnimator currentAnim;
    SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites,spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites,spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites,spriteRenderer);
        SetFaceDirection(defaultDirection);
        currentAnim = walkDownAnim;
    }

    private void Update() {

        var previousAnim = currentAnim;
        if(MoveX == 1)
        {
            currentAnim = walkRightAnim;
        }
        else if (MoveX == -1 )
        {
            currentAnim = walkLeftAnim;
        }
        else if(MoveY == 1)
        {
            currentAnim = walkUpAnim;
        }
        else if(MoveY == -1)
        {
            currentAnim = walkDownAnim;
        }

        if (currentAnim != previousAnim || IsMoving != wasMoving)
        {   

            currentAnim.Start();
        }
        if(IsMoving)
        {
            currentAnim.HandleUpdate();
        }
        else
        {
            spriteRenderer.sprite = currentAnim.Frames[0];
        }

        wasMoving = IsMoving;
    }
    public void SetFaceDirection(FacingDirections direction)
    {
        if(direction == FacingDirections.Right)
        {
            MoveX = 1;
        }else if(direction == FacingDirections.Left)
        {
            MoveX = -1;
        }else if(direction == FacingDirections.Up)
        {
            MoveY = 1;
        }else if(direction == FacingDirections.Down)
        {
            MoveY = -1;
        }
    }
    public FacingDirections DefaultDirection{
        get => defaultDirection;    
    }
}
public enum FacingDirections
{
    Up, Down, Left, Right
}
