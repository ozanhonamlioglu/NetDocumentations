namespace MyNamespace.Dynamics;

public class DynamicA : IDynamic
{
    public DynamicA()
    {
        
    }
    public void ExecuteMe()
    {
        Console.WriteLine("I am the dynamic A");
    }
}

public class DynamicB : IDynamic
{
    public DynamicB()
    {
        
    }
    public void ExecuteMe()
    {
        Console.WriteLine("I am the dynamic B");
    }
}