using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[System.Serializable]
public class Politician 
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;

    public Politician(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        level = pLevel;
        Init();
    }
    public PokemonBase Base {
        get
        {
            return _base;
        }
    }
    public int Level {
        get
        {
            return level;
        }
    }

    public int HP {get;set;}
    public List<Move> Moves {get;set;}
    public Move CurrentMove {get;set;}
    public Dictionary<Stat, int> Stats {get; private set;}

    public Dictionary<Stat, int> StatBoosts {get; private set;}
    public Condition Status {get; private set;}
    public int StatusTime { get; set; }
    public bool HPChanged {get; set;}

    public Condition VolitileStatus {get; private set;}
    public int VolitileStatusTime { get; set; }
    public int Exp {get; set;}
    public event System.Action OnStatusChanged;
    public Queue <string> StatusChanges {get; private set;}
    public void Init()
    {
        // Level based move adding, max moves is 4 
        Moves= new List<Move>();
        foreach(var move in Base.LernableMoves)
        {
            if(move.Level <= Level)
            {
                Moves.Add(new Move(move.Base));
            }
            if(Moves.Count >= 4)
            {
                break;
            }
        }
        Exp = Base.GetExpForLevel(Level);
        CalculateStats();
        HP = MaxHp;

        StatusChanges = new Queue<string>();
        ResetStatBoost();
        Status = null;
        VolitileStatus = null;
    }
    void ResetStatBoost()
    {
        StatBoosts = new Dictionary<Stat, int>()
        {
            {Stat.Attack,0},
            {Stat.Deffence,0},
            {Stat.SpAttack,0},
            {Stat.SpDeffence,0},
            {Stat.Speed,0},
            {Stat.Accuracy,0},
            {Stat.Evasion,0}

        };        
    }
    void CalculateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt((Base.Attack *Level) / 100f) + 5);
        Stats.Add(Stat.Deffence, Mathf.FloorToInt((Base.Deffence *Level) / 100f) + 5);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt((Base.SpAttack *Level) / 100f) + 5);
        Stats.Add(Stat.SpDeffence, Mathf.FloorToInt((Base.SpDeffence *Level) / 100f) + 5);
        Stats.Add(Stat.Speed, Mathf.FloorToInt((Base.Speed *Level) / 100f) + 5);

        MaxHp = Mathf.FloorToInt((Base.MaxHp *Level) / 100f) + 10 + level;
    }
    int GetStat(Stat stat)
    {
        int statval = Stats[stat];

        int boostpoints = StatBoosts[stat];
        var boostValues = new float[] {1f, 1.5f , 2f , 2.5f , 3f , 3.5f , 4f};

        if(boostpoints >= 0)
        {
            statval = Mathf.FloorToInt(statval * boostValues[boostpoints]);
        }
        else
        {
            statval = Mathf.FloorToInt(statval / boostValues[-boostpoints]);
        }

        return statval;
    }
    public void UpdateHP(int damage)
    {
        HP = Mathf.Clamp(HP-damage,0,MaxHp);
        HPChanged = true;
    }
    public void ApplyBoosts(List<StatBoost> statBoosts)
    {
        foreach(var statBoost in statBoosts)
        {
            var stat = statBoost.stat;
            var boost = statBoost.boost;

            StatBoosts[stat] = Mathf.Clamp(StatBoosts[stat] + boost,-6,6);
            if(boost > 0)
            {
                StatusChanges.Enqueue($"Your {Base.Name} is feeling better, {StatBoosts[stat]} rose !");
            }
            else
                StatusChanges.Enqueue($"Your {Base.Name} is not feeling so hot, {StatBoosts[stat]} fell !");
        }
    }

    public bool CheckForLevelUp()
    {
        if(Exp > Base.GetExpForLevel(level + 1))
        {
            ++level;
            return true;
        }
        return false; 
    }
    public int Attack{
        get {return GetStat(Stat.Attack);}
    }

    public int Deffence{
        get {return GetStat(Stat.Deffence);}
    }

    public int SpAttack{
        get {return GetStat(Stat.SpAttack);}
    }
    public int SpDeffence{
        get {return GetStat(Stat.SpDeffence);}
    }
    public int Speed{
        get {return GetStat(Stat.Speed);}
    }
    public int MaxHp{
       get; private set;
    }

    public DamageReport TakeDammage(Move move, Politician attacker)
    {   
        float critical = 1f;
        if(Random.value * 100f < 7.25f)
        {
            critical = 2f;
        }

        float type = TypeChart.GetAffectiveness(move.Base.Type, this.Base.type1) * TypeChart.GetAffectiveness(move.Base.Type, this.Base.type2);
        // Formula for calculating dammage, it scales as levels go

        var damageDetails = new DamageReport()
        {
            TypeEffectiveness = type,
            Critical = critical,
            Fainted = false 
        };
        
        float attack = (move.Base.Category == MoveCategory.Special) ? attacker.SpAttack : attacker.Attack;
        float deffence  = (move.Base.Category == MoveCategory.Special) ? SpDeffence : Deffence;

        float modifiers = Random.Range(0.85f, 1f) * type *critical;
        float a = (2*attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attack/deffence) +2 ;
        int dammage = Mathf.FloorToInt(d* modifiers);

        UpdateHP(dammage);

        return damageDetails;
        
    }

    public bool OnBeforeMove()
    {   
        bool canPerfromMove = true;
        if(Status?.OnBeforeMove != null)
        {
            if(!Status.OnBeforeMove(this))
            {
                canPerfromMove = false;
            }
        }
        if(VolitileStatus?.OnBeforeMove != null)
        {
            if(!VolitileStatus.OnBeforeMove(this))
            {
                canPerfromMove = false;
            }
        }

        return canPerfromMove;
    }
    public void OnAfterTurn()
    {
        Status?.OnAfterTurn?.Invoke(this);
        VolitileStatus?.OnAfterTurn?.Invoke(this);
    }
    public void SetStatus(ConditionID conditionId)
    {   
        if(Status != null)
        {
            return;
        }
        Status = ConditionsDB.Conditions[conditionId];
        Status?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {Status.StartMessage}");
        OnStatusChanged?.Invoke();
    }

    public void CureStatus()
    {
        Status = null;
        OnStatusChanged?.Invoke();
    }

    public void SetVolatileStatus(ConditionID conditionID)
    {   
        if(VolitileStatus != null)
        {
            return;
        }
        VolitileStatus = ConditionsDB.Conditions[conditionID];
        VolitileStatus?.OnStart?.Invoke(this);
        StatusChanges.Enqueue($"{Base.Name} {VolitileStatus.StartMessage}");
    }
    public void CureVolatileStatus()
    {
        VolitileStatus = null;
    }
    public Move GetRandomMove()
    {
        var possiblemoves = Moves.Where(x => x.RP > 0).ToList();

        int r = Random.Range(0 , possiblemoves.Count);
        return possiblemoves[r];
    }
    public void OnBattleOver()
    {
        VolitileStatus = null;
        ResetStatBoost();
    }
} 
public class DamageReport
{
    public bool Fainted { get; set;}
    public float Critical { get; set;}
    public float TypeEffectiveness { get; set;}

}

