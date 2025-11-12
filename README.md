# 🏙️ Municipal Services Web App

A lightweight ASP.NET Core MVC application for reporting municipal issues and viewing local events. Built with .NET 9, Bootstrap, and Razor views.

---

## 🎥 Video Demonstration

Watch a full demonstration of the website in action:

[![Watch the Demo](https://img.youtube.com/vi/gl6yZWCxuKY/0.jpg)](https://youtu.be/gl6yZWCxuKY)

[Direct Link: https://youtu.be/gl6yZWCxuKY](https://youtu.be/gl6yZWCxuKY)

---

## ✨ Features
- 📝 Report community issues with description, category, and optional image upload
- 🗂️ View a list of all reported issues
- 📅 Browse local community events
- 🔎 Track Service Request Status (search by Request ID, filter by status/category, view details, see top priority)
- 💾 Simple JSON file storage (no external DB needed)

---

## 📁 Project Structure
```
Task 2 - Issue Report(Web)/
├─ Controllers/                       # MVC controllers for Home, Report, Events, ServiceRequestStatus
│  ├─ ServiceRequestStatusController.cs
├─ Models/                            # View models & data models
│  ├─ ServiceRequest.cs               # Trackable request with status, priority, etc.
├─ Data/                              # File-backed stores for issues/events/requests
│  ├─ ServiceRequestStore.cs          # Integrates advanced data structures + persistence
├─ DataStructures/                    # Advanced DS/algorithms used by tracking
│  ├─ BinarySearchTree.cs
│  ├─ AVLTree.cs
│  ├─ RedBlackTree.cs
│  ├─ Heap.cs
│  └─ Graph.cs
├─ Views/
│  ├─ ServiceRequestStatus/
│  │  ├─ Index.cshtml                 # Search/filter list
│  │  ├─ Details.cshtml               # Request details + related requests
│  │  └─ Priority.cshtml              # Top priority queue view
│  ├─ Shared/_Layout.cshtml           # Includes “Track Requests” in nav
├─ App_Data/                          # JSON data files
│  ├─ issues.json
│  ├─ events.json
│  └─ serviceRequests.json            # Auto-created/migrated from issues
├─ wwwroot/                           # Static files, uploads, CSS/JS, libs
└─ Program.cs                         # App bootstrap + DI registration
```

Key data locations:
- 📄 Issues data: `App_Data/issues.json`
- 📄 Events data: `App_Data/events.json`
- 📄 Service requests: `App_Data/serviceRequests.json`
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
- 🔎 Track Requests: `/ServiceRequestStatus`

---

## 🧭 How to Use
1) Submit a new issue (creates a trackable Service Request)
   - Go to `/Report`
   - Enter location, description (≥ 10 chars), and choose a category
   - Optionally upload images (stored under `wwwroot/uploads/`)
   - Submit. You’ll see a success message with your unique Request ID (e.g., `REQ-2025-123456`) and a “Track Your Request” link.
2) Track requests
   - From the navbar, click “Track Requests” or go to `/ServiceRequestStatus`
   - Search using your Request ID OR filter by category/status
   - Click “View Details” to see full information, related requests, and priority/status insights
   - See “Top Priority” to view the highest-priority queue
3) View existing content
   - `/Report/List` shows previously submitted issues
   - `/Events` shows local events from `App_Data/events.json`

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

## ⚙️ Service Request Status — Data Structures & Algorithms

This feature uses multiple advanced data structures to make lookups, ordering, prioritization, and relationship discovery efficient even as the data grows.

### 1) Binary Search Tree (BST)
- Where: `DataStructures/BinarySearchTree.cs`
- Used by: `ServiceRequestStore.GetAllSortedByDate()`, `ServiceRequestStore.SearchBST(Guid id)`
- Role: keeps requests ordered by submission time (and Id as tie-breaker). In-order traversal returns requests sorted by date.
- Why it helps: listing requests chronologically is a core task; in-order traversal over a BST is O(n) after inserts/searches near O(h).
- Example:
  - On startup, all service requests are inserted into the BST.
  - The tracking list (`/ServiceRequestStatus`) uses “sorted by date” via BST in-order traversal to display a stable, ordered list efficiently.

