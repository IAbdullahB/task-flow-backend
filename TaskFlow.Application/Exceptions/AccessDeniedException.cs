namespace TaskFlow.Application.Exceptions;

public class AccessDeniedException(string message) : Exception(message)
{
}