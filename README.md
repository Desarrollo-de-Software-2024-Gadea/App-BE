# App-BE - Aplicación de Gestión de Series


## Tabla de Contenidos
- [Introducción](#introducción)
- [Características](#características)
- [Tecnologías](#tecnologías)

## Introducción

Este es el trabajo práctico final del año 2024 de la materia Desarrollo de Software. Consiste en el desarrollo de una aplicación web utilizando la plataforma .NET con el [Framework ABP](https://abp.io/). La aplicación interactúa con la [OMDB API](http://www.omdbapi.com/) para gestionar información relacionada con series de televisión, permitiendo a los usuarios realizar un seguimiento, calificar y gestionar sus series favoritas.

## Características

La aplicación proporciona las siguientes funcionalidades clave:

### 1. **Búsqueda de Series**
- **1.1 Buscar Series**: Los usuarios pueden buscar series utilizando el título o el género a través de la API externa OMDB.

### 2. **Gestión de Series**
- **2.1 Obtener Información de Series**: Recupera detalles de las series (título, género, fecha de lanzamiento, duración, equipo, imagen de portada, país de origen y calificación en IMDB) desde la base de datos interna.
- **2.2 Persistir Información de Series**: Guarda la información de las series obtenidas de la API OMDB en la base de datos interna.

### 3. **Lista de Seguimiento**
- **3.1 Ver Lista de Seguimiento**: Los usuarios pueden ver las series que han añadido a su lista de seguimiento.
- **3.2 Agregar a la Lista de Seguimiento**: Permite a los usuarios agregar series a su lista de seguimiento para recibir notificaciones sobre cambios importantes.
- **3.3 Eliminar de la Lista de Seguimiento**: Los usuarios pueden eliminar series de su lista de seguimiento.

### 4. **Notificaciones**
- **4.1 Notificaciones en Pantalla**: Muestra notificaciones sobre cambios relevantes en las series de la lista de seguimiento en la pantalla principal.
- **4.2 Notificaciones por Correo Electrónico**: Envío de notificaciones por email según las preferencias del usuario.
- **4.3 Preferencias de Notificaciones**: Los usuarios pueden seleccionar los tipos de notificaciones que desean recibir.
- **4.4 Generar Notificaciones**: Genera automáticamente notificaciones basadas en los cambios de las series en la lista de seguimiento y las guarda en la base de datos.

### 5. **Calificación de Series**
- **5.1 Calificar Series**: Los usuarios pueden calificar las series con una puntuación de 1 a 5 y agregar comentarios opcionales.
- **5.2 Editar Calificación**: Los usuarios pueden modificar sus calificaciones y comentarios.

### 6. **Autenticación**
- **6.1 Inicio de Sesión**: Los usuarios deben iniciar sesión con su nombre de usuario y contraseña para acceder a la aplicación.

### 7. **Funcionalidades de Administrador**
- **7.1 Gestión de Usuarios**: Los administradores pueden gestionar las cuentas de usuario, incluyendo la creación, eliminación y visualización de detalles completos.
- **7.2 Panel de Monitoreo de la API**: Visualización de estadísticas de acceso a la API, tiempos de respuesta, cantidad de errores, etc.
- **7.3 Generación de Bitácora de Monitoreo**: Registro de eventos en un archivo de log para diagnósticos en caso de errores.

### 8. **Administración de Usuarios**
- **8.1 Registrar Usuario**: Los administradores pueden crear nuevos usuarios con nombre de usuario, nombre completo, contraseña y foto de perfil.
- **8.2 Eliminar Usuario**: Los administradores pueden eliminar usuarios existentes.
- **8.3 Visualizar Usuarios**: Los administradores pueden ver los datos completos de todos los usuarios, mientras que los usuarios no administradores solo pueden ver los nombres de otros usuarios.
- **8.4 Modificar Perfil**: Los usuarios pueden editar su perfil (nombre completo, contraseña, foto de perfil).

## Tecnologías
- **Framework**: .NET con ABP Framework.
- **Integración de API**: OMDB API.
- **Lenguaje de Programación**: C#
- **Base de Datos**: Base de datos relacional SQL.
- **Notificaciones**: Notificaciones en pantalla y por correo electrónico.

