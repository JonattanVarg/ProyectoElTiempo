namespace API.Models.Logging
{
    public class LogEntry
    {
        public int Id { get; set; } // Identificador único del registro de log
        public LogLevel LogLevel { get; set; } // Nivel del log (Error, Warning, Information, etc.)
        public string Message { get; set; } = string.Empty; // Mensaje del log
        public string Exception { get; set; } = string.Empty; // Información sobre la excepción (si aplica)
        public DateTime Timestamp { get; set; } // Marca de tiempo del log
        public string UserId { get; set; } = string.Empty; // Identificador del usuario que generó el log (si aplica)
        public string UserEmail { get; set; } = string.Empty; // Correo electrónico del usuario que generó el log (si aplica)
        public string RequestPath { get; set; } = string.Empty; // Ruta de la solicitud en la que ocurrió el evento
        public string HttpMethod { get; set; } = string.Empty; // Método HTTP (GET, POST, PUT, etc.)
        public int StatusCode { get; set; } // Código de estado HTTP de la respuesta
        public string IpAddress { get; set; } = string.Empty; // Dirección IP del cliente
        public string ApplicationName { get; set; } = string.Empty; // Nombre de la aplicación para la que se generó el log
        public string AdditionalData { get; set; } = string.Empty; // Datos adicionales en formato JSON o texto libre


        public LogEntry()
        {
            Timestamp = DateTime.UtcNow; // Establece la marca de tiempo al momento de la creación
        }
    }
}