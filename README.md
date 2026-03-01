# Task Manager API
A RESTful Web API using ASP.NET Core in .NET 8.0 and Entity Framework Core implementing task management with user authentication.

## Tech Stack
- .NET 8 / ASP.NET Core Web API
- Entity Framework Core
- ASP.NET Core Identity
- SQLite
- xUnit (integration tests)

## Prerequisites

- .NET 8 SDK
- Entity Framework Core CLI

```bash
dotnet tool install --global dotnet-ef
```
  
### Setup
```bash
git clone https://github.com/boyanbotev/task-manager-api.git
cd task-manager-api/src/WorkApi
dotnet restore
dotnet ef database update
dotnet run
```

### Run tests:

```bash
dotnet test
```

---

## API Endpoints

### Authentication

* `POST /auth/register`
* `POST /auth/login`

Returns JWT token to be included as:

```
Authorization: Bearer {token}
```

### Tasks (Authenticated)

* `GET /tasks`
* `POST /tasks`
* `PUT /tasks/{id}`
* `DELETE /tasks/{id}`

---

You can also view the Swagger documentation at `localhost:<your_port>/swagger`.
