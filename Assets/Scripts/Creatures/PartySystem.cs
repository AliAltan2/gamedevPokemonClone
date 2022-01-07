using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class PartySystem : MonoBehaviour
{
    [SerializeField] List<Politician> politicians;

    public List <Politician> Politicians   
    {
        get 
        {
            return politicians;
        }
    }
    private void Start() 
    {
        foreach (var politician in politicians)
        {
            politician.Init();
        }
    }

    public Politician GetResonablePolitician()
    {
        return politicians.Where(x => x.HP > 0).FirstOrDefault();
    }
    public void AddPolitician(Politician newPolitician)
    {
        if(politicians.Count < 6)
        {
            politicians.Add(newPolitician);
        }else
        {
            // Nothing lmao
        }
         
    }
}
