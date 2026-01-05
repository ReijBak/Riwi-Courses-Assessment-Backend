using System.Net;
using System.Text.Json;
using Riwi_Courses_Assessment_Backend.Domain.Exceptions;

namespace Riwi_Courses_Assessment_Backend.WebApi.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            DomainException domainException => new
            {
                statusCode = GetStatusCode(domainException),
                message = domainException.Message,
                type = domainException.GetType().Name
            },
            _ => new
            {
                statusCode = (int)HttpStatusCode.InternalServerError,
                message = "An internal server error occurred",
                type = "InternalServerError"
            }
        };

        context.Response.StatusCode = response.statusCode;

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static int GetStatusCode(DomainException exception)
    {
        return exception switch
        {
            CourseNotFoundException => (int)HttpStatusCode.NotFound,
            LessonNotFoundException => (int)HttpStatusCode.NotFound,
            CourseCannotBePublishedException => (int)HttpStatusCode.BadRequest,
            CourseAlreadyPublishedException => (int)HttpStatusCode.BadRequest,
            CourseAlreadyDraftException => (int)HttpStatusCode.BadRequest,
            DuplicateLessonOrderException => (int)HttpStatusCode.BadRequest,
            InvalidLessonOrderException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }
}

