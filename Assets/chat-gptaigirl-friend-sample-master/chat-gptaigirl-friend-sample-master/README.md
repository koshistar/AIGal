# chatGPTAIGirlFriendSample

#### 介绍
使用chatGPT+unity制作的简单的AI女友(老婆)对话机器人的demo。

----------------------------------------------------------

这个仓库示例包含的Azure语音服务、百度语音服务以及对接大语言模型api的调用功能，我重构了代码，并统一整合到新的项目里，可以更方便的集成应用。
另外把所有涉及语音服务都修改为web api的方式，增强了平台适用性，以下是新的源码仓库，这个仓库后续就不再继续更新了。

Github地址：https://github.com/zhangliwei7758/unity-AI-Chat-Toolkit

Gitee地址：https://gitee.com/DammonSpace/unity-ai-chat-toolkit

----------------------------------------------------------
2023.5.13 更新

1、本次更新，新增[chatGLM]2023.5.13chatgpt-ai-girl-friend-sample.unitypackage工程包，提供了对chatGLM官方项目的api对接示例，可以在示例场景中，调用本地部署好的chatglm api，实现AI对话；

2、chatGLM工程包更新，依然需要依赖VRoid以及Azure的SDK包，如果使用这个示例工程，请按照上述流程，安装指定的插件包，才能正常使用

-------------------------------------------------------------
2023.4.15 更新

1、更新了[gpt-3.5-Turbo]chatgpt-ai-girl-friend-sample.unitypackage，示例代码里增加了azure语音识别的功能。
在示例场景里，运行时，按住空格键启动麦克风输入，松开空格键完成录制并进行语音识别，识别成功不需要再点击发送按钮。

2、新增了[4.15]AzureRecognize.unitypackage包，只包含azure语音识别的代码示例，可以参考学习


----------------------------------------------------------

原始内容

使用前须知：请从仓库中选择需要的工程包下载使用，如果选用的是微软Azure语音合成的工程包，需要下载微软语音合成的SDK，导入工程
工程包含：

1、chatgpt-ai-girl-friend-sample.unitypackage：此包使用的是gpt3的模型，以及微软语音合成

2、[gpt-3.5-Turbo]chatgpt-ai-girl-friend-sample.unitypackage：此包使用的是gpt-3.5-turbo的模型，以及微软语音合成

3、[百度AI开放平台]chatgpt-ai-girl-friend-sample.unitypackage：此包使用的是gpt-3.5-turbo的模型，以及百度语音合成api


#### 使用说明

 **如果使用了上述1、2两个包，需依赖以下两个插件，请下载并导入以下插件后，再导入工程包，不然会有报错的。
** 
1.  下载VRoid模型导入unity的插件包，并导入到你的工程文件里
插件包地址：https://github.com/vrm-c/UniVRM
下载最新的release版本的插件，导入到你的工程文件里
![输入图片说明](Vrm%20Plugin.png)

2.  下载微软Azure语音合成，unity插件包
打开微软Azure语音合成，关于SDK安装的说明：https://learn.microsoft.com/zh-cn/azure/cognitive-services/speech-service/quickstarts/setup-platform?pivots=programming-language-csharp&tabs=windows%2Cubuntu%2Cunity%2Cjre%2Cmaven%2Cnodejs%2Cmac%2Cpypi
从中找到unity的SDK插件包，下载并导入到你的工程文件里
![输入图片说明](image.png)

3.  下载本仓库提供的*.unitypackage文件，并导入到你的工程文件里

