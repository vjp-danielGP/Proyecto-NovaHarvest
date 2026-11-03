# Cuestiones UT2 – NovaHarvest

## 1. Cliente y servidor

Cuando abro `/status`, el cliente (navegador) envía una petición HTTP `GET /status`, recibe el HTML y lo interpreta para mostrar la página. Tambíen solicita recursos estáticos como `/css/estilos.css` y `/img/logo.png`.

En el servidor, la petición llega a nginx y este la reenvia al backend, donde Kestrel recibe la solicitud y ASP.NET Core ejecuta el `MapGet("/status", ...)` de `Program.cs`. El código C# obtiene la fecha y hora con `DateTime.Now` y construye el HTML que se devuelve al navegador.

Cuando se puedan reservar productos, la comprobación de que no se puedan reservar más kilos de los disponibles se hará en el servidor. El cliente puede mostrar avisos, pero nunca es fiable para aplicar esta regla, sus datos se pueden modificar y varias personas podrían reservar al mismo tiempo. El servidor es quien siempre debe validar la disponibilidad antes de confirmar la reserva.

## 2. Estático o dinámico

Las páginas que pueden ser HTML estático son `/` (Inicio) y `/about` (Acerca de), porque su contenido apenas cambia y no necesita calcularse en cada petición. Por el contrario, `/status` debe ser dinámica, ya que muestra la fecha y hora actuales del servidor y estas cambian al recargar la página. 

En el futuro, NovaHarvest necesitará, por ejemplo, una página con los productos y kilos disponibles en cada momento, y una página de detalle de una reserva con su estado actualizado. Estos contenidos dependen de los datos y de las acciones de los usuarios, asi que pueden servirse como HTML estático.

## 3. Ejecución en el servidor

Cuando escribo `http://localhost:8080/status` en el navegador, este envía una petición para abrir esa página. Primero, la petición llega a nginx, que está en el contenedor web. En `compose.yaml` hemos indicado que el puerto 8080 de nuestro ordenador se conecta con el puerto 80 de nginx. Hemos configuradp el archivo `web/nginx.conf` para que envie todas las peticiones al servicio backend, usando el puerto 8080. La petición llega entonces a Kestrel, que es el servidor que utiliza nuestra aplicación ASP.NET Core. Kestrel busca en `Program.cs` la ruta `MapGet("/status", ...)` y ejecuta el código C#, que genera el HTML con la fecha y hora. La respuesta vuelve a nginx y este la entrega al navegador. .

Si utilizáramos CGI, el servidor web tendría que iniciar otro programa para generar la página cuando recibe una petición. Con un módulo embebido, el código estaría integrado dentro del propio servidor web, en vez de ejecutarse en una aplicación separada como ocurre ahora con ASP.NET Core y Kestrel.

## 4. Reverse proxy

Al ejecutar `curl -I http://localhost:8080/status` con Docker, la cabecera que aparece es `Server: nginx`, porque la conexión externa termina en nginx, que actúa como reverse proxy. Si ejecuto la aplicación directamente con `dotnet run` y consulto la URL que muestra la consola, la cabecera que aparece es `Server: Kestrel`, porque ya no hay nginx delante.

Si para el backend (`docker compose stop backend`) y solicito `/status` o `/css/estilos.css` a través de nginx, recibo `502 Bad Gateway`, porque nginx intenta reenviar ambas peticiones, pero el backend está detenido y no puede responder. 

`backend` no publica ningún puerto porque no debe ser accesible directamente desde el exterior. Solo nginx, dentro de la red interna de Docker, necesita comunicarse con él.

## 5. nginx

nginx no sirve `estilos.css` ni `logo.png` por su cuenta porque en `web/nginx.conf` todas las peticiones que coinciden con `location /` se envían al backend mediante `proxy_pass http://backend:8080`. La carpeta `web/` solo contiene el `Dockerfile` y `nginx.conf`, estos recursos están en `backend/NovaHarvest.Web/wwwroot/` y ASP.NET Core los publica mediante `MapStaticAssets()` en `Program.cs`.  
De este modo, los mismos archivos se sirven tanto cuando ejecuto el proyecto con `dotnet run` como cuando accedo a través de nginx.

Si NovaHarvest se publicara en Internet, nginx podría encargarse de terminar las conexiones HTTPS y de repartir las peticiones entre varias instancias del backend (balanceo de carga). También podría servir directamente los archivos estáticos como optimización, evitando que esas peticiones llegasen a Kestrel.

## 6. Tecnologías

Si sustituyéramos ASP.NET Core por otra tecnología de servidor, cambiaría principalmente el código del backend, sus archivos de proyecto y su `Dockerfile`, que tendría que instalar o utilizar el entorno de ejecución correspondiente. Por ejemplo, con Python habría que usar el código y el servidor de aplicaciones de esa tecnología, en vez de `Program.cs`, Kestrel y el proyecto `.csproj`.  
También habría que adaptar `compose.yaml` si el nuevo backend utilizase otro puerto o necesitase otros servicios.  

