using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int SceneToLoad = -1;
    [SerializeField] Transform spawnPoint;

    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }
    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        yield return SceneManager.LoadSceneAsync(SceneToLoad);
        
        var destinationPortal = FindObjectsOfType<Portal>().First(x => x != this);
        player.Character.SetPositionAndSnap(destinationPortal.spawnPoint.position);

        Destroy(gameObject);
    }
    public Transform SpawnPoint() => spawnPoint;
}

