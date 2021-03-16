[toc]

# Unity程序基础框架说明

- 所有代码文件存放于`Assets/Scripts/ProjectBase/`下对应文件夹中
- 所有框架目前都只实现了基本功能，后续有待改进
- 编写新功能请注意尽量降低程序耦合性，减少重复代码

## 请注意一些行为规范！

- **Unity**通过代码加载资源其默认根目录是`Resources`，请务必设置此Resources文件夹，并将游戏所需要的各种资源**分类分层**置于其中
- 

## 单例模式基类 BaseManager.cs

**代码文件路径**：`Assets/Scripts/ProjectBase/Base`

**作用**：减少单例模式重复代码的书写

**使用方式**：

在设计你的单例类（通常是各种管理类）时使用泛型继承`BaseManager.cs`，例如当你设计一个**MyManger**管理类，并想把它做成单例：

```C#
public MyManager : BaseManager<MyManager>
{
    ...
    public void DoSomeThing() {...};
    ...
}

//在外部调用DoSomeThing()的方法：
MyManager.GetInstance().DoSomeThing();
```

**注意事项**：

- **BaseManager**没有继承**MonoBehaviour**，如果想用Unity的Update模块。参考[公共Mono模块](# 公共Mono模块 MonoManager.cs)
- 继承了**BaseManager**后，会保证你的程序在任意位置**第一次**调用**GetInstance()**时被实例化，且不会实例多次，无需再做其他操作
- 所有继承了**BaseManager**的实例调用方法都是`自定义类名.GetInstance().你要调用的函数或变量`，这非常重要，后续模块将会大量用到此方法



## 缓存池模块 PoolManager.cs

**文件路径**：`Assets/Scripts/ProjectBase/Pool/`

**作用**：节约性能，减少GC次数，降低cpu内存消耗

**使用方式**：

①添加对象：`void GetObj(string objNmae, UnityAction<GameObject> callBack)`

通过Resources下的预制件路径(**objNmae**)加载游戏对象，优先从池子中获取，没有则会通过路径异步加载，**callback**回调函数（即如果你希望模型加载完后做什么事，不需要可置空）

②回收对象：`void PushObj(string name, GameObject obj)`

将**obj**物体放回缓存池，如果是第一个被放入池子的某类别物体，缓存池会自动为该类别（**name**）创建子池；

③清空缓存池：`void Clear()`

切换场景时可使用，清空缓存池

参考代码文件：

```c#
/*
示例代码位于Assets/Scripts/Test/TestPool，
请将路径下的TestPool预制件拖到空场景，运行游戏
点击左键创建一个Cube，点击右键创建一个sphere（延迟1s自动放回池子）
*/

//利用缓存池创建Cube的方法
PoolManager.GetInstance().GetObj("Test/Cube", (o)=>
{
    // (paras) => { ... }即为lambda表达式
    //这里利用lambda表达式把创建的物体体积放大
    o.transform.localScale = Vector3.one * 2;
});

//利用缓存池回收Cube的方法
GameObject obj;
...
PoolManager.GetInstance().PushObj(obj.name, obj);

```

注意事项：

- 该类继承了**BaseManger**
- 用`GetObj()`创建物体传入的name参数、物体在Resources文件夹下的路径、`PushObj()`传入的name参数，**三者要保持一致**
- `GetObj()`所需的回调函数建议写成**lambda表达式**
- `GetObj()`可以获取缓存池没有的物体，缓存池会以**异步**形式加载预制件



## 事件中心模块 EventManager.cs

**文件路径**：`Assets/Scripts/ProjectBase/Event/`

**作用**：降低不同游戏模块之间的耦合性，减小程序复杂度

**使用方式**：

①添加监听事件：`void AddEventListener<T>(string name, UnityAction<T> action)`

``name`为事件的名字，`action`为该event被触发后执行的回调函数，泛型参数`<T>`为回调函数`action`需要的参数（如果需要的话）如果你的回调函数action不需要参数，脚本中重载了不带泛型参数的版本，即：（**后面介绍的函数同理**）
`void AddEventListener(string name, UnityAction action)`

②移除监听事件：`void RemoveEventListener<T>(string name, UnityAction<T> action)`

注意只移除**name**下指定的那一个**action**

③触发监听事件：` void EventTrigger<T>(string name, T info)`

参考代码文件：

```c#
/*
示例代码位于Assets/Scripts/Test/TestEvent，
请将路径下的TestEvent预制件拖到空场景，运行游戏
*/

//Player.cs脚本中添加函数监听的方法
void Start()
{
    //带参数的消息监听
    EventManager.GetInstance().AddEventListener<Monster>("MonsterDead", MonsterDeadDo); 
    //不带参数的消息监听
    EventManager.GetInstance().AddEventListener("Win", Win); 
}

//带参数的消息监听回调函数
public void MonsterDeadDo(Monster info)
{
    Debug.Log("玩家得到奖励" + info.name);
}

//Monster.cs脚本中触发事件的方法
 void Start()
{
	Dead();
}

void Dead()
{
    Debug.Log("怪物死亡");
    //触发事件
    EventManager.GetInstance().EventTrigger("MonsterDead", this); 
    //注意实际程序中可以简化掉EventTrigger后的<Monster>
}

```



**注意事项**：

- 该类继承了**BaseManger**
- 用于区分不同事件的**唯一标识符**是添加监听事件时的参数**string name**
- 如果使用AddEventListener**\<T\>**版本监听，移除对应事件也务必使用RemoveEventListener**\<T\>**，但注意实际程序中**触发事件可以简化**掉EventTrigger后的**\<T\>**
- 可以尝试去触发没有被监听的事件



## 公共Mono模块 MonoManager.cs

**文件路径**：`Assets/Scripts/ProjectBase/Mono/`

**作用**：让没有继承Mono的类可以开启协程，可以Update更新，并统一管理Update

**使用方式**：

①添加帧更新事件：` void AddUpdateListener(UnityAction func)`

`func`为你希望帧更新的函数

②移除帧更新事件：` void RemoveUpdateListener(UnityAction func)`

`func`为你希望移除帧更新的函数

参考代码文件：

```C#
/*
示例代码位于Assets/Scripts/Test/TestMono，
请将路径下的TestMono预制件拖到空场景，运行游戏
*/

//TestTest类没有继承MonoBehaviour
//继承了MonoBehaviour的Test类通过AddUpdateListener(t.MyUpdate)实现其帧更新
public class TestTest
{
    ...
    public void MyUpdate()
    {
        Debug.Log("TestTest");
    }
}

public class Test : MonoBehaviour
{
    void Start()
    {
        TestTest t = new TestTest();
        MonoManager.GetInstance().AddUpdateListener(t.Update);
    }
}
```

**注意事项**：

- 该类继承了**BaseManger**
- 开启/关闭协程功能还有待完善，如果项目需要可再做拓展...



## 场景切换模块 ScenesManager.cs

**文件路径**：`Assets/Scripts/ProjectBase/Scene/`

**作用**：提供场景切换的公共接口

**使用方式**：

①同步加载场景：`void LoadScene(string name, UnityAction func)`

同步加载名字为`name`的场景，加载完成后执行回调函数`func`

②异步加载场景：`void LoadSceneAsync(string name, UnityAction func)`

通过开启协程异步加载名字为`name`的场景，会不断触发一个名为`进度条更新`的事件，并传入加载进度（AsyncOperation ao; ao.progress）。如果需要获取场景加载进度请向[事件中心模块](# 事件中心模块 EventManager.cs)添加**对应名字**事件监听

**注意事项**：

- 该类继承了**BaseManger**
- 特意给Scene多加了一个s，以和**Unity**自带的**SceneManager**区分
- 只有在Unity菜单`File->Build Settings`中添加进`Scenes in Build`的场景才会被**Unity**识别，也才能使用上述两个函数通过识别名字的方式加载
- 如果你想在异步加载监听事件时传入更多数据或增加功能，请在`ScenesManager.cs`的`private IEnumerator ReallyLoadSceneAsync()`修改



## 输入控制模块 InputManager.cs

**文件路径**：`Assets/Scripts/ProjectBase/Input/`

**作用**：管理Input输入相关逻辑

**使用方式**：

①开启/关闭输入检测：`void StartOrEndCheck(bool isOpen)`

通过传入**isOpen**布尔值控制是否启用输入检测

②该模块通过`MonoManger`添加检测按键帧更新，再通过`EventManager`触发对应事件实现功能。具体游戏开发时只要玩家控制的对象对监听对应事件即可。

参考代码：

```c#
/*
示例代码位于Assets/Scripts/Test/TestInput，
请将路径下的TestInput预制件拖到空场景，运行游戏
*/

//TestTest类没有继承MonoBehaviour
//继承了MonoBehaviour的Test类通过AddUpdateListener(t.MyUpdate)实现其帧更新
public class TestInput : MonoBehaviour
{
    
    void OnEnable()
    {
        //开启输入检测
        InputManager.GetInstance().StartOrEndCheck(true);
        //添加监听事件
        EventManager.GetInstance().AddEventListener<KeyCode>("某键按下", CheckInputDown);
        EventManager.GetInstance().AddEventListener<KeyCode>("某键抬起", CheckInputUp);
    }

    //一定要记得在销毁对象时撤销监听
    void OnDisable()
    {
        //关闭输入检测
        InputManager.GetInstance().StartOrEndCheck(false);
        EventManager.GetInstance().RemoveEventListener<KeyCode>("某键按下", CheckInputDown);
        EventManager.GetInstance().RemoveEventListener<KeyCode>("某键抬起", CheckInputUp);
    }


    private void CheckInputDown(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                /*你要做的事*/
                break;
            default:
                /*...*/
                break;
        }
        Debug.Log("Input key: "+ key.ToString() + " down.");
    }

    private void CheckInputUp(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.W:
                /*你要做的事*/
                break;
            default:
                /*...*/
                break;
        }
        Debug.Log("Input key: " + key.ToString() + " up.");
    }

}
```

**注意事项**：

- 该类继承了**BaseManger**
- 该脚本目前只写了检测WASD键，其需要视情况往里面再加入大量功能，可在MyUpdate()函数中写入你想去监听的键。这个类可以在此基础上再封装拓展做出游戏里的改键功能



## 音效管理模块 AudioManager

**文件路径**：`Assets/Scripts/ProjectBase/Audio/`

**作用**：统一管理音乐音效

**使用方式**：

注意`name`需是**Audio Clip**在Resources文件夹下**包括名字在内的完整路径**

①背景音乐相关：

- 播放背景音乐：`void PlayBKMusic(string name)`
- 暂停背景音乐：`void PauseBKMusic()`
- 停止背景音乐：`void StopBKMusic()`
- 改变背景音乐音量：`void ChangeBKValue(float v)`

②一般音效相关

- 播放音效：` void PlayAudio(string name, bool isLoop, UnityAction<AudioSource> callBack = null)`

`isLoop`标识音效是否循环播放，`callBack `为可选的回调函数，默认为null

- 改变音量大小：`void ChangeSoundValue(float value)`

注意会改变所有音效音量大小

- 停止音效：`void StopAudio(AudioSource source)`

参考代码如下：

```c#
/*
示例代码位于Assets/Scripts/Test/TestAudio，
请将路径下的TestAudio预制件拖到空场景，运行游戏
*/

