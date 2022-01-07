using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
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
