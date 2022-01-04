using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB : MonoBehaviour
{   
    public static void Init()
    {
        foreach(var kvp in Conditions)
        {
            var conditionId = kvp.Key;
            var condition = kvp.Value;

            condition.Id = conditionId;
        }
    }
    public static Dictionary<ConditionID, Condition> Conditions {get; set;} = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.dem,
            new Condition()
            {
                Name = "Demoralized",
                StartMessage = "has been demoralized !",
                OnAfterTurn = (Politician Politician) => 
                {
                    Politician.UpdateHP(Politician.MaxHp/10);
                    Politician.StatusChanges.Enqueue($"{Politician.Base.Name} is getting demoralized !");
                }
            }
        },
        {
            ConditionID.hum,
            new Condition()
            {
                Name = "Humiliated",
                StartMessage = "has been humiliated !",
                OnAfterTurn = (Politician Politician) => 
                {
                    Politician.UpdateHP(Politician.MaxHp/7);
                    Politician.StatusChanges.Enqueue($"{Politician.Base.Name} is getting humiliated !");
                }
            }
        },
        {
            ConditionID.dep,
            new Condition()
            {
                Name = "Depressed",
                StartMessage = "has been depressed !",
                OnAfterTurn = (Politician politician) => 
                {
                    politician.UpdateHP(politician.MaxHp/4);
                    politician.StatusChanges.Enqueue($"{politician.Base.Name} is getting depressed !");
                }
            }
        },
        {
            ConditionID.sho,
            new Condition()
            {
                Name = "Shocked",
                StartMessage = "has been shocked !",
                OnBeforeMove = (Politician politician) =>
                {   
                    if(Random.Range(1,5) == 1)
                    {   
                        politician.StatusChanges.Enqueue($"{politician.Base.Name} has been shocked and can't say a word !");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.frz,
            new Condition()
            {
                Name = "Frozen",
                StartMessage = "has been freezed !",
                OnBeforeMove = (Politician politician) =>
                {   
                    if(Random.Range(1,4) == 1)
                    {   
                        politician.CureStatus();
                        politician.StatusChanges.Enqueue($"Looks like {politician.Base.Name}'s mouth is not frozen shut anymore !");
                        return true;
                    }
                    return false;
                }
            }
        },
        {
            ConditionID.sle,
            new Condition()
            {

                Name = "Sleep",
                StartMessage = "has falled asleep !",
                OnStart = (Politician politician) =>
                {   
                    politician.StatusTime = Random.Range(1,4);
                    Debug.Log($"Will be sleeping for {politician.StatusTime} moves");
                },
                OnBeforeMove = (Politician politician) =>
                {   
                    if(politician.StatusTime <= 0){
                        politician.CureStatus();
                        politician.StatusChanges.Enqueue($"{politician.Base.Name} finally woke up !");
                        return true;
                    }
                    politician.StatusTime--;
                    politician.StatusChanges.Enqueue($"{politician.Base.Name} is too bored from the talking and now he is taking a nap !");

                    return false;
                }
            }
        },
        {
            ConditionID.sil,
            new Condition()
            {
                Name = "Silanced",
                StartMessage = "has been silanced !",
                OnBeforeMove = (Politician politician) =>
                {   
                    if(Random.Range(1,6) == 1)
                    {   
                        politician.StatusChanges.Enqueue($"{politician.Base.Name} has been silanced and can't say a word !");
                        return false;
                    }
                    return true;
                }
            }
        },
        {
            ConditionID.con,
            new Condition()
            {

                Name = "Confusion",
                StartMessage = "has been confused !",
                OnStart = (Politician politician) =>
                {   
                    politician.VolitileStatusTime = Random.Range(1,4);
                    Debug.Log($"Will be confused for {politician.VolitileStatusTime} moves");
                },
                OnBeforeMove = (Politician politician) =>
                {   
                    if(politician.VolitileStatusTime <= 0){
                        politician.CureVolatileStatus();
                        politician.StatusChanges.Enqueue($"{politician.Base.Name} got out of his confusion !");
                        return true;
                    }

                    politician.VolitileStatusTime--;


                    if(Random.Range(1,3)==1)
                    {
                        return true;
                    }
                    politician.StatusChanges.Enqueue($"{politician.Base.Name} is confused !");
                    politician.UpdateHP(politician.MaxHp / 12);
                    politician.StatusChanges.Enqueue($"{politician.Base.Name} rambled around and made a weak argument!");
                    return false;
                }
            }
        },
    };
    public static float GetStatusBonus(Condition condition)
    {
        if(condition == null)
        {
            return 1f;
        }if(condition.Id == ConditionID.sle || condition.Id == ConditionID.frz)
        {
            return 2f;
        }if(condition.Id == ConditionID.dem || condition.Id == ConditionID.dep || condition.Id == ConditionID.hum)
        {
            return 1.25f;
        }
        return 1f;
    }
}

public enum ConditionID
{
    none,
    dem,
    hum,
    dep,
    sho,
    sle,
    sil,
    frz,
    con
}
