# 🏙️ Municipal Services Web App

A lightweight ASP.NET Core MVC application for reporting municipal issues and viewing local events. Built with .NET 9, Bootstrap, and Razor views.

---

## ✨ Features
- 📝 Report community issues with description, category, and optional image upload
- 🗂️ View a list of all reported issues
- 📅 Browse local community events
- 💾 Simple JSON file storage (no external DB needed)

---

## 📁 Project Structure
```
Task 2 - Issue Report(Web)/
├─ Controllers/           # MVC controllers for Home, Report, Events
├─ Models/                # View models & data models
├─ Data/                  # File-backed stores for issues/events
├─ Views/                 # Razor views (Home, Report, Events, Shared)
├─ App_Data/              # JSON data files (issues.json, events.json)
├─ wwwroot/               # Static files, uploads, CSS/JS, libs
└─ Program.cs             # App bootstrap
```

Key data locations:
- 📄 Issues data: `App_Data/issues.json`
- 📄 Events data: `App_Data/events.json`
- 🖼️ Uploaded images: `wwwroot/uploads/`

---

## 🚀 Getting Started

### 1) Prerequisites
- ✅ .NET SDK 9.0 or later
- ✅ Git (optional)
- ✅ Visual Studio 2022 17.10+ or VS Code (optional)

Verify your .NET install:
```bash
dotnet --version
```
It should report `9.x.x`.

### 2) Clone or extract the project
```bash
# If using git
git clone <your-fork-or-source>
cd "Task 2 - Issue Report(Web)"
```
Or open the folder directly in your IDE.

---

## 🧱 Build
Using the .NET CLI:
```bash
# From the project folder containing the .csproj
dotnet restore
dotnet build --configuration Release
```

Using Visual Studio:
- Open the solution/folder
- Set `Task 2 - Issue Report(Web).csproj` as startup project (if needed)
- Build ➜ Build Solution

---

## ▶️ Run
Using the .NET CLI:
```bash
# Development run with hot reload
dotnet run --project "Task 2 - Issue Report(Web).csproj"
```
The app will start on a local HTTP/HTTPS port (shown in the console). Typical URLs:
- `https://localhost:xxxx`
- `http://localhost:xxxx`

Using Visual Studio:
- Select the `Task 2 - Issue Report(Web)` profile
- Click Run (IIS Express or Project)

Launch pages:
- 🏠 Home: `/`
- ⚠️ Report Issue: `/Report`
- 📋 View Reports: `/Report/List`
- 📆 Events: `/Events`

---

## 🧭 How to Use
1) Go to `/Report` to submit a new issue:
   - Enter title, description, and choose a category
   - Optionally upload an image (stored under `wwwroot/uploads/`)
   - Submit to save into `App_Data/issues.json`
2) Visit `/Report/List` to see all submitted issues
3) Browse `/Events` to view local events from `App_Data/events.json`

Data persists in the JSON files so you can stop/start the app without losing entries.

---

## 🛠️ Configuration
- App settings: `appsettings.json` and `appsettings.Development.json`
- Launch profiles: `Properties/launchSettings.json`
- Static files and styles: `wwwroot/`

No database configuration is required. The stores in `Data/` read and write JSON in `App_Data/`.

---

## 🧪 Sample Data
The app includes starter JSON files:
- `App_Data/events.json`
- `App_Data/issues.json`

You can edit these files to seed content before running the app. The app will create files if missing.

---

## 🧩 Tech Stack
- ASP.NET Core MVC (.NET 9)
- Razor Views
- Bootstrap and jQuery (bundled under `wwwroot/lib`)

---

## 🐞 Troubleshooting
- 🔐 HTTPS dev certificate
  ```bash
  dotnet dev-certs https --trust
  ```
  Re-run the app after trusting the certificate.

- 🔌 Port in use
  - Stop any process using the same port, or
  - Remove `Properties/launchSettings.json` to regenerate, or
  - Run with a custom port: `dotnet run --urls "http://localhost:5005;https://localhost:5006"`

- 📦 Restore issues
  ```bash
  dotnet restore --no-cache
  ```

- 📝 File write permissions
  - Ensure the app has permission to write to `App_Data/` and `wwwroot/uploads/`.

---

## 📦 Publish (optional)
Create a self-contained or framework-dependent build:
```bash
# Framework-dependent (Windows x64 example)
dotnet publish -c Release -r win-x64 --no-self-contained

# Self-contained (no global .NET required)
dotnet publish -c Release -r win-x64 --self-contained true
```
Output will be under `bin/Release/<TFM>/<RID>/publish/`.

---

## 🙌 Credits
Created for a municipal services issue reporting and events showcase. Icons and libraries courtesy of Bootstrap and jQuery. 🚀
