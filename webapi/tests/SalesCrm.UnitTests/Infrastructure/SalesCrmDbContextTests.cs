using Microsoft.EntityFrameworkCore;
using SalesCrm.Infrastructure.EFCore.Persistence;
using Xunit;

namespace SalesCrm.UnitTests.Infrastructure;

public class SalesCrmDbContextTests
{
    [Fact]
    public void DbContext_ShouldExposeExpectedSets()
    {
        var options = new DbContextOptionsBuilder<SalesCrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new SalesCrmDbContext(options);

        Assert.NotNull(dbContext.Accounts);
        Assert.NotNull(dbContext.Contacts);
        Assert.NotNull(dbContext.Opportunities);
    }
}
