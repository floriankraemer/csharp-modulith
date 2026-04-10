using Microsoft.EntityFrameworkCore;

namespace CSharpModulith.Host;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
