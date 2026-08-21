# Todo

A simple todo list application.

- **Frontend:** Angular 22
- **Backend:** ASP.NET Core 10 Web API
- **Storage:** In-memory (no database)

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js LTS](https://nodejs.org/) (includes npm)

## Backend

### Run the API

```powershell
dotnet restore backend/TodoApi.slnx
dotnet run --project backend/TodoApi
```

The API listens on [http://localhost:5255](http://localhost:5255) (HTTP only).

OpenAPI document (Development): [http://localhost:5255/openapi/v1.json](http://localhost:5255/openapi/v1.json)

You can also exercise endpoints from `backend/TodoApi/TodoApi.http`.

### API

| Method | Route | Description |
|--------|-------|-------------|
| `GET` | `/api/todos` | List all todos |
| `GET` | `/api/todos/{id}` | Get a todo by id |
| `POST` | `/api/todos` | Create a todo (`{ "title": "..." }`) |
| `PUT` | `/api/todos/{id}` | Update a todo (`{ "title": "..." }`) |
| `DELETE` | `/api/todos/{id}` | Delete a todo |

Titles are required, trimmed on save, reject empty/whitespace, and are limited to 200 characters. CORS allows `http://localhost:4200` for the Angular app.

### Test the API

```powershell
dotnet test backend/TodoApi.slnx
```

## Frontend

### Run the UI

In a second terminal:

```powershell
cd frontend/todo-app
npm install
npm start
```

Open [http://localhost:4200](http://localhost:4200). The dev server proxies `/api` to `http://localhost:5255`, so start the API first.

### Test the UI

```powershell
cd frontend/todo-app
npm test
```
