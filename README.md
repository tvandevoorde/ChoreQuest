# ChoreQuest

A gamified chore management application with account management features.

## Features

- **User Registration**: Create new user accounts with username, email, and password
- **User Login**: Authenticate users with JWT tokens
- **User Profile Management**: View and update user profiles
- **Secure Authentication**: JWT-based authentication with password hashing using BCrypt

## Technology Stack

- **Backend**: ASP.NET Core 9.0 Web API
- **Database**: SQLite with Entity Framework Core
- **Authentication**: JWT Bearer tokens
- **Password Hashing**: BCrypt.Net
- **Testing**: xUnit with Microsoft.AspNetCore.Mvc.Testing

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/tvandevoorde/ChoreQuest.git
cd ChoreQuest
```

### 2. Build the solution

```bash
dotnet build
```

### 3. Run the tests

```bash
dotnet test
```

### 4. Run the API

```bash
cd src/ChoreQuest.Api
dotnet run
```

The API will be available at `http://localhost:5286` (or the port shown in the console output).

## API Endpoints

### Account Management

#### Register a new user
```bash
POST /api/account/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "securepassword123"
}
```

**Response:**
```json
{
  "token": "eyJhbGci...",
  "username": "johndoe",
  "email": "john@example.com"
}
```

#### Login
```bash
POST /api/account/login
Content-Type: application/json

{
  "username": "johndoe",
  "password": "securepassword123"
}
```

**Response:**
```json
{
  "token": "eyJhbGci...",
  "username": "johndoe",
  "email": "john@example.com"
}
```

#### Get user profile (requires authentication)
```bash
GET /api/account/profile
Authorization: Bearer {token}
```

**Response:**
```json
{
  "id": 1,
  "username": "johndoe",
  "email": "john@example.com",
  "createdAt": "2025-10-21T20:00:00"
}
```

#### Update user profile (requires authentication)
```bash
PUT /api/account/profile
Authorization: Bearer {token}
Content-Type: application/json

{
  "email": "newemail@example.com"
}
```

**Response:**
```json
{
  "id": 1,
  "username": "johndoe",
  "email": "newemail@example.com",
  "createdAt": "2025-10-21T20:00:00"
}
```

## Configuration

The application can be configured through `appsettings.json`:

- **ConnectionStrings:DefaultConnection**: SQLite database connection string
- **JwtSettings:SecretKey**: Secret key for JWT token generation (change in production!)
- **JwtSettings:Issuer**: JWT token issuer
- **JwtSettings:Audience**: JWT token audience

## Project Structure

```
ChoreQuest/
├── src/
│   └── ChoreQuest.Api/          # Main Web API project
│       ├── Controllers/          # API controllers
│       ├── Data/                 # Database context
│       ├── DTOs/                 # Data transfer objects
│       ├── Models/               # Entity models
│       └── Services/             # Business logic services
└── tests/
    └── ChoreQuest.Api.Tests/    # Integration tests
```

## Development

### Running in Development Mode

The application uses SQLite for local development. The database file (`chorequest.db`) will be created automatically on first run.

### Running Tests

The test suite uses an in-memory SQLite database for fast, isolated testing:

```bash
dotnet test --verbosity normal
```

## Security Considerations

- Passwords are hashed using BCrypt before storage
- JWT tokens expire after 7 days
- The default JWT secret key is for development only - use a secure key in production
- Consider using environment variables for sensitive configuration in production

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.