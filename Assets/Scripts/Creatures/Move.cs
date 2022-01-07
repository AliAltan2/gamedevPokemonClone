using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class Move 
{
    public MoveBase Base {get; set;}
    public int RP {get; set;}

    public Move(MoveBase pBase)
    {
        Base = pBase;
        RP = pBase.RP;
        
    }
}
