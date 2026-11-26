# Cuestiones UT3 – NovaHarvest

## 1. De .cshtml a HTML

### 1. ¿Qué ocurre desde dotnet build hasta que llega el HTML al navegador?

Cuando ejecuto `dotnet build`, Razor procesa los archivos `.cshtml` del proyecto y los transforma en código C#. Ese código generado se compila junto con el resto de la aplicación.

Por ejemplo, `Surplus.cshtml` contiene HTML mezclado con expresiones de Razor, como `@Model.Surplus.Product`. Razor convierte esas partes en instrucciones C# que permiten generar el HTML.

Cuando el usuario accede a `/surplus`, ASP.NET Core ejecuta el código generado para esa página. Se combinan las partes HTML con los valores que proporciona `SurplusModel` y se crea el documento HTML final.

Por último, el servidor envía ese HTML al navegador, que lo interpreta y muestra la página.

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

## 5. Efectos en el documento

### 1. Nota del productor

En mi archivo `Surplus.cshtml` tengo esta línea:

```cshtml
<p>Nota del productor: @Html.Raw(Model.Surplus.Note)</p>
```

En pantalla, la nota aparece en negrita porque contiene la etiqueta HTML `<b>`:

```html
<b>Recoger antes de las 14:00</b>
```

Sin embargo, al consultar el código fuente de `/surplus`, se puede ver que el navegador recibe esa etiqueta HTML. Razor no la convierte en texto, sino que la interpreta como HTML gracias a `Html.Raw()`.

Como utilizo `Html.Raw()`, un usuario malintencionado podría introducir código HTML o JavaScript que se ejecutase en el navegador de otros usuarios.

### 2. Elemento de los días y botón

En mi archivo `Surplus.cshtml` tengo estos dos elementos:

```cshtml
<p class="@(Model.IsUrgent ? "urgent" : null)">Días para consumirlo: @Model.DaysToConsume</p>

<button type="button" disabled="@Model.IsSoldOut">Reservar</button>
```

El atributo `class` depende de `Model.IsUrgent`:
- Si es igual a true, se genera `class="urgent"` y el texto aparece en rojo y negrita.
- Si es igual a false, no se genera el atributo `class`.

El atributo `disabled` depende de `Model.IsSoldOut`:
- Si es igual a true, el botón aparece deshabilitado.
- Si es igual a false, el atributo `disabled` desaparece y el botón se puede pulsar.

### 3. Escenarios

**Escenario A: Ningún cambio**

![EscenarioA](/Proyecto-NovaHarvest/cuestiones-teoricas/capturas/Ejercicio5-EscenarioA.PNG)

Están los valores originales. Hay 223 kg disponibles y quedan 5 días para consumir el producto.

HTML generado para los días y el botón:

```html
<p>Días para consumirlo: 5</p>
<button type="button">Reservar</button>
```

No tiene la clase urgente y tampoco aparece el botón disabled porque hay existencias.

**Escenario C: ReservedKg = 343**

![EscenarioC](/Proyecto-NovaHarvest/cuestiones-teoricas/capturas/Ejercicio5-EscenarioC.PNG)

No quedan kilos disponibles, por lo que hay 0 cajas y 0 kg sueltos. El valor del excedente es de 0,00 € y el estado es Agotado.

HTML generado para el botón:

```html
<button type="button" disabled="disabled">Reservar</button>
```

El botón aparece deshabilitado porque `Model.IsSoldOut` es igual a true. El elemento de los días sigue sin tener la clase urgente:

```html
<p>Días para consumirlo: 5</p>
```

**Escenario D: HarvestDate = DateTime.Today.AddDays(-6)**

![EscenarioD](/Proyecto-NovaHarvest/cuestiones-teoricas/capturas/Ejercicio5-EscenarioD.PNG)

Queda solo un día para consumirlo, por lo que el parrafo tiene la clase urgent.

HTML generado:

```html
<p class="urgent">Días para consumirlo: 1</p>
<button type="button">Reservar</button>
```

Ahora el parrafo tiene la clase urgente porque `Model.IsUrgent` es igual a true al quedar un día para consumirlo.

## 6. Directivas

