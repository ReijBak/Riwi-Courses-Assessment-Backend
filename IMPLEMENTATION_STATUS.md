# Implementación de Lógica de Negocio y Tests Unitarios

## ✅ Completado

### 1. **Excepciones del Dominio** (`Domain/Exceptions/`)

#### DomainException.cs
- Excepción base abstracta para todas las excepciones del dominio
- Proporciona constructores estándar

#### CourseException.cs
- `CourseCannotBePublishedException`: Cuando se intenta publicar un curso sin lecciones activas
- `CourseNotFoundException`: Cuando no se encuentra un curso
- `CourseAlreadyPublishedException`: Cuando se intenta publicar un curso ya publicado
- `CourseAlreadyDraftException`: Cuando se intenta despublicar un curso ya en borrador

#### LessonException.cs
- `LessonNotFoundException`: Cuando no se encuentra una lección
- `DuplicateLessonOrderException`: Cuando se intenta crear/actualizar una lección con un orden duplicado
- `InvalidLessonOrderException`: Cuando el orden de la lección es inválido (negativo)

### 2. **Interfaces de Repositorio** (`Domain/Interfaces/`)

#### IRepository<T>
- Interfaz genérica base con operaciones CRUD estándar
- `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`, `ExistsAsync`

#### ICourseRepository
- Extiende `IRepository<Course>`
- `SearchAsync`: Búsqueda con paginación y filtros
- `GetByIdWithLessonsAsync`: Obtiene curso con sus lecciones (eager loading)
- `CountAsync`: Cuenta cursos según filtros

#### ILessonRepository
- Extiende `IRepository<Lesson>`
- `GetByCourseIdAsync`: Obtiene todas las lecciones de un curso
- `HasDuplicateOrderAsync`: Valida si existe un orden duplicado
- `GetByOrderAsync`: Obtiene lección por orden
- `GetMaxOrderAsync`: Obtiene el orden máximo en un curso

### 3. **DTOs** (`Application/DTOs/`)

#### CourseDto.cs
- `CourseDto`: DTO para respuestas de cursos
- `CreateCourseDto`: DTO para crear cursos
- `UpdateCourseDto`: DTO para actualizar cursos
- `CourseSummaryDto`: DTO para el resumen del curso (incluye total de lecciones)
- `CourseSearchResultDto`: DTO para resultados paginados

#### LessonDto.cs
- `LessonDto`: DTO para respuestas de lecciones
- `CreateLessonDto`: DTO para crear lecciones
- `UpdateLessonDto`: DTO para actualizar lecciones

### 4. **Servicios de Aplicación** (`Application/Services/`)

#### CourseService
Implementa toda la lógica de negocio para cursos:

**Métodos:**
- `GetByIdAsync`: Obtiene un curso por ID
- `SearchAsync`: Búsqueda con paginación y filtros
- `GetSummaryAsync`: Obtiene resumen del curso (implementa regla de negocio)
- `CreateAsync`: Crea un nuevo curso
- `UpdateAsync`: Actualiza un curso existente
- `DeleteAsync`: Eliminación lógica (soft delete)
- `PublishAsync`: Publica un curso (valida que tenga lecciones activas)
- `UnpublishAsync`: Despublica un curso

**Reglas de Negocio Implementadas:**
✅ Un curso solo puede publicarse si tiene al menos una lección activa
✅ Validación de estado antes de publicar/despublicar
✅ Soft delete implementado
✅ Validación de existencia del curso

#### LessonService
Implementa toda la lógica de negocio para lecciones:

**Métodos:**
- `GetByIdAsync`: Obtiene una lección por ID
- `GetByCourseIdAsync`: Obtiene todas las lecciones de un curso (ordenadas)
- `CreateAsync`: Crea una nueva lección
- `UpdateAsync`: Actualiza una lección existente
- `DeleteAsync`: Eliminación lógica (soft delete)
- `ReorderAsync`: Reordena lecciones (evita duplicados mediante swap)

**Reglas de Negocio Implementadas:**
✅ El orden de las lecciones debe ser único dentro del mismo curso
✅ Validación de orden no negativo
✅ Soft delete implementado
✅ Reordenamiento inteligente que evita duplicados
✅ Validación de existencia del curso al crear lección

### 5. **Tests Unitarios** (`Tests/Services/`)

#### CourseServiceTests.cs

**Test 1: PublishCourse_WithLessons_ShouldSucceed**
- ✅ Verifica que un curso CON lecciones activas puede publicarse
- ✅ Valida que el estado cambia a Published
- ✅ Verifica que se llama a UpdateAsync

