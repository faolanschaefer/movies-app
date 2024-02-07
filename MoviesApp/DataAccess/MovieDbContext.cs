﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Movies.Entities;
using MoviesApp.Entities;

namespace MoviesApp.DataAccess
{
    // Define our DB context class that inherits from EF Core's base DbContext:
    public class MovieDbContext : IdentityDbContext<User>
    {
        public static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            UserManager<User> userManager =
                serviceProvider.GetRequiredService<UserManager<User>>();
            RoleManager<IdentityRole> roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string username = "admin";
            string password = "Sesame123#";
            string roleName = "Admin";

            // if role doesn't exist, create it
            if (await roleManager.FindByNameAsync(roleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
            // if username doesn't exist, create it and add it to role
            if (await userManager.FindByNameAsync(username) == null)
            {
                User user = new User { UserName = username };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, roleName);
                }
            }
        }

        // Define a constructor that accepts a context options object that
        // is simply passed to the base class:
        public MovieDbContext(DbContextOptions<MovieDbContext> options)
            : base(options)
        {
        }

        // Define a property for accessing/querying all movies from the DB:
        public DbSet<Movie> Movies { get; set; }

        public DbSet<Genre> Genres { get; set; }

        public DbSet<Actor> Actors { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<Casting> Castings { get; set; }

        // override the protected OnModelCreating method to seed the DB w some movies
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // seed for the identity base class
            base.OnModelCreating(modelBuilder);

            // setup the composite key in Castings:
            modelBuilder.Entity<Casting>().HasKey(c => new { c.ActorId, c.MovieId });

            // setup 1-to-many between movie & casting:
            modelBuilder.Entity<Casting>().HasOne(c => c.Movie).WithMany(m => m.Castings).HasForeignKey(c => c.MovieId);

            // setup 1-to-many between actor & casting:
            modelBuilder.Entity<Casting>().HasOne(c => c.Actor).WithMany(m => m.Castings).HasForeignKey(c => c.ActorId);

            // seeding genres:
            modelBuilder.Entity<Genre>().HasData(
                new Genre() { GenreId = "A", Name = "Action" },
                new Genre() { GenreId = "C", Name = "Comedy" },
                new Genre() { GenreId = "D", Name = "Drama" },
                new Genre() { GenreId = "H", Name = "Horror" },
                new Genre() { GenreId = "M", Name = "Musical" },
                new Genre() { GenreId = "R", Name = "RomCom" },
                new Genre() { GenreId = "S", Name = "SciFi" }
            );

            // seed actors:
            modelBuilder.Entity<Actor>().HasData(
                new Actor() { ActorId = 1, FirstName = "Humphrey", LastName = "Bogart" },
                new Actor() { ActorId = 2, FirstName = "Ingrid", LastName = "Bergman" },
                new Actor() { ActorId = 3, FirstName = "Keanu", LastName = "Reeves" },
                new Actor() { ActorId = 4, FirstName = "Carrie-Anne", LastName = "Moss" },
                new Actor() { ActorId = 5, FirstName = "John", LastName = "Travolta" },
                new Actor() { ActorId = 6, FirstName = "Uma", LastName = "Thurman" }
            );

            // seeding movies:
            modelBuilder.Entity<Movie>().HasData(
                new Movie() { 
                    MovieId = 1,
                    Name = "Casablanca",
                    Year = 1942,
                    GenreId = "D"
                },
                new Movie() {
                    MovieId = 2,
                    Name = "The Matrix",
                    Year = 1998,
                    GenreId = "S"
                },
                new Movie() {
                    MovieId = 3,
                    Name = "Pulp Fiction",
                    Year = 1996,
                    GenreId = "A"
                }
            );

            modelBuilder.Entity<Casting>().HasData(
                new Casting() { MovieId = 1, ActorId = 1, Role = "Rick Blaine" },
                new Casting() { MovieId = 1, ActorId = 2, Role = "Ilsa Lund" },
                new Casting() { MovieId = 2, ActorId = 3, Role = "Neo" },
                new Casting() { MovieId = 2, ActorId = 4, Role = "Trinity" },
                new Casting() { MovieId = 3, ActorId = 5, Role = "Vincet Vega" },
                new Casting() { MovieId = 3, ActorId = 6, Role = "Mia Wallace" }
            );

            modelBuilder.Entity<Review>().HasData(
                new Review()
                {
                    ReviewId = 1,
                    Rating = 5,
                    Comments = "A classic!",
                    MovieId = 1
                },
                new Review()
                {
                    ReviewId = 2,
                    Rating = 3,
                    Comments = "They should have gotten together in the end!",
                    MovieId = 1
                },
                new Review()
                {
                    ReviewId = 3,
                    Rating = 3,
                    Comments = "Too slow of a pace",
                    MovieId = 1
                },
                new Review()
                {
                    ReviewId = 4,
                    Rating = 4,
                    Comments = "Based on Descarte's \"brain in a vat\" thought experiment",
                    MovieId = 2
                },
                new Review()
                {
                    ReviewId = 5,
                    Rating = 3,
                    Comments = "Very philosophical",
                    MovieId = 2
                },
                new Review()
                {
                    ReviewId = 6,
                    Rating = 5,
                    Comments = "Very violent but also very funny and clever",
                    MovieId = 3
                }
            );
        }
    }
}
