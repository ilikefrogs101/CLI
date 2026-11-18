using System;

namespace ilikefrogs101.CLI;
public static class StandardParsers {
    public static object ParseString(string input) {
        return input.ToString();
    }
    public static object ParseInt(string input) {
        return Int32.Parse(input);
    }
}