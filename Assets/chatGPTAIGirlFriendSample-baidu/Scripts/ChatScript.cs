using System;
using System.Collections;
using System.Collections.Generic;
//using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Text;
//请你扮演一位名叫玲玲的高中生与我对话，你的性格温柔，文静，热爱文学，喜欢韩寒的书，有点害羞，回答不要超过50字
//你将扮演初音未来，角色设置年龄为16岁、生日8月31日，身高与体重则分别是158cm与42kg，有着苍绿色双马尾，脚穿黑色过膝靴，擅长流行歌曲，摇滚乐和舞蹈。其代表色为苍绿色。擅长由1980年代至最新的流行歌曲。擅长的节奏大约在70～245BPM之间，擅长的音域则在A3～E5之间。
public class ChatScript : MonoBehaviour
{
    public static ChatScript Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private string firstPromt = "请你扮演一位名叫玲玲的高中生与我对话，你的性格温柔，文静，热爱文学，喜欢韩寒的书，有点害羞，回答不要超过50字";
    //API key
    [SerializeField] private string m_OpenAI_Key = "填写你的Key";

    // 定义Chat API的URL
    private string m_ApiUrl = "https://api.deepseek.com/chat/completions";

    List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();

    //配置参数
    [SerializeField] private GetOpenAI.PostData m_PostDataSetting;

    //聊天UI层
    [SerializeField] private GameObject m_ChatPanel;

    //输入的信息
    [SerializeField] private InputField m_InputWord;

    //返回的信息
    [SerializeField] private Text m_TextBack;

    //播放设置
    [SerializeField] private Toggle m_PlayToggle;

    /// <summary>
    /// 百度语音识别
    /// </summary>
    [SerializeField] private BaiduSpeechSample m_SpeechSample;

    //gpt-3.5-turbo
    //[SerializeField] public GptTurboScript m_GptTurboScript;


    private void Start()
    {
        messages.Add(new Dictionary<string, string> { { "role", "system" }, { "content", firstPromt } });
    }

    //发送信息
    public void SendData()
    {
        if (m_InputWord.text.Equals(""))
            return;

        //记录聊天
        m_ChatHistory.Add(m_InputWord.text);

        string _msg = m_lan + " " + m_InputWord.text;
        //string _msg =m_lan + " " + m_InputWord.text;
        //发送数据
        //StartCoroutine (GetPostData (_msg,CallBack));
        messages.Add(new Dictionary<string, string> { { "role", "user" }, { "content", _msg } });
        //StartCoroutine(m_GptTurboScript.GetPostData(_msg, m_OpenAI_Key, CallBack));
        StartCoroutine (GetPostData (_msg,CallBack));
        m_InputWord.text = "";
        m_TextBack.text = "...";


    }

    //发送信息
    public void SendData(string _data)
    {
        //记录聊天
        m_ChatHistory.Add(_data);

        string _msg = m_lan + " " + _data;
        //string _msg =m_lan + " " + m_InputWord.text;
        //发送数据
        //StartCoroutine (GetPostData (_msg,CallBack));
        messages.Add(new Dictionary<string, string> { { "role", "user" }, { "content", _msg } });
        //StartCoroutine(m_GptTurboScript.GetPostData(_msg, m_OpenAI_Key, CallBack));
        StartCoroutine (GetPostData (_msg,CallBack));
        //m_InputWord.text = "";
        m_TextBack.text = "...";


    }


    //AI回复的信息
    private void CallBack(string _callback)
    {
        _callback = _callback.Trim();
        m_TextBack.text = "";
        //开始逐个显示返回的文本
        m_WriteState = true;
        StartCoroutine(SetTextPerWord(_callback));

        //记录聊天
        m_ChatHistory.Add(_callback);

        if (m_PlayToggle.isOn)
        {
            StartCoroutine(Speek(_callback));
        }


    }


    private IEnumerator Speek(string _msg)
    {
        yield return new WaitForEndOfFrame();
        //播放合成并播放音频
        m_SpeechSample.Speek(_msg);
    }

