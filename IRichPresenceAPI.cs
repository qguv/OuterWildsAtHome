using System;

namespace OWRichPresence.API;

public interface IRichPresenceAPI
{
	public void RegisterHandler(Action<string, string, string> handler);
}
