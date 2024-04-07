using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using OWML.Common;
using OWML.ModHelper;
using OWRichPresence.API;
using System.Threading.Tasks;

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
            Task.Run(async () => { await PostAsync(details, largeImageKey, largeImageText); });
        }

        private async Task PostAsync(string details, string largeImageKey, string largeImageText)
        {
            var data = JsonConvert.SerializeObject(new
            {
                state = largeImageText,
                attributes = new
                {
                    activity = details,
                    icon = largeImageKey,
                }
            });
            using StringContent jsonContent = new(data, Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await client.PostAsync("/", jsonContent);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            ModHelper.Console.WriteLine(jsonResponse);
        }
    }
}
