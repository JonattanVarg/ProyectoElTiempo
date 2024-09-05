namespace API.Dtos.Responses
{
    public class GenericResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; } 
    }
}
