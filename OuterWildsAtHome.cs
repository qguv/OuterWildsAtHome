using OWML.Common;
using OWML.ModHelper;
using OWRichPresence.API;

namespace OuterWildsAtHome
{
    public class OuterWildsAtHome : ModBehaviour
    {
        private void Start()
        {
            if (ModHelper.Interaction.ModExists("MegaPiggy.OWRichPresence"))
            {
                ModHelper.Console.WriteLine("Please install or update the Discord Rich Presence mod: https://outerwildsmods.com/mods/discordrichpresence owmods://install-mod/MegaPiggy.OWRichPresence", MessageType.Error);
                return;
            }

            var richPresence = ModHelper.Interaction.TryGetModApi<IRichPresenceAPI>("MegaPiggy.OWRichPresence");
            richPresence.RegisterHandler(StatusHandler);
            ModHelper.Console.WriteLine("Registered handler", MessageType.Debug);
        }

        private void StatusHandler(string details, string largeImageKey, string largeImageText) =>
            ModHelper.Console.WriteLine($"details: {details}; largeImageKey: {largeImageKey}; largeImageText: {largeImageText};", MessageType.Debug);
    }

}
