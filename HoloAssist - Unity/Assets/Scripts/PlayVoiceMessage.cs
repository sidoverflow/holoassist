using Microsoft.MixedReality.Toolkit;
using UnityEngine;

public class PlayVoiceMessage : MonoBehaviour {

    public static PlayVoiceMessage Instance { get; private set; }
    
    public GameObject photoCaptureManagerGmObj;
    
    void Awake()
    {
        Instance = this;
    }

    public void PlayTextToSpeechMessage(MCSComputerVisionOCRDto computerVisionOCR)
    {
        string message = string.Empty;

        if (string.IsNullOrEmpty(computerVisionOCR.text))
            message = "I couldn't detect text";
        else
            message = string.Format("The text says, {0}", computerVisionOCR.text);

        // Try and get a TTS Manager
        TextToSpeech tts = null;

        if (photoCaptureManagerGmObj != null)
        {
            tts = photoCaptureManagerGmObj.GetComponent<TextToSpeech>();
        }

        if (tts != null)
        {
            //Play voice message
            tts.StartSpeaking(message);
        }
    }

    public void PlayTextToSpeechMessage(ODResults result)
    {
        string message = string.Empty;

        if (string.IsNullOrEmpty(result.ToString()))
            message = "I couldn't analyze the scene.";
        else
            message = (result.description.captions[0].confidence > 0.85f) ? string.Format("The scene is likely, {0}", result.ToString()) : string.Format("The scene might be, {0}", result.ToString());


        // Try and get a TTS Manager
        TextToSpeech tts = null;

        if (photoCaptureManagerGmObj != null)
        {
            tts = photoCaptureManagerGmObj.GetComponent<TextToSpeech>();
        }

        if (tts != null)
        {
            //Play voice message
            tts.StartSpeaking(message);
        }
    }

    public void PlayTextToSpeechMessage(PDResults result)
    {
        string message = string.Empty;

        if (result.faces.Count == 0)
            message = "No people were detected on your path";
        else
            message = (result.faces.Count == 1) ? "1 person detected on the path" : result.faces.Count + " people detected on the path";


        // Try and get a TTS Manager
        TextToSpeech tts = null;

        if (photoCaptureManagerGmObj != null)
        {
            tts = photoCaptureManagerGmObj.GetComponent<TextToSpeech>();
        }

        if (tts != null)
        {
            //Play voice message
            tts.StartSpeaking(message);
        }
    }
}
