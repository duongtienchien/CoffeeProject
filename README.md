# CoffeeShop API - Kiến Trúc 3-Tier

![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework-0078D4?style=for-the-badge&logo=dotnet&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=JSON%20web%20tokens)

## Tổng Quan
CoffeeShop API là hệ thống RESTful API hiệu năng cao và có khả năng mở rộng tốt, được xây dựng để phục vụ vận hành chuỗi quán cà phê. Được phát triển trên nền tảng .NET Core và C#.

Hệ thống được thiết kế chặt chẽ theo Kiến trúc phân tầng (N-Tier Architecture), đảm bảo tách biệt rõ ràng các tầng trách nhiệm (Separation of Concerns), tối ưu khả năng bảo trì và sẵn sàng cho việc mở rộng quy mô.

## Tính Năng
* ** Kiến Trúc 3-Tier:** Phân tách các tầng như Presentation (API), BLL (Business Logic Layer) và DAL (Data Access Layer).
* ** Xác thực & Phân quyền:** Cơ chế bảo mật bằng JWT (JSON Web Token) kết hợp kiểm soát truy cập dựa trên vai trò (RBAC).
* ** Tăng cường bảo mật:** Tích hợp Rate Limiting chống tấn công Brute-force cùng cơ chế xử lý ngoại lệ tập trung chặt chẽ
* ** Quản trị cơ sở dữ liệu:** Áp dụng Entity Framework Core theo hướng Code-First kết hợp hệ quản trị cơ sở dữ liệu PostgreSQL.
* ** Dependency Injection:** Vận dụng triệt để DI nhằm giảm độ phụ thuộc (loose coupling) giữa các Repositories và Services.
* ** Xử lý nghiệp vụ:** Hoàn thiện luồng xử lý đơn hàng, quản trị người dùng và quản lý kho nguyên liệu thông qua các DTO tùy biến.

##  Tech Stack
* **Framework:** .NET (C#)
* **Database:** PostgreSQL
* **ORM:** Entity Framework Core
* **Authentication:** JSON Web Token (JWT)
* **API Documentation:** Swagger / OpenAPI

## Project Structure
```text
CoffeeShop.Solution/
│
├── Frontend
│   └── CoffeeShop.FrontEnd/ # Lớp Giao diện (UI) gồm các cổng thông tin cho Quản lý và Nhân viên
├── Backend (N-Tier)
│   ├── CoffeeShop.API/      # Lớp Trình Diễn (Controllers, DI Container, Middlewares)
│   ├── CoffeeShop.BLL/      # Lớp Xử lý Nghiệp Vụ (Services, DTOs, JWT Generation)
│   ├── CoffeeShop.DAL/      # Lớp Truy cập Dữ liệu (Repositories, DbContext, Migrations)
│   └── CoffeeShop.Models/   # Lớp Thực Thể (Entities: Auth, Catalog, Sales, System)
│
├── Testing & Tools
│   └── RaceConditionTester/ # Tool test tương tranh (Dùng để kiểm chứng cơ chế Khóa lạc quan – Optimistic Locking)
│
├── CoffeeShop.sln           # Visual Studio Solution file
└── README.md                # Project documentation
```

## Công Nghệ Sử Dụng (Tech Stack)
* **Backend:** .NET 8 (C# RESTful API), Entity Framework Core.
* **Database:** PostgreSQL (Neon Cloud Serverless).
* **Frontend:** Vite, TailwindCSS.

## Hướng Dẫn Khởi Chạy (Getting Started)

### Yêu Cầu Hệ Thống (Prerequisites)
* **.NET SDK 8.0** trở lên
* **Node.js** (khuyến nghị v18 trở lên) & **npm**

---

### 1. Khởi Chạy Backend (.NET 8 Web API)
Dự án đã tích hợp sẵn cơ chế **Auto-Migration** và **Data Seeding** tự động kết nối với NeonDB:

```bash
cd CoffeeShop.API
dotnet run
```
* **Swagger UI:** Truy cập theo đường dẫn mặc định hiển thị trên Terminal (thường là `https://localhost:5079/swagger` hoặc `http://localhost:5079/swagger`).

---

### 2. Khởi Chạy Frontend (React / Vite)
Mở một cửa sổ Terminal mới và chạy các lệnh sau:

```bash
cd CoffeeShop.FrontEnd
npm install
npm run dev
```
* **Giao diện Client:** Mặc định truy cập tại `http://localhost:5173`.

---

### 3. Tài Khoản Trải Nghiệm Mẫu (Demo Credentials)

Hệ thống đã nạp sẵn các tài khoản phân quyền mẫu phục vụ quá trình test chức năng:

| Vai trò | Email đăng nhập | Mật khẩu | Chi nhánh |
| :--- | :--- | :--- | :--- |
| **Manager (Quản lý)** | `manager.cg@coffeeshop.com` | `Manager@123` | Cầu Giấy |
| **Staff (Nhân viên)** | `staff1.cg@coffeeshop.com` | `Staff@123` | Cầu Giấy |
| **Manager (Quản lý)** | `manager.dd@coffeeshop.com` | `Manager@123` | Đống Đa |
| **Staff (Nhân viên)** | `staff1.dd@coffeeshop.com` | `Staff@123` | Đống Đa |

---

## Tác Giả (Author)

* **Dương Tiến Chiến** – *Backend Developer*
* **GitHub:** [(https://github.com/duongtienchien)
