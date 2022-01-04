using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition : MonoBehaviour
{
    public ConditionID Id {get; set;}
    public string Name { get; set; }
    public string Description { get; set; }

    public string StartMessage { get; set; }

    public Action<Politician> OnStart {get; set;}
    public Func<Politician,bool> OnBeforeMove { get; set; }

    public Action <Politician> OnAfterTurn { get; set; }
}
