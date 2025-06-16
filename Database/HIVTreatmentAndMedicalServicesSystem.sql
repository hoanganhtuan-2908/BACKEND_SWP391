USE master;
GO

-- Kiểm tra nếu database HIVCareDB đã tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HIVTreatmentAndMedicalServicesSystem')
BEGIN
    -- Nếu tồn tại thì hủy tất cả kết nối và xóa
    ALTER DATABASE HIVTreatmentAndMedicalServicesSystem SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HIVTreatmentAndMedicalServicesSystem;
    PRINT 'Database HIVTreatmentAndMedicalServicesSystem đã tồn tại và đã được xóa.';
END

-- Tạo database mới
CREATE DATABASE HIVTreatmentAndMedicalServicesSystem;
GO

PRINT 'Database HIVTreatmentAndMedicalServicesSystem đã được tạo mới.';
GO

-- Chuyển sang sử dụng database vừa tạo
USE HIVTreatmentAndMedicalServicesSystem

go
set dateformat dmy

-- USERS & ROLES
CREATE TABLE Roles (
    RoleID VARCHAR(10) PRIMARY KEY,
    RoleName NVARCHAR(50) NOT NULL
);

CREATE TABLE Users (
    UserID VARCHAR(10) PRIMARY KEY,
    RoleID VARCHAR(10) NOT NULL,
    Fullname NVARCHAR(100) NOT NULL,
    Password VARCHAR(255) NOT NULL,
    Email VARCHAR(100) UNIQUE NOT NULL,
    Image VARCHAR(100),
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- PATIENTS
CREATE TABLE Patients (
    PatientID VARCHAR(10) PRIMARY KEY,
    UserID VARCHAR(10) NOT NULL,
    DateOfBirth DATE,
    Gender NVARCHAR(20),
    Phone VARCHAR(20),
    BloodType VARCHAR(10),
    Allergy NVARCHAR(MAX),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- DOCTORS
CREATE TABLE Doctors (
    DoctorID VARCHAR(10) PRIMARY KEY,
    UserID VARCHAR(10) NOT NULL,
    Specialization NVARCHAR(100),
    LicenseNumber NVARCHAR(50) UNIQUE,
    ExperienceYears INT,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- SERVICES
CREATE TABLE Services (
    ServiceID VARCHAR(10) PRIMARY KEY,
    ServiceName NVARCHAR(100) NOT NULL,
    Type NVARCHAR(50),
    Price DECIMAL(10,2)
);

CREATE TABLE DoctorServices (
    DoctorServiceID VARCHAR(10) PRIMARY KEY,
    DoctorID VARCHAR(10) NOT NULL,
    ServiceID VARCHAR(10) NOT NULL,
    CONSTRAINT UK_DoctorService UNIQUE (DoctorID, ServiceID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID),
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID)
);

-- SLOTS
CREATE TABLE Slot (
    SlotID VARCHAR(10) PRIMARY KEY,
    SlotNumber INT,
    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL
);

-- DOCTOR SCHEDULE
CREATE TABLE DoctorWorkSchedule (
    ScheduleID VARCHAR(10) PRIMARY KEY,
    DoctorID VARCHAR(10) NOT NULL,
    SlotID VARCHAR(10) NOT NULL,
    DayOfWeek NVARCHAR(10),
    Status NVARCHAR(20),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID),
    FOREIGN KEY (SlotID) REFERENCES Slot(SlotID)
);

-- SCHEDULING & BOOKS
CREATE TABLE Books (
    BookID VARCHAR(10) PRIMARY KEY,
    PatientID VARCHAR(10) NOT NULL,
    DoctorID VARCHAR(10) NOT NULL,
    ServiceID VARCHAR(10) NOT NULL,
    BookDate DATETIME NOT NULL,
    Status NVARCHAR(20),
    LinkMeet NVARCHAR(255),
    Note NVARCHAR(MAX),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID),
    FOREIGN KEY (ServiceID) REFERENCES Services(ServiceID)
);


-- MEDICAL RECORDS
CREATE TABLE MedicalRecord (
    MedicalRecordID VARCHAR(10) PRIMARY KEY,
    PatientID VARCHAR(10) NOT NULL,
    DoctorID VARCHAR(10) NOT NULL,
    Diagnosis NVARCHAR(MAX),
    CreatedDate DATETIME,
    TreatmentResult NVARCHAR(MAX),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID)
);

-- MEDICATION & PRESCRIPTIONS
CREATE TABLE Medication (
    MedicationID VARCHAR(10) PRIMARY KEY,
    MedicationName NVARCHAR(100) NOT NULL,
    DosageForm NVARCHAR(50),
    Strength NVARCHAR(50),
    TargetGroup NVARCHAR(50),
    CreatedAt DATETIME
);

