using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshBaker : MonoBehaviour

    
{
    NavMeshSurface[] navMeshSurfaces;
    int j = 0;
    // Start is called before the first frame update
    void Awake()
    {
        var observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySpatialAwarenessMeshObserver>();
        navMeshSurfaces = new NavMeshSurface[observer.Meshes.Count];
        // Loop through all known Meshes
        foreach (SpatialAwarenessMeshObject meshObject in observer.Meshes.Values)
        {
           

            Debug.Log(meshObject.Id);
            navMeshSurfaces[j] = meshObject.GameObject.GetComponent<NavMeshSurface>();
            j++;
        }
        
        for (int i = 0; i < navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