### 2) AVL Tree (Self-Balancing BST)
- Where: `DataStructures/AVLTree.cs`
- Used by: `ServiceRequestStore.SearchAVL(Guid id)`
- Role: guarantees balanced height to keep search operations reliably O(log n).
- Why it helps: on large datasets, worst-case skew of a plain BST can degrade; AVL ensures consistent performance for ID lookups.
- Example:
  - If you paste a GUID Id for a request into a diagnostic tool or future admin UI, the AVL tree’s search finds it in O(log n) regardless of insertion order.

### 3) Red-Black Tree
- Where: `DataStructures/RedBlackTree.cs`
- Used by: `ServiceRequestStore.SearchRBT(Guid id)`
- Role: another balanced BST variant with fewer rotations in practice; guarantees O(log n) operations with different balancing semantics than AVL.
- Why it helps: complements AVL to demonstrate balanced tree behaviors; suitable for diverse real-world insertion patterns.
- Example:
  - Running a batch of imports (e.g., migration from `issues.json`) inserts many requests quickly while maintaining balanced search.

### 4) Heaps (Min-Heap and Max-Heap)
- Where: `DataStructures/Heap.cs`
- Used by: `ServiceRequestStore.GetTopPriorityRequests(int count)` and the `/ServiceRequestStatus/Priority` page
- Role: Min-Heap orders requests by priority and then by submission time to surface the most critical items first.
- Why it helps: retrieving “top K” urgent requests is O(k log n), much faster than sorting the entire dataset for each query.
- Example:
  - The “Top Priority” page builds a Min-Heap from current requests and extracts the smallest (highest urgency) K requests — ideal for dispatch dashboards.

### 5) Graphs + Traversals (DFS/BFS)
- Where: `DataStructures/Graph.cs`
- Used by: `ServiceRequestStore.GetRelatedRequests(Guid id)`, `ServiceRequestStore.GetRequestsByCategory(string category)`, `ServiceRequestStore.GetRequestsByStatus(RequestStatus status)` (via node iteration)
- Role: models relationships between requests (similar category, same location, similar timing, status proximity). Edges store a weight that encodes similarity strength.
- Traversals:
  - DFS: explore component of related requests deeply (useful for internal analytics).
  - BFS: discover nearest neighbors by similarity; `/GetRequestsByCategory` seeds BFS from a category request and filters by category.
- Why it helps: users often care about clusters (e.g., multiple pothole reports in the same area). Graphs capture and traverse these relationships efficiently.
- Example:
  - On a request’s Details page, “Related Requests” is populated by walking its adjacency list and ranking by edge weight (similarity).

### 6) Minimum Spanning Tree (MST, Kruskal)
- Where: `DataStructures/Graph.cs` ➜ `MinimumSpanningTree()`
- Used by: `ServiceRequestStore.GetMinimumSpanningTreeRequests()`
- Role: builds a minimal set of relationships that connect clustered requests with the least total “distance” (inverse of similarity).
- Why it helps: helps reveal clusters with minimal redundancy (e.g., planning combined field visits for similar/nearby requests).
- Example:
  - For a batch/ops view, the MST can quickly identify a representative subset of requests that connect many others, informing route planning for crews.

### 7) Basic Trees vs Binary Trees (Conceptual)
- The system’s BST/AVL/Red-Black implementations are specialized binary trees.
- “Basic trees” (n-ary) are relevant conceptually for hierarchical categorization, but here we choose binary variants to support strict ordering and O(log n) operations on scalar keys (dates/Ids).

### Putting It All Together (Request Lifecycle)
1) Submit report ➜ `ReportController` also creates a `ServiceRequest` with:
   - Status (Submitted/UnderReview/InProgress/OnHold/Resolved/Closed)
   - Priority (derived from category/description)
2) Store ➜ `ServiceRequestStore` persists JSON and indexes the request across:
   - BST/AVL/Red-Black (ordered + balanced searches)
   - Min-Heap (priority queue)
   - Graph (relationships between requests)
3) Query ➜ `ServiceRequestStatusController` serves views:
   - Sorted lists from BST traversal
   - Top priority from Min-Heap extraction
   - Related items and category exploration from Graph search
   - Stable searches from balanced trees

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
