namespace ilikefrogs101.CLI;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute {
    public string Name;
    public string Description;
}