namespace MagicMatchTracker.Client.Features.Common.Extensions;

public static class SessionExtensions
{
	extension(IEnumerable<Session> sessions)
	{
		public Session? GetActiveSession()
		{
			// Use the most recent session for testing
			return sessions.FirstOrDefault();
		}
	}
}