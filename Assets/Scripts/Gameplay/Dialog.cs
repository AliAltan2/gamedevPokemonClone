using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
[System.Serializable]
public class Dialog 
{
    [SerializeField] List<string> lines;
    public List<string> Lines
    {
        get{ return lines;}
    }
}
