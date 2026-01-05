using System;

namespace CursosOnline.Domain.Entities
{
    public class Lesson : BaseEntity
    {
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Order { get; set; }
        
        // Navigation property
        public Course? Course { get; set; }
    }
}
