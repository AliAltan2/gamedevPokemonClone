using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
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
