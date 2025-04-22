using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using POP3.Core.Data;
using POP3.Core.Models;

namespace POP3.Core
{
    public class POP3
    {
        // Properties
        public string Error { get; private set; }
        public int Timeout { get; set; } = 60;
        public bool Debug { get; set; } = false;
        public TcpClient Pop3Connection { get; private set; }
        private readonly AppDbContext _dbContext;

        public POP3(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Connects to the POP3 server.
        /// </summary>
        /// <param name="server">The POP3 server host.</param>
        /// <param name="port">The POP3 server port.</param>
        /// <returns>True if the connection was successful; otherwise, false.</returns>
        public async Task<bool> ConnectAsync(string server, int port = 110)
        {
            try
            {
                Pop3Connection = new TcpClient();
                await Pop3Connection.ConnectAsync(server, port);

                using (var stream = Pop3Connection.GetStream())
                {
                    var reader = new StreamReader(stream);
                    var response = await reader.ReadLineAsync();

                    // Log the connection
                    _dbContext.Pop3Logs.Add(new Pop3Log
                    {
                        Command = "CONNECT",
                        Response = response,
                        Timestamp = DateTime.UtcNow,
                        IsSuccessful = true
                    });
                    await _dbContext.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the error
                _dbContext.Pop3Logs.Add(new Pop3Log
                {
                    Command = "CONNECT",
                    Response = ex.Message,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                });
                await _dbContext.SaveChangesAsync();

                throw new Exception("Failed to connect to POP3 server.", ex);
            }
        }

        /// <summary>
        /// Sends a command to the POP3 server and reads the response.
        /// </summary>
        /// <param name="command">The POP3 command to send.</param>
        /// <returns>The server's response.</returns>
        public async Task<string> SendCommandAsync(string command)
        {
            try
            {
                using (var stream = Pop3Connection.GetStream())
                {
                    var writer = new StreamWriter(stream) { AutoFlush = true };
                    await writer.WriteLineAsync(command);

                    // Log the command
                    _dbContext.Pop3Logs.Add(new Pop3Log
                    {
                        Command = command,
                        Timestamp = DateTime.UtcNow
                    });
                    await _dbContext.SaveChangesAsync();

                    var reader = new StreamReader(stream);
                    var response = await reader.ReadLineAsync();

                    // Log the response
                    _dbContext.Pop3Logs.Add(new Pop3Log
                    {
                        Command = command,
                        Response = response,
                        Timestamp = DateTime.UtcNow,
                        IsSuccessful = true
                    });
                    await _dbContext.SaveChangesAsync();

                    return response;
                }
            }
            catch (Exception ex)
            {
                // Log the error
                _dbContext.Pop3Logs.Add(new Pop3Log
                {
                    Command = command,
                    Response = ex.Message,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                });
                await _dbContext.SaveChangesAsync();

                throw new Exception("Failed to send POP3 command.", ex);
            }
        }

        /// <summary>
        /// Logs in to the POP3 server.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if the login was successful; otherwise, false.</returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            try
            {
                var userResponse = await SendCommandAsync($"USER {username}");
                if (!userResponse.StartsWith("+OK"))
                    return false;

                var passResponse = await SendCommandAsync($"PASS {password}");
                if (!passResponse.StartsWith("+OK"))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to log in to POP3 server.", ex);
            }
        }

        /// <summary>
        /// Retrieves the list of messages from the POP3 server.
        /// </summary>
        /// <returns>A dictionary of message numbers and their sizes.</returns>
        public async Task<Dictionary<int, int>> ListMessagesAsync()
        {
            try
            {
                var response = await SendCommandAsync("LIST");
                var lines = response.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                var messages = new Dictionary<int, int>();
                foreach (var line in lines.Skip(1))
                {
                    var parts = line.Split(' ');
                    if (parts.Length == 2 && int.TryParse(parts[0], out var msgNum) && int.TryParse(parts[1], out var size))
                    {
                        messages[msgNum] = size;
                    }
                }

                return messages;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve message list.", ex);
            }
        }

        /// <summary>
        /// Retrieves the content of a specific message.
        /// </summary>
        /// <param name="msgNum">The message number.</param>
        /// <returns>The content of the message.</returns>
        public async Task<string> GetMessageAsync(int msgNum)
        {
            try
            {
                var response = await SendCommandAsync($"RETR {msgNum}");
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve message.", ex);
            }
        }

        /// <summary>
        /// Deletes a specific message from the POP3 server.
        /// </summary>
        /// <param name="msgNum">The message number.</param>
        /// <returns>True if the deletion was successful; otherwise, false.</returns>
        public async Task<bool> DeleteMessageAsync(int msgNum)
        {
            try
            {
                var response = await SendCommandAsync($"DELE {msgNum}");
                return response.StartsWith("+OK");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete message.", ex);
            }
        }

        /// <summary>
        /// Closes the POP3 connection.
        /// </summary>
        public void Close()
        {
            Pop3Connection?.Close();
            Pop3Connection = null;

            // Log the disconnection
            _dbContext.Pop3Logs.Add(new Pop3Log
            {
                Command = "QUIT",
                Timestamp = DateTime.UtcNow,
                IsSuccessful = true
            });
            _dbContext.SaveChanges();
        }
    }
}