    private IEnumerator GetPostData(string _postWord, System.Action<string> _callback)
    {
        var requestData = new
        {
            model = "deepseek-chat",
            messages = messages,
            stream = false
        };
        UnityWebRequest request = new UnityWebRequest(m_ApiUrl, "POST");
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
        if (request.result == UnityWebRequest.Result.Success)
        {
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
    


    #region 文字逐个显示
    //逐字显示的时间间隔
    [SerializeField]private float m_WordWaitTime=0.2f;
    //是否显示完成
    [SerializeField]private bool m_WriteState=false;
    private IEnumerator SetTextPerWord(string _msg){
        int currentPos=0;
        while(m_WriteState){
            yield return new WaitForSeconds(m_WordWaitTime);
            currentPos++;
            //更新显示的内容
            m_TextBack.text=_msg.Substring(0,currentPos);

            m_WriteState=currentPos<_msg.Length;

        }
    }

    #endregion


    #region 聊天记录
    //保存聊天记录
    [SerializeField]private List<string> m_ChatHistory;
    //缓存已创建的聊天气泡
    [SerializeField]private List<GameObject> m_TempChatBox;
    //聊天记录显示层
    [SerializeField]private GameObject m_HistoryPanel;
    //聊天文本放置的层
    [SerializeField]private RectTransform m_rootTrans;
    //发送聊天气泡
    [SerializeField]private ChatPrefab m_PostChatPrefab;
    //回复的聊天气泡
    [SerializeField]private ChatPrefab m_RobotChatPrefab;
    //滚动条
    [SerializeField]private ScrollRect m_ScroTectObject;
    //获取聊天记录
    public void OpenAndGetHistory(){
        m_ChatPanel.SetActive(false);
        m_HistoryPanel.SetActive(true);

        ClearChatBox();
        StartCoroutine(GetHistoryChatInfo());
    }
    //返回
    public void BackChatMode(){
        m_ChatPanel.SetActive(true);
        m_HistoryPanel.SetActive(false);
    }

    //清空已创建的对话框
    private void ClearChatBox(){
        while(m_TempChatBox.Count!=0){
            if(m_TempChatBox[0]){
                Destroy(m_TempChatBox[0].gameObject);
                m_TempChatBox.RemoveAt(0);
            }
        }
        m_TempChatBox.Clear();
    }

    //获取聊天记录列表
    private IEnumerator GetHistoryChatInfo()
    {

        yield return new WaitForEndOfFrame();

       for(int i=0;i<m_ChatHistory.Count;i++){
        if(i%2==0){
            ChatPrefab _sendChat=Instantiate(m_PostChatPrefab,m_rootTrans.transform);
            _sendChat.SetText(m_ChatHistory[i]);
            m_TempChatBox.Add(_sendChat.gameObject);
            continue;
        }

         ChatPrefab _reChat=Instantiate(m_RobotChatPrefab,m_rootTrans.transform);
        _reChat.SetText(m_ChatHistory[i]);
        m_TempChatBox.Add(_reChat.gameObject);
       }

        //重新计算容器尺寸
        LayoutRebuilder.ForceRebuildLayoutImmediate(m_rootTrans);
        StartCoroutine(TurnToLastLine());
    }

    private IEnumerator TurnToLastLine(){
        yield return new WaitForEndOfFrame();
         //滚动到最近的消息
        m_ScroTectObject.verticalNormalizedPosition=0;
    }


    #endregion


    #region 切换妹子
    //Lo娘
    [SerializeField]private GameObject m_LoGirl;
    [SerializeField]private GameObject m_Girl;
    [SerializeField]private string m_lan="使用中文回答";
    //
    public void SetLoGirlShowed(GameObject _settingPanel){
        if(!m_LoGirl.activeSelf)
        {
            m_LoGirl.SetActive(true);
            m_Girl.SetActive(false);
        }
        //m_AzurePlayer.SetSound("zh-CN-XiaoyiNeural");

        _settingPanel.SetActive(false);
    }
    //zh-CN-XiaoxiaoNeural
    public void SetGirlShowed(GameObject _settingPanel){
        if(!m_Girl.activeSelf)
        {
            m_LoGirl.SetActive(false);
            m_Girl.SetActive(true);
        }
         //m_AzurePlayer.SetSound("zh-CN-liaoning-XiaobeiNeural");

        _settingPanel.SetActive(false);
    }

    #endregion

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
