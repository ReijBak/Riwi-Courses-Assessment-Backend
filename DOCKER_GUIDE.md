# 🐳 Docker Compose - Guía Completa

## 📋 Archivos Creados

### 1. **Dockerfile** ✅
- Imagen multi-stage build
- Base: .NET 9.0
- Optimizado para producción
- Expone puertos 8080 y 8081

### 2. **docker-compose.yml** ✅
- PostgreSQL 16 Alpine
- API .NET 9.0
- Red interna configurada
- Volúmenes persistentes
- Health checks

### 3. **.dockerignore** ✅
- Excluye archivos innecesarios del build

### 4. **appsettings.Docker.json** ✅
- Configuración específica para Docker

---

## 🚀 Comandos para Ejecutar

### **Opción 1: Construir y ejecutar todo**
```bash
cd /home/coders/Documentos/Steve/Riwi-Courses-Assessment/Riwi-Courses-Assessment-Backend

# Levantar servicios (PostgreSQL + API)
docker-compose up --build
```

### **Opción 2: Ejecutar en segundo plano**
```bash
docker-compose up --build -d
```

### **Opción 3: Solo PostgreSQL (para desarrollo local)**
```bash
docker-compose up postgres -d
```

---

## 📊 Servicios Configurados

### **PostgreSQL** (puerto 6543)
- **Imagen**: postgres:16-alpine
- **Base de datos**: RiwiCoursesDb
- **Usuario**: postgres
- **Contraseña**: postgres
- **Puerto host**: 6543 → container: 5432
- **Volumen**: postgres_data (persistente)

### **API .NET** (puerto 5023)
- **Puerto host**: 5023 → container: 8080
- **Swagger**: http://localhost:5023
- **Depende de**: PostgreSQL (espera health check)
- **Restart**: unless-stopped

---

## 🔧 Aplicar Migraciones con Docker

### **Opción A: Antes de levantar los contenedores**
```bash
# 1. Solo levantar PostgreSQL
docker-compose up postgres -d

# 2. Esperar a que esté listo
sleep 5

# 3. Aplicar migraciones (desde tu máquina)
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project WebApi
dotnet ef database update --project Infrastructure --startup-project WebApi

# 4. Levantar la API
docker-compose up api -d
```

### **Opción B: Migración automática al iniciar la API**
Modificar `Program.cs` para aplicar migraciones al inicio (ver sección abajo).

---

## 📝 Migración Automática (Recomendado)

Actualiza `WebApi/Program.cs` agregando esto antes de `app.Run()`:

```csharp
// Apply migrations automatically on startup (only in Development)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}

app.Run();
```

---

## 🛠️ Comandos Útiles

### Ver logs:
```bash
# Todos los servicios
docker-compose logs -f

# Solo API
docker-compose logs -f api

# Solo PostgreSQL
docker-compose logs -f postgres
```

### Detener servicios:
```bash
docker-compose down
```

### Detener y eliminar volúmenes (¡cuidado, borra la BD!):
```bash
docker-compose down -v
```

### Reconstruir sin caché:
```bash
docker-compose build --no-cache
docker-compose up
```

### Ver estado de servicios:
```bash
docker-compose ps
```

### Acceder a PostgreSQL:
```bash
# Desde el contenedor
docker exec -it riwi_courses_db psql -U postgres -d RiwiCoursesDb

# Ver tablas
\dt

# Salir
\q
```

### Ejecutar comando en el contenedor de la API:
```bash
docker exec -it riwi_courses_api /bin/bash
```

---

## 🌐 URLs de Acceso

| Servicio | URL | Descripción |
|----------|-----|-------------|
| **Swagger UI** | http://localhost:5000 | Documentación interactiva |
| **API** | http://localhost:5000/api | Base URL de la API |
| **PostgreSQL** | localhost:5432 | Conexión desde host |

---

## 📂 Estructura de Volúmenes

```
docker volumes/
└── postgres_data/
    └── [datos de PostgreSQL persistentes]
```

**Importante**: Los datos de PostgreSQL persisten incluso si detienes los contenedores.

---

## 🔍 Verificar que Todo Funciona

### 1. Verificar que los contenedores están corriendo:
```bash
docker-compose ps
```

Deberías ver:
```
NAME                 STATUS         PORTS
riwi_courses_db      Up             0.0.0.0:5432->5432/tcp
riwi_courses_api     Up             0.0.0.0:5000->8080/tcp
```

### 2. Verificar logs de la API:
```bash
docker-compose logs api
```

Deberías ver: "Application started"

### 3. Probar Swagger:
Abre en el navegador: **http://localhost:5000**

### 4. Probar un endpoint:
```bash
curl http://localhost:5000/api/courses/search?page=1&pageSize=10
```

---

## 🐛 Solución de Problemas

### Error: "port is already allocated"
```bash
# Ver qué está usando el puerto 5000
sudo lsof -i :5000

# Cambiar el puerto en docker-compose.yml
ports:
  - "5001:8080"  # Usar 5001 en lugar de 5000
```

### Error: "Cannot connect to PostgreSQL"
```bash
# Ver logs de postgres
docker-compose logs postgres

# Verificar health check
docker inspect riwi_courses_db | grep Health -A 10
```

### Error: "Migration pending"
```bash
# Ejecutar migraciones manualmente
docker-compose up postgres -d
dotnet ef database update --project Infrastructure --startup-project WebApi
```

### La API no inicia:
```bash
# Ver logs detallados
docker-compose logs -f api

# Reconstruir imagen
docker-compose build --no-cache api
docker-compose up api
```

---

## 🔄 Flujo Completo de Trabajo

### Primera vez:
```bash
# 1. Construir y levantar todo
cd Riwi-Courses-Assessment-Backend
docker-compose up --build

# 2. En otra terminal, aplicar migraciones (si no son automáticas)
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project WebApi
dotnet ef database update --project Infrastructure --startup-project WebApi

# 3. Acceder a Swagger
# http://localhost:5000
```

### Desarrollo continuo:
```bash
# Levantar servicios
docker-compose up -d

# Ver logs
docker-compose logs -f api

# Hacer cambios en el código...

# Reconstruir solo la API
docker-compose up --build api -d

# Detener todo
docker-compose down
```

---

## 📊 Variables de Entorno

Configuradas en `docker-compose.yml`:

### PostgreSQL:
- `POSTGRES_DB=RiwiCoursesDb`
- `POSTGRES_USER=postgres`
- `POSTGRES_PASSWORD=postgres`

### API:
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:8080`
- `ConnectionStrings__DefaultConnection=Host=postgres;...`