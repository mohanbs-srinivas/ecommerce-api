# User Authentication Implementation Plan

## Overview

### Problem Statement
The e-commerce API currently has no authentication or authorization. All endpoints (`/api/customers`, `/api/orders`, `/api/products`, etc.) are publicly accessible, which exposes sensitive customer and order data and allows unauthorized modifications. This plan outlines how to add JWT-based user authentication to secure the API.

### Success Criteria
- Users can register with an email and password
- Registered users can log in and receive a JWT access token
- Protected endpoints reject requests without a valid token (`401 Unauthorized`)
- Admin-only endpoints reject non-admin users (`403 Forbidden`)
- Passwords are never stored in plain text
- All authentication flows have acceptance tests

### Who Will Use This
- **Customers**: Register, log in, and access their own orders and profile
- **Admins**: Manage all customers, products, and orders
- **Client apps** (web/mobile): Consume the API using JWT bearer tokens

---

## Technical Approach

### Authentication Strategy: JWT Bearer Tokens
We will use **JSON Web Tokens (JWT)** with the `Microsoft.AspNetCore.Authentication.JwtBearer` package. On successful login, the API issues a signed JWT that the client includes in the `Authorization: Bearer <token>` header on subsequent requests.

### Password Hashing
Passwords will be hashed using **BCrypt** via the `BCrypt.Net-Next` NuGet package. Plain-text passwords are never stored.

### Roles
Two roles are defined:
- `Customer` – can read and manage their own data
- `Admin` – full access to all resources

### Key Components

| Component | Purpose |
|---|---|
| `User` model | Stores username, email, hashed password, and role |
| `UserService` | Register, authenticate, and look up users |
| `AuthController` | `POST /api/auth/register` and `POST /api/auth/login` endpoints |
| `JwtTokenService` | Generates and validates JWT tokens |
| `appsettings.json` | JWT issuer, audience, and secret key configuration |
| `[Authorize]` attributes | Protect existing controllers/actions |

### Endpoint Security Plan

| Endpoint | Current | After Change |
|---|---|---|
| `GET /api/products` | Open | Open (public catalog) |
| `POST /api/products` | Open | Admin only |
| `PUT /api/products/{id}` | Open | Admin only |
| `DELETE /api/products/{id}` | Open | Admin only |
| `GET /api/customers` | Open | Admin only |
| `GET /api/customers/{id}` | Open | Authenticated (own record or Admin) |
| `POST /api/customers` | Open | Open (self-registration) |
| `PUT /api/customers/{id}` | Open | Authenticated (own record or Admin) |
| `DELETE /api/customers/{id}` | Open | Admin only |
| `GET /api/orders` | Open | Admin only |
| `GET /api/orders/{id}` | Open | Authenticated (own order or Admin) |
| `POST /api/orders` | Open | Authenticated |
| `PUT /api/orders/{id}` | Open | Admin only |
| `DELETE /api/orders/{id}` | Open | Admin only |
| `POST /api/auth/register` | N/A | Open |
| `POST /api/auth/login` | N/A | Open |

### Data Flow
```
Client → POST /api/auth/login (email + password)
       ← 200 OK { token: "eyJ..." }

Client → GET /api/orders  (Authorization: Bearer eyJ...)
       ← 200 OK [ ...orders... ]

Client → GET /api/orders  (no token)
       ← 401 Unauthorized
```

---

## Implementation Plan

### Phase 1: Foundation (Day 1)

**1.1 Add NuGet Packages**
- `Microsoft.AspNetCore.Authentication.JwtBearer` (8.x) — JWT middleware
- `BCrypt.Net-Next` (4.x) — password hashing
- Complexity: **Small**

**1.2 Create `User` Model** (`Models/User.cs`)
```csharp
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }  // "Customer" or "Admin"
}
```
- Complexity: **Small**

**1.3 Add JWT Configuration to `appsettings.json`**
```json
"Jwt": {
  "Key": "<minimum-32-character-secret-key>",
  "Issuer": "ecommerce-api",
  "Audience": "ecommerce-clients",
  "ExpiryMinutes": 60
}
```
- Complexity: **Small**
- **Note**: In production, the secret key must be stored in environment variables or a secrets manager (Azure Key Vault, AWS Secrets Manager), never in source control.

**1.4 Create `JwtTokenService`** (`Services/JwtTokenService.cs`)
- Reads JWT config from `IConfiguration`
- `GenerateToken(User user)` → creates and signs a JWT with `sub`, `email`, `role` claims and expiry
- Complexity: **Small**

**1.5 Create `UserService`** (`Services/UserService.cs`)
- `RegisterAsync(string username, string email, string password, string role)` → hashes password, saves user, returns `User`
- `AuthenticateAsync(string email, string password)` → finds user by email, verifies hash, returns `User` or `null`
- `GetByIdAsync(int id)` → look up user
- In-memory `List<User>` store (consistent with existing services)
- Complexity: **Small**

