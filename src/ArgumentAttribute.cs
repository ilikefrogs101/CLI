namespace ilikefrogs101.CLI;
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ArgumentAttribute : Attribute{
    public string Name;
    public string Description;
    public bool Boolean = false;
    public ArgumentType ArgumentType;
}
public enum ArgumentType {
    PositionalRequired,
    PositionalOptional,
    Flag
}