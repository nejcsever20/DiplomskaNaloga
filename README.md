🎓 DiplomskaNaloga
DiplomskaNaloga is a web application developed as part of a diploma thesis. Built using ASP.NET Core MVC and Razor Pages, it showcases a secure, responsive, and user-friendly platform for managing user authentication and related functionalities.

📌 Table of Contents
Features

Technologies Used

Project Structure

Getting Started

Usage

Contributing

License

🚀 Features
User Authentication: Secure login and registration using ASP.NET Identity.

Two-Factor Authentication (2FA): Enhanced security with authenticator app integration.

Responsive Design: Mobile-friendly interface with modern UI components.

Custom Styling: Tailored CSS with animations and wave effects for an engaging user experience.

Role Management: Assign and manage user roles and permissions.

Email Confirmation: Verify user emails during registration.

Password Recovery: Reset forgotten passwords securely.

🛠️ Technologies Used
Backend: ASP.NET Core MVC, Razor Pages

Frontend: HTML5, CSS3, JavaScript

Authentication: ASP.NET Identity

Database: Entity Framework Core with SQL Server

Styling: Custom CSS with animations
Profile Readme Generator

📁 Project Structure
The project follows a standard ASP.NET Core structure:

pgsql
Kopiraj
Uredi
DiplomskaNaloga/
├── Areas/
│   └── Identity/
│       └── Pages/
│           └── Account/
├── Controllers/
├── Data/
├── Migrations/
├── Models/
├── Pages/
├── Services/
├── wwwroot/
│   ├── css/
│   └── js/
├── appsettings.json
├── Program.cs
├── diplomska.csproj
└── README.md

🧰 Getting Started
Prerequisites
.NET 8.0 SDK

SQL Server

Visual Studio 2022 or later / Visual Studio Code

Installation
Clone the repository:

git clone https://github.com/nejcsever20/DiplomskaNaloga.git
cd DiplomskaNaloga
Configure the database:

Update the appsettings.json file with your SQL Server connection string:

json
Kopiraj
Uredi
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=DiplomskaNalogaDB;Trusted_Connection=True;"
}
Apply migrations and update the database:


dotnet ef database update
Run the application:


dotnet run
Navigate to https://localhost:5001 in your browser.

📖 Usage
Register: Create a new user account.

Login: Access your account using email and password.

Enable 2FA: Set up two-factor authentication for added security.

Manage Profile: Update personal information and settings.

Logout: Securely sign out of your account.

🤝 Contributing
Contributions are welcome! To contribute:

Fork the repository.

Create a new branch:

git checkout -b feature/YourFeature
Commit your changes:

git commit -m "Add YourFeature"
Push to the branch:

git push origin feature/YourFeature
Open a pull request.

📄 License
This project is licensed under the MIT License. See the LICENSE file for details.

For more information, visit the DiplomskaNaloga GitHub repository.