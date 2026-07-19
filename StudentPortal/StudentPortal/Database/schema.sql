/* ===================================================================
   STUDENT PORTAL - DATABASE SCHEMA (SQL Server)
   Mo phong he thong quan ly sinh vien (kieu HACTECH)
   =================================================================== */

CREATE DATABASE StudentPortalDB;
GO

USE StudentPortalDB;
GO

/* ------------------------------------------------------------------
   1. BANG NGANH / KHOA HOC (Major / Class)
   ------------------------------------------------------------------ */
CREATE TABLE Majors (
    MajorId         INT IDENTITY(1,1) PRIMARY KEY,
    MajorCode       NVARCHAR(20)  NOT NULL UNIQUE,   -- vd: LTMT
    MajorName       NVARCHAR(200) NOT NULL,          -- vd: Nghe Lap trinh may tinh
    Faculty         NVARCHAR(200) NOT NULL,          -- vd: Khoa Cong nghe Thong tin
    ClassCode       NVARCHAR(50)  NOT NULL,          -- vd: LTMT 3 K14
    Course          NVARCHAR(50)  NOT NULL           -- vd: Khoa 14 (2022 - 2025)
);
GO

/* ------------------------------------------------------------------
   2. BANG SINH VIEN (Students) - man hinh "Thong tin ca nhan"
   ------------------------------------------------------------------ */
