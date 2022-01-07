using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Source : https://www.youtube.com/channel/UCswdeChigkx5uN1PwgfTqzQ Also known as Youtube/Game Dev Experiments
public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectsPrefab;
    private void Awake() {
        var existingObjects = FindObjectsOfType<EssentialObjects>();

        if (existingObjects.Length == 0)
        {
            Instantiate(essentialObjectsPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
