using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int SceneToLoad = -1;
    [SerializeField] Transform spawnPoint;
    [SerializeField] DestinationIdentitiy destinationPortal;

    PlayerController player;
    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        StartCoroutine(SwitchScene());
    }
    Fader fader;
    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }
    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        GameController.Instance.PausedScene(true);
        yield return fader.FadeIn(0.5f); 
        yield return SceneManager.LoadSceneAsync(SceneToLoad);
        
        var destinationPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
        player.Character.SetPositionAndSnap(destinationPortal.spawnPoint.position);
        
        yield return fader.FadeOut(0.5f); 
        GameController.Instance.PausedScene(false);
        Destroy(gameObject);
    }
    public Transform SpawnPoint() => spawnPoint;
}
public enum DestinationIdentitiy
{
    A,B,C,D,E,F
}

