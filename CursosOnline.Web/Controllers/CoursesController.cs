using System;
using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Course;
using CursosOnline.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursosOnline.Web.Controllers
{
    [Route("Courses")]
    public class CoursesController : Controller
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet("Edit/{id}")]
        public IActionResult Edit(Guid id)
        {
            return View();
        }

        [HttpGet("Details/{id}")]
        public IActionResult Details(Guid id)
        {
            return View();
        }

        [Authorize]
        [HttpGet("/api/courses")]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [Authorize]
        [HttpGet("/api/courses/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [Authorize]
        [HttpPost("/api/courses")]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto createCourseDto)
        {
            var course = await _courseService.CreateCourseAsync(createCourseDto);
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }

        [Authorize]
        [HttpPut("/api/courses/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            try
            {
                await _courseService.UpdateCourseAsync(id, updateCourseDto);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize]
        [HttpDelete("/api/courses/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPatch("/api/courses/{id}/publish")]
        public async Task<IActionResult> Publish(Guid id)
        {
            try
            {
                await _courseService.PublishCourseAsync(id);
                return Ok(new { message = "Course published successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPatch("/api/courses/{id}/unpublish")]
        public async Task<IActionResult> Unpublish(Guid id)
        {
            try
            {
                await _courseService.UnpublishCourseAsync(id);
                return Ok(new { message = "Course unpublished successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("/api/courses/search")]
        public async Task<IActionResult> Search([FromQuery] string? q, [FromQuery] string? status, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _courseService.SearchCoursesAsync(q, status, page, pageSize);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("/api/courses/{id}/summary")]
        public async Task<IActionResult> Summary(Guid id)
        {
            var summary = await _courseService.GetCourseSummaryAsync(id);
            if (summary == null) return NotFound();
            return Ok(summary);
        }
    }
}