public class TestAudio : MonoBehaviour
{
	...
    //使用OnGUI简单实现5个功能演示
    void OnGUI()
    {
        ...
        //播放音乐
        AudioManager.GetInstance().PlayBKMusic("Audio/BGM/Main Theme");
        ...
        
        //暂停音乐
        AudioManager.GetInstance().PauseBKMusic();
		...
            
        //停止音乐
        AudioManager.GetInstance().StopBKMusic();
		...
        
        //播放音效
        AudioSource source;
        GUIStyle s;
        AudioManager.GetInstance().PlayAudio("Audio/Sound/egg", false, (s)=> 
        { source = s;} );

        //停止音效
        AudioManager.GetInstance().StopAudio(source);
        source = null;
    }
}

```

**注意事项**：

- 该类继承了**BaseManger**
- 本模块暂未考虑3D音效
- 请不要把AudioSource绑定给游戏对象，并在其脚本中控制音效播放，请将所有音效相关统一交给**AudioManager**处理
- **AudioManager**会创建一个名为**AudioObj**的`GameObject`，所有音效都会放在其下面。**AudioManager**有帧监听函数，其会自动销毁已经播放完的`AudioSource`



## UI管理模块 UIManager.cs & BasePanel.cs

**文件路径**：`Assets/Scripts/ProjectBase/UI/`

**作用**：统一管理音乐音效

**使用方式**：

注意`name`需是**Audio Clip**在Resources文件夹下**包括名字在内的完整路径**

**①UIManager**：

- 所有UI组件的父对象：`RectTransform canvas`

`canvas`下拥有`bot`、`mid`、`top`、`system`四个层级，你可以将你的UIPanel置于某个层级下，并用代码控制几个不同层级的显示优先级和遮挡关系。

通过函数`Transform GetLayerFather(E_UI_Layer layer)`可以直接**显式**获取各个层级的`Transform`，`E_UI_Layer `为自定义的**枚举类型**，`E_UI_Layer.Bot`、`E_UI_Layer.Mid`、`E_UI_Layer.Top`、`E_UI_Layer.System`与四个同名层级一一对应。

- 控制UI面板：

1. `void ShowPanel<T>(string panelName, E_UI_Layer layer = E_UI_Layer.Mid, UnityAction<T> callBack = null) where T: BasePanel`

该函数用于显示路径为`"UI/"+panelName`路径下的面板，`layer`为面板显示后的位于`canvas`下的层级（默认为mid，`callBack`为面板**异步**加载完成后执行的回调函数（默认为null）

`where T: BasePanel`表示`callback`参数T的泛型约束，**BasePanel**为自定义类

2. `static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> action)`

该函数用于添加自定义的事件监听

**②BasePanel**：

- `protected virtual void Awake()`

该函数会自动寻找面板下所有**UIBehaviour**组件并保存它们的引用，继承后记得在子类`Awake()`函数开头加上`base.Awake()`

该函数还会自动识别：

1. 如果子控件是**button**，自动绑定`OnClick()`函数，详细介绍见后文
2. 如果子控件是**Toggle**，自动绑定`OnValueChanged()`函数，详细介绍见后文

- `protected T GetControl<T>(string controlName) where T: UIBehaviour`

根据控件名称`controlName`返回对应`UIBehaviour`引用，并会自动转换成对应`T`类型

- `protected virtual void OnClick(string btnName)`

提供可重载的按钮响应函数，函数体为空。接受一个**string**参数，它会在绑定时被自动传入**Button**控件的`GameObject.name`

- `protected virtual void OnValueChanged(string toggleName, bool value)`

提供可重载的按钮响应函数，函数体为空。接受一个**string**和一个**bool**参数，**string**会在绑定时被自动传入**Toggle**控件的`GameObject.name`，`value`则是**toggle**的当前状态

参考代码如下：

```c#
/*
示例代码位于Assets/Scripts/Test/TestUI，
请将路径下的TestUI预制件拖到空场景，运行游戏
*/

