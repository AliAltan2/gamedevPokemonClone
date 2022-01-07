using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
[CreateAssetMenu(fileName = "PokemonBase", menuName = "Not Pokemon/Politician")]
public class PokemonBase : ScriptableObject 
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] public PoliticianType type1;
    [SerializeField] public PoliticianType type2;

    [SerializeField] int maxHP;
    [SerializeField] int attack;
    [SerializeField] int deffence;
    [SerializeField] int spAttack;
    [SerializeField] int spDeffence;
    [SerializeField] int speed;
    [SerializeField] int expYield;
    [SerializeField] int catchRate = 255;
    [SerializeField] GrowthRate growthRate;

    [SerializeField] List<LernableMoves> lernableMoves;
    public int GetExpForLevel(int level) // All of these formulas come from actual pokemon games 
    {
        if(growthRate == GrowthRate.Fast)
        {
            return 4 * (level * level *level) / 5;
        }else if(growthRate == GrowthRate.FastMedium)
        {
            return 3 * (level * level *level) / 5;
        }else if(growthRate == GrowthRate.Medium)
        {
            return 2 * (level * level *level) / 5;
        }else if(growthRate == GrowthRate.Slow)
        {
            return 1 * (level * level *level) / 5;
        }
        return -1;
    }
    public string Name
    {
        get{ return name;}
    }
    public string Description
    {
        get{ return description;}
    }
    public int MaxHp
    {
        get{ return maxHP;}
    }
    public int Attack
    {
        get{ return attack;}
    }
    public Sprite FrontSprite
    {
        get { return frontSprite;}
    }
    public Sprite BackSprite
    {
        get { return backSprite;}
    }
    public int Deffence
    {
        get{ return deffence;}
    }
    public int SpAttack
    {
        get{ return spAttack;}
    }
    public int SpDeffence
    {
        get{ return spDeffence;}
    }
    public int Speed
    {
        get{ return speed;}
    }
    public List<LernableMoves> LernableMoves
    {
        get { return lernableMoves;}
    }
    public int CatchRate
    {
        get{return catchRate;}
    }
    public int ExpYield
    {
        get{return expYield;}
    }
    public GrowthRate GrowthRate
    {
        get{return growthRate;}
    }

}
[System.Serializable]
public class LernableMoves
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get{ return moveBase;}
    }
    public int Level
    {
        get {return level;}
    }
}
public enum PoliticianType
{
    None,
    Normal,
    Centralist,
    Conservative,
    Liberal,
    Communist,
    Monarchist,
    Radical,
    Activist,
    Pasifist,
    Ninja,
    
}
public class TypeChart
{
    static float[][] chart =
    {                    // Nor  Cen  Con  Lib  Com  Mon  Rad  Act  Pas Nin
        /*Nor*/new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1f , 1f ,1f , 1f},
        /*Cen*/new float[] { 1f , 1f , 0.5f , 0.5f , 1.3f , 1.5f , 0.8f , 1.5f ,1f , 1f}, 
        /*Con*/new float[] { 1f , 1f , 1f , 1.5f , 1.5f , 0.5f , 0.5f , 0.5f ,1.5f , 1f},
        /*Lib*/new float[] { 1f , 1f , 1.5f , 1f , 0.5f , 1.5f , 0.5f , 1f ,1.5f , 1f},
        /*Com*/new float[] { 1f , 1.5f , 0.5f , 0.5f , 1f , 1.5f , 1f , 1.5f ,1.5f , 1f},
        /*Mon*/new float[] { 1f , 1.5f , 0.5f , 0.5f , 1.5f , 1f , 0.5f , 1f ,1.5f , 1.5f},
        /*Rad*/new float[] { 1f , 1.5f , 1.5f , 1.5f , 1.5f , 1.5f , 1f , 1f ,1f , 1f},
        /*Act*/new float[] { 1f , 0.5f , 0.5f , 0.5f , 1.5f , 1f , 0.5f , 1f ,2f , 2f},
        /*Pas*/new float[] { 1f , 1f , 1f , 1f , 1f , 1f , 1.5f , 1.5f ,1f , 1f},
        /*Nin*/new float[] { 1.25f , 1.25f , 1.25f , 1.25f , 1.25f , 1.25f , 1.25f , 1.25f ,2f , 1.25f}
    };
    public static float GetAffectiveness(PoliticianType attackType, PoliticianType deffenceType)
    {
        if(attackType == PoliticianType.None || deffenceType == PoliticianType.None)
        {
            return 1;
        }
        int row = (int)attackType -1;
        int col = (int)deffenceType -1;

        return chart[row][col];
    }
}
public enum Stat
{
    Attack,
    Deffence,
    SpAttack,
    SpDeffence,
    Speed,
    Accuracy,
    Evasion
}
public enum GrowthRate
{
    Fast,FastMedium,Medium,Slow
}
