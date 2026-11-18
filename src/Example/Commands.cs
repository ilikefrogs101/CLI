using System;
using System.Net.Sockets;
using ilikefrogs101.CLI;
using ilikefrogs101.Logging;

namespace ilikefrogs101.CLI.Example;
public static class Commands {
    public static void Main(string[] args) {
        Log.OnDebugInformation += Console.WriteLine;

        Command listAdd = new Command("add", "add an item to the list")
            .WithParameter(new Variable("itemId", "ID of the item to add", StandardParsers.ParseString))
            .WithHandler(Init);

        Command listRemove = new Command("add", "add an item to the list")
            .WithParameter(new Variable("itemId", "ID of the item to add", StandardParsers.ParseString))
            .WithHandler(_Init);


        Command list = new Command("list", "interact with lists")
            .WithParameter(new Variable("listName", "List name", StandardParsers.ParseString))
            .WithSubcommandPlaceholder()
            .WithSubcommand(listAdd)
            .WithSubcommand(listRemove);

        BaseCommand root = new BaseCommand()
            .WithSubcommand(list)
            .WithSubcommandPlaceholder();

        CommandParser.Parse(root, "list 1234 add 5678").Execute();

    }
    public static void Init(CommandContext context) {
        Log.DebugMessage($"Adding {context.GetParameterValue<string>("itemId")} to list {context.GetParameterValue<string>("listName")}");
    }
    public static void _Init(CommandContext context) {
        Log.DebugMessage($"Removing {context.GetParameterValue<string>("itemId")} from list {context.GetParameterValue<string>("listName")}");
    }
}