# NovaHarvest

## 1. Arranque

### Ejecución con Docker

Para ejecutar NovaHarvest utilizando Docker, es necesario tener Docker instalado en el equipo.

Desde la carpeta raíz del proyecto, hay que ejecutar el siguiente comando:

```bash
docker compose up -d --build
```

Este comando se encarga de construir las imágenes y poner en marcha los dos servicios que necesita la aplicación:

* **web:** utiliza nginx para recibir las peticiones que llegan desde el navegador.
* **backend:** ejecuta la aplicación ASP.NET Core, encargada de generar las páginas web.

Una vez que los servicios estén funcionando, se puede acceder a la aplicación desde el navegador a través de la siguiente dirección:

http://localhost:8080

Si se quiere detener la aplicación y eliminar los contenedores creados, solo hay que ejecutar:

```bash
docker compose down
```

### Ejecución sin Docker

También se puede ejecutar el proyecto sin Docker, directamente desde .NET.

Para ello, primero hay que entrar en la carpeta donde se encuentra el proyecto ASP.NET Core:

```bash
cd backend/NovaHarvest.Web
```

Después, se inicia la aplicación mediante el siguiente comando:

```bash
dotnet run
```

Al ejecutarlo, la consola mostrará la dirección local en la que se está ejecutando el proyecto. Solo hay que copiar esa dirección y abrirla en el navegador.

Para cerrar la aplicación, solo hay que pulsar `Ctrl + C` en el terminal.

## 2. Arquitectura

NovaHarvest está organizado en dos servicios que trabajan conjuntamente: nginx y ASP.NET Core.

nginx se encarga de recibir las peticiones que llegan desde el navegador y enviarlas al backend, donde se procesa cada solicitud y se genera el contenido que finalmente recibe el usuario.

### Diagrama de arquitectura

```text
             USUARIO
                |
                v
            NAVEGADOR
                |
                | HTTP
                | localhost:8080
                v
        +------------------+
        |       WEB        |
        |      nginx       |
        |    Puerto 80     |
        +------------------+
                |
                | Reverse Proxy
                | Todas las peticiones
                v
        +------------------+
        |     BACKEND      |
        |   ASP.NET Core   |
        |     Kestrel      |
        |    Puerto 8080   |
        +------------------+
                |
                v
         GENERACIÓN DE HTML
        Y ARCHIVOS ESTÁTICOS
                |
                v
            NAVEGADOR
```

### Recorrido de una petición

Cuando un usuario accede a NovaHarvest desde el navegador, la petición sigue estos pasos:

1. El usuario introduce `http://localhost:8080` en el navegador.
2. La petición llega al servicio **web**, donde nginx está escuchando en el puerto 80 del contenedor.
3. nginx actúa como *reverse proxy* y se encarga de reenviar la petición al servicio **backend**.
4. El backend recibe la petición a través de Kestrel, que escucha en el puerto 8080 del contenedor.
5. ASP.NET Core ejecuta el código C# correspondiente a la ruta solicitada y prepara la página HTML que se va a mostrar.
6. Por último, el backend devuelve la respuesta a nginx, que se encarga de enviarla de vuelta al navegador.