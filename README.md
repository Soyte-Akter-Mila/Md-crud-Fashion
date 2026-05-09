# 👗 MD-Crud-Fashion

A full-featured **ASP.NET Core MVC** web application for managing a fashion store — products, customers, orders, and role-based access control, all in one place.

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat&logo=dotnet&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET_Core-MVC-0078D4?style=flat&logo=microsoft&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/EF_Core-Code--First-68B534?style=flat)
![SQL Server](https://img.shields.io/badge/SQL_Server-Database-CC2927?style=flat&logo=microsoftsqlserver&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=flat&logo=bootstrap&logoColor=white)

---

## ✨ Features

- **Product Management** — Full CRUD for fashion products, restricted to `Admin` and `Manager` roles
- **Customer Management** — Create, edit, and delete customers with image upload, dress size, payment date, and urgent delivery flag
- **Dynamic Order Assignment** — Add or remove multiple products per customer using Ajax partial views (no page reload)
- **Order Statistics** — Aggregation view showing count, max, min, sum, and average of orders per customer
- **Active Orders Widget** — A ViewComponent in the navbar that shows a real-time count of customers with urgent delivery enabled
- **Authentication** — Register and login via ASP.NET Core Identity (email confirmation required)
- **Role Management** — Create roles and assign them to users, restricted to `Admin` only
- **Custom Route Shortcuts** — `/cs` → Customer Index, `/cr` → Customer Create

---

## 🧰 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 10.0) |
| ORM | Entity Framework Core (Code-First + Migrations) |
| Auth & Identity | ASP.NET Core Identity with `IdentityRole` |
| Database | SQL Server |
| Frontend | Bootstrap 5, jQuery, Ajax |
| Image Storage | Server-side file system (`wwwroot/Images/`) |
| Client Libraries | Managed via LibMan |

---

## 📁 Project Structure

```
MD-Crud-Fashion/
│
├── Controllers/
│   ├── HomeController.cs               # Home and Privacy pages
│   ├── CustomersController.cs          # CRUD + image upload + Ajax order assignment
│   ├── ProductsController.cs           # CRUD — Admin & Manager only
│   └── RoleController.cs               # Create roles, assign to users — Admin only
│
├── Models/
│   ├── Customer.cs                     # Customer entity
│   ├── Product.cs                      # Product entity
│   ├── OrderEntry.cs                   # Join table: Customer ↔ Product
│   ├── ErrorViewModel.cs               # Error page model
│   └── ViewModels/
│       ├── CustomerVM.cs               # Customer form ViewModel with IFormFile
│       └── HttpPostedFileBase.cs       # File upload helper stub
│
├── ViewComponents/
│   └── ActiveOrdersViewComponent.cs    # Counts UrgentDelivery orders for navbar
│
├── Views/
│   ├── Customers/
│   │   ├── Index.cshtml                # Customer list with linked products
│   │   ├── Create.cshtml               # New customer + dynamic product rows
│   │   ├── Edit.cshtml                 # Edit customer + re-assign products
│   │   ├── Delete.cshtml               # Delete confirmation
│   │   ├── Aggregation.cshtml          # Order stats dashboard
│   │   └── _addNewProduct.cshtml       # Ajax partial: single product dropdown row
│   ├── Products/
│   │   ├── Index.cshtml
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   └── Delete.cshtml
│   ├── Role/
│   │   ├── Index.cshtml                # Create role form
│   │   └── AssignRole.cshtml           # Assign role to user by email
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   └── Shared/
│       ├── _Layout.cshtml              # Navbar, footer, ActiveOrders ViewComponent
│       ├── _LoginPartial.cshtml        # Login / Logout links
│       ├── _success.cshtml             # Ajax success partial response
│       ├── _error.cshtml               # Ajax error partial response
│       ├── Error.cshtml
│       └── Components/ActiveOrders/
│           └── Default.cshtml          # Renders urgent order count badge
│
├── Areas/
│   └── Identity/Pages/Account/
│       ├── Login.cshtml
│       └── Register.cshtml
│
├── Migrations/                         # EF Core migration files
├── wwwroot/
│   ├── Images/                         # Customer images uploaded here at runtime
│   ├── css/
│   └── lib/                            # Bootstrap, jQuery (via LibMan)
│
├── appsettings.json
├── appsettings.Development.json
├── Program.cs
└── libman.json
```

---

## ⚙️ Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server) or SQL Server Express
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (recommended) or VS Code

### 1. Clone the Repository

```bash
git clone https://github.com/your-username/MD-Crud-Fashion.git
cd MD-Crud-Fashion/MD-Crud-Fashion
```

### 2. Set the Connection String

Edit `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=FashionDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

### 3. Apply Database Migrations

```bash
dotnet ef database update
```

### 4. Create the Image Upload Folder

```bash
mkdir wwwroot/Images
```

### 5. Run

```bash
dotnet run
```

Open `https://localhost:5001` in your browser.

---

## 🔐 Authentication & Authorization

ASP.NET Core Identity is used with **email confirmation required** (`RequireConfirmedAccount = true`). Roles are managed through `IdentityRole`.

| Role | Access |
|---|---|
| **Admin** | Everything — Products, Customers (create/edit/delete), Role Management, Assign Roles |
| **Manager** | Edit and delete Customers, full access to Products |
| *(Authenticated)* | Browse Customers list |
| *(Anonymous)* | Home page only |

### First-Time Setup

1. Register at `/Identity/Account/Register` and confirm your email
2. Log in, then navigate to **CreateRole** in the navbar
3. Create the `Admin` role
4. Navigate to **AssignRole** and assign `Admin` to your account by email
5. Log out and back in — you now have full admin access

---

## 🔄 How Orders Work

The `OrderEntry` table is a join between `Customer` and `Product`:

```
Customer (1) ────< OrderEntry >──── (1) Product
```

When creating or editing a customer, clicking **+** dynamically loads a new product dropdown row via an Ajax call to `Customers/AddNewProduct`. Each row can be individually removed before saving. On save, the controller clears existing entries and re-inserts the current selection — keeping order data clean on every update.

---

## 📊 Aggregation View

Available at `/Customers/Aggregation`, this page calculates order statistics across all customers:

| Stat | Description |
|---|---|
| **Count** | Total number of customers |
| **Max** | Most orders placed by a single customer |
| **Min** | Fewest orders placed by a single customer |
| **Sum** | Total orders across all customers |
| **Average** | Mean orders per customer |

---

## 🗂️ Entity Reference

### `Customer`
| Property | Type | Notes |
|---|---|---|
| `CustomersId` | `int` | Primary Key |
| `CustomersName` | `string?` | Required |
| `PaymentDate` | `DateTime?` | Required, stored as `date` |
| `CustomerSize` | `int` | Dress size |
| `Picture` | `string` | Path under `/Images/` |
| `UrgentDelivery` | `bool` | Drives Active Orders count |
| `OrderEntries` | `ICollection<OrderEntry>` | Navigation property |

### `Product`
| Property | Type | Notes |
|---|---|---|
| `ProductsId` | `int` | Primary Key |
| `ProductsName` | `string` | Required |

### `OrderEntry`
| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | Primary Key |
| `CustomersId` | `int` | FK → Customer |
| `ProductsId` | `int` | FK → Product |

---

## 📸 Screenshots

> *(Add screenshots of your running application here)*

---

## 🤝 Contributing

Pull requests are welcome. For significant changes, please open an issue first to discuss what you'd like to change.

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

> Built with ❤️ using ASP.NET Core MVC on .NET 10.0
