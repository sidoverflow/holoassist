using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.AI;

public class NavigationHelper : MonoBehaviour
{

    public NavMeshAgent agent;
    public static Animator animator;
    public GameObject routingManager;
    CalculateRoute destinationHandler;
    GameObject localActiveDestination;
    public TextToSpeech textToSpeech;

    // Start is called before the first frame update
    void Start()
    {
        agent.enabled = false;
        
    }

    // Update is called once per frame
    void Update()
    {
       if (agent.enabled == true && !textToSpeech.IsSpeaking())
        {

            textToSpeech.StartSpeaking("Follow me");
            if (Vector3.SqrMagnitude(Camera.main.transform.position - agent.transform.position) > 2.7)
            {
                agent.isStopped = true;
            }
            else
            {
                agent.isStopped = false;
            }
            if (Vector3.SqrMagnitude(agent.transform.position - localActiveDestination.transform.position) < 3)
            {
                agent.enabled = false;
                textToSpeech.StopSpeaking();
            }
        }

       
    }

    public void Navigate()
    {
        
        destinationHandler = routingManager.GetComponent<CalculateRoute>();
        localActiveDestination = destinationHandler.activeDestination;
        agent.transform.position = new Vector3(destinationHandler.origin.x, destinationHandler.origin.y + 2f , destinationHandler.origin.z);
        agent.enabled = true;
        agent.SetDestination(destinationHandler.activeDestination.transform.position);
    }
}