En cambio, la estructura general con los servicios `web` y `backend` podría mantenerse. `web/Dockerfile` y la función de reverse proxy de `web/nginx.conf` seguirían siendo prácticamente iguales si el backend continuase accesible con el mismo nombre y puerto. El navegador, las rutas públicas y los recursos estáticos podrían conservarse si la nueva aplicación ofreciese las mismas URL y respuestas.

## 7. Marcado y código

```csharp
app.MapGet("/status", () => Results.Content(
$"""
<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <title>NovaHarvest - Estado del servidor</title>
    <link rel="stylesheet" href="/css/estilos.css">
</head>
<body>
    <header>
        <img src="/img/logo.png" alt="Logotipo de NovaHarvest">
        <h2>NovaHarvest</h2>
        <nav>
            <a href="/">Inicio</a>
            <a href="/about">Acerca de</a>
            <a href="/status">Estado</a>
        </nav>
    </header>

    <main>
        <h1>Estado del servidor</h1>
        <p>Fecha y hora del servidor: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
        <p>Página generada por ASP.NET Core (Kestrel)</p>
    </main>

    <footer>
        NovaHarvest · IES Valle del Jerte · {DateTime.Now.Year}
    </footer>
</body>
</html>
""",
"text/html"));
```

Generar HTML de esta manera tiene varios inconvenientes. Primero, el HTML queda mezclado con la sintaxis de C#, por lo que resulta más difícil de leer y mantener. Segundo, un error al escribir una etiqueta o una cadena puede romper el marcado y es más complicado localizarlo, porque el editor no trabaja con el HTML como una plantilla normal.  

La parte que se repite en las tres páginas es la estructura común: el `head` con el enlace a la hoja de estilos, el `header` con el logotipo y el menú de navegación, y el `footer`.

## 8. Docker

Con el entorno levantado tengo **2 imágenes** (una para `web` y otra para `backend`) y **2 contenedores** en ejecución (uno por cada servicio). Una imagen es la plantilla que contiene la aplicación y su configuración, mientros que un contenedor es una instancia creada a partir de esa imagen que está ejecutándose. En el proyecto, `web` ejecuta nginx y `backend` ejecuta NovaHarvest sobre Kestrel. 

El `Dockerfile` del backend utiliza dos etapas. La primera parte de `mcr.microsoft.com/dotnet/sdk:10.0` para compilar y publicar el proyecto. La segunda utiliza `mcr.microsoft.com/dotnet/aspnet:10.0`, que contiene lo necesario para ejecutar la aplicación publicada. Por eso la imagen final no necesita incluir el SDK completo, que solo hace falta durante la compilación y aumentaría su tamaño.  

El archivo `backend/.dockerignore` excluye `bin/` y `obj/`. Sin él, Docker podría copiar al contexto de construcción archivos generados en mi equipo, que no son necesarios y pueden contener restos de compilaciones anteriores. Eso puede hacer la imagen más pesada y provocar conflictos durante la compilación dentro del contenedor.

## 9. Herramientas

| Herramienta | Uso en NovaHarvest |
|---|---|
| Visual Studio Code / editor | Editar `Program.cs`, los archivos de configuración, el HTML generado y los estilos CSS. |
| .NET 10 SDK y comandos `dotnet` | Crear el proyecto con `dotnet new web`, compilarlo y ejecutarlo con `dotnet build` y `dotnet run`. |
| C# y ASP.NET Core | Implementar las rutas `/`, `/about` y `/status`, generar el HTML y servir los archivos estáticos. |
| Kestrel | Recibir las peticiones HTTP del backend y ejecutar la aplicación ASP.NET Core. |
| Docker | Construir las imágenes y ejecutar los servicios de NovaHarvest en contenedores. |
| Docker Compose | Definir `web` y `backend` en `compose.yaml`, conectarlos y arrancarlos con un único comando. |
| nginx | Recibir las peticiones externas en el puerto 8080 del equipo y reenviarlas al backend mediante reverse proxy. |
| Git | Guardar el historial de cambios del proyecto y permitir volver a versiones anteriores. |
| Navegador y herramientas de inspección HTTP (`curl`) | Acceder a las páginas y comprobar las respuestas HTTP, las cabeceras y los códigos de estado. |

## 10. Observación

Las horas no coinciden. Con `dotnet run`, como el enterno es mi equipo, utiliza mi configuración horaria y por tanto, aparece la misma hora. Mientras que con Docker, aparece una hora menos porque se ejecuta dentro de un contenedor y este tiene una configuración horaria diferente.
