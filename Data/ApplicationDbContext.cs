using LibaryManagement.Identity;
using LibaryManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibaryManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
           DbContextOptions<ApplicationDbContext> options)
           : base(options)
        {
        }

        

        public DbSet<Author> Authors { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Book> Books { get; set; }

        public DbSet<Borrowing> Borrowings { get; set; }

        public DbSet<Purchase> Purchases { get; set; }
        

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);


            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId);

           

            modelBuilder.Entity<Borrowing>()
                .HasOne(br => br.Book)
                .WithMany(b => b.Borrowings)
                .HasForeignKey(br => br.BookId);


            modelBuilder.Entity<Purchase>()
    .HasOne(p => p.User)
    .WithMany()
    .HasForeignKey(p => p.UserId);
        }
    }
}