En mi proyecto NovaHarvest utilizo varias directivas y archivos especiales de Razor Pages. Cada uno tiene una función y modifica algún comportamiento de las páginas.

### 1. Tabla de directivas y archivos especiales

| Directiva o archivo | Función y comportamiento que cambia |
|---|---|
| `@page` | Convierte un archivo `.cshtml` en una página de Razor Pages y permite acceder a ella mediante una ruta. |
| `@page "/ticket"` | Define una ruta personalizada. En mi caso, permite acceder a `Ticket.cshtml` desde `/ticket`. |
| `@model` | Indica qué clase C# contiene la lógica y los datos de la página. Por ejemplo, `@model SurplusModel` permite utilizar propiedades como `Model.Surplus` o `Model.DaysToConsume`. |
| `@using` | Importa un espacio de nombres para poder utilizar sus clases sin escribir el nombre completo. |
| `@namespace` | Establece el espacio de nombres de la clase C# que Razor genera para una página. En mi caso no lo tengo en `_ViewImports.cshtml`, sino directamente en los archivos donde es necesario. |
| `@inject` | Permite utilizar un servicio de ASP.NET Core dentro de una página. En `_Layout.cshtml` utilizo `@inject IWebHostEnvironment Env` para mostrar el entorno de ejecución. |
| `_ViewStart.cshtml` | Se ejecuta antes de representar las páginas de una carpeta y permite establecer valores comunes, como el diseño (`Layout`) que utilizarán. |
| `Layout = null` | Desactiva el diseño compartido. En `Ticket.cshtml` lo utilizo para que el justificante no incluya la cabecera, el menú ni el pie de página de `_Layout.cshtml`. |

### 2. Quitar `@namespace` de `_ViewImports.cshtml`

Este es mi archivo `_ViewImports.cshtml`:

```cshtml
@using NovaHarvest.Web.Models
@using NovaHarvest.Web.Pages
@namespace NovaHarvest.Web
```

La directiva `@namespace` establece el espacio de nombres que utilizarán las páginas Razor de esa carpeta. De esta forma, Razor puede generar correctamente sus clases y relacionarlas con sus correspondientes archivos `.cshtml.cs`.

Al quitar esta directiva y ejecutar `dotnet build`, aparece el siguiente error:

```text id="0p3q1w"
C:\Users\Daniel\DWES\Proyecto\Proyecto-NovaHarvest\backend\NovaHarvest.Web\Pages\_ViewImports.cshtml(2,24): error CS0234: El tipo o el nombre del espacio de nombres 'Pages' no existe en el espacio de nombres 'NovaHarvest.Web' (¿falta alguna referencia de ensamblado?)
```

El problema aparece porque `_ViewImports.cshtml` intenta importar `NovaHarvest.Web.Pages` mediante `@using NovaHarvest.Web.Pages`, pero al quitar `@namespace` las páginas dejan de tener definido ese espacio de nombres común.

Por tanto, es necesario mantener `@namespace NovaHarvest.Web` en `_ViewImports.cshtml` para que Razor utilice el espacio de nombres correcto y la compilación funcione.

### 3. ¿Por qué `/reservations/ticket` da error 404?

En mi archivo `Ticket.cshtml` tengo esto definido:

```cshtml
@page "/ticket"
```

Esto hace que la página esté disponible en la dirección `/ticket`. La ruta personalizada sustituye a la que Razor podría obtener de la ubicación del archivo.

Por eso, al intentar acceder a `/reservations/ticket`, ASP.NET Core no encuentra ninguna página asociada a esa dirección y devuelve un error 404.

## 7. Tipos y operadores

En `SurplusModel` utilizo diferentes tipos de datos para realizar los cálculos del excedente. Cada uno está elegido según el tipo de información que necesito guardar.

### 1. Tabla de datos, tipos y operadores

