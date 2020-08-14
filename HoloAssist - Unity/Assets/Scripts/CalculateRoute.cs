using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Vuforia;

public class CalculateRoute : MonoBehaviour
{
    // Path, coordinate list, route display Renderer
    private NavMeshPath path = null;

    private Vector3[] positions = new Vector3[9];

    public LineRenderer lr;

    public GameObject[] destinations;

    private Dictionary<string, GameObject> searchDestinations =
    new Dictionary<string, GameObject>();

    public GameObject activeDestination;

    public Vector3 origin;

    public TextToSpeech textToSpeech;

    public GameObject importedMesh;

    public GameObject CrowdManager;

    private IMixedRealitySpatialAwarenessMeshObserver observer;

    public void Start()
    {
        observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        foreach (GameObject gO in destinations)
        {
            searchDestinations.Add(gO.name, gO);
        }
        lr.enabled = false;
        textToSpeech.StartSpeaking("Welcome to HoloAssist! Wait a few seconds to scan your surroundings. Say the destination out loud to see the path.");
    }
    private void Update()
    {
        if (activeDestination != null)
        {
            if (Vector3.SqrMagnitude(Camera.main.transform.position - activeDestination.transform.position) < 3 && !textToSpeech.IsSpeaking())
            {

                textToSpeech.StartSpeaking("You have arrived.");
                lr.enabled = false;
                activeDestination = null;
                //Resume mesh observation
                observer.Resume();
            } 
        }
    }

    public void OnStairs()
    {
        searchDestinations.TryGetValue("Stairs", out activeDestination);
        Route();
    }

    public void OnBedroom()
    {
        searchDestinations.TryGetValue("Room A", out activeDestination);
        Route();
    }

    public void OnKitchen()
    {
        searchDestinations.TryGetValue("Room C", out activeDestination);
        Route();
    }

    public void OnLiving()
    {
        searchDestinations.TryGetValue("Room B", out activeDestination);
        Route();
    }

    public void OnRoomD()
    {
        searchDestinations.TryGetValue("Room D", out activeDestination);
        Route();
    }

    async void Route()
    {
        // Suspends observation of spatial mesh data
        observer.Suspend();

        lr.enabled = true;
        path = new NavMeshPath();
        origin = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - 2.3f, Camera.main.transform.position.z);
        NavMesh.CalculatePath(origin, new Vector3(activeDestination.transform.position.x, activeDestination.transform.position.y , activeDestination.transform.position.z), NavMesh.AllAreas, path);
        positions = path.corners;

        // root drawing
        lr.widthMultiplier = 0.1F;
        lr.positionCount = positions.Length;

        for (int i = 0; i < positions.Length; i++)
        {
            Debug.Log("point" + i + "=" + positions[i]);

            lr.SetPosition(i, positions[i]);
        }

        if (path.corners.IsValidArray())
        {
            
            Debug.LogWarning("The origin is: " + origin);
            Debug.LogWarning("The destination is: " + new Vector3(activeDestination.transform.position.x, -0.7f, activeDestination.transform.position.z));
            float distance = Vector3.Distance(origin, new Vector3(activeDestination.transform.position.x, activeDestination.transform.position.y , activeDestination.transform.position.z));
            textToSpeech.StartSpeaking(activeDestination.name + " is " + (float)Mathf.Round(distance * 10f) / 10f + " metres away.");

                CrowdManager.GetComponent<PhotoCaptureHandler>().StartPhotoCapture(); 
        }
        else
        {
            textToSpeech.StartSpeaking("Could not find a route to " + activeDestination.name + ". Try again after moving around a few steps.");
        }
    }

/*    private static int _meshPhysicsLayer = 0;

    private static int GetSpatialMeshMask()
    {
        if (_meshPhysicsLayer == 0)
        {
            var spatialMappingConfig = CoreServices.SpatialAwarenessSystem.ConfigurationProfile as
                MixedRealitySpatialAwarenessSystemProfile;
            if (spatialMappingConfig != null)
            {
                foreach (var config in spatialMappingConfig.ObserverConfigurations)
                {
                    var observerProfile = config.ObserverProfile
                        as MixedRealitySpatialAwarenessMeshObserverProfile;
                    if (observerProfile != null)
                    {
                        _meshPhysicsLayer |= (1 << observerProfile.MeshPhysicsLayer);
                    }
                }
            }
        }

        return _meshPhysicsLayer;
    }

    public static Vector3? GetPositionOnSpatialMap(float maxDistance = 2)
    {
        RaycastHit hitInfo;
        var transform = Camera.main.transform;
        if (UnityEngine.Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDistance, GetSpatialMeshMask()))
        {
            return hitInfo.point;
        }
        return null;
    }*/
}