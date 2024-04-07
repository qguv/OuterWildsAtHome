using System;
using System.Net.Http;
using System.Net.Http.Json;
using OWML.Common;
using OWML.ModHelper;
using OWRichPresence.API;

namespace OuterWildsAtHome
{
    public class OuterWildsAtHome : ModBehaviour
    {
        private static readonly HttpClient client = new();

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

            Configure(ModHelper.Config);
        }
        public override void Configure(IModConfig config)
        {
            ModHelper.Console.WriteLine("SWEETPOTATO configure called");

            var url = ModHelper.Config.GetSettingsValue<string>("Home Assistant URL");
            var token = ModHelper.Config.GetSettingsValue<string>("Home Assistant long-lived access token");
            var sensor = ModHelper.Config.GetSettingsValue<string>("Home Assistant entity ID");

            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            client.BaseAddress = new Uri($"{url}api/states/{sensor}");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        private void StatusHandler(string details, string largeImageKey, string largeImageText)
        {
            ModHelper.Console.WriteLine($"details: {details}; largeImageKey: {largeImageKey}; largeImageText: {largeImageText};");
            client.PostAsJsonAsync("/", new
            {
                state = largeImageText,
                attributes = new
                {
                    activity = details,
                    icon = largeImageKey,
                }
            });
        }
    }
}