CREATE TABLE Prescription (
    PrescriptionID VARCHAR(10) PRIMARY KEY,
    MedicalRecordID VARCHAR(10) NOT NULL,
    MedicationID VARCHAR(10) NOT NULL,
    DoctorID VARCHAR(10) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE,
    Dosage NVARCHAR(100),
    LineOfTreatment NVARCHAR(50),
    CreatedAt DATETIME,
    FOREIGN KEY (MedicalRecordID) REFERENCES MedicalRecord(MedicalRecordID),
    FOREIGN KEY (MedicationID) REFERENCES Medication(MedicationID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID)
);

-- ARV REGIMEN
CREATE TABLE ARVRegimen (
    ARVRegimenID VARCHAR(10) PRIMARY KEY,
    MedicalRecordID VARCHAR(10) NOT NULL,
    DoctorID VARCHAR(10) NOT NULL,
    RegimenCode NVARCHAR(50),
    ARVName NVARCHAR(100),
    Description NVARCHAR(MAX),
    AgeRange NVARCHAR(50),
    ForGroup NVARCHAR(50),
    CreatedAt DATETIME,
    FOREIGN KEY (MedicalRecordID) REFERENCES MedicalRecord(MedicalRecordID),
    FOREIGN KEY (DoctorID) REFERENCES Doctors(DoctorID)
);

-- HIV TESTS
CREATE TABLE HIVTest (
    TestResultID VARCHAR(10) PRIMARY KEY,
    MedicalRecordID VARCHAR(10) NOT NULL,
    TestDate DATE NOT NULL,
    CD4Count INT,
    ViralLoad INT,
    Notes NVARCHAR(MAX),
    FOREIGN KEY (MedicalRecordID) REFERENCES MedicalRecord(MedicalRecordID)
);

-- REMINDERS
CREATE TABLE Reminder (
    ReminderCheckID VARCHAR(10) PRIMARY KEY,
    PatientID VARCHAR(10) NOT NULL,
    BookID VARCHAR(10) NOT NULL,
    ReminderTime DATETIME NOT NULL,
    Message NVARCHAR(MAX),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (BookID) REFERENCES Books(BookID)
);

CREATE TABLE ReminderMedication (
    ReminderMedicationID VARCHAR(10) PRIMARY KEY,
    PatientID VARCHAR(10) NOT NULL,
    PrescriptionID VARCHAR(10) NOT NULL,
    DosageTime TIME NOT NULL,
    Instruction NVARCHAR(MAX),
    MedicationInUse NVARCHAR(255),
    FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    FOREIGN KEY (PrescriptionID) REFERENCES Prescription(PrescriptionID)
);




-- Không cần thay đổi các lệnh insert đã có, vì chỉ sửa định nghĩa bảng cho chuẩn hóa và đảm bảo khóa ngoại hoạt động đúng.

-- Thêm dữ liệu vào bảng Roles (sử dụng stored procedure)
insert into Roles values ('R001', 'Admin');
insert into Roles values ('R002', 'Manager');
insert into Roles values ('R003', 'Doctor');
insert into Roles values ('R004', 'Staff');
insert into Roles values ('R005', 'Patient');
--select * from Roles


-- Tạo tài khoản quản trị viên
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000001', 'R001', N'Lê Quốc Việt', 'lequocviet123', 'lequocviet@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000002', 'R001', N'Nguyễn Văn Nguyên', 'nguyenvannguyen123', 'nguyenvannguyen@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000003', 'R001', N'Nguyễn Quản Trị 1', 'nguyenquantri123', 'nguyenquantri@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000004', 'R001', N'Nguyễn Quản Trị 2', 'nguyenquantri123', 'nguyenquantri2@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000005', 'R001', N'Nguyễn Quản Trị 3', 'nguyenquantri123', 'nguyenquantri3@gmail.com');




-- Tạo tài khoản quản lý
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000006', 'R002', N'Nguyễn Quản LÝ', 'nguyenquanly123', 'nguyenquanly@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000007', 'R002', N'Nguyễn Quản LÝ 2', 'nguyenquanly123', 'nguyenquanly2@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000008', 'R002', N'Nguyễn Quản LÝ 3', 'nguyenquanly123', 'nguyenquanly3@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000009', 'R002', N'Nguyễn Quản LÝ 4', 'nguyenquanly123', 'nguyenquanly4@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000010', 'R002', N'Nguyễn Quản LÝ 5', 'nguyenquanly123', 'nguyenquanly5@gmail.com');

