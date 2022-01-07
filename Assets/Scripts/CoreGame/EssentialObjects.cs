using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class EssentialObjects : MonoBehaviour
{
    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
