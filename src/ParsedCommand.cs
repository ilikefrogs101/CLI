using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

namespace ilikefrogs101.CLI;
public sealed record ParsedCommand (
    Action<CommandContext> Handler,
    CommandContext Context
)
{
    public ParsedCommand() : this(
        null,
        new()
    ) {}

    public ParsedCommand WithHandler(Action<CommandContext> newHandler) {
        return this with {
            Handler = newHandler
        };
    }
    public ParsedCommand WithContext(CommandContext context) {
        return this with {
            Context = context
        };
    }
    public void Execute() {
        Handler.Invoke(Context);
    }
}
public record CommandContext (
    ImmutableDictionary<string, object> Parameters,
    ImmutableDictionary<string, object> Options
)
{
    public CommandContext() : this(
        ImmutableDictionary<string, object>.Empty,
        ImmutableDictionary<string, object>.Empty
    ) {}

    public CommandContext CombinedWith(CommandContext combineWith) {
        return this with {
            Parameters = Parameters.SetItems(combineWith.Parameters).ToImmutableDictionary(),
            Options = Options.SetItems(combineWith.Options).ToImmutableDictionary()
        };
    }
    public CommandContext WithParameter(string name, object value) {
        return this with {
            Parameters = Parameters.SetItem(name, value)
        };
    }
    public CommandContext WithOption(string name, object value) {
        return this with {
            Options = Options.SetItem(name, value)
        };
    }
    public T GetParameterValue<T>(string parameterName) {
        return (T)Parameters[parameterName];
    }
    public T GetOptionValue<T>(string optionName) {
        return (T)Options[optionName];
    }
}