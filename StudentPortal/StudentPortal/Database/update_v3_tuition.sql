/* ===================================================================
   UPDATE SCRIPT - chi chay 1 lan tren database da co san (StudentPortalDB)
   Khong dung lai file schema.sql goc (se bi loi trung database/bang)
   =================================================================== */

USE StudentPortalDB;
GO

-- Bang Hoc phi (neu chua co)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tuitions')
BEGIN
    CREATE TABLE Tuitions (
        TuitionId     INT IDENTITY(1,1) PRIMARY KEY,
        StudentId     INT NOT NULL FOREIGN KEY REFERENCES Students(StudentId) ON DELETE CASCADE,
        Semester      INT NOT NULL,
        AcademicYear  NVARCHAR(20) NOT NULL,
        AmountDue     DECIMAL(12,0) NOT NULL,
        AmountPaid    DECIMAL(12,0) NOT NULL DEFAULT 0,
        DueDate       DATE NOT NULL,
        PaidDate      DATE NULL
    );

    -- Du lieu mau: 1 ky da nop du, 1 ky chua nop (de thay canh bao)
    INSERT INTO Tuitions (StudentId, Semester, AcademicYear, AmountDue, AmountPaid, DueDate, PaidDate) VALUES
    (1, 1, N'2022-2023', 6500000, 6500000, '2022-09-15', '2022-09-10'),
    (1, 2, N'2022-2023', 6500000, 0,       '2023-02-15', NULL);

    PRINT N'Da tao bang Tuitions va du lieu mau.';
END
ELSE
BEGIN
    PRINT N'Bang Tuitions da ton tai, khong tao lai.';
END
GO
