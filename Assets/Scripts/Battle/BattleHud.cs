using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hPBar;
    [SerializeField] GameObject expBar;
    [SerializeField] Color demColor;
    [SerializeField] Color humColor;
    [SerializeField] Color depColor;
    [SerializeField] Color shoColor;
    [SerializeField] Color sleColor;
    [SerializeField] Color silColor;
    [SerializeField] Color frzColor;


    Politician _politician;
    Dictionary<ConditionID, Color> StatusColors;
    public void SetData(Politician politician)
    {
        _politician = politician;
        nameText.text = politician.Base.Name;
        SetLevel();
        SetStatusText();
        _politician.OnStatusChanged += SetStatusText;
        hPBar.SetHP((float)politician.HP / politician.MaxHp);
        SetExp();
        StatusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.dem, demColor},
            {ConditionID.hum, humColor},
            {ConditionID.dep, depColor},
            {ConditionID.sho, shoColor},
            {ConditionID.sle, sleColor},
            {ConditionID.sil, silColor},
            {ConditionID.frz, frzColor},

        };


    }

    void SetStatusText()
    {
        if(_politician.Status == null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _politician.Status.Id.ToString().ToUpper();
            statusText.color = StatusColors[_politician.Status.Id];
        }
    }
    public IEnumerator UpdateHP()
    {
        if(_politician.HPChanged)
        {
            yield return hPBar.SmoothHP((float)_politician.HP / _politician.MaxHp);  
            _politician.HPChanged = false;
        }

    }
    public void SetExp()
    {
        if(expBar == null)
        {
            return;
        }
        float normalizedExp = GetNormalExp();
        expBar.transform.localScale = new Vector3(normalizedExp,1,1);
    }
    public IEnumerator SetExpSmoothly(bool reset= false)
    {
        if(expBar == null)
        {
            yield break;
        }
        if(reset)
        {
            expBar.transform.localScale = new Vector3(0,1,1);
        }
        float normalizedExp = GetNormalExp();
        yield return expBar.transform.DOScaleX(normalizedExp,1.5f).WaitForCompletion();
    }
    float GetNormalExp()
    {
        int CurrentLevelExp = _politician.Base.GetExpForLevel(_politician.Level);
        int NextLevelExp = _politician.Base.GetExpForLevel(_politician.Level +1);

        float normalizedExp = (float)(_politician.Exp - CurrentLevelExp) / (NextLevelExp -CurrentLevelExp) ;
        return Mathf.Clamp01(normalizedExp);
    }
    public void SetLevel()
    {
        levelText.text = "Lvl " + _politician.Level;
        
    }
}
