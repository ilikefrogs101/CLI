using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using ilikefrogs101.Logging;

namespace ilikefrogs101.CLI;
public interface IElement {
    string Name { get; }
    string Description { get; }
}

public record BaseCommand (
    Action<CommandContext> Handler,
    ImmutableArray<Command> Subcommands,
    ImmutableArray<IElement> PositionalElements,
    ImmutableArray<Option> Options
)
{
    public BaseCommand() : this(
        null,
        ImmutableArray<Command>.Empty,
        ImmutableArray<IElement>.Empty,
        ImmutableArray<Option>.Empty
    ) {}

    public BaseCommand WithHandler(Action<CommandContext> handler) {
        return this with {
            Handler = handler
        };
    }
    public BaseCommand WithSubcommand(Command subcommand) {
        return this with {
            Subcommands = Subcommands.Add(subcommand),
        };
    }
    public BaseCommand WithParameter(Variable parameter) {
        return this with {
            PositionalElements = PositionalElements.Add(parameter),
        };
    }
    public BaseCommand WithOption(Option option) {
        return this with {
            Options = Options.Add(option)
        };
    }
    public BaseCommand WithSubcommandPlaceholder() {
        if (PositionalElements.Any(element => element.GetType() == typeof(SubcommandPlaceholder))) return this;

        return this with {
            PositionalElements = PositionalElements.Add(new SubcommandPlaceholder())
        };
    }
}
public sealed record Command (
    string Name,
    string Description
) : BaseCommand, IElement 
{
    new public Command WithHandler(Action<CommandContext> handler) {
        return this with {
            Handler = handler
        };
    }
    new public Command WithSubcommand(Command subcommand) {
        return this with {
            Subcommands = Subcommands.Add(subcommand),
        };
    }
    new public Command WithParameter(Variable parameter) {
        return this with {
            PositionalElements = PositionalElements.Add(parameter),
        };
    }
    new public Command WithOption(Option option) {
        return this with {
            Options = Options.Add(option)
        };
    }
    new public Command WithSubcommandPlaceholder() {
        if (PositionalElements.Any(element => element.GetType() == typeof(SubcommandPlaceholder))) return this;

        return this with {
            PositionalElements = PositionalElements.Add(new SubcommandPlaceholder())
        };
    }
}
public sealed record Option(
    string Name,
    string Description,
    Func<string, object> Parse,
    object Default
) : Variable(Name, Description, Parse);
public record Variable(
    string Name,
    string Description,
    Func<string, object> Parse
) : IElement;
public sealed record SubcommandPlaceholder() : IElement {
    public string Name => "";
    public string Description => "";
}