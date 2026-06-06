using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;

namespace Tests.Common
{
    public abstract class BaseTestFixture
    {
        protected DbContextOptions<AppDbContext> CreateInMemoryOptions(string dbName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }
    }
}
