using Xunit;
using Moq;
using FluentAssertions;
using Riwi_Courses_Assessment_Backend.Application.DTOs;
using Riwi_Courses_Assessment_Backend.Application.Services;
using Riwi_Courses_Assessment_Backend.Domain.Entities;
using Riwi_Courses_Assessment_Backend.Domain.Exceptions;
using Riwi_Courses_Assessment_Backend.Domain.Interfaces;

namespace Riwi_Courses_Assessment_Backend.Tests.Services;

public class LessonServiceTests
{
    [Fact]
    public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            IsDeleted = false
        };

        var createDto = new CreateLessonDto
        {
            CourseId = courseId,
            Title = "Test Lesson",
            Order = 1
        };

        var mockLessonRepo = new Mock<ILessonRepository>();
        var mockCourseRepo = new Mock<ICourseRepository>();
        
        mockCourseRepo.Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        
        mockLessonRepo.Setup(r => r.HasDuplicateOrderAsync(courseId, 1, null))
            .ReturnsAsync(false);
        
        mockLessonRepo.Setup(r => r.AddAsync(It.IsAny<Lesson>()))
            .ReturnsAsync((Lesson l) => l);

        var service = new LessonService(mockLessonRepo.Object, mockCourseRepo.Object);

        // Act
        var result = await service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Lesson", result.Title);
        Assert.Equal(1, result.Order);
        Assert.Equal(courseId, result.CourseId);
        mockLessonRepo.Verify(r => r.AddAsync(It.Is<Lesson>(l => 
            l.Title == "Test Lesson" && l.Order == 1)), Times.Once);
    }

    [Fact]
    public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course
        {
            Id = courseId,
            Title = "Test Course",
            IsDeleted = false
        };

        var createDto = new CreateLessonDto
        {
            CourseId = courseId,
            Title = "Test Lesson",
            Order = 1
        };

        var mockLessonRepo = new Mock<ILessonRepository>();
        var mockCourseRepo = new Mock<ICourseRepository>();
        
        mockCourseRepo.Setup(r => r.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        
        // Simulate that order 1 already exists
        mockLessonRepo.Setup(r => r.HasDuplicateOrderAsync(courseId, 1, null))
            .ReturnsAsync(true);

        var service = new LessonService(mockLessonRepo.Object, mockCourseRepo.Object);

        // Act & Assert
        var act = async () => await service.CreateAsync(createDto);
        await act.Should().ThrowAsync<DuplicateLessonOrderException>();
        
        mockLessonRepo.Verify(r => r.AddAsync(It.IsAny<Lesson>()), Times.Never);
    }
}
