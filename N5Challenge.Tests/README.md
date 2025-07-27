# Tests - N5Challenge

## Estructura de Tests

### Integration/TestBase.cs
Clase base que configura el entorno de testing:
- Configura WebApplicationFactory para testing de la API
- Proporciona métodos para seedear la base de datos

### Integration/PermissionsControllerIntegrationTests.cs
Tests de integración para el controlador de permisos:
- Prueba todos los endpoints HTTP (GET, POST, PUT)
- Verifica respuestas HTTP correctas
- Prueba casos de error y validación
- Incluye un test end-to-end completo

### Unit/GetPermissionsQueryHandlerTests.cs
Tests unitarios para el handler del query GetPermissions

### Unit/ModifyPermissionCommandHandlerTests.cs
Tests unitarios para el handler del command ModifyPermission

### Unit/RequestPermissionCommandHandlerTests.cs
Tests unitarios para el handler del command RequestPermission

## Ejecución

```bash
# Tests del controlador
dotnet test 
```

### Cobertura
- Tests para todos los endpoints de la API
- Tests para todos los handlers de MediatR
