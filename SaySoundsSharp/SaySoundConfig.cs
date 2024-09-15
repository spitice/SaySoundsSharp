using CounterStrikeSharp.API;

namespace SaySoundsSharp;

public class SaySoundConfig {

    // trigger name, sound event name
    // e.g. hey!, sayounds.hey!
    public readonly Dictionary<string, string> saySounds = new();

    public SaySoundConfig(string configPath) {

        if(!parseConfig(configPath)) {
            throw new InvalidOperationException("Failed to parse config!");
        }
    }

    private bool parseConfig(string configPath) {
        string[] lines = File.ReadAllLines(configPath);
        foreach (var line in lines)
        {
            saySounds[line] = line;
        }

        Console.WriteLine("[SaySound parseConfig] Parsed saysound list.");
        Console.WriteLine("  - # of lines in config: " + lines.Length);
        Console.WriteLine("  - # of saysounds registered: " + saySounds.Count);

        return true;
    }
}