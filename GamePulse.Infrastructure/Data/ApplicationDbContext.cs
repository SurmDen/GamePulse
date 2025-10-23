using GamePulse.Core.Entites;
using GamePulse.Core.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePulse.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IPasswordHasher _passwordHasher;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPasswordHasher passwordHasher) : base (options)
        {
            _passwordHasher = passwordHasher;
        }

        public DbSet<Game> Games { get; set; }

        public DbSet<GameInfo> GameInfos { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(gameBuilder =>
            {
                gameBuilder.HasKey(x => x.Id);
                gameBuilder.HasIndex(x => x.SteamAppGameId);
                gameBuilder
                .HasMany(x => x.Tags)
                .WithMany(y => y.Games);

                gameBuilder
                .HasMany(x => x.Genres)
                .WithMany(y => y.Games);

                gameBuilder
                .HasMany(x => x.DatedGameInfo)
                .WithOne(y => y.Game)
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<User>(userBuilder =>
            {
                userBuilder.HasKey(x => x.Id);
                userBuilder.HasIndex(x => x.UserEmail);
                userBuilder.HasData(new User()
                {
                    Id = Guid.NewGuid(),
                    UserEmail = "surm@den",
                    UserName = "surman",
                    PasswordHash = _passwordHasher.GetHash("123456")
                });
            });
        }
    }
}
