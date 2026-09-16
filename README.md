# 🎫 NetWave Support API

A secure **IT Support Ticket Management REST API** built with **ASP.NET Core Web API**, **Entity Framework Core**, and **SQL Server**.

The project allows customers to create and manage their support tickets while implementing authentication, authorization, ownership protection, refresh tokens, rate limiting, validation, and security logging.

---

## 🚀 Technologies

- ⚙️ ASP.NET Core Web API
- 💻 C#
- 🗄️ SQL Server
- 🔗 Entity Framework Core
- 🔐 JWT Authentication
- ♻️ Refresh Tokens
- 🔑 BCrypt Password Hashing
- 🛡️ Role-Based Authorization
- ⏱️ Rate Limiting
- 📋 Data Annotations Validation
- 🧪 Swagger / OpenAPI

---

## ✨ Main Features

### 🔐 Authentication

Users can register and log in securely.

Passwords are never stored as plain text.

```text
Password
   ↓
BCrypt Hashing
   ↓
PasswordHash
   ↓
Database
```

After successful login, the API generates:

- JWT Access Token
- Refresh Token

---

### ♻️ Refresh Token Rotation

When the access token expires, the refresh token can be used to generate a new token pair.

```text
Refresh Token
      ↓
Validate
      ↓
Check Expiration
      ↓
Check Revocation
      ↓
BCrypt Verify
      ↓
New Access Token
+
New Refresh Token
```

The previous refresh token becomes unusable after rotation.

---

### 🚪 Secure Logout

Logout revokes the user's refresh token.

```text
Logout
   ↓
Verify Refresh Token
   ↓
Revoke Token
   ↓
Token Cannot Be Reused
```

---

## 🎫 Ticket Management

Authenticated customers can:

- ➕ Create a support ticket
- 📋 View their own tickets
- 🔎 View a specific ticket
- ✏️ Update their tickets
- 🗑️ Delete their tickets

New tickets are created with:

```text
Status = Open
```

---

## 👤 Ownership Protection

Customers can only access tickets that belong to them.

```text
Customer
   ↓
Request Ticket
   ↓
Check CustomerId
   ↓
Owner?
 ┌───────┴───────┐
Yes              No
 ↓                ↓
Allow          403 Forbidden
```

Administrators can access tickets belonging to other customers.

---

## 👑 Role-Based Authorization

The API supports:

```text
Customer
Admin
```

Customers can manage their own tickets.

Admins can access all support tickets through protected Admin endpoints.

```csharp
[Authorize(Roles = "Admin")]
```

---

## 🛡️ API Security

The project includes several security mechanisms:

- 🔐 JWT Authentication
- ♻️ Refresh Token Rotation
- 🚪 Refresh Token Revocation
- 🔑 BCrypt Password Hashing
- 👤 Ownership Authorization
- 👑 Role-Based Authorization
- ⏱️ Rate Limiting
- ✅ Request Validation
- 📝 Security Logging

---

## ⏱️ Rate Limiting

Login and refresh endpoints are protected against excessive requests.

The API uses a fixed-window rate limiter:

```text
5 Requests
Per Minute
Per IP Address
```

Exceeding the limit returns:

```text
429 Too Many Requests
```

---

## ✅ Request Validation

Incoming DTOs are validated using **Data Annotations**.

Examples:

```csharp
[Required]
[EmailAddress]
[MinLength(6)]
```

Invalid requests automatically return:

```text
400 Bad Request
```

---

## 📝 Security Logging

The API records important security-related events such as:

- Failed login attempts
- Invalid refresh token attempts
- Expired refresh tokens
- Revoked refresh token usage
- Forbidden ticket access attempts
- Ticket deletion auditing

Example:

```text
Forbidden access attempt by User 2 to Ticket 3
```

---

## 🌐 API Flow

```text
Client / Swagger
       ↓
HTTP Request
       ↓
Rate Limiting
       ↓
Authentication
       ↓
JWT Claims
       ↓
Authorization
       ↓
Controller
       ↓
DTO Validation
       ↓
Entity Framework Core
       ↓
SQL Server
       ↓
HTTP Response
       ↓
JSON
```

---

## 📡 Main Endpoints

### Authentication

```text
POST   /api/Auth/register
POST   /api/Auth/login
POST   /api/Auth/refresh
POST   /api/Auth/logout
```

### Tickets

```text
POST     /api/Tickets
GET      /api/Tickets
GET      /api/Tickets/{id}
PUT      /api/Tickets/{id}
DELETE   /api/Tickets/{id}
```

### Admin

```text
GET      /api/Tickets/all
```

---

## 📊 HTTP Status Codes

| Status | Meaning |
|---|---|
| `200` | OK |
| `201` | Created |
| `204` | No Content |
| `400` | Bad Request |
| `401` | Unauthorized |
| `403` | Forbidden |
| `404` | Not Found |
| `429` | Too Many Requests |

---

## 🗄️ Database

The project uses **SQL Server** with **Entity Framework Core**.

Main entities:

```text
Customer
   │
   │ 1
   │
   │ *
   ▼
SupportTicket
```

A customer can have multiple support tickets.

---

## 🧪 Testing

The API was tested through **Swagger UI**, including:

- ✅ Registration and Login
- ✅ JWT Authentication
- ✅ Refresh Token Rotation
- ✅ Logout / Token Revocation
- ✅ Ticket CRUD Operations
- ✅ Ownership Protection
- ✅ Admin Authorization
- ✅ 400 Validation
- ✅ 401 Unauthorized
- ✅ 403 Forbidden
- ✅ 404 Not Found
- ✅ 429 Rate Limiting
- ✅ Security Logging

---

## 🎯 Project Purpose

This project was built to practice and demonstrate a complete backend API workflow with a strong focus on **API security and authorization**.

Rather than implementing only CRUD operations, the project demonstrates how authentication, authorization, token management, validation, rate limiting, logging, database access, and HTTP responses work together inside a real ASP.NET Core Web API.

---

## 👨‍💻 Author

**Shady Mahmoud**

Backend Developer — C# / .NET

⭐ If you find this project useful, feel free to explore the repository.
