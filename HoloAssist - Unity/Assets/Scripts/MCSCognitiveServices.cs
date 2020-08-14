using Microsoft.MixedReality.Toolkit;
using System;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.UI;

public class MCSCognitiveServices: MonoBehaviour {

    //MCS Computer Vision OCR API 
    public IEnumerator<object> PostToComputerVisionOCR(byte[] imageData)
    {
        //Parameters 
/*        string language = "unk";
        bool detectOrientation = true;*/
  

        string requestParameters = "detectOrientation=true&language=en";

        string uri = Constants.MCS_BASEURL + "/vision/v3.0/ocr?" + requestParameters;
        //var url = string.Format("https://holoassistcomputervision.cognitiveservices.azure.com/v1.0/{0}?language={1}&detectOrientation={2}", type, language, detectOrientation);
        var headers = new Dictionary<string, string>() {
            { "Ocp-Apim-Subscription-Key", Constants.MCS_COMPUTERVISIONKEY },
            {"Content-Type", "application/octet-stream" }
        };
        
        WWW www = new WWW(uri, imageData, headers);
        yield return www;

        string responseString = www.text;

        //Json Response
        JSONObject j = new JSONObject(responseString);
        
        if(j != null)
            SaveJsonToTextModel(j);
    }

    public IEnumerator<object> MakeAnalysisRequest(byte[] bytes)
    {
       
        var headers = new Dictionary<string, string>() {
            {"Ocp-Apim-Subscription-Key", Constants.MCS_COMPUTERVISIONKEY },
            {"Content-Type","application/octet-stream"}
        };


        string requestParameters = "?visualFeatures=Description&language=en";
        string uri = Constants.MCS_BASEURL + "/vision/v3.0/analyze" + requestParameters;
        //if ((bytes != null) && (bytes.Length > 0))
        //{
            WWW www = new WWW(uri, bytes, headers);
            yield return www;

            if (www.error != null)
            {
                Debug.Log(www.error);
            }
            else
            {
                ODResults results = JsonUtility.FromJson<ODResults>(www.text);
                PlayVoiceMessage.Instance.PlayTextToSpeechMessage(results);
            }
        //}
    }

    public IEnumerator<object> PersonAnalysisRequest(byte[] bytes)
    {

        var headers = new Dictionary<string, string>() {
            {"Ocp-Apim-Subscription-Key", Constants.MCS_COMPUTERVISIONKEY },
            {"Content-Type","application/octet-stream"}
        };


        string requestParameters = "?visualFeatures=Faces&language=en";
        string uri = Constants.MCS_BASEURL + "/vision/v3.0/analyze" + requestParameters;
        //if ((bytes != null) && (bytes.Length > 0))
        //{
        WWW www = new WWW(uri, bytes, headers);
        yield return www;

        if (www.error != null)
        {
            Debug.Log(www.error);
        }
        else
        {
            PDResults results = JsonUtility.FromJson<PDResults>(www.text);
            PlayVoiceMessage.Instance.PlayTextToSpeechMessage(results);
        }
        //}
    }

    private void SaveJsonToTextModel(JSONObject j)
    {
        List<string> wordList = new List<string>();
        string textAngle = string.Empty;

        try {textAngle = j.GetField("textAngle").ToString();}
        catch { }

        //Add computerVisionOCRDto
        MCSComputerVisionOCRDto computerVisionOCR = new MCSComputerVisionOCRDto()
        {
            language = j.GetField("language").ToString(),
            textAngle = textAngle,
            orientation = j.GetField("orientation").ToString()
        };

        //Add region, words
        var region = j.GetField("regions");
        if (region.list.Count != 0)
        {
            foreach (var regionItem in region.list)
            {
                var lines = regionItem.GetField("lines");
                foreach (var line in lines.list)
                {
                    var words = line.GetField("words");
                    foreach (var word in words.list)
                    {
                        wordList.Add(word.GetField("text").ToString().Replace("\"", ""));
                    }
                }
            }
        }

        computerVisionOCR.text = string.Join(" ", wordList.ToArray());

        PlayVoiceMessage.Instance.PlayTextToSpeechMessage(computerVisionOCR);
    }

    private void SaveJsonToObjectModel(JSONObject j)
    {
        List<string> wordList = new List<string>();
        String objects = String.Empty;

        try { objects = j.GetField("objects").ToString(); }
        catch { }

        //Add computerVisionOCRDto
        MCSComputerVisionOCRDto computerVisionOCR = new MCSComputerVisionOCRDto()
        {
            language = j.GetField("language").ToString(),
            orientation = j.GetField("orientation").ToString()
        };

        //Add region, words
        var region = j.GetField("regions");
        if (region.list.Count != 0)
        {
            foreach (var regionItem in region.list)
            {
                var lines = regionItem.GetField("lines");
                foreach (var line in lines.list)
                {
                    var words = line.GetField("words");
                    foreach (var word in words.list)
                    {
                        wordList.Add(word.GetField("text").ToString().Replace("\"", ""));
                    }
                }
            }
        }

        computerVisionOCR.text = string.Join(" ", wordList.ToArray());

        PlayVoiceMessage.Instance.PlayTextToSpeechMessage(computerVisionOCR);
    }
}
