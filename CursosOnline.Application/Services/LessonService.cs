using System;
using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Course;
using CursosOnline.Application.Interfaces;
using CursosOnline.Domain.Entities;
using CursosOnline.Domain.Interfaces;

namespace CursosOnline.Application.Services
{
    public class LessonService : ILessonService
    {
        private readonly IGenericRepository<Lesson> _lessonRepository;
        private readonly ICourseRepository _courseRepository;

        public LessonService(IGenericRepository<Lesson> lessonRepository, ICourseRepository courseRepository)
        {
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
        }

        public async Task<LessonDto> CreateLessonAsync(CreateLessonDto createLessonDto)
        {
            var existingLessons = await _lessonRepository.FindAsync(l => l.CourseId == createLessonDto.CourseId && l.Order == createLessonDto.Order && !l.IsDeleted);
            if (existingLessons.Any())
            {
                throw new Exception($"A lesson with order {createLessonDto.Order} already exists in this course.");
            }

            var lesson = new Lesson
            {
                CourseId = createLessonDto.CourseId,
                Title = createLessonDto.Title,
                Order = createLessonDto.Order
            };

            await _lessonRepository.AddAsync(lesson);

            return new LessonDto
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                Order = lesson.Order
            };
        }

        public async Task DeleteLessonAsync(Guid id)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null) return;

            var courseId = lesson.CourseId;
            await _lessonRepository.DeleteAsync(id);

            // Reorder remaining lessons using CourseRepository to ensure we get the correct active lessons
            var course = await _courseRepository.GetCourseWithLessonsAsync(courseId);
            if (course == null) return;

            var sortedLessons = course.Lessons.OrderBy(l => l.Order).ToList();

            for (int i = 0; i < sortedLessons.Count; i++)
            {
                sortedLessons[i].Order = i + 1;
                await _lessonRepository.UpdateAsync(sortedLessons[i]);
            }
        }

        public async Task UpdateLessonAsync(Guid id, UpdateLessonDto updateLessonDto)
        {
            var lesson = await _lessonRepository.GetByIdAsync(id);
            if (lesson == null) throw new Exception("Lesson not found");

            if (lesson.Order != updateLessonDto.Order)
            {
                var existingLessons = await _lessonRepository.FindAsync(l => l.CourseId == lesson.CourseId && l.Order == updateLessonDto.Order && !l.IsDeleted && l.Id != id);
                if (existingLessons.Any())
                {
                    throw new Exception($"A lesson with order {updateLessonDto.Order} already exists in this course.");
                }
            }

            lesson.Title = updateLessonDto.Title;
            lesson.Order = updateLessonDto.Order;

            await _lessonRepository.UpdateAsync(lesson);
        }
    }
}
