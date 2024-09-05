using Serilog.Core;
using Serilog.Events;
using System.Data.SQLite;

namespace API.Models.Logging
{
    public class CustomSQLiteSink : ILogEventSink
    {
        private readonly string _connectionString;

        public CustomSQLiteSink(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Emit(LogEvent logEvent)
        {
            Task.Run(() => WriteLogAsync(logEvent)).GetAwaiter().GetResult();
        }

        private async Task WriteLogAsync(LogEvent logEvent)
        {
            var logEntry = new LogEntry
            {
                LogLevel = (LogLevel)logEvent.Level, // Extrae el nivel de log
                Message = logEvent.RenderMessage(), // Renderiza el mensaje de log
                Exception = logEvent.Exception?.ToString() ?? string.Empty, // Extrae la excepción si está presente
                Timestamp = logEvent.Timestamp.UtcDateTime, // Marca de tiempo del evento de log
                UserId = ExtractPropertyValue(logEvent, "UserId") ?? string.Empty, // Extrae la propiedad personalizada o deja vacío
                UserEmail = ExtractPropertyValue(logEvent, "UserEmail") ?? string.Empty, // Extrae la propiedad personalizada o deja vacío
                RequestPath = ExtractPropertyValue(logEvent, "RequestPath") ?? string.Empty, // Extrae la propiedad personalizada o deja vacío
                HttpMethod = ExtractPropertyValue(logEvent, "HttpMethod") ?? string.Empty, // Extrae la propiedad personalizada o deja vacío
                StatusCode = ExtractStatusCode(logEvent), // Extrae el código de estado si está disponible, por defecto a 0
                IpAddress = ExtractPropertyValue(logEvent, "IpAddress") ?? string.Empty, // Extrae la propiedad personalizada o deja vacío
                ApplicationName = "API", // Nombre de la aplicación codificado o configurable
                AdditionalData = ExtractAdditionalData(logEvent) // Extrae datos adicionales si están presentes
            };

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = connection.CreateCommand();
                command.CommandText = @"
                INSERT INTO LogEntries (LogLevel, Message, Exception, Timestamp, UserId, UserEmail, RequestPath, HttpMethod, StatusCode, IpAddress, ApplicationName, AdditionalData)
                VALUES (@LogLevel, @Message, @Exception, @Timestamp, @UserId, @UserEmail, @RequestPath, @HttpMethod, @StatusCode, @IpAddress, @ApplicationName, @AdditionalData)";

                command.Parameters.AddWithValue("@LogLevel", logEntry.LogLevel.ToString());
                command.Parameters.AddWithValue("@Message", logEntry.Message);
                command.Parameters.AddWithValue("@Exception", logEntry.Exception);
                command.Parameters.AddWithValue("@Timestamp", logEntry.Timestamp);
                command.Parameters.AddWithValue("@UserId", logEntry.UserId);
                command.Parameters.AddWithValue("@UserEmail", logEntry.UserEmail);
                command.Parameters.AddWithValue("@RequestPath", logEntry.RequestPath);
                command.Parameters.AddWithValue("@HttpMethod", logEntry.HttpMethod);
                command.Parameters.AddWithValue("@StatusCode", logEntry.StatusCode);
                command.Parameters.AddWithValue("@IpAddress", logEntry.IpAddress);
                command.Parameters.AddWithValue("@ApplicationName", logEntry.ApplicationName);
                command.Parameters.AddWithValue("@AdditionalData", logEntry.AdditionalData);

                await command.ExecuteNonQueryAsync();
            }
        }

        private string? ExtractPropertyValue(LogEvent logEvent, string propertyName)
        {
            return logEvent.Properties.ContainsKey(propertyName)
                ? logEvent.Properties[propertyName].ToString()
                : null;
        }

        private int ExtractStatusCode(LogEvent logEvent)
        {
            if (logEvent.Properties.ContainsKey("StatusCode"))
            {
                if (int.TryParse(logEvent.Properties["StatusCode"].ToString(), out var statusCode))
                {
                    return statusCode;
                }
            }
            return 0;
        }

        private string ExtractAdditionalData(LogEvent logEvent)
        {
            // Implement logic to extract additional data if any
            return string.Empty; // Adjust as needed
        }
    }
}
