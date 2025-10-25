# ChoreQuest Development Guide

## Architecture Overview

ChoreQuest is a dual-project solution with a **ASP.NET Core Web API** backend and **Angular standalone components** frontend. The API uses Entity Framework Core with SQLite for data persistence and follows a straightforward controller-service pattern without explicit service layer abstraction.

### Key Components
- **Backend**: `ChoreQuest.API/` - .NET 9.0 Web API with EF Core and SQLite
- **Frontend**: `ChoreQuest.UI/` - Angular 20 with standalone components and TypeScript
- **Database**: SQLite file (`chorequest.db`) with auto-creation via `EnsureCreated()`

## Development Workflow

### Running the Application
```powershell
# Backend (run from ChoreQuest.API/)
dotnet restore && dotnet run  # Starts at localhost:5169

# Frontend (run from ChoreQuest.UI/)  
npm install && npm start     # Starts at localhost:4200
```

### Database Management
- No migrations used - relies on `DbContext.Database.EnsureCreated()` in `Program.cs`
- Schema changes require deleting `chorequest.db` and restarting the API
- All database configuration is in `ChoreQuestDbContext.OnModelCreating()`

## Code Patterns & Conventions

### Backend Architecture
- **Controllers**: Direct EF Core queries in controllers (no service layer)
- **Include Patterns**: Always include related entities: `Include(cl => cl.Owner).Include(cl => cl.Chores).Include(cl => cl.Shares).ThenInclude(s => s.SharedWithUser)`
- **DTO Mapping**: Manual mapping in controller methods using `MapToDto()` private methods
- **Query Parameters**: Use `[FromQuery]` for filtering (e.g., `userId` parameter)

### Entity Relationships (Critical for Queries)
```csharp
// User -> ChoreList (One-to-Many as Owner)
// User -> Chore (One-to-Many as AssignedTo, nullable)
// ChoreList -> ChoreListShare (One-to-Many for sharing)
// ChoreList -> Chore (One-to-Many with cascade delete)
```

### Frontend Patterns
- **Standalone Components**: All components use `standalone: true` with explicit imports
- **Services**: Injectable services with `providedIn: 'root'` and `HttpClient` injection
- **State Management**: Simple `BehaviorSubject` for auth state, no NgRx
- **API Base URL**: Hardcoded `http://localhost:5169/api` in services

### Authentication Pattern
- Simple username/password with BCrypt hashing (no JWT tokens)
- Frontend stores user object in `localStorage` via `AuthService`
- No authorization middleware - permission checks in controllers

## Project-Specific Quirks

### Recurring Chores Logic
- Completion triggers creation of next instance based on `RecurrencePattern` enum
- Handle in `ChoresController.UpdateChore()` when `IsCompleted` changes to `true`

### Notification System
- Created automatically for assignments, completions, and shares
- Manual cleanup required - no automatic expiration

### CORS Configuration
- Specific policy for Angular dev server: `WithOrigins("http://localhost:4200")`
- Required for local development setup

### File Organization
- DTOs in separate files by entity type: `ChoreDtos.cs`, `ChoreListDtos.cs`
- Models use nullable reference types with EF Core configuration
- Angular services follow entity-based naming: `auth.service.ts`, `chore-list.service.ts`

## Common Tasks

### Adding New Entities
1. Create model in `Models/` with navigation properties
2. Add `DbSet<T>` to `ChoreQuestDbContext`
3. Configure relationships in `OnModelCreating()`
4. Create DTOs in `DTOs/` for API contracts
5. Add controller with standard CRUD + Include patterns

### Database Schema Changes
Delete `chorequest.db` file and restart API - no migration support

### Adding API Endpoints
Follow existing pattern: `[HttpGet]` with query parameters, Include related entities, map to DTOs manually