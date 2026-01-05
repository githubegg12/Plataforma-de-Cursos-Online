using System;
using System.Collections.Generic;

namespace CursosOnline.Domain.Entities
{
    public enum CourseStatus
    {
        Draft,
        Published
    }

    public class Course : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
        
        // Navigation property
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
