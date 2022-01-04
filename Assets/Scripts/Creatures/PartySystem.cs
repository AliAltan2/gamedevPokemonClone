using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
