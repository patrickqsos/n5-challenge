# Tests - N5Challenge

## Test Structure

### Integration/TestBase.cs
Base class that sets up the testing environment:
- Configures WebApplicationFactory for API testing
- Provides methods to seed the database

### Integration/PermissionsControllerIntegrationTests.cs
Integration tests for the permissions controller:
- Tests all HTTP endpoints (GET, POST, PUT)
- Verifies correct HTTP responses
- Tests error cases
- Includes a complete end-to-end test

### Unit/GetPermissionsQueryHandlerTests.cs
Unit tests for the GetPermissions query handler

### Unit/ModifyPermissionCommandHandlerTests.cs
Unit tests for the ModifyPermission command handler

### Unit/RequestPermissionCommandHandlerTests.cs
Unit tests for the RequestPermission command handler

## Execution

```bash
dotnet test
```

### Coverage
- Tests for all API endpoints
- Tests for all MediatR handlers
