# User Management API

A RESTful API for managing users, built with **ASP.NET Core (.NET 8)**.  
Developed with assistance from **Microsoft Copilot** for code generation, debugging, and middleware implementation.

---

## Features

- ✅ Full CRUD — GET, POST, PUT, DELETE for users
- ✅ Input validation on all endpoints
- ✅ Middleware: API Key Authentication + Request Logging + Global Error Handling
- ✅ Swagger UI for testing endpoints
- ✅ Proper HTTP status codes (200, 201, 204, 400, 401, 404, 409, 500)

---

## Endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/users` | Get all users |
| GET | `/api/users/{id}` | Get user by ID |
| POST | `/api/users` | Create a new user |
| PUT | `/api/users/{id}` | Update a user |
| DELETE | `/api/users/{id}` | Delete a user |

---

## How to Run

```bash
cd UserManagementAPI
dotnet run
```

Then open: `http://localhost:5000/swagger`

---

## Authentication

All endpoints require an API key in the request header:

```
X-API-Key: nexbridge-secret-2024
```

---

## Create User — Request Body

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "password": "password123"
}
```

## Update User — Request Body

```json
{
  "name": "John Updated",
  "email": "john.updated@example.com"
}
```

---

## Validation Rules

| Field | Rules |
|-------|-------|
| Name | Required, 2–100 characters |
| Email | Required, must contain @ and . |
| Password | Required, minimum 6 characters |

---

## Middleware Pipeline

1. Global Exception Handler
2. HTTPS Redirection
3. API Key Authentication
4. Request Logging
5. Controllers

---

## Microsoft Copilot Usage

Copilot was used to:
- Generate initial controller structure and CRUD scaffolding
- Debug middleware ordering issues
- Suggest DTO patterns for separating input from the model

Copilot suggestions were reviewed and modified — for example:
- API key was moved from hardcoded string → `appsettings.json`
- Logger was placed **after** auth middleware (not before)
- Validation rules were added based on business requirements
