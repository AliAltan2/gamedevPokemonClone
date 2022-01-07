using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class Fader : MonoBehaviour
{
    Image image;
    private void Awake() {
        image = GetComponent<Image>();
    }

    public IEnumerator FadeIn(float time)
    {
        yield return image.DOFade(1f, time).WaitForCompletion();
    }

    public IEnumerator FadeOut(float time)
    {
        yield return image.DOFade(0f, time).WaitForCompletion();
    }
    
}
