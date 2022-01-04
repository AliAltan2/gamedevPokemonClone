using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
