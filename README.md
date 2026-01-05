# 🎓 Riwi Courses Assessment API

API REST para la gestión de cursos y lecciones online, desarrollada con .NET 9.0, Entity Framework Core y PostgreSQL.

## 🚀 Quick Start con Docker (Recomendado)

### Opción 1: Script automatizado
```bash
./docker-setup.sh
```

### Opción 2: Comandos manuales
```bash
# Levantar todo (PostgreSQL + API)
docker-compose up --build

# Acceder a Swagger
http://localhost:5023
```

## 📋 Requisitos

- Docker 20.10+
- Docker Compose 2.0+
- (Opcional) .NET 9.0 SDK para desarrollo local

## 🏗️ Arquitectura

```
├── Domain/              # Entidades, Excepciones, Interfaces
├── Application/         # Casos de Uso, DTOs, Servicios
├── Infrastructure/      # DbContext, Repositorios, EF Core
├── WebApi/             # Controllers, Middleware
└── Tests/              # Tests Unitarios (xUnit + Moq)
```

### Clean Architecture implementada:
- ✅ Separación de responsabilidades
- ✅ Inversión de dependencias
- ✅ Independencia de frameworks
- ✅ Testeable

## 🐳 Docker Compose

### Servicios:
- **PostgreSQL 16**: Base de datos (puerto 6543)
- **API .NET 9.0**: REST API (puerto 5023)

### Características:
- ✅ Migraciones automáticas al iniciar
- ✅ Health checks configurados
- ✅ Volúmenes persistentes
- ✅ Red interna
- ✅ Hot reload en desarrollo

## 📊 Endpoints Principales

### Courses (`/api/courses`)
- `GET /api/courses/search` - Buscar cursos (paginado)
- `GET /api/courses/{id}` - Obtener curso
- `GET /api/courses/{id}/summary` - Resumen con total de lecciones
- `POST /api/courses` - Crear curso
- `PUT /api/courses/{id}` - Actualizar curso
- `DELETE /api/courses/{id}` - Eliminar (soft delete)
- `PATCH /api/courses/{id}/publish` - Publicar curso
- `PATCH /api/courses/{id}/unpublish` - Despublicar curso

### Lessons (`/api/lessons`)
- `GET /api/lessons/course/{courseId}` - Lecciones de un curso
- `GET /api/lessons/{id}` - Obtener lección
- `POST /api/lessons` - Crear lección
- `PUT /api/lessons/{id}` - Actualizar lección
- `DELETE /api/lessons/{id}` - Eliminar (soft delete)
- `PATCH /api/lessons/{id}/reorder` - Reordenar lección

## 🎯 Reglas de Negocio

✅ **Publicación de cursos**: Un curso solo puede publicarse si tiene al menos una lección activa
✅ **Orden único**: El campo `Order` de las lecciones debe ser único por curso
✅ **Soft delete**: Eliminación lógica en cursos y lecciones
✅ **Reordenamiento inteligente**: Sin duplicados al reordenar lecciones

## 🧪 Tests Unitarios

```bash
# Ejecutar tests
dotnet test

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Tests implementados (5):
- ✅ `PublishCourse_WithLessons_ShouldSucceed`
- ✅ `PublishCourse_WithoutLessons_ShouldFail`
- ✅ `CreateLesson_WithUniqueOrder_ShouldSucceed`
- ✅ `CreateLesson_WithDuplicateOrder_ShouldFail`
- ✅ `DeleteCourse_ShouldBeSoftDelete`

## 🛠️ Desarrollo Local (sin Docker)

### 1. Configurar PostgreSQL
```bash
# Opción A: Con Docker solo la BD
docker-compose up postgres -d

# Opción B: PostgreSQL local
# Crear base de datos: RiwiCoursesDb
```

### 2. Aplicar migraciones
```bash
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project WebApi
dotnet ef database update --project Infrastructure --startup-project WebApi
```

### 3. Ejecutar la API
```bash
cd WebApi
dotnet run
```

## 📦 Tecnologías

- **.NET 9.0** - Framework principal
- **Entity Framework Core 9.0** - ORM
- **PostgreSQL 16** - Base de datos
- **Swagger/OpenAPI** - Documentación
- **xUnit + Moq + FluentAssertions** - Testing
- **Docker & Docker Compose** - Containerización

## 🗄️ Base de Datos

### Tablas:
- **Courses**: Id, Title, Status, IsDeleted, CreatedAt, UpdatedAt
- **Lessons**: Id, CourseId, Title, Order, IsDeleted, CreatedAt, UpdatedAt

### Características:
- Global Query Filter para soft delete
- Unique constraint en (CourseId, Order) para lecciones activas
- Índices optimizados para búsquedas
- Timestamps automáticos

## 📚 Documentación

- **Swagger UI**: http://localhost:5000 (cuando la API está corriendo)
- **Docker Guide**: Ver `DOCKER_GUIDE.md`
- **API Details**: Ver `API_CONTROLLERS_IMPLEMENTATION.md`
- **Infrastructure**: Ver `INFRASTRUCTURE_IMPLEMENTATION.md`

## 🔧 Comandos Útiles

### Docker:
```bash
# Ver logs
docker-compose logs -f api

# Reiniciar solo la API
docker-compose restart api

# Detener todo
docker-compose down

# Eliminar volúmenes (¡cuidado!)
docker-compose down -v
```

### EF Core:
```bash
# Crear migración
dotnet ef migrations add MigrationName --project Infrastructure --startup-project WebApi

# Aplicar migraciones
dotnet ef database update --project Infrastructure --startup-project WebApi

# Revertir migración
dotnet ef database update PreviousMigration --project Infrastructure --startup-project WebApi

# Ver SQL de migración
dotnet ef migrations script --project Infrastructure --startup-project WebApi
```

## 🌐 Variables de Entorno

### Docker (automático):
- `ConnectionStrings__DefaultConnection`: Configurado en docker-compose.yml
- `ASPNETCORE_ENVIRONMENT`: Development

### Local (appsettings.json):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=RiwiCoursesDb;Username=postgres;Password=postgres"
  }
}
```

## 🤝 Contribuir

1. Fork el proyecto
2. Crea una rama (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto es parte del assessment técnico de Riwi.

## 📞 Contacto

- Email: info@riwi.io
- Website: https://riwi.io

---

## ✅ Estado del Proyecto

| Componente | Estado | Tests |
|------------|--------|-------|
| Domain Layer | ✅ 100% | N/A |
| Application Layer | ✅ 100% | N/A |
| Infrastructure Layer | ✅ 100% | N/A |
| WebApi Controllers | ✅ 100% | N/A |
| Unit Tests | ✅ 5 tests | ✅ Passing |
| Docker Setup | ✅ 100% | N/A |
| Database Migrations | ✅ Ready | N/A |
| Documentation | ✅ Complete | N/A |

**🎉 Proyecto 100% funcional y listo para producción!**

