# Seed de datos iniciales (Book + User)

Este proyecto ya quedó **preparado** para ejecutar seed controlado por configuración.

## ¿Qué ya está creado?

- `Seeding/InitialDataSeeder.cs`
  - Inserta (si no existen):
	- 1 rol: `User`
	- 1 library: `Seed Central Library`
	- 1 branch: `Seed Main Branch`
	- 1 usuario: `seed.user@thelibrary.local`
	- 1 libro: ISBN `SEED-BOOK-0001`
- `Seeding/SeedApplicationBuilderExtensions.cs`
  - Expone `UseInitialDataSeedAsync()` para correr seed desde `Program.cs`.
- `Program.cs`
  - Ya tiene `await app.UseInitialDataSeedAsync();`.
- `appsettings.json`
  - Sección `SeedData` para habilitar/deshabilitar seed y definir password del usuario seed.

---

## ¿Dónde se ejecuta el seed?

Se ejecuta durante el arranque de la API, en `Program.cs`, al llamar:

- `await app.UseInitialDataSeedAsync();`

Pero **solo corre si** en configuración está:

- `"SeedData": { "Enabled": true, ... }`

---

## ¿Cómo usarlo?

1. En `TheLibrary/appsettings.json`, cambiar:

```json
"SeedData": {
  "Enabled": true,
  "DefaultUserPassword": "Seed1234"
}
```

2. Ejecutar la API normalmente.
3. Verificar en logs que el seed corrió.
4. Opcional: volver `Enabled` a `false` para no ejecutar en cada inicio.

---

## ¿Cómo agregar más datos manualmente?

Editar archivo:

- `TheLibrary/Seeding/InitialDataSeeder.cs`

### Para agregar más usuarios

Dentro de `SeedAsync`, repetir el patrón de búsqueda + creación:

1. Buscar por email (`FindByEmailAsync`).
2. Si no existe, crear con `CreateAsync`.
3. Asignar rol con `AddToRoleAsync`.

### Para agregar más libros

Dentro de `SeedAsync`, repetir patrón por ISBN:

1. Buscar por ISBN en `dbContext.Books`.
2. Si no existe, crear `Book` y guardar.

---

## Recomendaciones importantes

- El seeder está hecho para ser **idempotente** (no duplica si ya existe).
- Si falla creación de usuario, revisar que el password cumpla políticas de Identity.
- Para ambientes reales, preferí password de seed en secretos/variables de entorno.

---

## Ejecución alternativa (manual)

Si preferís, podés comentar temporalmente la llamada en `Program.cs` y crear un endpoint o comando interno que invoque `InitialDataSeeder.SeedAsync(...)` bajo demanda.

Para entorno de desarrollo, el enfoque actual por configuración (`SeedData:Enabled`) suele ser suficiente.
