# Seed de datos iniciales (Book + User)

Este proyecto ya quedó **preparado** para ejecutar seed controlado por configuración.

## ¿Qué ya está creado?

- `Seeding/InitialDataSeeder.cs`
  - Inserta (si no existen):
	- 1 rol: `User`
	- 1 library: `Seed Central Library`
	- 1 branch: `Seed Main Branch`
	- 100 usuarios con propiedades completas de Identity y datos personales válidos.
	- 100 libros con título, autor, ISBN, año, disponibilidad y sucursal.
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

### Identificadores de los datos

- El primer usuario conserva el email `seed.user@thelibrary.local`; los restantes usan el formato
  `seed.userNNN@thelibrary.local`.
- Los libros usan ISBN en el formato `SEED-BOOK-NNNN`.
- Estos identificadores permiten ejecutar el seed varias veces sin duplicar registros.

---

## Recomendaciones importantes

- El seeder está hecho para ser **idempotente** (no duplica si ya existe).
- Si falla la creación de usuarios, revisar que el password cumpla las políticas de Identity.
- `DefaultUserPassword` se usa para los 100 usuarios seed. Para ambientes reales, debe configurarse
  mediante secretos o variables de entorno y no reutilizar la contraseña de desarrollo.

---

## Ejecución alternativa (manual)

Si preferís, podés comentar temporalmente la llamada en `Program.cs` y crear un endpoint o comando interno que invoque `InitialDataSeeder.SeedAsync(...)` bajo demanda.

Para entorno de desarrollo, el enfoque actual por configuración (`SeedData:Enabled`) suele ser suficiente.
