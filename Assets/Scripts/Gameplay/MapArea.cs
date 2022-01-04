using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Politician> partylessPoliticians;

    public Politician GetRandomPolitician()
    {
        var partylessPol = partylessPoliticians[Random.Range(0,partylessPoliticians.Count)];
        partylessPol.Init();
        return partylessPol;
    }
}
