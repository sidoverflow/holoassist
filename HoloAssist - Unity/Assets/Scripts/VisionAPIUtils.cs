/*using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Windows.WebCam;

public class VisionAPIUtils : MonoBehaviour
{
    const string VISION_API_SUBSCRIPTION_KEY = "db5e46cfb2cb4d608ae9ede4f28010be";
    const string VISION_API_BASE_URL = "https://holoassistcomputervision.cognitiveservices.azure.com/";

    public  TextToSpeech textToSpeechManager;
    IEnumerator coroutine;
    PhotoCapture _photoCaptureObject = null;

    public IEnumerator MakeOCRRequest(byte[] bytes, string textComponent, Type type)
    {
      
        var headers = new Dictionary<string, string>()
    {
        {"Ocp-Apim-Subscription-Key", VISION_API_SUBSCRIPTION_KEY },
        {"Content-Type","application/octet-stream"}
    };
        string requestParameters = "visualFeatures=Description&language=en";
        string uri = VISION_API_BASE_URL + "/vision/v1.0/ocr?" + requestParameters;
        if ((bytes != null) && (bytes.Length > 0))
        {
            WWW www = new WWW(uri, bytes, headers);
            yield return www;

            if (www.error != null)
            {
                Debug.Log(www.error);
            }
            else
            {
                OCRAPIResults results = JsonUtility.FromJson<OCRAPIResults>(www.text);
                //TextUtils.setText(results.ToString(), textComponent, type);
                textToSpeechManager.StartSpeaking(results.ToString());
            }
        }
    }


    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        _photoCaptureObject = captureObject;

        // find the best supported resolution
        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        CameraParameters c = new CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = CapturePixelFormat.BGRA32;

        // start the capture
        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {       // take the picture
            _photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {       // couldn't take the picture. Show an error
            Debug.Log("Unable to Start Photo Mode");
        }
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        _photoCaptureObject.Dispose();
        _photoCaptureObject = null;
    }

    private void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            // Create our Texture2D for use and set the correct resolution
            Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
            Texture2D targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

            // Copy the raw image data into our target texture
            photoCaptureFrame.UploadImageDataToTexture(targetTexture);

            // encode as JPEG to send to cognitiva service api's
            var imageBytes = targetTexture.EncodeToJPG();

            ReadWords(imageBytes);
        }
        else
        {       // show error
            Debug.Log("Error: " + result.hResult);
        }
        // stop handling the picture
        _photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    public async void AnalyzeScene()
    {
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }


    public void ReadWords(byte[] image)
    {
        try
        {
            coroutine = MakeOCRRequest(image, "txtImageInfo", typeof(Text));
            StartCoroutine(coroutine);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

}*/