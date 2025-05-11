using System;
using System.Collections.Generic;
using System.Linq;

namespace WordPress.Session
{
    /// <summary>
    /// Meta-based user sessions token manager.
    /// </summary>
    public class WP_User_Meta_Session_Tokens : WP_Session_Tokens
    {
        /// <summary>
        /// User ID for whom the sessions are managed.
        /// </summary>
        private int UserId { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userId">User ID.</param>
        public WP_User_Meta_Session_Tokens(int userId)
        {
            UserId = userId;
        }

        /// <summary>
        /// Retrieves all sessions of the user.
        /// </summary>
        /// <returns>Sessions of the user.</returns>
        protected override Dictionary<string, Session> GetSessions()
        {
            var sessions = GetUserMeta(UserId, "session_tokens");

            if (sessions == null || !sessions.Any())
            {
                return new Dictionary<string, Session>();
            }

            sessions = sessions.ToDictionary(
                kvp => kvp.Key,
                kvp => PrepareSession(kvp.Value)
            );

            return sessions.Where(kvp => IsStillValid(kvp.Value)).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Converts an expiration to a session information object.
        /// </summary>
        /// <param name="session">Session or expiration.</param>
        /// <returns>Session.</returns>
        protected Session PrepareSession(object session)
        {
            if (session is int expiration)
            {
                return new Session { Expiration = expiration };
            }

            return session as Session;
        }

        /// <summary>
        /// Retrieves a session based on its verifier (token hash).
        /// </summary>
        /// <param name="verifier">Verifier for the session to retrieve.</param>
        /// <returns>The session, or null if it does not exist.</returns>
        protected override Session GetSession(string verifier)
        {
            var sessions = GetSessions();

            return sessions.ContainsKey(verifier) ? sessions[verifier] : null;
        }

        /// <summary>
        /// Updates a session based on its verifier (token hash).
        /// </summary>
        /// <param name="verifier">Verifier for the session to update.</param>
        /// <param name="session">Optional. Session. Omitting this argument destroys the session.</param>
        protected override void UpdateSession(string verifier, Session session = null)
        {
            var sessions = GetSessions();

            if (session != null)
            {
                sessions[verifier] = session;
            }
            else
            {
                sessions.Remove(verifier);
            }

            UpdateSessions(sessions);
        }

        /// <summary>
        /// Updates the user's sessions in the database.
        /// </summary>
        /// <param name="sessions">Sessions.</param>
        protected void UpdateSessions(Dictionary<string, Session> sessions)
        {
            if (sessions.Any())
            {
                UpdateUserMeta(UserId, "session_tokens", sessions);
            }
            else
            {
                DeleteUserMeta(UserId, "session_tokens");
            }
        }

        /// <summary>
        /// Destroys all sessions for this user, except the single session with the given verifier.
        /// </summary>
        /// <param name="verifier">Verifier of the session to keep.</param>
        protected override void DestroyOtherSessions(string verifier)
        {
            var session = GetSession(verifier);
            UpdateSessions(new Dictionary<string, Session> { { verifier, session } });
        }

        /// <summary>
        /// Destroys all session tokens for the user.
        /// </summary>
        protected override void DestroyAllSessions()
        {
            UpdateSessions(new Dictionary<string, Session>());
        }

        /// <summary>
        /// Destroys all sessions for all users.
        /// </summary>
        public static void DropSessions()
        {
            DeleteMetadata("user", 0, "session_tokens", true);
        }

        /// <summary>
        /// Checks if a session is still valid.
        /// </summary>
        /// <param name="session">Session to check.</param>
        /// <returns>Whether the session is still valid.</returns>
        private bool IsStillValid(Session session)
        {
            return session.Expiration > DateTime.UtcNow.ToUnixTime();
        }

        /// <summary>
        /// Simulates fetching user meta from the database.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="key">Meta key.</param>
        /// <returns>User meta data.</returns>
        private Dictionary<string, Session> GetUserMeta(int userId, string key)
        {
            // Simulate fetching user meta from the database
            return new Dictionary<string, Session>
            {
                { "token1", new Session { Expiration = DateTime.UtcNow.AddHours(1).ToUnixTime() } },
                { "token2", new Session { Expiration = DateTime.UtcNow.AddMinutes(-10).ToUnixTime() } }
            };
        }

        /// <summary>
        /// Simulates updating user meta in the database.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="key">Meta key.</param>
        /// <param name="value">Meta value.</param>
        private void UpdateUserMeta(int userId, string key, Dictionary<string, Session> value)
        {
            // Simulate updating user meta in the database
        }

        /// <summary>
        /// Simulates deleting user meta from the database.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="key">Meta key.</param>
        private void DeleteUserMeta(int userId, string key)
        {
            // Simulate deleting user meta from the database
        }

        /// <summary>
        /// Simulates deleting metadata for all users.
        /// </summary>
        /// <param name="type">Type of metadata (e.g., "user").</param>
        /// <param name="objectId">Object ID (0 for all objects).</param>
        /// <param name="metaKey">Meta key.</param>
        /// <param name="deleteAll">Whether to delete all metadata.</param>
        private static void DeleteMetadata(string type, int objectId, string metaKey, bool deleteAll)
        {
            // Simulate deleting metadata from the database
        }
    }

    /// <summary>
    /// Represents a session.
    /// </summary>
    public class Session
    {
        public long Expiration { get; set; }
    }

    /// <summary>
    /// Base class for session tokens.
    /// </summary>
    public abstract class WP_Session_Tokens
    {
        protected abstract Dictionary<string, Session> GetSessions();
        protected abstract Session GetSession(string verifier);
        protected abstract void UpdateSession(string verifier, Session session = null);
        protected abstract void DestroyOtherSessions(string verifier);
        protected abstract void DestroyAllSessions();
    }

    /// <summary>
    /// Extension methods for DateTime.
    /// </summary>
    public static class DateTimeExtensions
    {
        public static long ToUnixTime(this DateTime date)
        {
            return ((DateTimeOffset)date).ToUnixTimeSeconds();
        }
    }
}