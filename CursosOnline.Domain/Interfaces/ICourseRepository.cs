using System.Threading.Tasks;
using CursosOnline.Domain.Entities;

namespace CursosOnline.Domain.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course?> GetCourseWithLessonsAsync(Guid id);
    }
}
