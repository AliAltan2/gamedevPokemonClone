using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class SpriteAnimator 
{
    SpriteRenderer   spriteRenderer;
    List<Sprite>    frames;
    float frameRate;
    int currentFrame;
    float time;

    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer,float frameRate=0.16f)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        time = 0f;
        spriteRenderer.sprite = frames[0];

    }
    public void HandleUpdate() {
        time += Time.deltaTime;
        if(time > frameRate)
        {
            currentFrame = (currentFrame+1) % frames.Count ;
            spriteRenderer.sprite = frames[currentFrame];
            time -= frameRate;
        }
    }
    public List <Sprite> Frames
    {
        get {return frames;}
    }
}
