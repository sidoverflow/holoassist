using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

[AddComponentMenu("Scripts/MRTK/Examples/SpawnOnPointerEvent")]
public class SpawnOnPointerEvent : MonoBehaviour
{
    // Path, coordinate list, route display Renderer
    private NavMeshPath path = null;

    private Vector3[] positions = new Vector3[9];

    public LineRenderer lr;

    public GameObject[] destinations;

    private Dictionary<string, GameObject> searchDestinations =
    new Dictionary<string, GameObject>();

    private GameObject activeDestination;

    public TextToSpeech textToSpeech;

    public GameObject importedMesh;

    public void Start()
    {
        foreach(GameObject gO in destinations)
        {
            searchDestinations.Add(gO.name, gO);
        }
        importedMesh.GetComponent<Renderer>().enabled = false;
        lr.enabled = false;
        textToSpeech.StartSpeaking("Welcome to HoloAssist! Say the destination out loud to see the path.");
    }
    private void Update()
    {
        if (activeDestination != null)
        {
            if (Vector3.SqrMagnitude(Camera.main.transform.position - activeDestination.transform.position) < 2.8)
            {

                textToSpeech.StartSpeaking("You have arrived.");
            } 
        }
    }

    public async void OnStairs()
    {
        searchDestinations.TryGetValue("Stairs", out activeDestination);
        await Route();
    }

    public async void OnBedroom()
    {
        searchDestinations.TryGetValue("Bedroom", out activeDestination);
        await Route();
    }

    public async void OnKitchen()
    {
        searchDestinations.TryGetValue("Kitchen", out activeDestination);
        await Route();
    }

    public async void OnLiving()
    {
        searchDestinations.TryGetValue("Living", out activeDestination);
        await Route();
    }

    async Task Route()
    {
            
            lr.enabled = true;
            path = new NavMeshPath();
            NavMesh.CalculatePath(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z), activeDestination.transform.position, NavMesh.AllAreas, path);
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
                float distance = Vector3.Distance(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z), new Vector3(activeDestination.transform.position.x, 0, activeDestination.transform.position.z));
                textToSpeech.StartSpeaking("Calculating route! " + activeDestination.name + " is " + (float)Mathf.Round(distance * 10f) / 10f + " metres away.");
            }
    }

    private static int _meshPhysicsLayer = 0;

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
    }
}