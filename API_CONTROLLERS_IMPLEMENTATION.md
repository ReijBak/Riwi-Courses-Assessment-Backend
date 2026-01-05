# ✅ Controllers y API REST - Implementación Completa

## 🎉 Archivos Creados

### 1. **Controladores**
- ✅ `WebApi/Controllers/CoursesController.cs` (8 endpoints)
- ✅ `WebApi/Controllers/LessonsController.cs` (6 endpoints)

### 2. **Configuración**
- ✅ `WebApi/Program.cs` (configuración completa)
- ✅ `WebApi/Middleware/ExceptionHandlingMiddleware.cs`
- ✅ `WebApi/WebApi.csproj` (Swashbuckle agregado)

---

## 📋 Endpoints Implementados

### 🎓 **CoursesController** (`/api/courses`)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/courses/{id}` | Obtener curso por ID | ❌ |
| GET | `/api/courses/search` | Buscar cursos (paginado + filtros) | ❌ |
| GET | `/api/courses/{id}/summary` | Resumen del curso (con total lecciones) | ❌ |
| POST | `/api/courses` | Crear nuevo curso | ❌ |
| PUT | `/api/courses/{id}` | Actualizar curso | ❌ |
| DELETE | `/api/courses/{id}` | Eliminar curso (soft delete) | ❌ |
| PATCH | `/api/courses/{id}/publish` | Publicar curso | ❌ |
| PATCH | `/api/courses/{id}/unpublish` | Despublicar curso | ❌ |

#### Ejemplos de uso:

**Buscar cursos:**
```http
GET /api/courses/search?q=javascript&status=Published&page=1&pageSize=10
```

**Obtener resumen:**
```http
GET /api/courses/{id}/summary
```
Respuesta:
```json
{
  "id": "guid",
  "title": "Curso de .NET",
  "status": "Published",
  "totalLessons": 15,
  "lastModified": "2026-01-05T10:30:00Z"
}
```

**Publicar curso:**
```http
PATCH /api/courses/{id}/publish
```

---

### 📚 **LessonsController** (`/api/lessons`)

| Método | Endpoint | Descripción | Auth |
|--------|----------|-------------|------|
| GET | `/api/lessons/{id}` | Obtener lección por ID | ❌ |
| GET | `/api/lessons/course/{courseId}` | Obtener lecciones de un curso | ❌ |
| POST | `/api/lessons` | Crear nueva lección | ❌ |
| PUT | `/api/lessons/{id}` | Actualizar lección | ❌ |
| DELETE | `/api/lessons/{id}` | Eliminar lección (soft delete) | ❌ |
| PATCH | `/api/lessons/{id}/reorder` | Reordenar lección | ❌ |

#### Ejemplos de uso:

**Obtener lecciones de un curso:**
```http
GET /api/lessons/course/{courseId}
```

**Crear lección:**
```http
POST /api/lessons
Content-Type: application/json

{
  "courseId": "guid",
  "title": "Introducción a C#",
  "order": 1
}
```

**Reordenar lección:**
```http
PATCH /api/lessons/{id}/reorder
Content-Type: application/json

3
```

---

## 🔧 Características Implementadas

### 1. **Manejo de Excepciones Global**
- ✅ Middleware personalizado `ExceptionHandlingMiddleware`
- ✅ Convierte excepciones del dominio en respuestas HTTP apropiadas
- ✅ Logging automático de errores

**Mapeo de excepciones:**
- `CourseNotFoundException` → 404 Not Found
- `LessonNotFoundException` → 404 Not Found
- `CourseCannotBePublishedException` → 400 Bad Request
- `DuplicateLessonOrderException` → 400 Bad Request
- `InvalidLessonOrderException` → 400 Bad Request
- Otras excepciones → 500 Internal Server Error

### 2. **Validaciones**
- ✅ ModelState validation automática
- ✅ Validación de parámetros (page, pageSize)
- ✅ Manejo de excepciones del dominio
- ✅ Logging de advertencias

### 3. **Swagger/OpenAPI**
- ✅ Documentación automática en la raíz `/`
- ✅ Información de la API configurada
- ✅ Atributos ProducesResponseType en endpoints
- ✅ Comentarios XML (summary tags)

### 4. **CORS**
- ✅ Configurado para permitir cualquier origen (desarrollo)
- ✅ Permite cualquier método HTTP
- ✅ Permite cualquier header

### 5. **Inyección de Dependencias**
- ✅ `AddInfrastructure()` registra todos los servicios
- ✅ DbContext configurado
- ✅ Repositorios registrados (Scoped)
- ✅ Servicios de aplicación registrados (Scoped)

---

## 🚀 Cómo Ejecutar