| Dato calculado | Tipo de C# | Operadores utilizados | ¿Por qué utilizo ese tipo? |
|---|---|---|---|
| `NetAvailableKg` | `int` | Resta (`-`) | Guarda los kilos disponibles después de restar los reservados. |
| `ConsumeBeforeDate` | `DateTime` | `AddDays()` | Guarda la fecha límite de consumo, sumando los días de conservación con la fecha de cosecha. |
| `DaysToConsume` | `int` | Resta de fechas (`-`) y `.Days` | Guarda el número entero de días que quedan hasta la fecha límite de consumo. |
| `SurplusPricePerKg` | `decimal` | Multiplicación (`*`) y resta (`-`) | Representa el precio por kg. |
| `FullBoxes` | `int` | División entera (`/`) | Guarda el número de cajas completas de 5 kg. |
| `LooseKg` | `int` | Módulo (`%`) | Guarda los kilos que sobran después de hacer las cajas completas. |
| `TotalSurplusValue` | `decimal` | Multiplicación (`*`) | Representa el valor total del excedente. |
| `StockStatus` | `string` | Comparaciones (`==`, `<`) y condiciones (`if`, `else if`, `else`) | Guarda un el estado de las existencias. |
| `IsSoldOut` | `bool` | Comparación (`==`) | Indica si el excedente está agotado o no. |
| `IsUrgent` | `bool` | Comparación (`<=`) | Indica si queda un día o menos para consumir el producto. |

### 2. ¿Por qué `NetAvailableKg / BoxKg` da cajas completas?

Porque las dos variables que utilizo para calcularlo son enteros (int), por lo tanto el resultado tambíen es un entero.

### 3. ¿Qué pasaría si los kilos fueran `decimal`?

Si fueran decimal en lugar de enteros, la división devolvería un resultado con decimales. Tendría que utilizar `Math.Floor()` para obtener la parte entera de la división y calcular los kilos sobrantes por separado.

### 4. ¿Qué escribe Razor si muestras directamente un `bool`?

Si muestro directamente un booleano con Razor, este escribe su valor como literal, True o False.

### 5. ¿Qué escribe Razor si muestras un `null`?

Si intento mostrar un `null`, Razor no muestraría nada. Es decir, si por ejemplo tengo:

```cshtml
<p>@Model.Surplus.Note</p>
```

Y `Note` vale `null`, el resultado sería:

```html
<p></p>
```

Por eso, en mi método `OnGet()` compruebo si la nota es `null` y, en ese caso, la sustituyo por «Sin observaciones». Así evito que aparezca vacía en la página.

## 8. Cultura

### 1. Comparación sin `UseRequestLocalization("es-ES")`

**Con Docker:**

```bash
docker compose up -d --build
```

- Precio original en `/surplus`: ¤3.20 / kg
- Precio del excedente en `/surplus`: ¤2.24 / kg
- Fecha mostrada en `/`: Hoy es Thursday, 26 November 2026

**Con dotnet run:**

```bash
cd backend/NovaHarvest.Web
dotnet run
```

- Precio original en `/surplus`: 3,20 € / kg
- Precio del excedente en `/surplus`: 2,24 € / kg
- Fecha mostrada en `/`: Hoy es jueves, 26 de noviembre de 2026

### 2. Explicación de las diferencias

Las diferencias aparecen porque Docker y mi ordenador utilizan culturas regionales distintas.

La cultura determina cómo se representan los números, los precios y las fechas. Por ejemplo, un precio puede mostrarse con coma o con punto decimal, y una fecha puede utilizar el formato español o el formato de otro país.

### 3. ¿Por qué `UseRequestLocalization("es-ES")` elimina las diferencias?

Esta configuración establece la localización en España en las peticiones de la aplicación.

De esta forma, tanto cuando ejecuto el proyecto con Docker como cuando utilizo `dotnet run`, los precios y las fechas tienen el mismo formato regional.

## 9. Ámbito

### 1. Sustituir el contador por una variable local

Sustituyo temporalmente el contador por una variable local dentro de `Status.cshtml`:

```cshtml
@{
    int visits = 0;
    visits++;
}

<p>Visitas: @visits</p>
```

Al recargar varias veces la página, el contador siempre muestra `1`.

![EscenarioD](/Proyecto-NovaHarvest/cuestiones-teoricas/capturas/Ejercicio9-VistasVariableLocal.PNG)

Esto ocurre porque la variable local se crea cada vez que se procesa la página. Esto hace que el contador nunca pueda ser diferente a 1.

