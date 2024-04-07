using OWML.Common;
using OWML.ModHelper;
using OWRichPresence.API;

namespace OuterWildsAtHome
{
    public class OuterWildsAtHome : ModBehaviour
    {
        private void Awake() {}

        private void Start()
        {
            ModHelper.Console.WriteLine("Checking for MegaPiggy.OWRichPresence...");
            if (!ModHelper.Interaction.ModExists("MegaPiggy.OWRichPresence"))
            {
                ModHelper.Console.WriteLine("Please install or update the Discord Rich Presence mod: https://outerwildsmods.com/mods/discordrichpresence owmods://install-mod/MegaPiggy.OWRichPresence", MessageType.Error);
                return;
            }

            ModHelper.Console.WriteLine("Registering handler...");
            var richPresence = ModHelper.Interaction.TryGetModApi<IRichPresenceAPI>("MegaPiggy.OWRichPresence");
            richPresence.RegisterHandler(StatusHandler);
            ModHelper.Console.WriteLine("Registered handler");
        }

        private void StatusHandler(string details, string largeImageKey, string largeImageText) =>
            ModHelper.Console.WriteLine($"details: {details}; largeImageKey: {largeImageKey}; largeImageText: {largeImageText};");
    }

}