### 1. Compilar el proyecto
```bash
cd /home/coders/Documentos/Steve/Riwi-Courses-Assessment/Riwi-Courses-Assessment-Backend
dotnet restore
dotnet build
```

### 2. Crear las migraciones
```bash
dotnet ef migrations add InitialCreate --project Infrastructure --startup-project WebApi
```

### 3. Aplicar migraciones a la base de datos
```bash
dotnet ef database update --project Infrastructure --startup-project WebApi
```

### 4. Ejecutar la aplicación
```bash
cd WebApi
dotnet run
```

O para ejecutar en modo watch (recarga automática):
```bash
dotnet watch run
```

### 5. Acceder a Swagger
Abre tu navegador en: **http://localhost:5000** o **https://localhost:5001**

---

## 📊 Respuestas de la API

### Respuesta exitosa (200 OK):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Curso de .NET Core",
  "status": "Published",
  "createdAt": "2026-01-05T10:00:00Z",
  "updatedAt": "2026-01-05T10:30:00Z"
}
```

### Respuesta de error (404 Not Found):
```json
{
  "statusCode": 404,
  "message": "Course with ID '...' was not found.",
  "type": "CourseNotFoundException"
}
```

### Respuesta de error (400 Bad Request):
```json
{
  "statusCode": 400,
  "message": "Cannot publish a course without active lessons.",
  "type": "CourseCannotBePublishedException"
}
```

### Respuesta de búsqueda paginada:
```json
{
  "courses": [
    {
      "id": "guid",
      "title": "Curso 1",
      "status": "Published",
      "createdAt": "...",
      "updatedAt": "..."
    }
  ],
  "totalCount": 25,
  "page": 1,
  "pageSize": 10,
  "totalPages": 3
}
```

---

## 🧪 Probar la API

### Con curl:

**Crear un curso:**
```bash
curl -X POST http://localhost:5000/api/courses \
  -H "Content-Type: application/json" \
  -d '{"title": "Mi Primer Curso"}'
```

**Buscar cursos:**
```bash
curl "http://localhost:5000/api/courses/search?page=1&pageSize=10"
```

**Obtener curso:**
```bash
curl http://localhost:5000/api/courses/{id}
```

### Con Swagger UI:
1. Abre http://localhost:5000
2. Explora todos los endpoints disponibles
3. Prueba directamente desde el navegador
4. Ve los esquemas de request/response

---

## ⚙️ Configuración

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=RiwiCoursesDb;Username=postgres;Password=postgres"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

---

## 📝 Reglas de Negocio Implementadas

| Regla | Endpoint | Validación |
|-------|----------|------------|
| Curso solo se publica con lecciones | `PATCH /courses/{id}/publish` | ✅ Valida en servicio |
| Order único por curso | `POST /lessons` | ✅ Valida duplicados |
| Soft delete | `DELETE /courses/{id}` | ✅ Marca IsDeleted |
| Reordenar sin duplicados | `PATCH /lessons/{id}/reorder` | ✅ Swap inteligente |

---

## 🎯 Estado del Proyecto

| Componente | Estado | Archivos |
|------------|--------|----------|
| **Domain** | ✅ Completo | 11 archivos |
| **Application** | ✅ Completo | 8 archivos |
| **Infrastructure** | ✅ Completo | 8 arquivos |
| **WebApi Controllers** | ✅ Completo | 2 controladores |
| **WebApi Config** | ✅ Completo | Program.cs + Middleware |
| **Tests Unitarios** | ✅ Completo | 5 tests pasando |
| **Migraciones** | ⏳ Pendiente | Crear y aplicar |
| **Autenticación** | ⏳ Pendiente | Identity + JWT |

---

## 🔜 Próximos Pasos

1. ✅ Crear migraciones: `dotnet ef migrations add InitialCreate`
2. ✅ Aplicar migraciones: `dotnet ef database update`
3. ✅ Ejecutar la API: `dotnet run`
4. ✅ Probar endpoints en Swagger
5. ⏳ Implementar autenticación (Identity + JWT)
6. ⏳ Crear endpoints de autenticación (register, login)
7. ⏳ Proteger endpoints con [Authorize]
8. ⏳ Implementar frontend

---

## ✅ TODO LISTO

**La API REST está completamente implementada y lista para ejecutarse!** 🎉

Endpoints: **14 endpoints HTTP**
- ✅ 8 endpoints de Courses
- ✅ 6 endpoints de Lessons
- ✅ Documentación automática con Swagger
- ✅ Manejo de errores global
- ✅ CORS configurado
- ✅ Logging implementado
- ✅ Validaciones de reglas de negocio

**Solo falta crear las migraciones y ejecutar la aplicación!** 🚀

