using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using PHPMailer.Core.Data;
using PHPMailer.Core.Models;

namespace PHPMailer.Core.SMTP
{
    public class SMTP
    {
        // Properties
        public int Timeout { get; set; } = 300;
        public int DebugLevel { get; set; } = 0;
        public string LastReply { get; private set; }
        public Dictionary<string, object> ServerCapabilities { get; private set; } = new Dictionary<string, object>();
        public TcpClient SmtpConnection { get; private set; }
        public string LastTransactionId { get; private set; }

        private readonly AppDbContext _dbContext;

        public SMTP(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Connects to the SMTP server.
        /// </summary>
        /// <param name="host">The SMTP server host.</param>
        /// <param name="port">The SMTP server port.</param>
        /// <returns>True if the connection was successful; otherwise, false.</returns>
        public async Task<bool> ConnectAsync(string host, int port)
        {
            try
            {
                SmtpConnection = new TcpClient();
                await SmtpConnection.ConnectAsync(host, port);
                await ReadResponseAsync();

                // Log the connection
                _dbContext.SmtpLogs.Add(new SmtpLog
                {
                    Command = "CONNECT",
                    Response = LastReply,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = true
                });
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Log the error
                _dbContext.SmtpLogs.Add(new SmtpLog
                {
                    Command = "CONNECT",
                    Response = ex.Message,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                });
                await _dbContext.SaveChangesAsync();

                throw new Exception("Failed to connect to SMTP server.", ex);
            }
        }

        /// <summary>
        /// Sends a command to the SMTP server and reads the response.
        /// </summary>
        /// <param name="command">The SMTP command to send.</param>
        /// <param name="expectedCode">The expected response code.</param>
        /// <returns>True if the response code matches the expected code; otherwise, false.</returns>
        public async Task<bool> SendCommandAsync(string command, int expectedCode)
        {
            try
            {
                using (var stream = SmtpConnection.GetStream())
                {
                    var writer = new StreamWriter(stream) { AutoFlush = true };
                    await writer.WriteLineAsync(command);

                    // Log the command
                    _dbContext.SmtpLogs.Add(new SmtpLog
                    {
                        Command = command,
                        Timestamp = DateTime.UtcNow
                    });
                    await _dbContext.SaveChangesAsync();

                    await ReadResponseAsync();

                    // Check the response code
                    var responseCode = int.Parse(LastReply.Substring(0, 3));
                    if (responseCode == expectedCode)
                    {
                        return true;
                    }

                    throw new Exception($"Unexpected response: {LastReply}");
                }
            }
            catch (Exception ex)
            {
                // Log the error
                _dbContext.SmtpLogs.Add(new SmtpLog
                {
                    Command = command,
                    Response = ex.Message,
                    Timestamp = DateTime.UtcNow,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message
                });
                await _dbContext.SaveChangesAsync();

                throw new Exception("Failed to send SMTP command.", ex);
            }
        }

        /// <summary>
        /// Reads the response from the SMTP server.
        /// </summary>
        private async Task ReadResponseAsync()
        {
            using (var stream = SmtpConnection.GetStream())
            {
                var reader = new StreamReader(stream);
                var response = await reader.ReadLineAsync();
                LastReply = response;

                // If the response is multi-line, read all lines
                while (!string.IsNullOrEmpty(response) && response[3] == '-')
                {
                    response = await reader.ReadLineAsync();
                    LastReply += Environment.NewLine + response;
                }
            }
        }

        /// <summary>
        /// Closes the SMTP connection.
        /// </summary>
        public void Close()
        {
            SmtpConnection?.Close();
            SmtpConnection = null;

            // Log the disconnection
            _dbContext.SmtpLogs.Add(new SmtpLog
            {
                Command = "QUIT",
                Timestamp = DateTime.UtcNow,
                IsSuccessful = true
            });
            _dbContext.SaveChanges();
        }
    }
}