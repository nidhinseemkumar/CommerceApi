namespace CommerceApi.Exceptions;

public class CommerceException(string message, int statusCode = 400) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}

public class UnauthorizedException(string message = "Invalid credentials.") : CommerceException(message, 401);
public class ConflictException(string message) : CommerceException(message, 409);
public class NotFoundException(string message) : CommerceException(message, 404);
public class BadRequestException(string message) : CommerceException(message, 400);
