using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Course;

namespace CursosOnline.Application.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto?> GetCourseByIdAsync(Guid id);
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
        Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto);
        Task DeleteCourseAsync(Guid id);
        
        // New methods
        Task PublishCourseAsync(Guid id);
        Task UnpublishCourseAsync(Guid id);
        Task<CursosOnline.Application.DTOs.Common.PagedResult<CourseDto>> SearchCoursesAsync(string? q, string? status, int page, int pageSize);
        Task<CursosOnline.Application.DTOs.Course.CourseSummaryDto?> GetCourseSummaryAsync(Guid id);
    }

    public interface ILessonService
    {
        Task<LessonDto> CreateLessonAsync(CreateLessonDto createLessonDto);
        Task UpdateLessonAsync(Guid id, UpdateLessonDto updateLessonDto);
        Task DeleteLessonAsync(Guid id);
    }
}
