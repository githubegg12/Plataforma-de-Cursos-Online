using System;
using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Course;
using CursosOnline.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CursosOnline.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLessonDto createLessonDto)
        {
            var lesson = await _lessonService.CreateLessonAsync(createLessonDto);
            return Ok(lesson);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLessonDto updateLessonDto)
        {
            try
            {
                await _lessonService.UpdateLessonAsync(id, updateLessonDto);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _lessonService.DeleteLessonAsync(id);
            return NoContent();
        }
    }
}
