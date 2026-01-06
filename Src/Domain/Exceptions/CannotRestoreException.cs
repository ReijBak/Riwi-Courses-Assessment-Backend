namespace Riwi.CoursesAssessment.Domain.Exceptions;

public class CannotRestoreException : DomainException
{
    public CannotRestoreException(string entityType, Guid id)
        : base($"{entityType} with ID {id} is not deleted and cannot be restored")
    {
    }

    public CannotRestoreException(string message)
        : base(message)
    {
    }
}

