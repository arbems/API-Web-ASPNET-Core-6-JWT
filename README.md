# JSON Web Tokens en ASP.NET Core 6
Este repositorio contiene los ejemplos que muestran como autenticar usuarios en ASP.NET Core 6 usando JSON Web Tokens(JWT), para manejar la autenticación en aplicaciones.

De este modo cuando el usuario se quiere autenticar envía sus datos de inicio del sesión al servidor, este genera el JWT y se lo manda a la aplicación cliente, luego en cada petición el cliente envía este token que el servidor usa para verificar que el usuario este correctamente autenticado y permitiendo al usuario acceder a rutas, servicios o recursos que solo están permitidos con su debido token. Este token se usa como método de autenticación y autorización por parte de la aplicación cliente frente al servidor que aloja el recurso.

## Tecnologías

* ASP.NET Core 6
* JSON Web Tokens
* ASP.NET Core Identity.
* Entity Framework Core

## Enlaces
[JSON Web Tokens para autenticar usuarios en ASP.NET Core 6](https://arbems.com/json-web-token-aspnet-core-6)
