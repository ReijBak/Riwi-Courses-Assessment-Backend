using Microsoft.AspNetCore.Mvc;
}
    }
        }
            return BadRequest(new { message = ex.Message });
            _logger.LogWarning(ex, "Course already in draft: {CourseId}", id);
        {
        catch (CourseAlreadyDraftException ex)
        }
            return NotFound(new { message = ex.Message });
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
        {
        catch (CourseNotFoundException ex)
        }
            return Ok(new { message = "Course unpublished successfully" });
            await _courseService.UnpublishAsync(id);
        {
        try
    {
    public async Task<IActionResult> Unpublish(Guid id)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPatch("{id:guid}/unpublish")]
    /// </summary>
    /// Unpublish a course
    /// <summary>

    }
        }
            return BadRequest(new { message = ex.Message });
            _logger.LogWarning(ex, "Course already published: {CourseId}", id);
        {
        catch (CourseAlreadyPublishedException ex)
        }
            return BadRequest(new { message = ex.Message });
            _logger.LogWarning(ex, "Cannot publish course: {CourseId}", id);
        {
        catch (CourseCannotBePublishedException ex)
        }
            return NotFound(new { message = ex.Message });
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
        {
        catch (CourseNotFoundException ex)
        }
            return Ok(new { message = "Course published successfully" });
            await _courseService.PublishAsync(id);
        {
        try
    {
    public async Task<IActionResult> Publish(Guid id)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPatch("{id:guid}/publish")]
    /// </summary>
    /// Publish a course
    /// <summary>

    }
        }
            return NotFound(new { message = ex.Message });
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
        {
        catch (CourseNotFoundException ex)
        }
            return NoContent();
            await _courseService.DeleteAsync(id);
        {
        try
    {
    public async Task<IActionResult> Delete(Guid id)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [HttpDelete("{id:guid}")]
    /// </summary>
    /// Delete a course (soft delete)
    /// <summary>

    }
        }
            return NotFound(new { message = ex.Message });
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
        {
        catch (CourseNotFoundException ex)
        }
            return Ok(course);
            var course = await _courseService.UpdateAsync(id, dto);
        {
        try

            return BadRequest(ModelState);
        if (!ModelState.IsValid)
    {
    public async Task<ActionResult<CourseDto>> Update(Guid id, [FromBody] UpdateCourseDto dto)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [HttpPut("{id:guid}")]
    /// </summary>
    /// Update a course
    /// <summary>

    }
        return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        var course = await _courseService.CreateAsync(dto);

            return BadRequest(ModelState);
        if (!ModelState.IsValid)
    {
    public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto dto)
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
    [HttpPost]
    /// </summary>
    /// Create a new course
    /// <summary>

    }
        }
            return NotFound(new { message = ex.Message });
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
        {
        catch (CourseNotFoundException ex)
        }
            return Ok(summary);
            var summary = await _courseService.GetSummaryAsync(id);
        {
        try
    {
    public async Task<ActionResult<CourseSummaryDto>> GetSummary(Guid id)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CourseSummaryDto), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}/summary")]
    /// </summary>
    /// Get course summary with lesson count
    /// <summary>

    }
        return Ok(result);
        var result = await _courseService.SearchAsync(q, status, page, pageSize);

        if (pageSize > 100) pageSize = 100;
        if (pageSize < 1) pageSize = 10;
        if (page < 1) page = 1;
    {
        [FromQuery] int pageSize = 10)
        [FromQuery] int page = 1,
        [FromQuery] string? status = null,
        [FromQuery] string? q = null,
    public async Task<ActionResult<CourseSearchResultDto>> Search(
    [ProducesResponseType(typeof(CourseSearchResultDto), StatusCodes.Status200OK)]
    [HttpGet("search")]
    /// </summary>
    /// Search courses with pagination and filters
    /// <summary>

    }
        }
            return NotFound(new { message = ex.Message });
            _logger.LogWarning(ex, "Course not found: {CourseId}", id);
        {
        catch (CourseNotFoundException ex)
        }
            return Ok(course);
            var course = await _courseService.GetByIdAsync(id);
        {
        try
    {
    public async Task<ActionResult<CourseDto>> GetById(Guid id)
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
    [HttpGet("{id:guid}")]
    /// </summary>
    /// Get a course by ID
    /// <summary>

    }
        _logger = logger;
        _courseService = courseService;
    {
    public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)

    private readonly ILogger<CoursesController> _logger;
    private readonly ICourseService _courseService;
{
public class CoursesController : ControllerBase
[Route("api/[controller]")]
[ApiController]

namespace Riwi_Courses_Assessment_Backend.WebApi.Controllers;

using Riwi_Courses_Assessment_Backend.Domain.Exceptions;
using Riwi_Courses_Assessment_Backend.Application.Interfaces;
using Riwi_Courses_Assessment_Backend.Application.DTOs;

