namespace TaskFlow.Application.Exceptions;

public class NotFoundException(string message) : Exception(message)
{
}