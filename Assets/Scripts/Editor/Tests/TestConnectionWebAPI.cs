using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;

public class TestConnectionWebAPI
{
    private static readonly string LocalHostApiRoot = "http://localhost:8080/";

    [Test]
    public async Task API接続テスト()
    {
        //http://localhost:8080/health
        var healthUrl = Path.Combine(LocalHostApiRoot, "health");
        Debug.Log($"request url : {healthUrl}");

        using var webRequest = new UnityWebRequest(healthUrl);
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        await webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log("成功した");
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("コンテナが立ち上がっているか確認してください。");
                return;
            case UnityWebRequest.Result.InProgress:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError($"予期しないエラーが発生した。{webRequest.result}");
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var response = webRequest.downloadHandler;
        if (response == null)
        {
            Debug.LogError("レスポンスの取得に失敗した");
            return;
        }

        Debug.Log(response.text);
    }
    [Test] //[test]属性をつける
    public void ほげTest() // メソッド名に日本語ok. 何の単体テストか分かりやすくすると良い
    {
        Debug.Log("hoge");
    }

    [Serializable]
    internal class SampleRequestData
    {
        //NOTE:Unity標準のJsonUtility.ToJson()メソッドだと、「Property」はSerializeされないことに注意.
        //「Property」もSerializeさせたい場合は、MessagePack等のライブラリを別途使用する必要がある
        public string Name { get; set; } = string.Empty;
        public string name;
    }

    [Test]
    public async Task ユーザー作成()
    {
        //http://localhost:8080/create/user/
        var createUserUrl = LocalHostApiRoot + "create/user";
        Debug.Log($"request url : {createUserUrl}");

        using var webRequest = new UnityWebRequest(createUserUrl, UnityWebRequest.kHttpVerbPOST);
        webRequest.SetRequestHeader("Accept", "application/json");
        webRequest.SetRequestHeader("Content-Type", "application/json");
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        //Body作成
        string json = JsonUtility.ToJson(new SampleRequestData
        {
            Name = "test_Name", //こっちはSerializeされない
            name = "test_name"
        });

        //リクエストで送るjsonデータを確認する
        Debug.Log(json);

        // json -> byteにしてデータをサーバーへ送る設定をする
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        webRequest.uploadHandler.contentType = "application/json";

        await webRequest.SendWebRequest();

        switch (webRequest.result)
        {
            case UnityWebRequest.Result.Success:
                Debug.Log("成功した");
                break;
            case UnityWebRequest.Result.ConnectionError:
            case UnityWebRequest.Result.ProtocolError:
                Debug.LogError("コンテナが立ち上がっているか確認してください。");
                return;
            case UnityWebRequest.Result.InProgress:
            case UnityWebRequest.Result.DataProcessingError:
                Debug.LogError($"予期しないエラーが発生した。{webRequest.result}");
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var response = webRequest.downloadHandler;
        if (response == null)
        {
            Debug.LogError("レスポンスの取得に失敗した");
            return;
        }

        Debug.Log(response.text);

    }
}
