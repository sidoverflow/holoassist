using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
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

    public GameObject dest;
    public TextToSpeech textToSpeech;

    public void Start()
    {
        
        lr.enabled = false;
        textToSpeech.StartSpeaking("Welcome to HoloAssist! Say the destination out loud to see the path.");
    }
    private void Update()
    {
        if (Vector3.SqrMagnitude(Camera.main.transform.position - dest.transform.position) < 2.8)
        {

            textToSpeech.StartSpeaking("You have arrived at your destination");
        }
    }

    public async void OnStairs()
    {
        await Spawn();
    }

    async Task Spawn()
    {
        //RaycastHit hitInfo;
        //Vector3 UiRayCastOrigin = Camera.main.transform.position;
        //Vector3 UiRayCastDirection = Camera.main.transform.forward;

        //if (UnityEngine.Physics.Raycast(UiRayCastOrigin, UiRayCastDirection, out hitInfo))
        //{
            // path computation
            lr.enabled = true;
            path = new NavMeshPath();
            NavMesh.CalculatePath(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z), dest.transform.position, NavMesh.AllAreas, path);
            positions = path.corners;

            // root drawing
            lr.widthMultiplier = 0.1F;
            lr.positionCount = positions.Length;

            for (int i = 0; i < positions.Length; i++)
            {
                Debug.Log("point" + i + "=" + positions[i]);

                lr.SetPosition(i, positions[i]);
            }
        //}
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