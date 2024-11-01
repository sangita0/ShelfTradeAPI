using Microsoft.EntityFrameworkCore;
using ServicesForShelfSwap.Models;
using System.Collections.Generic;

namespace ServicesForShelfSwap.Data
{
    public class BookExchangeContext : DbContext
    {
        public BookExchangeContext(DbContextOptions<BookExchangeContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().Property(b => b.BookId).IsRequired().ValueGeneratedOnAdd();
            modelBuilder.Entity<Book>().Property(b => b.CreatedAt).IsRequired(false);
            modelBuilder.Entity<Book>().Property(b => b.UpdatedAt).IsRequired(false);
        }
    }
}
