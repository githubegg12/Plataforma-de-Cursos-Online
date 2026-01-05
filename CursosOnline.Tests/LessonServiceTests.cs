using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Course;
using CursosOnline.Application.Services;
using CursosOnline.Domain.Entities;
using CursosOnline.Domain.Interfaces;
using Moq;
using Xunit;

namespace CursosOnline.Tests
{
    public class LessonServiceTests
    {
        private readonly Mock<IGenericRepository<Lesson>> _lessonRepositoryMock;
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly LessonService _lessonService;

        public LessonServiceTests()
        {
            _lessonRepositoryMock = new Mock<IGenericRepository<Lesson>>();
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _lessonService = new LessonService(_lessonRepositoryMock.Object, _courseRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateLesson_WithUniqueOrder_ShouldSucceed()
        {
            // Arrange
            var createLessonDto = new CreateLessonDto
            {
                CourseId = Guid.NewGuid(),
                Title = "New Lesson",
                Order = 1
            };

            _lessonRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Lesson, bool>>>()))
                .ReturnsAsync(new List<Lesson>()); // No existing lessons with this order

            // Act
            var result = await _lessonService.CreateLessonAsync(createLessonDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createLessonDto.Title, result.Title);
            Assert.Equal(createLessonDto.Order, result.Order);
            _lessonRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Lesson>()), Times.Once);
        }

        [Fact]
        public async Task CreateLesson_WithDuplicateOrder_ShouldFail()
        {
            // Arrange
            var createLessonDto = new CreateLessonDto
            {
                CourseId = Guid.NewGuid(),
                Title = "Duplicate Order Lesson",
                Order = 1
            };

            var existingLesson = new Lesson
            {
                Id = Guid.NewGuid(),
                CourseId = createLessonDto.CourseId,
                Title = "Existing Lesson",
                Order = 1
            };

            _lessonRepositoryMock.Setup(repo => repo.FindAsync(It.IsAny<Expression<Func<Lesson, bool>>>()))
                .ReturnsAsync(new List<Lesson> { existingLesson });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _lessonService.CreateLessonAsync(createLessonDto));
            Assert.Equal($"A lesson with order {createLessonDto.Order} already exists in this course.", exception.Message);
            _lessonRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Lesson>()), Times.Never);
        }
    }
}
