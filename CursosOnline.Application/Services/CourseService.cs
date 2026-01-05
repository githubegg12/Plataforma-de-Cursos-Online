using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Course;
using CursosOnline.Application.Interfaces;
using CursosOnline.Domain.Entities;
using CursosOnline.Domain.Interfaces;

namespace CursosOnline.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly IGenericRepository<Lesson> _lessonRepository;

        public CourseService(ICourseRepository courseRepository, IGenericRepository<Lesson> lessonRepository)
        {
            _courseRepository = courseRepository;
            _lessonRepository = lessonRepository;
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<CourseDto?> GetCourseByIdAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseWithLessonsAsync(id);
            if (course == null) return null;

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Status = course.Status.ToString(),
                CreatedAt = course.CreatedAt,
                UpdatedAt = course.UpdatedAt,
                Lessons = course.Lessons.Select(l => new LessonDto
                {
                    Id = l.Id,
                    CourseId = l.CourseId,
                    Title = l.Title,
                    Order = l.Order,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                }).ToList()
            };
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            var course = new Course
            {
                Title = createCourseDto.Title,
                Status = CourseStatus.Draft
            };

            await _courseRepository.AddAsync(course);

            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Status = course.Status.ToString(),
                CreatedAt = course.CreatedAt
            };
        }

        public async Task UpdateCourseAsync(Guid id, UpdateCourseDto updateCourseDto)
        {
            var course = await _courseRepository.GetCourseWithLessonsAsync(id);
            if (course == null) throw new Exception("Course not found");

            if (Enum.TryParse<CourseStatus>(updateCourseDto.Status, out var status))
            {
                if (status == CourseStatus.Published && !course.Lessons.Any())
                {
                    throw new Exception("Cannot publish a course with no active lessons.");
                }
                course.Status = status;
            }

            if (!string.IsNullOrWhiteSpace(updateCourseDto.Title))
            {
                course.Title = updateCourseDto.Title;
            }
            
            await _courseRepository.UpdateAsync(course);
        }

        public async Task DeleteCourseAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseWithLessonsAsync(id);
            if (course == null) return;

            if (course.Lessons.Any())
            {
                throw new Exception("Cannot delete a course that has lessons. Please delete all lessons first.");
            }

            await _courseRepository.DeleteAsync(id);
        }

        public async Task PublishCourseAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseWithLessonsAsync(id);
            if (course == null) throw new Exception("Course not found");

            if (!course.Lessons.Any())
            {
                throw new Exception("Cannot publish a course with no active lessons.");
            }

            course.Status = CourseStatus.Published;
            await _courseRepository.UpdateAsync(course);
        }

        public async Task UnpublishCourseAsync(Guid id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null) throw new Exception("Course not found");

            course.Status = CourseStatus.Draft;
            await _courseRepository.UpdateAsync(course);
        }

        public async Task<CursosOnline.Application.DTOs.Common.PagedResult<CourseDto>> SearchCoursesAsync(string? q, string? status, int page, int pageSize)
        {
            var courses = await _courseRepository.GetAllAsync(); // Ideally this should be done in Repository with IQueryable for performance
            
            // Filter
            if (!string.IsNullOrEmpty(q))
            {
                courses = courses.Where(c => c.Title.Contains(q, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<CourseStatus>(status, true, out var statusEnum))
            {
                courses = courses.Where(c => c.Status == statusEnum);
            }

            var totalCount = courses.Count();
            var items = courses
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToList();

            return new CursosOnline.Application.DTOs.Common.PagedResult<CourseDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<CursosOnline.Application.DTOs.Course.CourseSummaryDto?> GetCourseSummaryAsync(Guid id)
        {
            var course = await _courseRepository.GetCourseWithLessonsAsync(id);
            if (course == null) return null;

            return new CursosOnline.Application.DTOs.Course.CourseSummaryDto
            {
                Id = course.Id,
                Title = course.Title,
                Status = course.Status.ToString(),
                TotalLessons = course.Lessons.Count(),
                UpdatedAt = course.UpdatedAt
            };
        }
    }
}
