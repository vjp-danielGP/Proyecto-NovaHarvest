# Cuestiones UT3 – NovaHarvest

## 1. De .cshtml a HTML

## 2. Tecnologías

En mi página `Surplus.cshtml`, la línea que muestra el precio del excedente es:

```cshtml
<p>Precio de excedente: @Model.SurplusPricePerKg.ToString("C") / kg</p>
```

En **PHP**, se escribiría así:

```php
<p>Precio de excedente: <?= number_format($surplusPricePerKg, 2, ',', '.') ?> € / kg</p>
```

En **ASP clásico**, se escribiría así:

```asp
<p>Precio de excedente: <%= FormatCurrency(SurplusPricePerKg) %> / kg</p>
```

La diferencia está en cómo se introduce el código dentro del HTML:

- **PHP:** utiliza `<?php ... ?>` para indicar dónde empieza y acaba el código. Con `<?= ... ?>` muestra directamente el resultado de una expresión.
- **ASP clásico:** utiliza `<% ... %>` para el código y `<%= ... %>` para mostrar un valor.
- **Razor:** utiliza `@` para indicar que comienza una expresión C# dentro del HTML. No necesita cerrarla.

### ¿Qué has ganado al pasar de los MapGet de UT2 a Razor Pages?

En la unidad 2 utilizaba `MapGet` en `Program.cs` para definir las rutas y generar el HTML mediante cadenas de texto. Esto hacía que el código HTML y la lógica estuvieran mezclados, resultando más difícil de leer y mantener.

Ahora, con Razor Pages, tengo cada página separada en archivos `.cshtml` y puedo colocar la lógica C# en su correspondiente `.cshtml.cs`. Por ejemplo, `Surplus.cshtml` se encarga de mostrar la información y `Surplus.cshtml.cs` realiza los cálculos del excedente.

### Comparación de Program.cs

En la unidad 2, el `Program.cs` contenía las rutas `MapGet` y el código necesario para construir las respuestas HTML manualmente.

Ahora, mi `Program.cs` es mucho más corto y sencillo, y se encarga principalmente de configurar la aplicación:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
var app = builder.Build();

app.UseRequestLocalization("es-ES");

app.MapStaticAssets();
app.MapRazorPages();

app.Run();
```

Con `AddRazorPages()` habilito el uso de Razor Pages y con `MapRazorPages()` se registran sus rutas. De esta forma, `Program.cs` ya no necesita contener el HTML de cada página.

## 3. Etiquetas de código

### 1. Expresión implícita

Ejemplo de mi proyecto:

```chstml
<h2>@Model.Surplus.Product</h2>
```

El HTML generado sería:

```html
<h2>Cereza Picota</h2>
```

### 2. Expresión explícita

Ejemplo de mi proyecto:

```cshtml
<p>@(Model.SurplusPricePerKg.ToString("C"))</p>
```

El HTML generado sería:

```html
<p>2,24 €</p>
```

### 3. Bloque @{ }

Ejemplo de mi proyecto:

```cshtml
@{
    ViewData["Title"] = "Excedente destacado";
}
```

El HTML generado sería:

Este bloque no genera HTML directamente. Establece el título que después utiliza `_Layout.cshtml`.

### 4. Escape @@

Ejemplo de mi proyecto:

```cshtml
<p>Síguenos en @@novaharvest</p>
```

El HTML generado sería:

```html
<p>Síguenos en @novaharvest</p>
```

### 5. Comentario @* *@

Ejemplo de mi proyecto:

```cshtml
@* Este apartado muestra los datos del excedente *@
```

El HTML generado sería:

No genera HTML. El comentario desaparece al procesar la página.

### ¿Por qué `info@novaharvest.es` no necesita `@@` y `@novaharvest` sí?

Porque Razor reconoce que `info@novaharvest.es` forma parte de una dirección de correo electrónico y no lo interpreta como una expresión C#.

En cambio, `@novaharvest` comienza directamente por `@`, por lo que Razor puede interpretarlo como una expresión. Para mostrarlo como texto normal hay que escribir `@@novaharvest`.

## 4. Sintaxis

### 1. Escribir `@Model.SurplusPrize` en lugar de `@Model.SurplusPrice`

Al cambiar el nombre de la propiedad, al ejecutar `dotnet build` aparece un error de compilación porque `SurplusPrize` no existe en el modelo `SurplusModel`.

![Error-SurplusPrize](/Proyecto-NovaHarvest/cuestiones-teoricas/capturas/ErrorEjercicio4-Model.SurplusPrize.PNG)

El error indica que `SurplusModel` no contiene una definición para `SurplusPrize`.

**¿Por qué no compila?**

Porque Razor interpreta `@Model.SurplusPrize` como una expresión C# y comprueba que esa propiedad exista. Como en mi proyecto la propiedad correcta se llama `SurplusPricePerKg`, el compilador detecta el error.

### 2. Escribir `@Model.Surplus.PricePerKg * 0.7m`

Si escribo lo siguiente:

```cshtml
<p>Precio de excedente: @Model.Surplus.PricePerKg * 0.7m</p>
```

El proyecto sí compila, pero el HTML generado muestra algo parecido a esto:

```html
<p>Precio de excedente: 3,20 € * 0.7m</p>
```

![Error-SurplusPrize](/Proyecto-NovaHarvest/cuestiones-teoricas/capturas/ErrorEjercicio4-Model.Surplus.PricePerKgx0.7m.PNG)

**¿Por qué compila aunque el resultado esté mal?**

Porque Razor interpreta como expresión C# solamente `@Model.Surplus.PricePerKg`. El resto, `* 0.7m`, lo considera texto normal y lo muestra tal cual en el navegador.

Para corregirlo, hay que incluir toda la operación dentro de una expresión explícita con `@(...)`:

```cshtml
<p>Precio de excedente: @(Model.Surplus.PricePerKg * 0.7m) / kg</p>
```