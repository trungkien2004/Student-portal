# Student Portal — Web App (HTML/CSS/JS + C# ASP.NET Core + SQL Server)

Project mô phỏng hệ thống quản lý sinh viên (kiểu HACTECH): Thông tin cá nhân,
Chương trình đào tạo, Bảng điểm, Lịch học.

## Cấu trúc thư mục

```
StudentPortal/
├── Database/
│   └── schema.sql              # Tạo database + bảng + dữ liệu mẫu
├── Backend/
│   └── StudentPortal.API/      # ASP.NET Core Web API (C#)
│       ├── Controllers/        # Auth, Student, Curriculum, Grades, Schedule
│       ├── Models/             # Entity classes (map với bảng SQL)
│       ├── Data/                # DbContext + TokenService (JWT)
│       ├── DTOs/                 # Object trả JSON cho frontend
│       ├── Program.cs
│       └── appsettings.json
└── Frontend/
    ├── index.html
    ├── css/style.css
    └── js/api.js, app.js
```

## 1. Cài đặt Database (SQL Server)

1. Cài SQL Server (hoặc SQL Server Express / LocalDB) + SQL Server Management Studio (SSMS).
2. Mở `Database/schema.sql` trong SSMS, bấm **Execute** để tạo database `StudentPortalDB`
   cùng dữ liệu mẫu giống trong ảnh (sinh viên Phạm Trung Kiên...).
3. Cột `PasswordHash` trong bảng `Students` hiện đang là chuỗi giả — bạn cần thay
   bằng mật khẩu đã hash bằng BCrypt (xem mục 3 bên dưới) rồi `UPDATE` lại,
   hoặc đăng ký user mới qua code.

## 2. Chạy Backend (C#)

Yêu cầu: **.NET 8 SDK** (`dotnet --version` để kiểm tra).

```bash
cd Backend/StudentPortal.API
dotnet restore
```

Sửa `appsettings.json` → `ConnectionStrings:DefaultConnection` cho đúng SQL Server
của bạn (server name, user/pass hoặc Windows Auth).

Chạy:
```bash
dotnet run
```
Mặc định API sẽ chạy ở `https://localhost:5001` (xem log console để biết chính xác port).
Swagger UI để test API: `https://localhost:5001/swagger`.

## 3. Tạo mật khẩu đăng nhập (BCrypt hash)

Vì bảo mật, mật khẩu không lưu dạng chữ thường. Cách nhanh nhất để tạo hash:

Tạo 1 file console nhỏ (hoặc dùng LINQPad / 1 unit test tạm) chạy:
```csharp
string hash = BCrypt.Net.BCrypt.HashPassword("123456"); // mật khẩu bạn muốn dùng
Console.WriteLine(hash);
```
Copy chuỗi hash in ra, rồi update vào DB:
```sql
UPDATE Students SET PasswordHash = N'<hash_vua_copy>' WHERE StudentCode = N'CD220823';
```
Sau đó đăng nhập trên frontend bằng: `CD220823` / `123456`.

## 4. Chạy Frontend

`Frontend/index.html` là HTML/CSS/JS thuần, không cần build.

1. Mở `Frontend/js/api.js`, sửa `API_BASE_URL` cho đúng port backend đang chạy
   (mặc định đang để `https://localhost:5001/api`).
2. Mở `Frontend/index.html` trực tiếp bằng trình duyệt, hoặc dùng Live Server
   (extension VS Code) để tránh lỗi CORS/file://.
3. Đăng nhập bằng mã sinh viên + mật khẩu đã tạo ở bước 3.

## Luồng hoạt động

```
Frontend (HTML/CSS/JS)
   │  fetch() + JWT token trong header Authorization
   ▼
Backend ASP.NET Core Web API (C#)
   │  Entity Framework Core
   ▼
SQL Server (StudentPortalDB)
```

- `POST /api/auth/login` → kiểm tra mã SV + mật khẩu (BCrypt), trả về JWT token.
- `GET /api/student/me` → Thông tin cá nhân (yêu cầu token).
- `GET /api/curriculum` → Chương trình đào tạo theo ngành của sinh viên.
- `GET /api/grades` → Bảng điểm (điểm rèn luyện, môn không qua, điểm theo từng kỳ).
- `GET /api/schedule` → Lịch học.

## Mở rộng thêm (gợi ý cho CV / phỏng vấn)

- Thêm chức năng đổi mật khẩu, quên mật khẩu.
- Phân trang / lọc cho bảng Lịch học, Chương trình đào tạo.
- Vai trò Admin để nhập điểm, quản lý sinh viên (CRUD đầy đủ).
- Viết Unit Test cho Controllers bằng xUnit + Moq.
- Deploy backend lên Azure/Render, frontend lên Netlify/Vercel, DB lên Azure SQL.
