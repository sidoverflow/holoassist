using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PhotoCaptureHandler : MonoBehaviour
{
    UnityEngine.Windows.WebCam.PhotoCapture photoCapture = null;

    public void StartPhotoCapture()
    {
        //Create capture async
        UnityEngine.Windows.WebCam.PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(UnityEngine.Windows.WebCam.PhotoCapture captureObject)
    {
        photoCapture = captureObject;

        Resolution cameraResolution = UnityEngine.Windows.WebCam.PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        UnityEngine.Windows.WebCam.CameraParameters c = new UnityEngine.Windows.WebCam.CameraParameters();
        c.hologramOpacity = 0.0f;
        c.cameraResolutionWidth = cameraResolution.width;
        c.cameraResolutionHeight = cameraResolution.height;
        c.pixelFormat = UnityEngine.Windows.WebCam.CapturePixelFormat.JPEG;

        captureObject.StartPhotoModeAsync(c, OnPhotoModeStarted);
    }

    private void OnStoppedPhotoMode(UnityEngine.Windows.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        photoCapture.Dispose();
        photoCapture = null;
    }

    private void OnPhotoModeStarted(UnityEngine.Windows.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        /* Load to Memory */
        if (result.success)
        {
            try
            {
                photoCapture.TakePhotoAsync(OnCapturedPhotoToMemory);
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError("System.ArgumentException:\n" + e.Message);
            }
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToDisk(UnityEngine.Windows.WebCam.PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            Debug.Log("Saved Photo to disk!");
            photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
        }
        else
        {
            Debug.Log("Failed to save Photo to disk");
        }
    }

    void OnCapturedPhotoToMemory(UnityEngine.Windows.WebCam.PhotoCapture.PhotoCaptureResult result, UnityEngine.Windows.WebCam.PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            List<byte> imageBufferList = new List<byte>();

            Debug.Log("OnCapturedPhotoToMemory Copy Started");

            photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

            Debug.Log("OnCapturedPhotoToMemory " + imageBufferList.Count);

            //Execute OCR Coroutine
            ExecuteMCSComputerVisionOCR(imageBufferList);
        }
        else
        {
            Debug.Log("Failed to save Photo to memory");
        }

        photoCapture.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    public void ExecuteMCSComputerVisionOCR(List<byte> imageBufferList)
    {
        if (this.gameObject.name == "DetectManager")
        {
            Debug.Log("Started PostToComputerVision OCR processing");
            MCSCognitiveServices mCSPostApi = gameObject.GetComponent<MCSCognitiveServices>();
            StartCoroutine(mCSPostApi.MakeAnalysisRequest(imageBufferList.ToArray()));
            Debug.Log("Ended PostToComputerVision OCR processing coroutine");
        }
        else if (this.gameObject.name == "OCRManager")
        {
            Debug.Log("Started PostToComputerVision OCR processing");
            MCSCognitiveServices mCSPostApi = gameObject.GetComponent<MCSCognitiveServices>();
            StartCoroutine(mCSPostApi.PostToComputerVisionOCR(imageBufferList.ToArray()));
            Debug.Log("Ended PostToComputerVision OCR processing coroutine");
        }
        else
        {
            Debug.Log("Started PostToComputerVision OCR processing");
            MCSCognitiveServices mCSPostApi = gameObject.GetComponent<MCSCognitiveServices>();
            StartCoroutine(mCSPostApi.PersonAnalysisRequest(imageBufferList.ToArray()));
            Debug.Log("Ended PostToComputerVision OCR processing coroutine");
        }
        
    }

}
