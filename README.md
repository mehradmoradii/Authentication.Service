# 🔐 Authentication & Authorization Service (.NET)

A production-ready **Authentication & Authorization API** built with **ASP.NET (.NET 8/9)** following **DDD + CQRS** principles.  
The system provides **secure JWT authentication**, **refresh token rotation**, **Redis-backed session management**, and **fine-grained authorization**.

This project is designed as a **scalable auth microservice** usable by web, mobile, and SPA clients.

---

## 🚀 Features

### 🔑 Authentication
- JWT **Access Tokens**
- **Refresh Token Rotation**
- Token **blacklisting** on logout
- Prevents reuse of revoked tokens
- IP-bound refresh tokens

### 🛡 Authorization
- Role-based authorization
- Per-user URL permission validation
- Redis-cached permissions for high performance
- Middleware-based authorization pipeline

### 🧠 Security
- Stateless access tokens
- Stateful refresh sessions (Redis)
- Logout invalidates tokens immediately
- Prevents re-login when already authenticated
- Token expiration & replay protection

### ⚡ Performance
- Redis caching for:
  - Refresh tokens
  - Permissions
  - Token blacklist
- Minimal database hits
- Optimized middleware flow

---

## 🧱 Architecture

The project follows **Domain-Driven Design (DDD)** with **CQRS**:

