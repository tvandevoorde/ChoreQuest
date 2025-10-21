# ChoreQuest

A shared chore management application built with ASP.NET Web API and Angular. Users can create multiple chore lists, add and assign tasks, set due dates, and track progress. Lists can be shared with others for real-time collaboration.

## Features

### Core Functionality
- **User Management**: Register and login with secure password hashing
- **Multiple Chore Lists**: Create and manage multiple chore lists
- **Task Management**: Add, edit, complete, and delete chores
- **Task Assignment**: Assign chores to specific users
- **Due Dates**: Set due dates and get visual indicators for overdue and upcoming tasks
- **Progress Tracking**: Track completion progress for each chore list
- **List Sharing**: Share chore lists with other users with different permission levels (View, Edit, Admin)
- **Recurring Chores**: Set up chores that repeat daily, weekly, monthly, or yearly
- **Notifications**: Get notified when chores are assigned, due soon, completed, or when lists are shared

### Technical Features
- RESTful API built with ASP.NET Core Web API
- Entity Framework Core with SQLite database
- Angular standalone components with TypeScript
- Responsive UI design
- CORS enabled for local development

## Prerequisites

- .NET 9.0 SDK or later
- Node.js 20.x or later
- npm 10.x or later

## Project Structure

```
ChoreQuest/
├── ChoreQuest.API/          # ASP.NET Web API backend
│   ├── Controllers/         # API controllers
│   ├── Models/             # Domain models
│   ├── Data/               # Database context
│   └── DTOs/               # Data transfer objects
├── ChoreQuest.UI/          # Angular frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/ # UI components
│   │   │   └── services/   # API services
│   └── dist/               # Built Angular app
└── ChoreQuest.sln          # Solution file
```

## Getting Started

### Backend Setup

1. Navigate to the API directory:
   ```bash
   cd ChoreQuest.API
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the API:
   ```bash
   dotnet run
   ```

   The API will start at `http://localhost:5167` (or `https://localhost:7167` for HTTPS)

### Frontend Setup

1. Navigate to the UI directory:
   ```bash
   cd ChoreQuest.UI
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

3. Start the development server:
   ```bash
   npm start
   ```

   The Angular app will start at `http://localhost:4200`

## Using the Application

### First Time Setup

1. Open `http://localhost:4200` in your browser
2. Click "Register here" to create a new account
3. Fill in your username, email, and password
4. After registration, you'll be automatically logged in to the dashboard

### Creating a Chore List

1. From the dashboard, click "+ New List"
2. Enter a name and description
3. Click "Create"

### Adding Chores

1. Click on a chore list to view its details
2. Click "+ Add Chore"
3. Fill in the chore details:
   - Title and description
   - Assign to a user (optional)
   - Set a due date (optional)
   - Enable recurring if needed and set the pattern
4. Click "Create"

### Sharing a List

1. Open a chore list
2. Click "Share List"
3. Select a user and permission level
4. Click "Share"

### Managing Chores

- Click the checkbox to mark a chore as complete
- Click the trash icon to delete a chore
- Overdue chores are highlighted in red
- Chores due within 3 days are highlighted in yellow
- Recurring chores automatically create the next instance when completed

## API Endpoints

### Users
- `POST /api/users/register` - Register a new user
- `POST /api/users/login` - Login
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID

### Chore Lists
- `GET /api/chorelists?userId={id}` - Get all lists for a user
- `GET /api/chorelists/{id}` - Get a specific list
- `POST /api/chorelists?userId={id}` - Create a new list
- `PUT /api/chorelists/{id}` - Update a list
- `DELETE /api/chorelists/{id}` - Delete a list
- `POST /api/chorelists/{id}/share` - Share a list
- `DELETE /api/chorelists/{id}/share/{shareId}` - Remove a share

### Chores
- `GET /api/chorelists/{listId}/chores` - Get all chores in a list
- `GET /api/chorelists/{listId}/chores/{id}` - Get a specific chore
- `POST /api/chorelists/{listId}/chores` - Create a new chore
- `PUT /api/chorelists/{listId}/chores/{id}` - Update a chore
- `DELETE /api/chorelists/{listId}/chores/{id}` - Delete a chore

### Notifications
- `GET /api/notifications?userId={id}` - Get notifications for a user
- `PUT /api/notifications/{id}/read` - Mark notification as read
- `PUT /api/notifications/read-all?userId={id}` - Mark all as read
- `DELETE /api/notifications/{id}` - Delete a notification

## Database

The application uses SQLite for data storage. The database file (`chorequest.db`) is automatically created in the API directory when the application first runs.

### Models

- **User**: User accounts with authentication
- **ChoreList**: Container for chores
- **Chore**: Individual tasks with assignment, due dates, and recurrence
- **ChoreListShare**: Sharing permissions for lists
- **Notification**: User notifications for various events

## Development

### Running Tests

Backend tests can be run with:
```bash
cd ChoreQuest.API
dotnet test
```

Frontend tests can be run with:
```bash
cd ChoreQuest.UI
npm test
```

### Building for Production

Backend:
```bash
cd ChoreQuest.API
dotnet publish -c Release
```

Frontend:
```bash
cd ChoreQuest.UI
npm run build
```

The built Angular app will be in `ChoreQuest.UI/dist/ChoreQuest.UI/browser`

## Technologies Used

### Backend
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- SQLite
- BCrypt.Net for password hashing
- Swagger/OpenAPI for API documentation

### Frontend
- Angular 20
- TypeScript
- RxJS for reactive programming
- Angular Router for navigation
- Angular Forms for form handling

## License

This project is licensed under the terms included in the LICENSE file.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
