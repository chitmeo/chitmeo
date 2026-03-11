namespace ChitMeo.Shared.Abstractions.Modules;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute : Attribute
{
    public Type ModuleType { get; }

    public DependsOnAttribute(Type moduleType)
    {
        ModuleType = moduleType;
    }
}