--tạo tk bác sĩ
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000011', 'R003', N'Nguyễn Bác sĩ', 'nguyenbacsi123', 'nguyenbacsi@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000012', 'R003', N'Nguyễn Bác sĩ 2', 'nguyenbacsi123', 'nguyenbacsi2@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000013', 'R003', N'Nguyễn Bác sĩ 3', 'nguyenbacsi123', 'nguyenbacsi3@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000014', 'R003', N'Nguyễn Bác sĩ 4', 'nguyenbacsi123', 'nguyenbacsi4@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000015', 'R003', N'Nguyễn Bác sĩ 5', 'nguyenbacsi123', 'nguyenbacsi5@gmail.com');


--tạo tk staff
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000016', 'R004', N'Nguyễn Nhân Viên', 'nguyennhanvien123', 'nguyennhanvien@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000017', 'R004', N'Nguyễn Nhân Viên 2', 'nguyennhanvien123', 'nguyennhanvien2@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000018', 'R004', N'Nguyễn Nhân Viên 3', 'nguyennhanvien123', 'nguyennhanvien3@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000019', 'R004', N'Nguyễn Nhân Viên 4', 'nguyennhanvien123', 'nguyennhanvien4@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000020', 'R004', N'Nguyễn Nhân Viên 5', 'nguyennhanvien123', 'nguyennhanvien5@gmail.com');



