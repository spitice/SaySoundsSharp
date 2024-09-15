namespace SaySoundsSharp;

public static class SaySoundUtil {
    public static UserSaySoundInput processUserInput(string argString) {
        argString = argString.Trim();
        argString = argString.Trim('\"');

        string[] args = argString.Split(" ");

        string saySound = "";
        int pitch = 100;

        foreach(string arg in args) {
            if(arg.StartsWith("@") && arg.Length > 1) {
                pitch = int.Parse(arg.Substring(1));
            }
            else if (arg.Contains("%", StringComparison.InvariantCultureIgnoreCase)) {
                // TODO()
            }
            else {
                saySound += $" {arg}";
            }
        }

        if(saySound != "")
        {
            saySound = saySound.TrimStart();  // Remove the space
        }

        return new UserSaySoundInput(saySound, pitch);
    }
}