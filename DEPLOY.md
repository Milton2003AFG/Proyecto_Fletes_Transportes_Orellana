# Guía de Despliegue - Transportes Orellana

Este documento detalla la arquitectura de infraestructura, los requisitos técnicos, las variables de entorno y el procedimiento operativo para la puesta en producción del sistema de gestión de fletes **Transportes Orellana**.

---

## 1. Arquitectura de Despliegue

La solución opera bajo un modelo de nube desacoplado:

* **Plataforma de Aplicación (PaaS / CaaS):** Render (Web Service basado en contenedores Linux Docker).
* **Motor de Base de Datos (DBaaS):** Neon Serverless PostgreSQL.
* **Entorno de Ejecución:** Contenedor Docker multi-stage optimizado sobre .NET 10 Runtime con dependencias nativas de renderizado (`libfontconfig1`) para la exportación de reportes PDF vía QuestPDF.
* **Seguridad en Tránsito:** Terminación TLS/HTTPS con HSTS activo hacia el cliente y cifrado SSL (`SslMode=Require;TrustServerCertificate=true;`) hacia la base de datos PostgreSQL.

---

## 2. Requisitos Previos

* Cuenta activa en [Render](https://render.com).
* Proyecto configurado en [Neon](https://neon.tech) con base de datos PostgreSQL provisionada.
* Repositorio de código fuente en GitHub/GitLab vinculado a Render.
* .NET SDK 10 instalado localmente (para gestión de migraciones EF Core y CLI).

---

## 3. Variables de Entorno

En el panel de administración del servicio en Render (**Settings $\rightarrow$ Environment**), se deben configurar las siguientes variables para evitar credenciales planas en el código fuente:

| Variable | Tipo | Descripción | Ejemplo de Valor |
| :--- | :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Config | Entorno de ejecución de la aplicación | `Production` |
| `ConnectionStrings__DefaultConnection` | Secreto | Cadena canónica Npgsql con SSL obligatorio | `Host=ep-withered-poetry-b4c06sax.c-6.us-east-2.aws.neon.tech;Port=5432;Database=neondb;Username=neondb_owner;Password=TU_PASSWORD;SslMode=Require;TrustServerCertificate=true;` |
| `AdminSeed__Email` | Config | Correo del superadministrador inicial | `admin@transportesorellana.com` |
| `AdminSeed__Password` | Secreto | Contraseña de arranque para el rol Admin | `Admin!Transportes#Orellana_2026` |

> **Nota:** La cadena de conexión no debe usar el formato URL (`postgresql://...`) ni el endpoint con `-pooler` para evitar interferencias con las transacciones DDL de Entity Framework Core.

---

## 4. Configuración del Servicio en Render

1. **Creación del Web Service:**
    * Conectar el repositorio Git del proyecto.
    * **Root Directory:** `src/Transportes Orellana`
    * **Runtime:** `Docker`
    * **Instance Type:** `Free` o superior.
2. **Puertos y Health Check:**
    * El contenedor expone y escucha peticiones en el puerto `8080`.
    * **Health Check Path:** `/Account/Login`

---

## 5. Estrategia de Migraciones y Carga Inicial (Seeding)

La sincronización de base de datos se gestiona automáticamente en el arranque de la aplicación (`Program.cs`):

1. **Migraciones:** Al iniciar el contenedor, el runtime ejecuta `context.Database.MigrateAsync()`, aplicando cualquier migración pendiente registrada en la carpeta `Migrations/`.
2. **Roles del Sistema:** Se asegura la existencia de los roles base (`Admin` y `Motorista`).
3. **Usuario Administrador:** Si la tabla `AspNetUsers` no contiene al usuario definido en `AdminSeed__Email`, se crea automáticamente aplicando las políticas de contraseñas seguras (mínimo 12 caracteres, mayúsculas, minúsculas, números y caracteres especiales).

---

## 6. Procedimiento de Despliegue Continuo (CI/CD)

El ciclo de actualización y entrega continua se ejecuta mediante Git:

1. Clonar el repositorio y situarse en la rama principal:
   ```bash
   git checkout main

2. Si se realizaron cambios en las entidades de base de datos, generar la migración localmente antes de desplegar:
   ```bash
   cd "src/Transportes Orellana"
   dotnet ef migrations add NombreDeLaMigracion
   
3. Verificar que la carpeta Migrations/ no esté ignorada en el archivo .gitignore ni en el .dockerignore.
4. Enviar los cambios al repositorio remoto:
   ```bash
   git add .
   git commit -m "feat: implementar nuevas funcionalidades"
   git push origin main
   
5. Render detectará el push vía webhook, compilará la imagen multi-stage y realizará el despliegue automático sin caída de servicio.

---

## 7. Verificación y Monitoreo Post-Despliegue

1. Navegación: Abrir la URL pública generada por Render (`https://<servicio>.onrender.com`).
2. Seguridad Web: Comprobar la redirección automática a HTTPS y la presentación de la pantalla de inicio de sesión (`Account/Login)`.
3. Persistencia: Iniciar sesión con las credenciales de administrador para verificar la conexión activa con Neon PostgreSQL.
4. Logs del Servidor: Inspeccionar la consola de Render para confirmar:
   * La correcta carga de fuentes gráficas (`libfontconfig1`).
   * La ausencia de advertencias de modelos desincronizados (`PendingModelChangesWarning`).
   * La ausencia del error `42P01: relation does not exist`.