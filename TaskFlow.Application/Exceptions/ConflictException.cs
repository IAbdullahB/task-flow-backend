namespace TaskFlow.Application.Exceptions;

public class ConflictException(string message) : Exception(message)
{
}