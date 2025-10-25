using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GachaSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private string _apiKey = "http://localhost:8080/health";

    private void Start()
    {
        StartCoroutine(Get());
    }

    private IEnumerator Get()
    {
        UnityWebRequest www = UnityWebRequest.Get(_apiKey);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Advice API error: " + www.error);
            yield break;
        }

        string json = www.downloadHandler.text;
        Debug.Log(json);
        try
        {
            _text.text = json;
        }
        catch { _text.text = "おなかすいたかもね"; }
    }
}