**Test 2: PublishCourse_WithoutLessons_ShouldFail**
- ✅ Verifica que un curso SIN lecciones NO puede publicarse
- ✅ Valida que se lanza CourseCannotBePublishedException
- ✅ Verifica que el estado NO cambia
- ✅ Verifica que NO se llama a UpdateAsync

**Test 3: DeleteCourse_ShouldBeSoftDelete**
- ✅ Verifica que la eliminación es lógica (soft delete)
- ✅ Valida que IsDeleted se marca como true
- ✅ Verifica que se llama a UpdateAsync (no a DeleteAsync)

#### LessonServiceTests.cs

**Test 4: CreateLesson_WithUniqueOrder_ShouldSucceed**
- ✅ Verifica que se puede crear una lección con orden único
- ✅ Valida que se llama a AddAsync
- ✅ Verifica los datos de la lección creada

**Test 5: CreateLesson_WithDuplicateOrder_ShouldFail**
- ✅ Verifica que NO se puede crear lección con orden duplicado
- ✅ Valida que se lanza DuplicateLessonOrderException
- ✅ Verifica que NO se llama a AddAsync

## 📊 Estructura Final

```
Domain/
├── Entities/
│   ├── BaseEntity.cs
│   ├── Course.cs (con métodos de negocio)
│   └── Lesson.cs (con métodos de negocio)
├── Enums/
│   └── CourseStatus.cs
├── Exceptions/
│   ├── DomainException.cs
│   ├── CourseException.cs
│   └── LessonException.cs
└── Interfaces/
    ├── IRepository.cs
    ├── ICourseRepository.cs
    └── ILessonRepository.cs

Application/
├── DTOs/
│   ├── CourseDto.cs
│   └── LessonDto.cs
├── Interfaces/
│   ├── ICourseService.cs
│   └── ILessonService.cs
└── Services/
    ├── CourseService.cs
    └── LessonService.cs

Tests/
└── Services/
    ├── CourseServiceTests.cs (3 tests)
    └── LessonServiceTests.cs (2 tests)
```

## 🎯 Reglas de Negocio Implementadas

### ✅ Reglas Obligatorias del Assessment

1. **Un curso solo puede publicarse si tiene al menos una lección activa**
   - Implementado en: `Course.CanBePublished()` y `Course.Publish()`
   - Validado en: `CourseService.PublishAsync()`
   - Test: `PublishCourse_WithLessons_ShouldSucceed` y `PublishCourse_WithoutLessons_ShouldFail`

2. **El campo Order de las lecciones debe ser único dentro del mismo curso**
   - Implementado en: `LessonService.CreateAsync()` y `LessonService.UpdateAsync()`
   - Validado mediante: `ILessonRepository.HasDuplicateOrderAsync()`
   - Test: `CreateLesson_WithUniqueOrder_ShouldSucceed` y `CreateLesson_WithDuplicateOrder_ShouldFail`

3. **La eliminación es lógica (soft delete), no física**
   - Implementado en: `Course.SoftDelete()` y `Lesson.SoftDelete()`
   - Usado en: `CourseService.DeleteAsync()` y `LessonService.DeleteAsync()`
   - Test: `DeleteCourse_ShouldBeSoftDelete`

4. **El reordenamiento de lecciones no debe generar órdenes duplicados**
   - Implementado en: `LessonService.ReorderAsync()` con lógica de swap
   - Valida duplicados antes de actualizar

## 🧪 Ejecutar Tests

```bash
cd Riwi-Courses-Assessment-Backend
dotnet test
```

## 📝 Notas Técnicas

- **Patrón Repository**: Interfaces definidas en Domain, implementaciones en Infrastructure
- **Inyección de Dependencias**: Los servicios reciben repositorios via constructor
- **Tests con Mocks**: Uso de Moq para simular repositorios
- **Excepciones Personalizadas**: Todas heredan de DomainException
- **Separación de Responsabilidades**: 
  - Domain: Entidades y reglas de negocio básicas
  - Application: Casos de uso y orquestación
  - Tests: Validación de lógica de negocio

## ⏭️ Próximos Pasos

1. Implementar capa de Infrastructure con Entity Framework Core
2. Crear DbContext con configuraciones
3. Implementar repositorios concretos
4. Configurar migraciones
5. Implementar autenticación con Identity + JWT
6. Crear controladores en WebApi
7. Configurar Swagger/OpenAPI

