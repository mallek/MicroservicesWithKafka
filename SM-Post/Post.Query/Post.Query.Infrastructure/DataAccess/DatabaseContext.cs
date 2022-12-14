using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;

namespace Post.Query.Infrastructure.DataAccess
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<PostEntity> Posts => Set<PostEntity>();
        public DbSet<CommentEntity> Comments => Set<CommentEntity>();

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<PostEntity>()
        //         .HasMany(p => p.Comments)
        //         .WithOne(c => c.Post)
        //         .HasForeignKey(c => c.PostId);

        //     modelBuilder.Entity<CommentEntity>()
        //         .HasOne(c => c.Post)
        //         .WithMany(p => p.Comments)
        //         .HasForeignKey(c => c.PostId);
        // }
    }
}