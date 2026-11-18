using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

namespace ilikefrogs101.CLI;
public static class CommandParser {
    public static ParsedCommand Parse(string toParse, BaseCommand root) {
        ImmutableArray<string> tokens = _tokenise(toParse);
        ParseState state = _parseTokens(tokens, root);

        ParsedCommand parsed = new ParsedCommand()
            .WithHandler(state.Handler)
            .WithContext(_constructContext(state, tokens));

        return parsed;
    }

    private static CommandContext _constructContext(ParseState state, ImmutableArray<string> tokens) {
        CommandContext initialContext = state.Parameters.Aggregate(
            new CommandContext(),
            (context, parameter) => context
                .WithParameter(parameter.Key, parameter.Value)
        );
        CommandContext context = state.Options.Aggregate(
            initialContext,
            (context, option) => context
                .WithOption(option.Name, option.Parse(option.Default.ToString()))
        );
        return tokens
            .Select((token, index) => (token, index))
            .Where(token => state.Options.Any(option => option.Name == token.token))
            .Aggregate(
                (context, skipNext: false),
                (current, token) => {
                    if (current.skipNext) return (current.context, false);

                    Option option = state.Options.First(option => option.Name == token.token);
                    string value = option.Default is bool defaultValue ? (!defaultValue).ToString() : tokens[token.index + 1];

                    return (current.context.WithOption(token.token, option.Parse(value)), option.Default is not bool);
                }
            ).context;
    }
    private static ParseState _parseTokens(
        ImmutableArray<string> tokens, 
        BaseCommand command,
        int tokenIndex = 0,
        int positionalOffset = 0
    ) {        
        if (tokenIndex >= tokens.Length) return new();
        
        ParseState baseState = command.Options.Aggregate(
            new ParseState(),
            (state, option) => state
                .WithOption(option)
        );

        if (baseState.Options.Any(option => option.Name == tokens[tokenIndex])) {
            Option opt = baseState.Options.FirstOrDefault(option => option.Name == tokens[tokenIndex]);
            if (opt.Default is bool) {
                return _parseTokens(tokens, command, tokenIndex + 1, positionalOffset + 1);
            }
            else {
                return _parseTokens(tokens, command, tokenIndex + 1, positionalOffset + 2);
            }
        }

        int counter = tokenIndex - positionalOffset;
        if (counter >= command.PositionalElements.Length) return new();
        
        if (command.PositionalElements[counter] is SubcommandPlaceholder) {
            Command subcommand = command.Subcommands.FirstOrDefault(command => command.Name == tokens[tokenIndex]);
            ParseState parseState = baseState
                .CombinedWith(_parseTokens(tokens, subcommand, tokenIndex + 1, tokenIndex + 1))
                .CombinedWith(_parseTokens(tokens, command, tokenIndex + 1, positionalOffset));
            
            return parseState.WithHandler(parseState.Handler ?? subcommand.Handler);
        }
        else {
            Variable parameter = (Variable)command.PositionalElements[counter];
            ParseState parseState = baseState
                .CombinedWith(_parseTokens(tokens, command, tokenIndex + 1, positionalOffset))
                .WithParameter(parameter.Name, parameter.Parse(tokens[tokenIndex]));

            return parseState;
        }
    }
    private static ImmutableArray<string> _tokenise(string command) {
        return Regex.Matches(command, @"(?<=^| )(""([^""]|(""""))*""|[^\s""]+)")
            .Select(m => m.Value.Trim('"').Replace("\"\"", "\""))
            .ToImmutableArray();
    }
}

public sealed record ParseState(
    ImmutableHashSet<Option> Options,
    ImmutableDictionary<string, object> Parameters,
    Action<CommandContext> Handler
) {
    public ParseState() : this(
        ImmutableHashSet<Option>.Empty,
        ImmutableDictionary<string, object>.Empty,
        null
    ) {}
    public ParseState CombinedWith(ParseState state) {
        Action<CommandContext> handler = state.Handler ?? Handler;
        return this with {
            Options = Options.Union(state.Options),
            Parameters = Parameters.SetItems(state.Parameters),
            Handler = handler
        };
    }
    public ParseState WithOption(Option option) {
        return this with {
            Options = Options.Add(option)
        };
    }
    public ParseState WithParameter(string name, object value) {
        return this with {
            Parameters = Parameters.SetItem(name, value)
        };
    }
    public ParseState WithHandler(Action<CommandContext> handler) {
        return this with {
            Handler = handler
        };
    }
}