CREATE TABLE Students (
    StudentId           INT IDENTITY(1,1) PRIMARY KEY,
    StudentCode         NVARCHAR(20)  NOT NULL UNIQUE,  -- CD220823
    FullName            NVARCHAR(150) NOT NULL,
    DateOfBirth         DATE          NOT NULL,
    Gender              NVARCHAR(10)  NOT NULL,         -- Nam / Nu
    PlaceOfBirth        NVARCHAR(150),
    Phone               NVARCHAR(20),
    IdentityCard        NVARCHAR(20),
    InsuranceCode       NVARCHAR(30),
    Ethnicity           NVARCHAR(50),
    MajorId             INT NOT NULL FOREIGN KEY REFERENCES Majors(MajorId),
    PersonalEmail       NVARCHAR(150),
    SchoolEmail         NVARCHAR(150),
    PermanentAddress    NVARCHAR(300),
    ParentAddress       NVARCHAR(300),
    Status              NVARCHAR(50) NOT NULL DEFAULT N'Dang hoc', -- Dang hoc / Da tot nghiep
    BankName             NVARCHAR(100),
    BankAccountNumber    NVARCHAR(50),
    BankAccountHolder    NVARCHAR(150),
    PasswordHash         NVARCHAR(256) NOT NULL,        -- luu mat khau da hash (dang nhap)
    CreatedAt            DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

/* ------------------------------------------------------------------
   3. NGUOI LIEN HE GIA DINH (Family contacts, 1-nhieu voi Student)
   ------------------------------------------------------------------ */
CREATE TABLE FamilyContacts (
    ContactId    INT IDENTITY(1,1) PRIMARY KEY,
    StudentId    INT NOT NULL FOREIGN KEY REFERENCES Students(StudentId) ON DELETE CASCADE,
    FullName     NVARCHAR(150) NOT NULL,
    Relationship NVARCHAR(50),                 -- Bo / Me / Nguoi giam ho
    Phone        NVARCHAR(20)
);
GO

/* ------------------------------------------------------------------
   4. MON HOC (Subjects) - dung chung cho ca truong
   ------------------------------------------------------------------ */
CREATE TABLE Subjects (
    SubjectId     INT IDENTITY(1,1) PRIMARY KEY,
    SubjectCode   NVARCHAR(20)  NOT NULL UNIQUE,   -- MH01, LMH08 ...
    SubjectName   NVARCHAR(200) NOT NULL,
    ShortName     NVARCHAR(200),
    SubjectType   NVARCHAR(50)  NOT NULL,          -- 'Mon tinh diem' / 'Mon dieu kien'
    CreditsLT     INT NOT NULL DEFAULT 0,           -- so TC ly thuyet
    CreditsTH     INT NOT NULL DEFAULT 0,           -- so TC thuc hanh
    PeriodsLT     INT NOT NULL DEFAULT 0,           -- so tiet ly thuyet
    PeriodsTH     INT NOT NULL DEFAULT 0            -- so tiet thuc hanh
);
GO

/* ------------------------------------------------------------------
   5. CHUONG TRINH DAO TAO (Curriculum) - Mon hoc gan voi Nganh + Ky hoc
   ------------------------------------------------------------------ */
CREATE TABLE CurriculumItems (
    CurriculumItemId INT IDENTITY(1,1) PRIMARY KEY,
    MajorId          INT NOT NULL FOREIGN KEY REFERENCES Majors(MajorId),
    SubjectId        INT NOT NULL FOREIGN KEY REFERENCES Subjects(SubjectId),
    Semester         INT NOT NULL       -- Ky hoc (1, 2, 3 ...)
);
GO

/* ------------------------------------------------------------------
   6. BANG DIEM (Grades) - man hinh "Bang diem"
   ------------------------------------------------------------------ */
CREATE TABLE Grades (
    GradeId         INT IDENTITY(1,1) PRIMARY KEY,
    StudentId       INT NOT NULL FOREIGN KEY REFERENCES Students(StudentId) ON DELETE CASCADE,
    SubjectId       INT NOT NULL FOREIGN KEY REFERENCES Subjects(SubjectId),
    Semester        INT NOT NULL,             -- Hoc ky 1, 2 ...
    MidtermLT       DECIMAL(4,2) NULL,        -- Diem giua ky LT
    MidtermTH       DECIMAL(4,2) NULL,        -- Diem giua ky TH
    FinalLT         DECIMAL(4,2) NULL,        -- Diem LT cao nhat
    FinalTH         DECIMAL(4,2) NULL,        -- Diem TH cao nhat
    AverageScore    DECIMAL(4,2) NULL,        -- Diem TB mon
    IsPassed        BIT NOT NULL DEFAULT 1
);
GO

/* ------------------------------------------------------------------
   7. DIEM REN LUYEN (Conduct scores theo tung nam)
   ------------------------------------------------------------------ */
CREATE TABLE ConductScores (
    ConductScoreId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId      INT NOT NULL FOREIGN KEY REFERENCES Students(StudentId) ON DELETE CASCADE,
    AcademicYear   INT NOT NULL,      -- 1, 2, 3 (nam hoc thu may)
    Score          INT NOT NULL,
    Rating         NVARCHAR(30)       -- Tot / Kha / Trung binh ...
);
GO

/* ------------------------------------------------------------------
   8. LICH HOC (Schedule) - man hinh "Lich hoc"
   ------------------------------------------------------------------ */
CREATE TABLE Schedules (
    ScheduleId    INT IDENTITY(1,1) PRIMARY KEY,
    StudentId     INT NOT NULL FOREIGN KEY REFERENCES Students(StudentId) ON DELETE CASCADE,
    SubjectId     INT NOT NULL FOREIGN KEY REFERENCES Subjects(SubjectId),
    ClassDate     DATE NOT NULL,
    WeekNumber    INT NOT NULL,
    Semester      INT NOT NULL,
    AcademicYear  NVARCHAR(20) NOT NULL,    -- '2024-2025'
    StartPeriod   NVARCHAR(20) NOT NULL,    -- 'Tiet 1'
    EndPeriod     NVARCHAR(20) NOT NULL,    -- 'Tiet 4'
    Room          NVARCHAR(200) NOT NULL,
    Lecturer      NVARCHAR(150) NOT NULL
);
GO

/* ------------------------------------------------------------------
   9. HOC PHI (Tuitions) - man hinh "Nop hoc phi" + canh bao chua nop
   ------------------------------------------------------------------ */
CREATE TABLE Tuitions (
    TuitionId     INT IDENTITY(1,1) PRIMARY KEY,
    StudentId     INT NOT NULL FOREIGN KEY REFERENCES Students(StudentId) ON DELETE CASCADE,
    Semester      INT NOT NULL,
    AcademicYear  NVARCHAR(20) NOT NULL,     -- '2024-2025'
    AmountDue     DECIMAL(12,0) NOT NULL,    -- so tien phai nop
    AmountPaid    DECIMAL(12,0) NOT NULL DEFAULT 0,  -- so tien da nop
    DueDate       DATE NOT NULL,             -- han nop
    PaidDate      DATE NULL                  -- ngay nop (neu da nop du)
);
GO

/* ===================================================================
   SEED DATA - du lieu mau (khop voi cac anh minh hoa)
   =================================================================== */

INSERT INTO Majors (MajorCode, MajorName, Faculty, ClassCode, Course)
VALUES (N'LTMT', N'Nghe Lap trinh may tinh', N'Khoa Cong nghe Thong tin', N'LTMT 3 K14', N'Khoa 14 (2022 - 2025)');

INSERT INTO Students (StudentCode, FullName, DateOfBirth, Gender, PlaceOfBirth, Phone, IdentityCard,
    InsuranceCode, Ethnicity, MajorId, PersonalEmail, SchoolEmail, PermanentAddress, ParentAddress,
    Status, BankName, BankAccountNumber, BankAccountHolder, PasswordHash)
VALUES (N'CD220823', N'Pham Trung Kien', '2004-10-07', N'Nam', N'Thanh pho Ha Noi', N'0936246827',
    N'001204004687', N'HS01012525632', N'Kinh', 1, N'ptrungkien901234@gmail.com',
    N'kien.220823@student.hactech.edu.vn', N'53 Ha Trung, Phuong Hoan Kiem, Thanh pho Ha Noi',
    N'53 Ha Trung, Phuong Hoan Kiem, Thanh pho Ha Noi', N'Da tot nghiep', N'', N'', N'',
    N'$2a$11$REPLACE_WITH_REAL_BCRYPT_HASH');  -- xem README de biet cach tao hash mat khau

INSERT INTO FamilyContacts (StudentId, FullName, Relationship, Phone) VALUES
(1, N'Pham Xuan Thanh', N'Bo', N'0921127258'),
(1, N'Nguyen Tuong Vi', N'Me', NULL);

-- Mon hoc
INSERT INTO Subjects (SubjectCode, SubjectName, ShortName, SubjectType, CreditsLT, CreditsTH, PeriodsLT, PeriodsTH) VALUES
(N'MH01',  N'Chinh tri',                       N'Chinh tri',                     N'Mon tinh diem',  5, 0, 75, 0),
(N'MH02',  N'Phap luat',                       N'Phap luat',                     N'Mon tinh diem',  2, 0, 30, 0),
(N'MH03',  N'Toan cao cap',                    N'Toan cao cap',                  N'Mon tinh diem',  3, 0, 45, 0),
(N'MH04',  N'Tin hoc can ban',                 N'Tin hoc can ban',               N'Mon tinh diem',  1, 2, 15, 60),
(N'MH05',  N'Anh van 1',                       N'Anh van 1',                     N'Mon tinh diem',  4, 0, 60, 0),
(N'MH0006',N'Giao duc An ninh - Quoc phong',   N'Giao duc An ninh - Quoc phong', N'Mon dieu kien',  4, 0, 60, 0),
(N'LMH08', N'Anh van 2',                       N'Anh van 2',                     N'Mon tinh diem',  4, 0, 60, 0),
(N'LMH09', N'Do hoa ung dung',                 NULL,                             N'Mon tinh diem',  2, 1, 30, 30),
(N'LMH10', N'Cau truc may tinh',               NULL,                             N'Mon tinh diem',  2, 1, 30, 30),
(N'LMH11', N'Lap trinh can ban',               NULL,                             N'Mon tinh diem',  2, 1, 30, 30),
(N'LMH13', N'Nhap mon mang may tinh',          NULL,                             N'Mon tinh diem',  2, 1, 30, 30),
(N'TN01',  N'Tot nghiep TH',                   N'Tot nghiep TH',                 N'Mon tinh diem',  0, 4, 0, 120);

-- Chuong trinh dao tao: gan mon vao nganh LTMT theo ky hoc
INSERT INTO CurriculumItems (MajorId, SubjectId, Semester) VALUES
(1, 1, 1), (1, 2, 1), (1, 3, 1), (1, 4, 1), (1, 5, 1), (1, 6, 1),
(1, 7, 2), (1, 8, 2), (1, 9, 2), (1, 10, 2), (1, 11, 2);

-- Diem ren luyen
INSERT INTO ConductScores (StudentId, AcademicYear, Score, Rating) VALUES
(1, 1, 85, N'Tot'),
(1, 2, 80, N'Tot'),
(1, 3, 80, N'Tot');

-- Bang diem hoc ky 1
INSERT INTO Grades (StudentId, SubjectId, Semester, FinalLT, FinalTH, AverageScore, IsPassed) VALUES
(1, 6, 1, 6, NULL, 6, 1),
(1, 1, 1, 8, NULL, 8, 1),
(1, 2, 1, 8, NULL, 8, 1),
(1, 3, 1, 5, NULL, 5, 1),
(1, 4, 1, 5, 5, 5, 1),
(1, 5, 1, 5, NULL, 5, 1);

-- Lich hoc (mon Tot nghiep TH)
INSERT INTO Schedules (StudentId, SubjectId, ClassDate, WeekNumber, Semester, AcademicYear, StartPeriod, EndPeriod, Room, Lecturer) VALUES
(1, 12, '2025-06-22', 41, 2, N'2024-2025', N'Tiet 1', N'Tiet 4', N'Khoa CNTT - Nha 1A/17/TQB - P.TH 301', N'Tran Viet Cuong'),
(1, 12, '2025-06-19', 41, 2, N'2024-2025', N'Tiet 1', N'Tiet 4', N'Khoa CNTT - Nha 1A/17/TQB - P.TH 305', N'Bui Thi Hoa'),
(1, 12, '2025-06-18', 41, 2, N'2024-2025', N'Tiet 5', N'Tiet 8', N'Khoa CNTT - Nha 1A/17/TQB - 405',      N'Bui Thi Hoa'),
(1, 12, '2025-06-16', 41, 2, N'2024-2025', N'Tiet 5', N'Tiet 8', N'Khoa CNTT - Nha 1A/17/TQB - P.TH 205', N'Bui Thi Hoa'),
(1, 12, '2025-06-12', 40, 2, N'2024-2025', N'Tiet 1', N'Tiet 4', N'Khoa CNTT - Nha 1A/17/TQB - P.TH 305', N'Bui Thi Hoa');
GO

-- Hoc phi mau: 1 ky da nop du, 1 ky chua nop (de test canh bao)
INSERT INTO Tuitions (StudentId, Semester, AcademicYear, AmountDue, AmountPaid, DueDate, PaidDate) VALUES
(1, 1, N'2022-2023', 6500000, 6500000, '2022-09-15', '2022-09-10'),
(1, 2, N'2022-2023', 6500000, 0,       '2023-02-15', NULL);
GO

PRINT N'Da tao xong database StudentPortalDB va du lieu mau.';
