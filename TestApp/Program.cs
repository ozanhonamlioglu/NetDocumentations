// See https://aka.ms/new-console-template for more information

using System.Reflection;
using MyNamespace.Dynamics;

namespace MyNamespace;

internal class Program
{
    static void Main()
    {
        var currentAssemly = Assembly.GetExecutingAssembly();
        var dynamicClasses = currentAssemly.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IDynamic)));

        foreach (var dc in dynamicClasses)
        {
            object classInstance = Activator.CreateInstance(dc, null);
            dc.GetMethod("ExecuteMe").Invoke(classInstance, null);
        }
    }
}