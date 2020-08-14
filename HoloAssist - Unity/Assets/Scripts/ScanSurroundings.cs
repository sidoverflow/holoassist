using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanSurroundings : MonoBehaviour
{

    public GameObject[] destinations;
    public TextToSpeech textToSpeech;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Scan()
    {
        ArrayList closeDestinations = new ArrayList();
        var cameraPos = new Vector3(Camera.main.transform.position.x, -0.7f, Camera.main.transform.position.z);
        foreach (GameObject destination in destinations)
        {
            var destinationPos = new Vector3(destination.transform.position.x, -0.7f, destination.transform.position.z);
            if (Vector3.SqrMagnitude(cameraPos - destinationPos) < 9)
            {
                closeDestinations.Add(destination.name);
            }
        }
        if (closeDestinations.Count == 1)
        {
            textToSpeech.StartSpeaking(closeDestinations[0] + " is within 3 metres of your location."); 
        }
        else if (closeDestinations.Count > 1)
        {
            var announcement = "";
            foreach (var destination in closeDestinations)
            {
                announcement += destination + ", ";
            }
            textToSpeech.StartSpeaking(announcement + " are within 3 metres of your location.");
        }
        else
        {
            textToSpeech.StartSpeaking("There are no points of interest within 3 metres of your location.");
        }
       
    }
}
