using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class BattleUnit : MonoBehaviour
{

    [SerializeField] bool isPlayerUnit; 
    [SerializeField] BattleHud hud;
    public bool IsPlayerUnit{
        get{return isPlayerUnit;}
    }
    public BattleHud Hud
    {
        get{return hud;}
    }
    public Politician Politician {get; set;}
    Image image;
    Vector3 originalPos;
    Color originalCol;
    private void Awake() {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalCol = image.color;
    }
    public void CleanUp(){
        hud.gameObject.SetActive(false);
    }
    public void Setup(Politician politician)
    {
        Politician = politician;
        if(isPlayerUnit)
        {
            image.sprite= Politician.Base.BackSprite;
        }
        else
        {
            image.sprite = Politician.Base.FrontSprite;
        }

        hud.gameObject.SetActive(true);

        hud.SetData(politician);
        transform.localScale = new Vector3(1,1,1);
        image.color = originalCol;
        PlayerEnterAnimation();
    }
    public void PlayerEnterAnimation()
    {
        if(isPlayerUnit)
        {
            image.transform.localPosition = new Vector3 (-500f, originalPos.y);
        }
        else
        {
            image.transform.localPosition = new Vector3 (500f, originalPos.y);
        }
        image.transform.DOLocalMoveX(originalPos.x ,1.5f);
    }
    public void AttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if(isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 40f, 0.25f));
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 40f, 0.25f));
        }

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void HitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.red, 0.1f));
        sequence.Append(image.DOColor(originalCol, 0.1f));
    }
    public void FaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y -150f, 0.5f));
        sequence.Join(image.DOFade(0f,0.5f));
    }
    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(0,1f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f,0.3f,1f),0.5f));
        yield return sequence.WaitForCompletion();
    }
    public IEnumerator NoCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOFade(1,1f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y , 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f,1f,1f),0.5f));
        yield return sequence.WaitForCompletion();        
    }
}
