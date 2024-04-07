using Newtonsoft.Json;
using OWML.Common;
using OWML.ModHelper;
using OWRichPresence.API;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

namespace OuterWildsAtHome
{
    public class OuterWildsAtHome : ModBehaviour
    {
        private string url, token, sensor;

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
            url = ModHelper.Config.GetSettingsValue<string>("Home Assistant URL");
            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            token = ModHelper.Config.GetSettingsValue<string>("Home Assistant long-lived access token");
            sensor = ModHelper.Config.GetSettingsValue<string>("Home Assistant entity ID");
        }

        private void StatusHandler(string details, string largeImageKey, string largeImageText)
        {
            ModHelper.Console.WriteLine($"details: {details}; largeImageKey: {largeImageKey}; largeImageText: {largeImageText};");
            StartCoroutine(SendUpdate(details, largeImageKey, largeImageText));
        }
        private void OnApplicationQuit()
        {
            StartCoroutine(SendUpdate("not playing", "", ""));
        }

        IEnumerator SendUpdate(string details, string largeImageKey, string largeImageText)
        {
            var obj = new
            {
                state = largeImageText,
                attributes = new
                {
                    activity = details,
                    icon = largeImageKey,
                }
            };

            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

            var uploader = new UploadHandlerRaw(data)
            {
                contentType = "application/json"
            };

            var www = new UnityWebRequest($"{url}api/states/{sensor}")
            {
                method = UnityWebRequest.kHttpVerbPOST,
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = uploader,
            };
            www.SetRequestHeader("Authorization", $"Bearer {token}");
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");

            yield return www.SendWebRequest();

            if (www.responseCode == 200 || www.responseCode == 201)
            {
                ModHelper.Console.WriteLine(www.downloadHandler.text);
            }
            else
            {
                ModHelper.Console.WriteLine(www.downloadHandler.text, MessageType.Error);
            }
        }
    }
}
