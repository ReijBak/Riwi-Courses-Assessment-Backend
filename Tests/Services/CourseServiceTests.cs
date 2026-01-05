using Xunit;
using Moq;
using FluentAssertions;
using Riwi_Courses_Assessment_Backend.Application.Services;
using Riwi_Courses_Assessment_Backend.Domain.Entities;
using Riwi_Courses_Assessment_Backend.Domain.Enums;
using Riwi_Courses_Assessment_Backend.Domain.Exceptions;
using Riwi_Courses_Assessment_Backend.Domain.Interfaces;

namespace Riwi_Courses_Assessment_Backend.Tests.Services;

public class CourseServiceTests
{
    [Fact]
    public async Task PublishCourse_WithLessons_ShouldSucceed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Draft
        };
        
        var lesson = new Lesson
        {
            Id = Guid.NewGuid(),
            CourseId = courseId,
            Title = "Test Lesson",
            Order = 1,
            IsDeleted = false
        };
        
        course.Lessons.Add(lesson);

        var mockCourseRepo = new Mock<ICourseRepository>();
        var mockLessonRepo = new Mock<ILessonRepository>();
        
        mockCourseRepo.Setup(r => r.GetByIdWithLessonsAsync(courseId))
            .ReturnsAsync(course);
        
        mockCourseRepo.Setup(r => r.UpdateAsync(It.IsAny<Course>()))
            .Returns(Task.CompletedTask);

        var service = new CourseService(mockCourseRepo.Object, mockLessonRepo.Object);

        // Act
        await service.PublishAsync(courseId);

        // Assert
        Assert.Equal(CourseStatus.Published, course.Status);
        mockCourseRepo.Verify(r => r.UpdateAsync(It.Is<Course>(c => 
            c.Id == courseId && c.Status == CourseStatus.Published)), Times.Once);
    }

    [Fact]
    public async Task PublishCourse_WithoutLessons_ShouldFail()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            Status = CourseStatus.Draft
        };
        // No lessons added

        var mockCourseRepo = new Mock<ICourseRepository>();
        var mockLessonRepo = new Mock<ILessonRepository>();
        
        mockCourseRepo.Setup(r => r.GetByIdWithLessonsAsync(courseId))
            .ReturnsAsync(course);

        var service = new CourseService(mockCourseRepo.Object, mockLessonRepo.Object);

        // Act & Assert
        await Assert.ThrowsAsync<CourseCannotBePublishedException>(
            () => service.PublishAsync(courseId));
        
        Assert.Equal(CourseStatus.Draft, course.Status);
        mockCourseRepo.Verify(r => r.UpdateAsync(It.IsAny<Course>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCourse_ShouldBeSoftDelete()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            IsDeleted = false
        };

        var mockCourseRepo = new Mock<ICourseRepository>();
        var mockLessonRepo = new Mock<ILessonRepository>();
        
        mockCourseRepo.Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        
        mockCourseRepo.Setup(r => r.UpdateAsync(It.IsAny<Course>()))
            .Returns(Task.CompletedTask);

        var service = new CourseService(mockCourseRepo.Object, mockLessonRepo.Object);

        // Act
        await service.DeleteAsync(courseId);

        // Assert
        course.IsDeleted.Should().BeTrue();
        mockCourseRepo.Verify(r => r.UpdateAsync(It.Is<Course>(c => 
            c.Id == courseId && c.IsDeleted == true)), Times.Once);
        mockCourseRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
