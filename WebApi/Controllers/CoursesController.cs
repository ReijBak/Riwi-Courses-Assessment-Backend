using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Riwi.CoursesAssessment.Application.DTOs;
using Riwi.CoursesAssessment.Application.Interfaces;
using Riwi.CoursesAssessment.Domain.Constants;
using Riwi.CoursesAssessment.Domain.Exceptions;

namespace Riwi.CoursesAssessment.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere autenticación para todos los endpoints
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;
    private readonly ILogger<CoursesController> _logger;

    public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
    {
        _courseService = courseService;
        _logger = logger;
    }

    /// <summary>
    /// Get a course by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> GetById(Guid id)
    {
        try
        {
            var course = await _courseService.GetByIdAsync(id);
            return Ok(course);
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Search courses with pagination and filters
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(CourseSearchResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CourseSearchResultDto>> Search(
        [FromQuery] string? q = null,
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var result = await _courseService.SearchAsync(q, status, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get course summary with lesson count
    /// </summary>
    [HttpGet("{id:guid}/summary")]
    [ProducesResponseType(typeof(CourseSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseSummaryDto>> GetSummary(Guid id)
    {
        try
        {
            var summary = await _courseService.GetSummaryAsync(id);
            return Ok(summary);
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Create a new course
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = await _courseService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
    }

    /// <summary>
    /// Update a course
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CourseDto>> Update(Guid id, [FromBody] UpdateCourseDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var course = await _courseService.UpdateAsync(id, dto);
            return Ok(course);
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a course (soft delete)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _courseService.DeleteAsync(id);
            return NoContent();
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Hard delete a course (Admin only) - eliminación física
    /// </summary>
    [HttpDelete("{id:guid}/hard")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> HardDelete(Guid id)
    {
        try
        {
            await _courseService.HardDeleteAsync(id);
            return NoContent();
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Publish a course
    /// </summary>
    [HttpPatch("{id:guid}/publish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Publish(Guid id)
    {
        try
        {
            await _courseService.PublishAsync(id);
            return Ok(new { message = "Course published successfully" });
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (CourseCannotBePublishedException ex)
        {
            _logger.LogWarning(ex, "Cannot publish course: {CourseId}", id);
            return BadRequest(new { message = ex.Message });
        }
        catch (CourseAlreadyPublishedException ex)
        {
            _logger.LogWarning(ex, "Course already published: {CourseId}", id);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Unpublish a course
    /// </summary>
    [HttpPatch("{id:guid}/unpublish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Unpublish(Guid id)
    {
        try
        {
            await _courseService.UnpublishAsync(id);
            return Ok(new { message = "Course unpublished successfully" });
        }
        catch (CourseNotFoundException ex)
        {
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
            return NotFound(new { message = ex.Message });
        }
        catch (CourseAlreadyDraftException ex)
        {
            _logger.LogWarning(ex, "Course already in draft: {CourseId}", id);
            return BadRequest(new { message = ex.Message });
        }
    }
}

