This is a README file for ShelfTrade Backend project:

---

# ShelfTrade Backend Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Tech Stack](#tech-stack)
4. [Getting Started](#getting-started)
   - [Prerequisites](#prerequisites)
   - [Installation](#installation)
   - [Environment Variables](#environment-variables)
5. [Database Schema](#database-schema)
6. [API Documentation](#api-documentation)
   - [Authentication](#authentication)
   - [Endpoints](#endpoints)
   - [Error Handling](#error-handling)
7. [Deployment](#deployment)
8. [Security](#security)

---

## Project Overview
The ShelfTrade Backend is a .NET Web API for a book exchange platform that enables users to share, borrow, and exchange books with others. This backend system handles authentication, book management, and notifications, providing endpoints for user registration, login, book data management, and search functionalities.

## System Architecture
The backend architecture consists of several key components:
- **AuthController**: Handles user authentication, including registration, login, password management, and token generation.
- **BooksController**: Manages CRUD operations for books, including adding, viewing, and filtering books.
- **Database**: Stores user and book data, allowing efficient retrieval for search and filtering.
- **JWT Authentication**: Provides secure access to protected endpoints.

## Tech Stack
- **Framework**: .NET Web API
- **Database**: SQL Server (or any compatible database with an ORM setup)
- **Authentication**: JWT (JSON Web Token)
- **Email Service**: SMTP for sending notifications

---

## Getting Started

### Prerequisites
- .NET SDK (version compatible with .NET Core or later)
- SQL Server or compatible database
- SMTP server details for email service (for notifications)
- Postman or similar tool for API testing (optional)

### Installation
1. Clone this repository:
   ```bash
   git clone https://github.com/your-username/ShelfTrade-Backend.git
   ```
2. Navigate to the project directory:
   ```bash
   cd ShelfTrade-Backend
   ```
3. Install dependencies:
   ```bash
   dotnet restore
   ```

### Environment Variables
Create a `.env` file in the root directory and configure the following variables:

```plaintext
DATABASE_CONNECTION_STRING="Your SQL Server connection string"
JWT_SECRET_KEY="Your JWT secret key"
EMAIL_SMTP_SERVER="Your SMTP server"
EMAIL_PORT="SMTP server port"
EMAIL_USER="SMTP email username"
EMAIL_PASSWORD="SMTP email password"
```

---

## Database Schema

### Users Table
| Field               | Type    | Description                  |
|---------------------|---------|------------------------------|
| UserId              | int     | Primary Key                  |
| Email               | string  | User's email                 |
| Password            | string  | Encrypted password           |
| Name                | string  | User's full name             |
| FavouriteGenre      | string  | User's favorite genre        |
| ReadingPreferences  | string  | User's reading preferences   |

### Books Table
| Field               | Type    | Description                  |
|---------------------|---------|------------------------------|
| BookId              | int     | Primary Key                  |
| Title               | string  | Book title                   |
| Author              | string  | Author's name                |
| Genre               | string  | Book genre                   |
| Condition           | string  | Book condition               |
| AvailabilityStatus  | string  | Availability status          |
| Location            | string  | Book's location              |
| UserId              | int     | Foreign key (User owner)     |

---

## API Documentation

### Authentication
- **JWT (Bearer token)** is used for secure access to API endpoints.
- Token must be included in the `Authorization` header as `Bearer <token>`.

### Endpoints
#### AuthController
- **POST** `/api/auth/register` - Register a new user.
- **POST** `/api/auth/login` - Log in and obtain a JWT token.
- **POST** `/api/auth/reset-password` - Reset password with verification.

#### BooksController
- **GET** `/api/books` - Get a list of books.
- **GET** `/api/books/excludeUser/{userId}` - Get books excluding those of a specific user.
- **POST** `/api/books` - Add a new book.
- **DELETE** `/api/books/{bookId}` - Delete a book by ID.

### Error Handling
- Standardized error responses:
  - **400** Bad Request: Input validation errors.
  - **401** Unauthorized: Invalid or missing JWT token.
  - **404** Not Found: Requested resource does not exist.
  - **500** Internal Server Error: General server errors.

---

## Deployment
1. **Prepare Environment**: Ensure all environment variables are set in the deployment environment.
2. **Build and Publish**:
   ```bash
   dotnet publish -c Release -o out
   ```
3. **Run the Application**:
   - On a hosting server, configure your database and SMTP settings and start the application using:
   ```bash
   dotnet out/ShelfTrade-Backend.dll
   ```
4. **Monitor and Log**: Set up application monitoring and logging (e.g., Application Insights).

---

## Security
- **JWT Authentication**: Ensures only authenticated users can access protected endpoints.
- **Password Hashing**: Passwords are securely stored with a hashing algorithm (e.g., BCrypt).
- **Environment Variables**: Sensitive information (e.g., database credentials, SMTP) is kept in environment variables.
- **HTTPS**: Ensure HTTPS is enabled for secure data transmission.

---

This README provides a comprehensive overview of the ShelfTrade backend, including setup instructions, API details, and security practices.
