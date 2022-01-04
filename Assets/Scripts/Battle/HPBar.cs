using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health;

    public void SetHP(float hpNormalized)
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f);

    }
    public IEnumerator SmoothHP(float NewHP)
    {
        float curHP = health.transform.localScale.x;
        float changeAmt = curHP - NewHP;

        while(curHP - NewHP > Mathf.Epsilon)
        {
            curHP -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3 (curHP, 1f);
            yield return null;
        }
        health.transform.localScale = new Vector3 (NewHP,1f);
    }
}