--tạo tk patient
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email) 
VALUES ('UID000021', 'R005', N'Nguyễn Bệnh Nhân 1', 'nguyenbenhnhan123', 'nguyenbenhnhan@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email)
VALUES ('UID000022', 'R005', N'Nguyễn Bệnh Nhân 2', 'nguyenbenhnhan123', 'nguyenbenhnhan2@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email)
VALUES ('UID000023', 'R005', N'Nguyễn Bệnh Nhân 3', 'nguyenbenhnhan123', 'nguyenbenhnhan3@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email)
VALUES ('UID000024', 'R005', N'Nguyễn Bệnh Nhân 4', 'nguyenbenhnhan123', 'nguyenbenhnhan4@gmail.com');
INSERT INTO Users (UserID, RoleID, Fullname, Password, Email)
VALUES ('UID000025', 'R005', N'Nguyễn Bệnh Nhân 5', 'nguyenbenhnhan123', 'nguyenbenhnhan5@gmail.com');


-- Thêm dữ liệu vào bảng Doctors
INSERT INTO Doctors (DoctorID, UserID, Specialization, LicenseNumber, ExperienceYears) 
VALUES ('DT000001', 'UID000011', N'Miễn dịch', 'LIC123456', 10);
INSERT INTO Doctors (DoctorID, UserID, Specialization, LicenseNumber, ExperienceYears) 
VALUES ('DT000002', 'UID000012', N'Bệnh truyền nhiễm', 'LIC789012', 15);




-- Thêm dữ liệu vào bảng Patients
INSERT INTO Patients (PatientID, UserID, DateOfBirth, Gender, Phone, BloodType, Allergy) 
VALUES ('PT000001', 'UID000021', '15-05-1980', N'Nam', '0901234567', 'O+', N'Không có');
INSERT INTO Patients (PatientID, UserID, DateOfBirth, Gender, Phone, BloodType, Allergy) 
VALUES ('PT000002', 'UID000022', '20-10-1990', N'Nữ', '0912345678', 'A+', N'Dị ứng với penicillin');
INSERT INTO Patients (PatientID, UserID, DateOfBirth, Gender, Phone, BloodType, Allergy) 
VALUES ('PT000003', 'UID000023', '08-03-1975', N'Nam', '0923456789', 'B-', N'Dị ứng với hải sản');

-- Thêm dữ liệu vào bảng Services
INSERT INTO Services (ServiceID, ServiceName, Type, Price) 
VALUES ('SV000001', N'Khám tổng quát', N'Khám bệnh', 700000);
INSERT INTO Services (ServiceID, ServiceName, Type, Price) 
VALUES ('SV000002', N'Tư vấn điều trị', N'Tư vấn', 300000);
INSERT INTO Services (ServiceID, ServiceName, Type, Price) 
VALUES ('SV000003', N'Xét nghiệm HIV', N'Xét nghiệm', 1500000);
INSERT INTO Services (ServiceID, ServiceName, Type, Price) 
VALUES ('SV000004', N'Xét nghiệm CD4', N'Xét nghiệm', 850000);
INSERT INTO Services (ServiceID, ServiceName, Type, Price) 
VALUES ('SV000005', N'Tư vấn điều trị ARV', N'Tư vấn', 400000);
INSERT INTO Services (ServiceID, ServiceName, Type, Price) 
VALUES ('SV000006', N'Xét nghiệm tải lượng virus', N'Xét nghiệm', 1000000);

-- Thêm dữ liệu vào bảng DoctorServices
INSERT INTO DoctorServices (DoctorServiceID, DoctorID, ServiceID) 
VALUES ('DSV000001', 'DT000001', 'SV000004');
INSERT INTO DoctorServices (DoctorServiceID, DoctorID, ServiceID) 
VALUES ('DSV000002', 'DT000001', 'SV000005');
INSERT INTO DoctorServices (DoctorServiceID, DoctorID, ServiceID) 
VALUES ('DSV000003', 'DT000002', 'SV000005');
INSERT INTO DoctorServices (DoctorServiceID, DoctorID, ServiceID) 
VALUES ('DSV000004', 'DT000002', 'SV000004');
INSERT INTO DoctorServices (DoctorServiceID, DoctorID, ServiceID) 
VALUES ('DSV000005', 'DT000002', 'SV000006');

-- Thêm dữ liệu vào bảng Slot
INSERT INTO Slot (SlotID, SlotNumber, StartTime, EndTime) 
VALUES ('SL000001', 1, '07:00', '9:30');
INSERT INTO Slot (SlotID, SlotNumber, StartTime, EndTime) 
VALUES ('SL000002', 2, '09:30', '12:00');
INSERT INTO Slot (SlotID, SlotNumber, StartTime, EndTime) 
VALUES ('SL000003', 3, '12:00', '14:30');
INSERT INTO Slot (SlotID, SlotNumber, StartTime, EndTime) 
VALUES ('SL000004', 4, '14:30', '17:00');
INSERT INTO Slot (SlotID, SlotNumber, StartTime, EndTime) 
VALUES ('SL000005', 5, '17:00', '19:30');
INSERT INTO Slot (SlotID, SlotNumber, StartTime, EndTime) 
VALUES ('SL000006', 6, '19:30', '22:00');

-- Thêm dữ liệu vào bảng DoctorWorkSchedule
INSERT INTO DoctorWorkSchedule (ScheduleID, DoctorID, SlotID, DayOfWeek, Status) 
VALUES ('SCH000001', 'DT000001', 'SL000001', N'Thứ hai', N'Khả dụng');
INSERT INTO DoctorWorkSchedule (ScheduleID, DoctorID, SlotID, DayOfWeek, Status) 
VALUES ('SCH000002', 'DT000001', 'SL000002', N'Thứ hai', N'Khả dụng');
INSERT INTO DoctorWorkSchedule (ScheduleID, DoctorID, SlotID, DayOfWeek, Status) 
VALUES ('SCH000003', 'DT000002', 'SL000003', N'Thứ ba', N'Khả dụng');
INSERT INTO DoctorWorkSchedule (ScheduleID, DoctorID, SlotID, DayOfWeek, Status) 
VALUES ('SCH000004', 'DT000002', 'SL000004', N'Thứ ba', N'Khả dụng');
INSERT INTO DoctorWorkSchedule (ScheduleID, DoctorID, SlotID, DayOfWeek, Status) 
VALUES ('SCH000005', 'DT000002', 'SL000005', N'Thứ tư', N'Không khả dụng');


-- Thêm dữ liệu vào bảng Books
INSERT INTO Books (BookID, PatientID, DoctorID, ServiceID, BookDate, Status, LinkMeet, Note) 
VALUES ('BK000001', 'PT000001', 'DT000001', 'SV000001', '10-06-2025 08:00:00', N'Đã xác nhận', 'https://meet.google.com/abc-defg-hij', N'Khám định kỳ');

INSERT INTO Books (BookID, PatientID, DoctorID, ServiceID, BookDate, Status, LinkMeet, Note) 
VALUES ('BK000002', 'PT000002', 'DT000002', 'SV000003', '12-06-2025 09:00:00', N'Đã xác nhận', 'https://meet.google.com/klm-nopq-rst', N'Xét nghiệm thường niên');

INSERT INTO Books (BookID, PatientID, DoctorID, ServiceID, BookDate, Status, LinkMeet, Note) 
VALUES ('BK000003', 'PT000003', 'DT000002', 'SV000004', '15-06-2025 10:00:00', N'Đang chờ', 'https://meet.google.com/uvw-xyza-bcd', N'Tư vấn lần đầu');
-- Thêm dữ liệu vào bảng MedicalRecord
INSERT INTO MedicalRecord (MedicalRecordID, PatientID, DoctorID, Diagnosis, CreatedDate, TreatmentResult) 
VALUES ('MR000001', 'PT000001', 'DT000001', N'Tình trạng sức khỏe ổn định', '10-05-2025 09:15:00', N'Tiếp tục theo dõi');
INSERT INTO MedicalRecord (MedicalRecordID, PatientID, DoctorID, Diagnosis, CreatedDate, TreatmentResult) 
VALUES ('MR000002', 'PT000002', 'DT000002', N'Nhiễm HIV giai đoạn đầu', '15-05-2025 10:30:00', N'Bắt đầu điều trị ARV');

-- Thêm dữ liệu vào bảng Medication
INSERT INTO Medication (MedicationID, MedicationName, DosageForm, Strength, TargetGroup, CreatedAt) 
VALUES ('MD000001', N'Tenofovir', N'Viên nén', '300mg', N'Người trưởng thành', '2025-02-01 12:20:16');
INSERT INTO Medication (MedicationID, MedicationName, DosageForm, Strength, TargetGroup, CreatedAt) 
VALUES ('MD000002', N'Lamivudine', N'Viên nén', '150mg', N'Người trưởng thành', '2025-02-01 10:02:00');
INSERT INTO Medication (MedicationID, MedicationName, DosageForm, Strength, TargetGroup, CreatedAt) 
VALUES ('MD000003', N'Efavirenz', N'Viên nén', '600mg', N'Người trưởng thành', '2025-02-01 10:03:00');

-- Thêm dữ liệu vào bảng Prescription
INSERT INTO Prescription (PrescriptionID, MedicalRecordID, MedicationID, DoctorID, StartDate, EndDate, Dosage, LineOfTreatment, CreatedAt) 
VALUES ('PR000001', 'MR000002', 'MD000001', 'DT000002', '15-05-2025', '15-08-2025', N'1 viên mỗi ngày', N'Đầu tiên', '15-05-2025 10:45:00');
INSERT INTO Prescription (PrescriptionID, MedicalRecordID, MedicationID, DoctorID, StartDate, EndDate, Dosage, LineOfTreatment, CreatedAt) 
VALUES ('PR000002', 'MR000002', 'MD000002', 'DT000002', '15-05-2025', '15-08-2025', N'1 viên mỗi ngày', N'Đầu tiên', '15-05-2025 10:45:00');

-- Thêm dữ liệu vào bảng ARVRegimen
INSERT INTO ARVRegimen (ARVRegimenID, MedicalRecordID, DoctorID, RegimenCode, ARVName, Description, AgeRange, ForGroup, CreatedAt) 
VALUES ('ARV000001', 'MR000002', 'DT000002', 'TDF+3TC+EFV', N'Tenofovir + Lamivudine + Efavirenz', N'Phác đồ bậc 1 cho người mới điều trị', N'Trên 15 tuổi', N'Người lớn', '15-05-2025 10:50:00');

-- Thêm dữ liệu vào bảng HIVTest
INSERT INTO HIVTest (TestResultID, MedicalRecordID, TestDate, CD4Count, ViralLoad, Notes) 
VALUES ('HT000001', 'MR000002', '15-05-2025', 450, 10000, N'Xét nghiệm trước khi bắt đầu điều trị');

-- Thêm dữ liệu vào bảng Reminder
INSERT INTO Reminder (ReminderCheckID, PatientID, BookID, ReminderTime, Message) 
VALUES ('REC000001', 'PT000001', 'BK000001', '09-06-2025 09:09:00', N'Nhắc nhở: Lịch khám ngày mai lúc 07:00');
INSERT INTO Reminder (ReminderCheckID, PatientID, BookID, ReminderTime, Message) 
VALUES ('REC000002', 'PT000002', 'BK000002', '15-08-2025 10:15:00', N'Nhắc nhở: Lịch xét nghiệm CD4 ngày mai lúc 09:00');

-- Thêm dữ liệu vào bảng ReminderMedication
INSERT INTO ReminderMedication (ReminderMedicationID, PatientID, PrescriptionID, DosageTime, Instruction, MedicationInUse) 
VALUES ('REM000001', 'PT000002', 'PR000001', '08:00', N'Uống sau bữa sáng', N'Tenofovir đang sử dụng');
INSERT INTO ReminderMedication (ReminderMedicationID, PatientID, PrescriptionID, DosageTime, Instruction, MedicationInUse) 
VALUES ('REM000002', 'PT000002', 'PR000002', '08:00', N'Uống sau bữa sáng', N'Lamivudine đang sử dụng');