**1.6 Register Authentication in `Program.cs`**
- Add `services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)`
- Add `services.AddAuthorization()`
- Add `app.UseAuthentication()` before `app.UseAuthorization()`
- Register `JwtTokenService` and `UserService` as singletons
- Complexity: **Small**

---

### Phase 2: Core Functionality (Day 2)

**2.1 Create `AuthController`** (`Controllers/AuthController.cs`)

`POST /api/auth/register`
- Request body: `{ "username": "...", "email": "...", "password": "...", "role": "Customer" }`
- Validates that email is not already registered
- Calls `UserService.RegisterAsync()`
- Returns `201 Created` with the new user (excluding password hash)

`POST /api/auth/login`
- Request body: `{ "email": "...", "password": "..." }`
- Calls `UserService.AuthenticateAsync()`
- On success: returns `200 OK { "token": "...", "expiresAt": "..." }`
- On failure: returns `401 Unauthorized`
- Complexity: **Medium**

**2.2 Create Request/Response DTOs**
- `RegisterRequest` – username, email, password, role
- `LoginRequest` – email, password
- `AuthResponse` – token, expiresAt
- `UserResponse` – id, username, email, role (no password hash)
- Complexity: **Small**

**2.3 Apply `[Authorize]` to Existing Controllers**
- Add `[Authorize]` at controller level where appropriate
- Add `[Authorize(Roles = "Admin")]` for admin-only actions
- Add `[AllowAnonymous]` for public actions (e.g., `GET /api/products`)
- Complexity: **Medium**

**2.4 Add Swagger JWT Support**
- Update `AddSwaggerGen` in `Program.cs` to include `SecurityDefinition` for Bearer tokens
- This allows testing authenticated endpoints directly from the Swagger UI
- Complexity: **Small**

**2.5 Seed an Admin User**
- In `MockDataInitializer.Initialize()`, add a default admin user via `UserService.RegisterAsync()`
- Credentials: `admin@example.com` / `Admin@12345`
- Complexity: **Small**

---

### Phase 3: Polish & Deploy (Day 3)

**3.1 Input Validation**
- Validate email format, minimum password length (8 chars), required fields
- Return `400 Bad Request` with descriptive error messages on validation failure
- Complexity: **Small**

**3.2 Error Handling**
- Return consistent error response shapes: `{ "error": "Invalid credentials" }`
- Never leak internal details (stack traces, password hashes) in error responses
- Complexity: **Small**

**3.3 Acceptance Tests** (`tests/authentication-acceptance.md`)
- Register: success, duplicate email, missing fields
- Login: success, wrong password, unknown email
- Protected endpoint: with token, without token, wrong role
- Complexity: **Medium**

**3.4 Documentation Updates**
- Update `README.md` with authentication setup instructions
- Document default admin credentials for local development
- Complexity: **Small**

---

## Considerations

### Assumptions
- The in-memory store is acceptable for this phase (no persistent database required yet)
- A single shared JWT secret key is acceptable for development; per-environment keys for production
- Role assignment is trusted at registration time (can be tightened later)

### Constraints
- Must stay on .NET 8 and existing ASP.NET Core patterns
- Must remain consistent with existing in-memory service architecture
- No external identity provider (OAuth/OIDC) in this phase

### Risks

| Risk | Likelihood | Impact | Mitigation |
|---|---|---|---|
| JWT secret leaked via `appsettings.json` in source control | Medium | High | Use environment variables or .NET User Secrets; add `appsettings.json` secret scanning |
| Weak password policy allows brute-force | Medium | Medium | Enforce minimum complexity; add rate limiting in a future phase |
| No token revocation (logout) | Low | Medium | Use short expiry times; implement refresh token + blocklist in Phase 2 |
| In-memory user store lost on restart | High (dev) | Low (dev) | Acceptable for now; replace with EF Core + database in a future phase |
| Role escalation via self-registration as Admin | Medium | High | Restrict `Admin` role assignment to existing admins only; validate role in `RegisterAsync` |

---

## Not Included (Future Phases)
- **Refresh tokens** – long-lived sessions with short-lived access tokens
- **OAuth 2.0 / OpenID Connect** – third-party login (Google, GitHub)
- **Persistent database** – replace in-memory store with EF Core + SQL Server/PostgreSQL
- **Rate limiting** – protect `/api/auth/login` from brute-force attacks
- **Email verification** – confirm email address on registration
- **Password reset flow** – forgot password / reset via email link
- **Multi-factor authentication (MFA)**
- **Audit logging** – track login attempts and security events
