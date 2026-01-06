namespace Riwi.CoursesAssessment.Domain.Exceptions;

public class CourseCannotBePublishedException : DomainException
{
    public CourseCannotBePublishedException() 
        : base("Cannot publish a course without active lessons.")
    {
    }

    public CourseCannotBePublishedException(string message) 
        : base(message)
    {
    }
}

public class CourseNotFoundException : DomainException
{
    public CourseNotFoundException(Guid courseId) 
        : base($"Course with ID '{courseId}' was not found.")
    {
    }
}

public class CourseAlreadyPublishedException : DomainException
{
    public CourseAlreadyPublishedException(Guid courseId) 
        : base($"Course with ID '{courseId}' is already published.")
    {
    }
}

public class CourseAlreadyDraftException : DomainException
{
    public CourseAlreadyDraftException(Guid courseId) 
        : base($"Course with ID '{courseId}' is already in draft status.")
    {
    }
}

