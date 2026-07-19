# 🎓 Cổng thông tin Sinh viên (Student Portal)

Ứng dụng web quản lý thông tin sinh viên, gồm phần **Backend** viết bằng C# (ASP.NET Core Web API) kết nối **SQL Server**, và phần **Frontend** bằng HTML/CSS/JavaScript.

Đây là dự án cá nhân, thực hiện trong quá trình tự học, với mục tiêu làm quen với quy trình xây dựng một ứng dụng web hoàn chỉnh: từ thiết kế database, viết API, đến hiển thị dữ liệu ra giao diện.

## Công nghệ sử dụng

- **Backend:** C#, ASP.NET Core Web API, Entity Framework Core
- **Database:** SQL Server
- **Đăng nhập:** JWT, mật khẩu được mã hoá bằng BCrypt
- **Frontend:** HTML, CSS, JavaScript
- **Khác:** xuất báo cáo ra file PDF (jsPDF)

## Các chức năng chính

- Đăng nhập
- Xem và sửa thông tin cá nhân
- Xem và quản lý chương trình đào tạo (thêm/sửa/xoá môn học)
- Xem và quản lý bảng điểm, tự tính điểm trung bình, có cảnh báo nếu môn nào chưa qua
- Xem và quản lý lịch học
- Xem và quản lý học phí, cảnh báo nếu chưa đóng đủ
- Xuất bảng điểm ra file PDF

## Cách hoạt động (tóm tắt)

Khi người dùng thao tác trên giao diện (VD: xem bảng điểm), trình duyệt sẽ gửi yêu cầu tới Backend kèm theo mã xác thực (JWT token). Backend kiểm tra mã này, lấy dữ liệu tương ứng từ SQL Server, rồi trả kết quả về để hiển thị lên màn hình.

## Cấu trúc thư mục

```
StudentPortal/
├── Database/           # File tạo database và dữ liệu mẫu
├── Backend/             # Code C# (ASP.NET Core Web API)
│   └── StudentPortal.API/
│       ├── Controllers/   # Xử lý các yêu cầu từ Frontend
│       ├── Models/        # Các bảng dữ liệu
│       ├── Data/           # Kết nối database, xử lý đăng nhập
│       └── DTOs/            # Dữ liệu trả về cho Frontend
└── Frontend/
    ├── index.html
    ├── css/
    └── js/
```

## Kỹ năng đạt được

Qua quá trình làm dự án này, đã thực hành được:

- Xây dựng một Web API bằng C#/ASP.NET Core, có các chức năng thêm/sửa/xoá dữ liệu
- Làm quen với cách một request đi từ giao diện tới Backend rồi tới database và ngược lại
- Thiết kế các bảng dữ liệu có quan hệ với nhau (khoá chính, khoá ngoại) trong SQL Server
- Áp dụng đăng nhập bằng JWT cho ứng dụng web
- Gọi API và hiển thị dữ liệu bằng JavaScript (fetch)
- Tự tìm và sửa các lỗi phát sinh khi chạy thử: lỗi kết nối SQL Server, lỗi CORS khi Frontend gọi Backend

## Cách chạy thử

Cần cài sẵn: .NET 8 SDK, SQL Server

1. Mở file `Database/schema.sql` bằng SQL Server Management Studio, bấm chạy để tạo database.
2. Sửa lại thông tin kết nối trong file `Backend/StudentPortal.API/appsettings.json` cho đúng với SQL Server trên máy.
3. Mở terminal, chạy:
   ```
   cd Backend/StudentPortal.API
   dotnet run
   ```
4. Mở file `Frontend/index.html` bằng Live Server (extension của VS Code).

Hướng dẫn chi tiết hơn xem trong file [README.md](./README.md).

---

📬 Liên hệ: trungkienphamdev18@gmail.com

