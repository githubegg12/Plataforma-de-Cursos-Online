using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CursosOnline.Application.DTOs.Course;
using CursosOnline.Application.Services;
using CursosOnline.Domain.Entities;
using CursosOnline.Domain.Interfaces;
using Moq;
using Xunit;

namespace CursosOnline.Tests
{
    public class CourseServiceTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<IGenericRepository<Lesson>> _lessonRepositoryMock;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _lessonRepositoryMock = new Mock<IGenericRepository<Lesson>>();
            _courseService = new CourseService(_courseRepositoryMock.Object, _lessonRepositoryMock.Object);
        }

        [Fact]
        public async Task PublishCourse_WithLessons_ShouldSucceed()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Title = "Test Course",
                Status = CourseStatus.Draft,
                Lessons = new List<Lesson> { new Lesson { Id = Guid.NewGuid(), Title = "Lesson 1" } }
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseWithLessonsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            await _courseService.PublishCourseAsync(courseId);

            // Assert
            Assert.Equal(CourseStatus.Published, course.Status);
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(course), Times.Once);
        }

        [Fact]
        public async Task PublishCourse_WithoutLessons_ShouldFail()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Title = "Test Course",
                Status = CourseStatus.Draft,
                Lessons = new List<Lesson>() // No lessons
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseWithLessonsAsync(courseId))
                .ReturnsAsync(course);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _courseService.PublishCourseAsync(courseId));
            Assert.Equal("Cannot publish a course with no active lessons.", exception.Message);
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task DeleteCourse_ShouldBeSoftDelete()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Title = "Test Course",
                Status = CourseStatus.Draft,
                Lessons = new List<Lesson>() // No lessons, safe to delete
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseWithLessonsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            await _courseService.DeleteCourseAsync(courseId);

            // Assert
            _courseRepositoryMock.Verify(repo => repo.DeleteAsync(courseId), Times.Once);
        }
        [Fact]
        public async Task UpdateCourse_WithMissingStatus_ShouldUpdateTitleAndKeepStatus()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Title = "Old Title",
                Status = CourseStatus.Published
            };

            var updateDto = new UpdateCourseDto
            {
                Title = "New Title",
                Status = "" // Simulating missing status in JSON
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseWithLessonsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            await _courseService.UpdateCourseAsync(courseId, updateDto);

            // Assert
            Assert.Equal("New Title", course.Title);
            Assert.Equal(CourseStatus.Published, course.Status); // Status should remain unchanged
            _courseRepositoryMock.Verify(repo => repo.UpdateAsync(course), Times.Once);
        }

        [Fact]
        public async Task SearchCourses_ShouldReturnUpdatedAt_WhenCourseIsUpdated()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var updatedAt = DateTime.UtcNow;
            var courses = new List<Course>
            {
                new Course
                {
                    Id = courseId,
                    Title = "Test Course",
                    Status = CourseStatus.Published,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = updatedAt
                }
            };

            _courseRepositoryMock.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(courses);

            // Act
            var result = await _courseService.SearchCoursesAsync(null, null, 1, 10);

            // Assert
            var courseDto = result.Items.First();
            Assert.Equal(updatedAt, courseDto.UpdatedAt);
        }

        [Fact]
        public async Task DeleteCourse_WithLessons_ShouldFail()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Title = "Test Course",
                Status = CourseStatus.Draft,
                Lessons = new List<Lesson> { new Lesson { Id = Guid.NewGuid(), Title = "Lesson 1" } }
            };

            _courseRepositoryMock.Setup(repo => repo.GetCourseWithLessonsAsync(courseId))
                .ReturnsAsync(course);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _courseService.DeleteCourseAsync(courseId));
            Assert.Equal("Cannot delete a course that has lessons. Please delete all lessons first.", exception.Message);
            _courseRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        }
    }
}
