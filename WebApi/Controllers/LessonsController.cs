using Microsoft.AspNetCore.Mvc;
using Riwi_Courses_Assessment_Backend.Application.DTOs;
using Riwi_Courses_Assessment_Backend.Application.Interfaces;
using Riwi_Courses_Assessment_Backend.Domain.Exceptions;

namespace Riwi_Courses_Assessment_Backend.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;
    private readonly ILogger<LessonsController> _logger;

    public LessonsController(ILessonService lessonService, ILogger<LessonsController> logger)
    {
        _lessonService = lessonService;
        _logger = logger;
    }

    /// <summary>
    /// Get a lesson by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(LessonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LessonDto>> GetById(Guid id)
    {
        try
        {
            var lesson = await _lessonService.GetByIdAsync(id);
            return Ok(lesson);
        }
        catch (LessonNotFoundException ex)
        {
            _logger.LogWarning(ex, "Lesson not found: {LessonId}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all lessons for a course (ordered by Order)
    /// </summary>
    [HttpGet("course/{courseId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<LessonDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LessonDto>>> GetByCourseId(Guid courseId)
    {
        try
        {
            var lessons = await _lessonService.GetByCourseIdAsync(courseId);
            return Ok(lessons);
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", courseId);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new lesson
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LessonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LessonDto>> Create([FromBody] CreateLessonDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var lesson = await _lessonService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", dto.CourseId);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidLessonOrderException ex)
        {
            _logger.LogWarning(ex, "Invalid lesson order: {Order}", dto.Order);
            return BadRequest(new { message = ex.Message });
        }
        catch (DuplicateLessonOrderException ex)
        {
            _logger.LogWarning(ex, "Duplicate lesson order: {Order} for course {CourseId}", dto.Order, dto.CourseId);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Update a lesson
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(LessonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LessonDto>> Update(Guid id, [FromBody] UpdateLessonDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var lesson = await _lessonService.UpdateAsync(id, dto);
            return Ok(lesson);
        }
        catch (LessonNotFoundException ex)
        {
            _logger.LogWarning(ex, "Lesson not found: {LessonId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidLessonOrderException ex)
        {
            _logger.LogWarning(ex, "Invalid lesson order: {Order}", dto.Order);
            return BadRequest(new { message = ex.Message });
        }
        catch (DuplicateLessonOrderException ex)
        {
            _logger.LogWarning(ex, "Duplicate lesson order");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a lesson (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _lessonService.DeleteAsync(id);
            return NoContent();
        }
        catch (LessonNotFoundException ex)
        {
            _logger.LogWarning(ex, "Lesson not found: {LessonId}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Reorder a lesson (move up/down)
    /// </summary>
    [HttpPatch("{id:guid}/reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] int newOrder)
    {
        try
        {
            await _lessonService.ReorderAsync(id, newOrder);
            return Ok(new { message = "Lesson reordered successfully" });
        }
        catch (LessonNotFoundException ex)
        {
            _logger.LogWarning(ex, "Lesson not found: {LessonId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidLessonOrderException ex)
        {
            _logger.LogWarning(ex, "Invalid order: {Order}", newOrder);
            return BadRequest(new { message = ex.Message });
        }
        catch (DuplicateLessonOrderException ex)
        {
            _logger.LogWarning(ex, "Duplicate order");
            return BadRequest(new { message = ex.Message });
        }
    }
}

