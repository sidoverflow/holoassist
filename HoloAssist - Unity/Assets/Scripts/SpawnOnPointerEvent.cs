using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


// Example script that spawns a prefab at the pointer hit location.


[AddComponentMenu("Scripts/MRTK/Examples/SpawnOnPointerEvent")]
public class SpawnOnPointerEvent : MonoBehaviour
    {
        

        NavMeshAgent agent;


        
            // Path, coordinate list, route display Renderer 
        NavMeshPath path = null;
        Vector3[] positions = new Vector3[9];

        public LineRenderer lr;

        public GameObject cl;

        public GameObject dest;

        public GameObject parentObject;

    public void Start()
        {


            
            cl.SetActive(false);
            lr.enabled = false;
           
    
        }

        public void Spawn(MixedRealityPointerEventData eventData)
        {
                var foundPosition = GetPositionOnSpatialMap();
                Vector3 hitPos, hitNormal;
                RaycastHit hitInfo;
                Vector3 UiRayCastOrigin = Camera.main.transform.position;
                Vector3 UiRayCastDirection = Camera.main.transform.forward;
                //CoreServices.SpatialAwarenessSystem.SuspendObservers();
                if (UnityEngine.Physics.Raycast(UiRayCastOrigin, UiRayCastDirection, out hitInfo))
                {

                    agent = cl.GetComponent<NavMeshAgent>();
                    if (!cl.activeSelf)
                    {
                        cl.GetComponent<NavMeshAgent>().Warp(hitInfo.point);
                        cl.SetActive(true);

                    }
                    else
                    {
                        cl.GetComponent<NavMeshAgent>().destination = dest.transform.position;
                        lr.enabled = true;

                        // path computation 
                        path = new NavMeshPath();
                        NavMesh.CalculatePath(cl.GetComponent<NavMeshAgent>().transform.position, cl.GetComponent<NavMeshAgent>().destination, NavMesh.AllAreas, path);
                        positions = path.corners;

                        // root drawing 
                        lr.widthMultiplier = 0.2F;
                        lr.positionCount = positions.Length;

                        for (int i = 0; i < positions.Length; i++)
                        {
                            Debug.Log("point" + i + "=" + positions[i]);

                            lr.SetPosition(i, positions[i]);

                        }
                        
                    }

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
