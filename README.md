# PayRetailers

This is a .NET 8 solution that implements a clean, modular architecture following Domain-Driven Design (DDD) principles. The system integrates with external providers and persists data using PostgreSQL.

## 🚀 How to Run the Project

### 🐳 Using Docker Compose

To start the application with Docker and its dependencies (e.g., PostgreSQL, Portainer), run:

```bash
docker compose up -d
```

Once the services are running, you can access:

- **API (Swagger)**: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)
- **Portainer (UI for Docker)**: [http://localhost:9000](http://localhost:9000)

> ℹ️ Make sure Docker Engine is running before executing the command.

---

### 🧪 Using Visual Studio (F5 / Debug)

If you run the API directly from Visual Studio, Swagger will be available at one of the following URLs:

- **HTTP:** [http://localhost:5166/swagger/index.html](http://localhost:5166/swagger/index.html)
- **HTTPs:** [https://localhost:7239/swagger/index.html](https://localhost:7239/swagger/index.html)
- **Docker:** [https://localhost:55000/swagger/index.html](https://localhost:55000/swagger/index.html)

You can find the correct port in the **launchSettings.json** file or in the Visual Studio output window.

---

## 🛢️ Database (PostgreSQL)

This project uses **PostgreSQL** as the main data store, but the code is designed to be easily portable to other relational databases.

To manage the DB you can use [pgAdmin](https://www.pgadmin.org/download/).

**Default Connection String**:

```
Host=localhost;Port=5432;Database=integrations;Username=pr;Password=pr123
```

Make sure PostgreSQL is running and accessible on `localhost:5432`.

---

## 🧱 Technologies

- .NET 8
- Entity Framework Core
- PostgreSQL
- Docker + Docker Compose
- Moq, xUnit (Unit & Integration Tests)

---

## 📁 Project Structure

The solution follows Clean Architecture with DDD:

- `Domain`: Entities, Enums, Domain Services
- `Application`: Use Cases, DTOs, Interfaces
- `Infrastructure`: EF Core, HTTP clients, caching, external integrations
- `Api`: ASP.NET Core Web API (entrypoint)
- `Tests`: Unit and integration tests using xUnit + Moq

---

## 📦 Commands

Build the solution:

```bash
dotnet build
```

Run all tests:

```bash
dotnet test
```

---

## 📝 Notes

- Swagger is enabled by default on DEV environment.
- Caching can be in-memory (for development) or Redis (in production).
- You can modify the connection string from `appsettings.json`.
