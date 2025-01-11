using System.Collections;
using System.Collections.Generic;
//using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Text;

public class GetOpenAI : MonoBehaviour
{
    //API key
	[SerializeField]private string m_OpenAI_Key="sk-b11f5b8f95b34c488108b000d9f63257";
	// 定义Chat API的URL
	private string m_ApiUrl = "https://api.deepseek.com/chat/completions";
	List<Dictionary<string,string>> messages = new List<Dictionary<string,string>>();
    //配置参数
    [SerializeField]private PostData m_PostDataSetting;

    //输入的信息
    [SerializeField]private InputField m_InputWord;
    //聊天文本放置的层
    [SerializeField]private RectTransform m_rootTrans;
    //发送聊天气泡
    [SerializeField]private ChatPrefab m_PostChatPrefab;
    //回复的聊天气泡
    [SerializeField]private ChatPrefab m_RobotChatPrefab;
    //滚动条
    [SerializeField]private ScrollRect m_ScroTectObject;

    void Start()
    {
	    messages.Add(new Dictionary<string, string> { { "role", "system" }, { "content", "You are a helpful assistant." } });
    }
    //发送信息
    public void SendData()
    {
        if(m_InputWord.text.Equals(""))
            return;

        string _msg=m_InputWord.text;
        ChatPrefab _chat=Instantiate(m_PostChatPrefab,m_rootTrans.transform);
        _chat.SetText(_msg);
        //重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
        Debug.Log("end TuenToLastLine "+_msg);
        messages.Add(new Dictionary<string, string> { { "role", "user" }, { "content", _msg } });
        StartCoroutine (GetPostData (_msg,CallBack));
        Debug.Log("end CallBack");
        m_InputWord.text="";
    }

    //AI回复的信息
    private void CallBack(string _callback){
	    Debug.Log(_callback);
        _callback=_callback.Trim();
        Debug.Log("Instantiate");
        ChatPrefab _chat=Instantiate(m_RobotChatPrefab,m_rootTrans.transform);
        _chat.SetText(_callback);
        //重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
       
       StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine(){
        yield return new WaitForEndOfFrame();
         //滚动到最近的消息
        m_ScroTectObject.verticalNormalizedPosition=0;
    }

    //设置AI模型
    public void SetAIModel(Toggle _modelType){
        if(_modelType.isOn){
            m_PostDataSetting.model=_modelType.name;
        }
    }


	[System.Serializable]public class PostData{
		public string model;
		public List<Dictionary<string, string>> prompt;

		public bool stream;
		// public int max_tokens; 
		//       public float temperature;
		//       public int top_p;
		//       public float frequency_penalty;
		//       public float presence_penalty;
		//       public string stop;
	}

	private IEnumerator GetPostData(string _postWord,System.Action<string> _callback)
	{
		Debug.Log("GetPostData "+_postWord);
		// using(UnityWebRequest request = new UnityWebRequest (m_ApiUrl, "POST")){   
		// 	GetOpenAI.PostData _postData = new GetOpenAI.PostData
		// 	{
		// 		model = m_PostDataSetting.model,
		// 		prompt = _postWord,
		// 		stream = false
		// 		// max_tokens = m_PostDataSetting.max_tokens,
		// 		// temperature=m_PostDataSetting.temperature,
		// 		// top_p=m_PostDataSetting.top_p,
		// 		// frequency_penalty=m_PostDataSetting.frequency_penalty,
		// 		// presence_penalty=m_PostDataSetting.presence_penalty,
		// 		// stop=m_PostDataSetting.stop
		// 	};
		var requestData = new
		{
			model = "deepseek-chat",
			messages = messages,
			stream = false
		};
		UnityWebRequest request=new UnityWebRequest(m_ApiUrl,"POST");
			// string _jsonText = JsonUtility.ToJson (_postData);
			string _jsonText = JsonConvert.SerializeObject(requestData);
			byte[] bodyRaw = Encoding.UTF8.GetBytes(_jsonText);
			request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = new DownloadHandlerBuffer();
			request.SetRequestHeader("Content-Type", "application/json");
			request.SetRequestHeader("Authorization", "Bearer " + m_OpenAI_Key);
			Debug.Log("SendData " + requestData);
			yield return request.SendWebRequest();
//request.responseCode == 200
			Debug.Log(request.responseCode);
			Debug.Log(request.result);
			if (request.result==UnityWebRequest.Result.Success) {
				// Debug.Log("Get request");
				// string _msg = request.downloadHandler.text;
				// Debug.Log(_msg);
				// GetOpenAI.TextCallback _textback = JsonUtility.FromJson<GetOpenAI.TextCallback> (_msg);
				// Debug.Log(_textback);
				// if (_textback!=null && _textback.choices.Count > 0) {
    //                 
				// 	string _backMsg=Regex.Replace(_textback.choices[0].text, @"[\r\n]", "").Replace("？","");
				// 	_callback(_backMsg);
				// }
				var response = JsonConvert.DeserializeObject<DeepSeekResponse>(request.downloadHandler.text);
				string botMessage = response.choices[0].message.content;
				Debug.Log(botMessage);
				_callback(botMessage);
			}
        }
	

    public void Quit(){
        Application.Quit();
    }

    void Update(){

        if(Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }
    }

	/// <summary>
	/// 返回的信息
	/// </summary>
	[System.Serializable]public class TextCallback{
		public string id;
		public string created;
		public string model;
		public List<TextSample> choices;

		[System.Serializable]public class TextSample{
			public string text;
			public string index;
			public string finish_reason;
		}

	}
	[System.Serializable]
	public class DeepSeekResponse
	{
		public Choice[] choices;
	}

	[System.Serializable]
	public class Choice
	{
		public Message message;
	}

	[System.Serializable]
	public class Message
	{
		public string content;
	}

}
