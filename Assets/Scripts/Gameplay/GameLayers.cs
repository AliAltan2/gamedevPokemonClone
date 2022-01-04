using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidobjectsLayer;
    [SerializeField] LayerMask interactableLayer; 
    [SerializeField] LayerMask grassLayer;    
    [SerializeField] LayerMask playerLayer;    
    [SerializeField] LayerMask fovLayer;    
    [SerializeField] LayerMask portalLayer;    

    public static GameLayers i {get; set;} // i stands for instance
    private void Awake() {
        i=this;
    }
    public LayerMask SolidLayer
    {
        get => solidobjectsLayer;
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }

    public LayerMask FOVLayer
    {
        get => fovLayer;
    }
    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }
    public LayerMask GrassLayer
    {
        get => grassLayer;
    }
    public LayerMask PortalLayer
    {
        get => portalLayer;
    }
    public LayerMask TriggerableLayers
    {
        get => grassLayer | fovLayer | portalLayer;
    }
}
