# SyntaxSquad - Retail Ordering System

# Team Members:
- **Vikash Agrahari A2305222453**
- **Khushi Jain A023167022042**
- **Akshit Arora A023167022053**
- **Harshit Juneja A41105222053**


## Overview

This is a full-stack retail ordering application built with ASP.NET Core backend and Angular frontend. The system allows users to browse products, manage shopping carts, and place orders through a modern web interface.

## Tech Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **Language**: C#
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger/OpenAPI
- **ORM**: Entity Framework Core
- **Password Hashing**: BCrypt.Net-Next

### Frontend
- **Framework**: Angular 19
- **Language**: TypeScript
- **Styling**: Bootstrap 5
- **Icons**: Bootstrap Icons
- **Build Tool**: Angular CLI

### Database
- **Type**: SQL Server
- **Migration Tool**: Entity Framework Core Migrations

## Prerequisites

Before running the application, ensure you have the following installed:

1. **.NET 8.0 SDK** - Download from [Microsoft](https://dotnet.microsoft.com/download/dotnet/8.0)
2. **Node.js** (version 18 or higher) - Download from [nodejs.org](https://nodejs.org/)
3. **Angular CLI** - Install globally with `npm install -g @angular/cli`
4. **SQL Server** - Local instance or Docker container
   - For Docker: `docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=PizzaOrder@123" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest`

## Setup and Installation

### 1. Clone the Repository
```bash
git clone <repository-url>
cd frontend-master (2)
```

### 2. Backend Setup
```bash
cd backend

# Restore NuGet packages
dotnet restore

# Apply database migrations
dotnet ef database update

# Run the backend API
dotnet run
```

The backend will start on `https://localhost:5001` (or `http://localhost:5000` in development).

### 3. Frontend Setup
```bash
cd ../frontend-master

# Install dependencies
npm install

# Start the development server
npm start
# or
ng serve
```

The frontend will start on `http://localhost:4200`.

## Database Configuration

The application uses SQL Server with the following connection string (configured in `backend/appsettings.Development.json`):

```
Server=localhost,1433;Database=RetailOrderingDB;User Id=sa;Password=PizzaOrder@123;TrustServerCertificate=True;
```

### Database Seeding

The application automatically seeds the database with sample data on startup:
- **Brands**: Pizza Palace, Beverage Co, Appetizer House
- **Categories**: Pizzas, Drinks, Appetizers
- **Products**: Sample products in each category with placeholder images

## API Documentation

When running in development mode, Swagger UI is available at:
- `https://localhost:5001/swagger`

## Project Structure

```
frontend-master (2)/
├── backend/                    # ASP.NET Core API
│   ├── Controllers/           # API Controllers
│   ├── Models/                # Entity Models
│   ├── Data/                  # Database Context
│   ├── Services/              # Business Logic Services
│   ├── Migrations/            # EF Core Migrations
│   └── appsettings.json       # Configuration
├── frontend-master/           # Angular Application
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/    # Angular Components
│   │   │   ├── services/      # Angular Services
│   │   │   └── models/        # TypeScript Models
│   │   └── ...
│   └── package.json
└── README.md
```

## Workflow

### User Registration and Authentication
1. User registers with email, password, and personal details
2. JWT token is issued upon successful login
3. Token is used for subsequent API requests

### Product Browsing
1. Users can browse products by categories
2. Each category belongs to a brand
3. Products display name, description, price, and image

### Shopping Cart Management
1. Users can add products to their cart
2. Cart persists across sessions
3. Users can update quantities or remove items

### Order Placement
1. Users can place orders from their cart
2. Orders have status tracking (Pending → Confirmed → Processing → Shipped → Delivered)
3. Order history is maintained

### Admin Features (Future)
- Product management
- Category management
- Order processing
- User management

## ER Diagram



![ER Diagram]
<img width="895" height="598" alt="image" src="https://github.com/user-attachments/assets/75d0963e-7805-4457-a3c9-1946c9e59ddb" />


### Entities and Relationships

- **User**: Stores user information (email, password, name, phone, address)
- **Brand**: Represents product brands (Pizza Palace, Beverage Co, etc.)
- **Category**: Product categories belonging to brands (Pizzas, Drinks, Appetizers)
- **Product**: Individual products with pricing and inventory
- **Cart**: User's shopping cart
- **CartItem**: Items in a cart with quantities
- **Order**: Customer orders with status tracking
- **OrderItem**: Items in an order with purchase price

**Relationships**:
- User → Cart (1:1)
- Cart → CartItem (1:many)
- CartItem → Product (many:1)
- User → Order (1:many)
- Order → OrderItem (1:many)
- OrderItem → Product (many:1)
- Brand → Category (1:many)
- Category → Product (1:many)

## Application Workflow

*Insert Workflow Diagram here*

<img width="1091" height="662" alt="image" src="https://github.com/user-attachments/assets/a7bbee2f-da8d-4698-af21-0b61dbe0d38b" />


### High-Level Flow
1. **User Registration/Login** → JWT Token Generation
2. **Browse Categories** → View Products by Category
3. **Add to Cart** → Update Cart Items
4. **Checkout** → Create Order
5. **Order Processing** → Status Updates
6. **Delivery** → Order Completion

## Development Commands

### Backend
```bash
# Run with hot reload
dotnet watch run

# Create new migration
dotnet ef migrations add <MigrationName>

# Update database
dotnet ef database update

# Build for production
dotnet publish -c Release
```

### Frontend
```bash
# Run with hot reload
ng serve

# Build for production
ng build --prod

# Run tests
ng test

# Generate component
ng generate component <component-name>
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License.
