using MD_Crud_Fashion.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MD_Crud_Fashion.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<OrderEntry> OrderEntries { get; set; }
        public virtual DbSet<Product> Products { get; set; }
    }
}


