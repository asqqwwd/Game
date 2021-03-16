//单例模式基类（不继承Monobehaviour）
/*
public class Test
{
    void Main()
    {
        GameManager.GetInstance();
        ObjectManager.GetInstance();
    }
}
*/

//1.C#泛型
//2.设计模式中，单例模式
public class BaseManager<T> where T: new() // where T: new()是泛型约束
{
    //可以用private BaseManager() { } 私有化构造函数，防止从外部去New，但不是必要的
    //关键是一个静态变量和一个静态函数

    private static T instance;

    public static T GetInstance()
    {
        //保证唯一性
        //多线程还需要加双锁*
        if (instance == null)
            instance = new T();
        return instance;
    }
}

public class GameManager : BaseManager<GameManager> { }
public class ObjectManager : BaseManager<ObjectManager> { }
/* ... */