using System;
using System.Reflection;

namespace GeometriaComputacional;

public static class ControllerSelector
{
    public static Controller Select(string param)
    {
        var assembly = Assembly.GetExecutingAssembly();
        foreach (var type in assembly.GetTypes())
        {
            if (type.BaseType != typeof(Controller))
                continue;
            
            string name = type.Name
                .ToLower()
                .Replace("controller", "");
            if (name != param)
                continue;
            
            var constructor = type.GetConstructor(new Type[0]);
            if (constructor == null)
                continue;
            
            var obj = constructor.Invoke(new object[0]) as Controller;
            if (obj == null)
                continue;
            
            return obj;
        }
        return new Controller();
    }
}