using System;
using System.Threading.Tasks;
using CursosOnline.Domain.Entities;
using CursosOnline.Domain.Interfaces;
using CursosOnline.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CursosOnline.Infrastructure.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Course?> GetCourseWithLessonsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
