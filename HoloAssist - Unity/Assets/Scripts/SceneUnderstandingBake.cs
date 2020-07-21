using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using NumericsConversion;
using UnityEngine;
#if ENABLE_WINMD_SUPPORT
using Microsoft.MixedReality.SceneUnderstanding;
using Windows.Perception.Spatial;
using Windows.Perception.Spatial.Preview;
using UnityEngine.XR.WSA;
#endif

    public class SceneUnderstandingBake : MonoBehaviour
    {
        public GameObject parentObject;
        public GameObject markerPrefab;
        public GameObject plane;
        public Material quadMaterial;

        public SceneUnderstandingBake()
        {
            this.markers = new List<GameObject>();
            this.quads = new List<GameObject>();
            this.initialised = false;
        }

    void Update()
        {
#if ENABLE_WINMD_SUPPORT
        // TODO: Doing all of this every update right now based on what I saw in the doc 
        // here https://docs.microsoft.com/en-us/windows/mixed-reality/scene-understanding-sdk#dealing-with-transforms
        // but that might be overkill.
        // Additionally, wondering to what extent I should be releasing these COM objects
        // as I've been lazy to date.
        // Hence - apply a pinch of salt to this...
        if (this.lastScene != null)
        {
            var node = this.lastScene.OriginSpatialGraphNodeId;
 
            var sceneCoordSystem = SpatialGraphInteropPreview.CreateCoordinateSystemForNode(node);
 
            var unityCoordSystem =
                (SpatialCoordinateSystem)System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(
                    WorldManager.GetNativeISpatialCoordinateSystemPtr());
 
            var transform = sceneCoordSystem.TryGetTransformTo(unityCoordSystem);
 
            if (transform.HasValue)
            {
                var sceneToWorldUnity = transform.Value.ToUnity();
 
                this.parentObject.transform.SetPositionAndRotation(
                    sceneToWorldUnity.GetColumn(3), sceneToWorldUnity.rotation);
            }
        }
#endif
        }
        // These 4 methods are wired to voice commands via MRTK...
        public async void OnWalls()
        {
#if ENABLE_WINMD_SUPPORT
        await this.ComputeAsync(SceneObjectKind.Wall);
#endif
        }
        public async void OnFloor()
        {
#if ENABLE_WINMD_SUPPORT
        await this.ComputeAsync(SceneObjectKind.Floor);
#endif
        }
        public async void OnCeiling()
        {
#if ENABLE_WINMD_SUPPORT
        await this.ComputeAsync(SceneObjectKind.Ceiling);
#endif
        }
        public async void OnPlatform()
        {
#if ENABLE_WINMD_SUPPORT
        await this.ComputeAsync(SceneObjectKind.Platform);
#endif
        }
        void ClearChildren()
        {
            foreach (var child in this.markers)
            {
                Destroy(child);
            }
            foreach (var child in this.quads)
            {
                Destroy(child);
            }
            this.markers.Clear();
            this.quads.Clear();
        }

#if ENABLE_WINMD_SUPPORT
    async Task InitialiseAsync()
    {
        if (!this.initialised)
        {
            if (SceneObserver.IsSupported())
            {
                var access = await SceneObserver.RequestAccessAsync();
 
                if (access == SceneObserverAccessStatus.Allowed)
                {
                    this.initialised = true;
                }
            }
        }
    }
    async Task ComputeAsync(SceneObjectKind sceneObjectKind)
    {
        this.ClearChildren();
 
        await this.InitialiseAsync();
 
        if (this.initialised)
        {
            var querySettings = new SceneQuerySettings()
            {
                EnableWorldMesh = false,
                EnableSceneObjectQuads = true,
                EnableSceneObjectMeshes = false,
                EnableOnlyObservedSceneObjects = false
            };
            this.lastScene = await SceneObserver.ComputeAsync(querySettings, searchRadius);
 
            if (this.lastScene != null)
            {
                foreach (var sceneObject in this.lastScene.SceneObjects)
                {
                    if (sceneObject.Kind == sceneObjectKind)
                    {
                        var marker = GameObject.Instantiate(this.markerPrefab);
 
                        marker.transform.SetParent(this.parentObject.transform);;

                        marker.transform.localPosition = sceneObject.Position.ToUnity();
                        marker.transform.localRotation = sceneObject.Orientation.ToUnity();
  
      
    
                        this.markers.Add(marker);
 
                        foreach (var sceneQuad in sceneObject.Quads)
                        {
                            var quad = GameObject.CreatePrimitive(PrimitiveType.Cube);
 
                            quad.transform.SetParent(marker.transform, false);
 
                            quad.transform.localScale = new Vector3(
                                sceneQuad.Extents.X, sceneQuad.Extents.Y, 0.025f);
 
                            quad.GetComponent<Renderer>().material = this.quadMaterial;
                            quad.AddComponent<NavMeshSourceTag>();
                            quad.GetComponent<Renderer>().enabled = false;
                        }
                    }
                }
            }
        }
    }
#endif

#if ENABLE_WINMD_SUPPORT
    Scene lastScene;
#endif

    List<GameObject> markers;
        List<GameObject> quads;
        bool initialised;
        static readonly float searchRadius = 5.0f;
    }

