namespace Riwi_Courses_Assessment_Backend.Domain.Exceptions;

public class LessonNotFoundException : DomainException
{
    public LessonNotFoundException(Guid lessonId) 
        : base($"Lesson with ID '{lessonId}' was not found.")
    {
    }
}

public class DuplicateLessonOrderException : DomainException
{
    public DuplicateLessonOrderException(Guid courseId, int order) 
        : base($"A lesson with order '{order}' already exists in course '{courseId}'.")
    {
    }
}

public class InvalidLessonOrderException : DomainException
{
    public InvalidLessonOrderException(int order) 
        : base($"Lesson order '{order}' is invalid. Order must be greater than or equal to 0.")
    {
    }

    public InvalidLessonOrderException(string message) 
        : base(message)
    {
    }
}

