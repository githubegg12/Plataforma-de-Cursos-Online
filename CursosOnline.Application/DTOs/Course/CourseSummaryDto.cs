using System;

namespace CursosOnline.Application.DTOs.Course
{
    public class CourseSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int TotalLessons { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