Por tanto, no conserva el valor de las visitas anteriores.

### 2. Utilizar SiteStats

Al utilizar `SiteStats`, el contador se comparte entre las peticiones que recibe la misma aplicación.

Si otro navegador accede a `/status`, también verá el contador actualizado, porque ambos navegadores están utilizando la misma instancia del backend.

Sin embargo, si ejecuto:

```bash
docker compose restart backend
```

El contador vuelve a empezar desde cero. Esto sucede porque `SiteStats.Visits` se guarda en memoria y, al reiniciar el backend, se inicia un nuevo proceso que vuelve a inicializar la variable estática.

### 3. ¿Por qué el layout no puede leer una variable de Status.cshtml?

El layout y la página `Status.cshtml` tienen sus propios ámbitos de variables.

Una variable local declarada dentro de `Status.cshtml` solamente existe dentro de esa página y no puede utilizarse directamente desde `_Layout.cshtml`.

Para compartir la información entre la página y el layout utilizo `ViewData`.

En `Surplus.cshtml` tengo:

```cshtml
@{
    ViewData["Title"] = "Excedente destacado";
}
```

Y en `_Layout.cshtml` tengo:

```cshtml
<title>@ViewData["Title"] - NovaHarvest</title>
```

De esta forma, la página establece el título mediante `ViewData["Title"]` y el layout puede acceder a él para mostrarlo en la pestaña del navegador.

Así puedo compartir información entre una página y su layout sin necesidad de utilizar variables locales.

## 10. PageModel

### 1. ¿Qué he puesto en `SurplusModel` y qué en `Surplus.cshtml`, y por qué?

En mi proyecto he separado la lógica de la página y su presentación para que el código esté más organizado y sea más fácil de mantener.

En `Surplus.cshtml.cs`, dentro de la clase `SurplusModel`, he puesto la parte relacionada con los datos y los cálculos:

- Los datos del producto, como su nombre, productor, fecha de cosecha, kilos disponibles y precio.
- Las constantes que utilizo en los cálculos, como el descuento, los kilos por caja y los días de conservación.
- Las propiedades que guardan los resultados de los cálculos.
- El método `OnGet()`, que realiza los cálculos cuando se carga la página.

En cambio, en `Surplus.cshtml` he puesto el HTML y las expresiones Razor que muestran los datos.

Por ejemplo, utilizo:

```cshtml
<h2>@Model.Surplus.Product</h2>
<p>Kilos disponibles: @Model.NetAvailableKg</p>
<p>Precio de excedente: @Model.SurplusPricePerKg.ToString("C") / kg</p>
```

Así, el archivo `.cshtml` no necesita realizar los cálculos, solamente mostrar los resultados que recibe de `SurplusModel`.

### 2. Cita un cálculo concreto

Un ejemplo es el cálculo de los kilos disponibles:

```csharp
NetAvailableKg = Surplus.AvailableKg - Surplus.ReservedKg;
```

En mi proyecto hay 343 kg disponibles y 120 kg reservados. Por tanto, quedan 223 kg.

Este cálculo se realiza en `SurplusModel` y después se muestra en `Surplus.cshtml` mediante `@Model.NetAvailableKg`.

### 3. ¿Cuántos objetos SurplusModel se crean al cargar `/surplus` tres veces?

Cada vez que se realiza una petición a `/surplus`, ASP.NET Core crea una nueva instancia de `SurplusModel`.

Por tanto, si cargo la página tres veces, se crean **3 objetos `SurplusModel`**.

Cada objeto vive durante el procesamiento de su petición. Cuando termina de generarse la respuesta, esa instancia deja de utilizarse.

### 4. ¿Y qué ocurre con el contador de SiteStats?

A diferencia de `SurplusModel`, `SiteStats` es una clase estática y su contador también es estático:

```csharp
public static class SiteStats
{
    public static int Visits;
}
```

Esto significa que no se crea una nueva copia del contador en cada petición. Todas las peticiones que llegan a la misma instancia de la aplicación comparte el mismo valor.

Por ejemplo, si cargo `/status` tres veces, el contador aumenta hasta 3. Si otro navegador accede a la misma aplicación, también verá ese contador actualizado.