public class TestUI : MonoBehaviour
{
    void Start()
    {
        UIManager.GetInstance().ShowPanel<LoginPanel>("LoginPanel", E_UI_Layer.Mid, ShowPanelOver); //加载LoginPanel组件，加完完后调用ShowPanelOver回调函数
    }
	
    //回调函数
    private void ShowPanelOver(LoginPanel panel)
    {
        panel.InitInfo();
    }
}

public class LoginPanel : BasePanel
{
    protected override void Awake()
    {
        //一定不能少，因为需要执行父类的awake来初始化一些信息，比如找控件，加入事件监听
        base.Awake();
        ... //在这里处理自己的一些初始化逻辑
    }
    
    void Start()
    {	//绑定自定义事件监听
        UIManager.AddCustomEventListener(GetControl<Button>("btnStart"), EventTriggerType.PointerEnter, (data) =>
        {
            Debug.Log("进入");
        });
        ...
    }
    
    protected override void OnClick(string btnName) { ... } //重载基类的按钮点击响应函数
    ...
}
```

**注意事项**：

- **UIManager**类继承了**BaseManger**，**BasePanel**类继承了**MonoBehaviour**
- UI模块的基本实现依赖`Assets/Resources/UI/UIBase/`路径下的`Canvas`、`EventSystem`两个预制件
- 请让你设计的所有UIPanel继承**BasePanel**
- UI模块较复杂，建议先跑跑示例代码
- 后续UI开发请尽量依赖用初始化过程中的代码去绑定需要的按钮、响应函数等组件，不要依赖把预制件拖入**Inspector**对应选框的形式
- **BasePanel**中还可以去实现很多面板功能，并设计成*virtual*以便重载