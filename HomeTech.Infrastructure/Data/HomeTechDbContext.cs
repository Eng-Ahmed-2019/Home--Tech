using HomeTech.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HomeTech.Infrastructure.Data
{
    public class HomeTechDbContext :
        IdentityDbContext<ApplicationUser>
    {
        public HomeTechDbContext(DbContextOptions<HomeTechDbContext> options)
            : base(options)
        { }
    }
}