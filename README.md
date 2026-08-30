# Complaint Project

A comprehensive ASP.NET Core web application for submitting, tracking, and managing user complaints. Built with role-based access control, real-time notifications via SignalR, and multi-language support (English & Arabic).

## 🎯 Key Features

### Authentication & Authorization
- Secure user authentication using ASP.NET Core Identity
- Password hashing and session management
- **Role-based access control (RBAC):**
  - **Admin**: Full system access, report generation
  - **Patient**: Submit and track personal complaints
  - **HealthcareProvider**: Assigned complaints, staff replies
  - **PatientServices**: Staff role for complaint management and approval

### Complaint Management
- **Full CRUD operations** on complaints
- **Status workflow:** New → In Progress (Assigned/ReAssigned/Replied/Approve) → Closed
- **Complaint types:** Medical & Non-Medical
- **Multiple categories:** Waiting Time, Billing & Fees, Behavior & Attitude, Medication Errors, Communication, and more
- **Assignment & Re-assignment** to healthcare providers
- **Soft delete support** for data retention

### Rich Features
- **File Attachments**: Upload and manage complaint attachments with validation
- **Staff Replies**: Internal notes and staff responses with audit trail
- **Feedback System**: Patient satisfaction tracking (Satisfied/Not Satisfied)
- **Real-time Notifications**: SignalR-powered in-app notifications
- **Multi-language Support**: Full localization for English (en-US) and Arabic (ar-SA)
- **Audit Trail**: Automatic tracking of changes via EF Core interceptors

## 💾 Tech Stack

| Layer | Technology |
|-------|-----------|
| **Backend** | C# with ASP.NET Core 10.0 |
| **Database** | SQL Server with Entity Framework Core 10.0 |
| **Authentication** | ASP.NET Core Identity |
| **Real-time** | SignalR for live notifications |
| **Frontend** | HTML5, CSS3, Bootstrap 5, JavaScript |
| **Localization** | .NET Core localization framework |

## 📁 Project Structure

```
ComplaintProj/
├── Controllers/
│   ├── AccountController.cs      # Login, Register, Logout
│   ├── ComplaintsController.cs   # Main CRUD operations
│   └── HomeController.cs         # Landing page
├── Models/
│   └── ComplaintModel.cs         # Complaint entity with validations
├── Views/
│   ├── Account/                  # Authentication views
│   ├── Complaints/               # Complaint views
│   └── Shared/                   # Layout & shared views
├── Data/
│   └── AppDbContext.cs           # EF Core context
├── Hubs/
│   └── ComplaintHub.cs           # SignalR hub for notifications
├── Migrations/                   # EF Core migrations
├── Interceptors/
│   └── AuditInterceptor.cs       # Audit trail logging
├── Program.cs                    # Startup configuration
└── appsettings.json              # Configuration & connection strings
```

## 🔄 Complaint Workflow

```
1. Patient Submits Complaint
        ↓
2. Status: "New" (Admin/PatientServices review)
        ↓
3. Status: "In Progress, Assigned" (Assigned to HealthcareProvider)
        ↓
4. Provider Adds Reply → Status: "In Progress, Replied"
        ↓
5. Admin Reviews → Status: "In Progress, Approve"
        ↓
6. Status: "Closed" (Complaint resolved)
        ↓
7. Patient Provides Feedback (Satisfied/Not Satisfied)
        ↓
8. If Not Satisfied → Status: "Reopened, Unresolved" (Returns to step 3)
```

## 🚀 Getting Started

### Prerequisites
- **.NET 10 SDK** installed
- **SQL Server LocalDB** or SQL Server instance
- **Visual Studio 2022** or VS Code (recommended)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/arwaalshebl/Complaint-Project.git
   cd ComplaintProj
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Configure database:**
   - Edit `appsettings.json` and update the connection string if needed
   - Default: `Server=(localdb)\mssqllocaldb;Database=ComplaintDB;...`

4. **Apply migrations:**
   ```bash
   dotnet ef database update
   ```

5. **Run the application:**
   ```bash
   dotnet run
   ```

6. **Access the app:**
   - Navigate to `https://localhost:7000`
   - Default credentials:
     - **Admin:** `admin@gmail.com` / `@Arwa123`
     - **Healthcare Provider:** `provider@gmail.com` / `@Arwa123`
     - **Patient Services:** `services@gmail.com` / `@Arwa123`
   - Or register a new **Patient** account

## 🌍 Localization

The application supports two languages:
- **English (en-US)** — Default
- **Arabic (ar-SA)** — Full RTL support

## 🤝 Contributing

We welcome contributions! Please follow these steps:

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/YourFeatureName`
3. Commit your changes: `git commit -m 'Add YourFeatureName'`
4. Push to the branch: `git push origin feature/YourFeatureName`
5. Open a Pull Request with a clear title and description

---

**Last Updated:** August 30, 2026  
**Repository:** [arwaalshebl/Complaint-Project](https://github.com/arwaalshebl/Complaint-Project)
