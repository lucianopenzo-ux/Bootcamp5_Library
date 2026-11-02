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
        /// <summary>
        /// Semilla de datos inicial para la aplicación
        /// </summary>
        /// <param name="services">El proveedor de servicios</param>
        /// <param name="configuration">La configuración de la aplicación</param>
        /// <param name="logger">El logger</param>
        /// <returns>Una tarea que representa la operación asincrónica</returns>
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

            var seedUserPassword = configuration["SeedData:DefaultUserPassword"] ?? "Seed1234";

            var firstNames = new[]
            {
                "Ana", "Bruno", "Carla", "Diego", "Elena",
                "Facundo", "Gabriela", "Hugo", "Irene", "Julián"
            };
            var lastNames = new[]
            {
                "Acosta", "Benítez", "Cabrera", "Domínguez", "Escobar",
                "Fernández", "Gómez", "Herrera", "Ibarra", "Juárez"
            };

            for (var index = 1; index <= 100; index++)
            {
                var email = index == 1
                    ? "seed.user@thelibrary.local"
                    : $"seed.user{index:000}@thelibrary.local";
                var firstName = index == 1 ? "Seed" : firstNames[(index - 2) % firstNames.Length];
                var lastName = index == 1 ? "User" : lastNames[(index - 2) / firstNames.Length % lastNames.Length];
                // PostgreSQL (timestamp with time zone) solo acepta DateTime en UTC
                var dateOfBirth = new DateTime(1980 + (index % 25), (index % 12) + 1, (index % 28) + 1, 0, 0, 0, DateTimeKind.Utc);
                var user = await userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        DateOfBirth = dateOfBirth,
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(user, seedUserPassword);
                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(" | ", createResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"No se pudo crear el usuario '{email}': {errors}");
                    }
                }
                else if (user.DateOfBirth == null)
                {
                    user.DateOfBirth = dateOfBirth;
                    var updateResult = await userManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                    {
                        var errors = string.Join(" | ", updateResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"No se pudo completar el usuario '{email}': {errors}");
                    }
                }

                if (!await userManager.IsInRoleAsync(user, userRole))
                {
                    var roleResult = await userManager.AddToRoleAsync(user, userRole);
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(" | ", roleResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"No se pudo asignar el rol '{userRole}' al usuario '{email}': {errors}");
                    }
                }
            }

            logger.LogInformation("Se verificaron 100 usuarios de seed.");

            var titles = new[]
            {
                "El arte de leer", "Historias del mundo", "Introducción a la ciencia",
                "Viaje por las palabras", "Principios de programación", "La ciudad y sus historias",
                "Guía de pensamiento crítico", "Atlas de conocimientos", "Cuentos para compartir",
                "Fundamentos de investigación"
            };
            var authors = new[]
            {
                "Seed System", "María López", "Carlos Méndez", "Laura Ramírez", "Jorge Navarro",
                "Sofía Torres", "Pablo Castro", "Valentina Ruiz", "Martín Silva", "Lucía Ortiz"
            };
            var booksToAdd = new List<Book>();

            for (var index = 1; index <= 100; index++)
            {
                var isbn = $"SEED-BOOK-{index:0000}";
                var existingBook = await dbContext.Books.FirstOrDefaultAsync(b => b.ISBN == isbn);
                if (existingBook != null)
                {
                    continue;
                }

                booksToAdd.Add(new Book
                {
                    Title = index == 1
                        ? "Seed Book - Introducción a TheLibrary"
                        : $"{titles[(index - 2) % titles.Length]} {index:000}",
                    Author = authors[(index - 1) % authors.Length],
                    ISBN = isbn,
                    Year = (2000 + (index % 27)).ToString(),
                    IsAvailable = index % 4 != 0,
                    LibraryBranchId = branch.Id
                });
            }

            if (booksToAdd.Count > 0)
            {
                await dbContext.Books.AddRangeAsync(booksToAdd);
                await dbContext.SaveChangesAsync();
            }

            logger.LogInformation("Se verificaron 100 libros de seed.");

            logger.LogInformation("Seed inicial finalizado.");
        }
    }
}
