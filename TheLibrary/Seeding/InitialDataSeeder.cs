using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using TheLibrary_DataAccessLayer.Data;
using TheLibrary_DataAccessLayer.Models;

namespace TheLibrary.Seeding
{
    public static class InitialDataSeeder
    {
        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration configuration,
            ILogger logger)
        {
            var dbContext = services.GetRequiredService<TheLibraryDbContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            await dbContext.Database.MigrateAsync();

            const string userRole = "User";
            if (!await roleManager.RoleExistsAsync(userRole))
            {
                await roleManager.CreateAsync(new IdentityRole(userRole));
                logger.LogInformation("Rol '{Role}' creado para seed.", userRole);
            }

            var library = await dbContext.Libraries.FirstOrDefaultAsync(l => l.Name == "Seed Central Library");
            if (library == null)
            {
                library = new Library
                {
                    Name = "Seed Central Library",
                    Address = "Av. Seed 123"
                };
                dbContext.Libraries.Add(library);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Library de seed creada.");
            }

            var branch = await dbContext.LibraryBranches.FirstOrDefaultAsync(b => b.Name == "Seed Main Branch");
            if (branch == null)
            {
                branch = new LibraryBranch
                {
                    Name = "Seed Main Branch",
                    Address = "Av. Seed 123 - Piso 1",
                    LibraryId = library.Id
                };
                dbContext.LibraryBranches.Add(branch);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("LibraryBranch de seed creada.");
            }

            var seedUserEmail = "seed.user@thelibrary.local";
            var seedUserPassword = configuration["SeedData:DefaultUserPassword"] ?? "Seed1234";

            var user = await userManager.FindByEmailAsync(seedUserEmail);
            if (user == null)
            {
                user = new User
                {
                    UserName = seedUserEmail,
                    Email = seedUserEmail,
                    FirstName = "Seed",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, seedUserPassword);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"No se pudo crear el usuario de seed: {errors}");
                }

                logger.LogInformation("User de seed creado.");
            }

            if (!await userManager.IsInRoleAsync(user, userRole))
            {
                await userManager.AddToRoleAsync(user, userRole);
                logger.LogInformation("Rol '{Role}' asignado al user de seed.", userRole);
            }

            var seedIsbn = "SEED-BOOK-0001";
            var existingBook = await dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == seedIsbn);
            if (existingBook == null)
            {
                dbContext.Books.Add(new Book
                {
                    Title = "Seed Book - Introducción a TheLibrary",
                    Author = "Seed System",
                    ISBN = seedIsbn,
                    Year = "2026",
                    IsAvailable = true,
                    LibraryBranchId = branch.Id
                });

                await dbContext.SaveChangesAsync();
                logger.LogInformation("Book de seed creado.");
            }

            logger.LogInformation("Seed inicial finalizado.");
        }
    }
}
