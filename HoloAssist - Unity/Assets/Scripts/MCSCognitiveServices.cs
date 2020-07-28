using System.Collections.Generic;
using UnityEngine;

public class MCSCognitiveServices: MonoBehaviour {
    
    //MCS Computer Vision OCR API 
    public IEnumerator<object> PostToComputerVisionOCR(byte[] imageData, string type)
    {
        //Parameters 
/*        string language = "unk";
        bool detectOrientation = true;*/
  

        string requestParameters = "visualFeatures=Description&language=en";
        string uri = Constants.MCS_BASEURL + "/vision/v1.0/ocr?" + requestParameters;
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
            SaveJsonToModel(j);
    }

    private void SaveJsonToModel(JSONObject j)
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
}
