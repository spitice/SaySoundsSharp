namespace SaySoundsSharp;

public class UserSaySoundInput {

    public readonly string soundName;
    public readonly int pitch;

    public UserSaySoundInput(string soundName, int pitch = 100) {
        this.soundName = soundName.ToLower();
        //this.pitch = Math.Clamp(pitch, 25, 255);

        // Round the pitch so we have p50, 100, 150, and 200
        pitch = Math.Clamp(pitch, 50, 200);
        pitch = (int)(Math.Round(pitch / 50.0d) * 50);
        this.pitch = pitch;
    }

    public bool isPitchModified {
        get {
            return pitch != 100;
        }
    }

}