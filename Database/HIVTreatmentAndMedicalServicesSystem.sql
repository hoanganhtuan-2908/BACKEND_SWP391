CREATE TABLE [Roles] (
  [RoleID] varchar(5) PRIMARY KEY, --R001
  [RoleName] nvarchar(50) NOT NULL
)
GO

CREATE TABLE [Users] (
  [UserID] varchar(5) PRIMARY KEY, --UI001
  [RoleID] varchar(5) NOT NULL,
  [Fullname] nvarchar(100) NOT NULL,
  [Password] varchar(255) NOT NULL,
  [Email] varchar(100) UNIQUE NOT NULL,
  [Address] nvarchar(100),
  [Image] varchar(100)
)
GO

CREATE TABLE [Patients] ( 
  [PatientID] varchar(5) PRIMARY KEY, --PT001
  [UserID] varchar(5) NOT NULL,
  [DateOfBirth] date,
  [Gender] nvarchar(20),
  [Phone] varchar(20),
  [BloodType] varchar(10),
  [Allergy] nvarchar
)
GO

CREATE TABLE [Doctors] (
  [DoctorID] varchar(5) PRIMARY KEY, --DT001
  [UserID] varchar(5) NOT NULL,
  [Specialization] nvarchar(100),
  [LicenseNumber] nvarchar(50) UNIQUE,
  [ExperienceYears] int
)
GO

CREATE TABLE [Slot] (
  [SlotID] varchar(5) PRIMARY KEY, --SL001
  [SlotNumber] int,
  [StartTime] time NOT NULL,
  [EndTime] time NOT NULL
)
GO

CREATE TABLE [DoctorWorkSchedule] (
  [ScheduleID] varchar(5) PRIMARY KEY, --DW001
  [DoctorID] varchar(5) NOT NULL,
  [SlotID] varchar(5) NOT NULL,
  [DayOfWeek] nvarchar(10)
)
GO

CREATE TABLE [Booking] (
  [BookID] varchar(5) PRIMARY KEY, --BK001
  [PatientID] varchar(5) NOT NULL,
  [DoctorID] varchar(5) NOT NULL,
  [BookingType] nvarchar(50),
  [LabRequest] nvarchar(50),
  [BookDate] datetime NOT NULL,
  [Status] nvarchar(20) DEFAULT N'Đang chờ' CHECK (Status IN (N'Đang chờ', N'Xác nhận', N'Từ chối')),
  [LinkMeet] nvarchar(255),
  [Note] nvarchar
)
GO

CREATE TABLE [TreatmentPlan] (
  [TreatmentPlanID] varchar(5) PRIMARY KEY, --TP001
  [PatientID] varchar(5) NOT NULL,
  [DoctorID] varchar(5) NOT NULL,
  [ARVProtocol] varchar(5) NOT NULL,
  [TreatmentLine] int,
  [Diagnosis] nvarchar,
  [TreatmentResult] nvarchar,
  [CreatedDate] datetime DEFAULT (CURRENT_TIMESTAMP)
)
GO

CREATE TABLE [Medication] (
  [MedicationID] varchar(5) PRIMARY KEY, --MD001
  [MedicationName] nvarchar(100) NOT NULL,
  [DosageForm] nvarchar(50),
  [Strength] nvarchar(50),
  [TargetGroup] nvarchar(50),
  [CreatedAt] datetime DEFAULT (CURRENT_TIMESTAMP)
)
GO

CREATE TABLE [Prescription] (
  [PrescriptionID] varchar(5) PRIMARY KEY, --PR001
  [MedicalRecordID] varchar(5) NOT NULL,
  [MedicationID] varchar(5) NOT NULL,
  [DoctorID] varchar(5) NOT NULL,
  [StartDate] date NOT NULL,
  [EndDate] date,
  [Dosage] nvarchar(100),
  [LineOfTreatment] nvarchar(50),
  [CreatedAt] datetime DEFAULT (CURRENT_TIMESTAMP)
)
GO

CREATE TABLE [ARVProtocol] (
  [ARVID] varchar(5) PRIMARY KEY, --AP001
  [ARVCode] nvarchar(50),
  [ARVName] nvarchar(100),
  [Description] nvarchar,
  [AgeRange] nvarchar(50),
  [ForGroup] nvarchar(50),
  [CreatedAt] datetime DEFAULT (CURRENT_TIMESTAMP)
)
GO

CREATE TABLE [Reminder] (
  [ReminderCheckID] varchar(5) PRIMARY KEY, --RM001
  [PatientID] varchar(5) NOT NULL,
  [TreatmentPlantID] varchar(5) NOT NULL,
  [PrescriptionID] varchar(5) NOT NULL,
  [ReminderType] varchar(50),
  [ReminderTime] datetime NOT NULL,
  [Message] nvarchar
)
GO

CREATE TABLE [LabTests] (
  [LabTestID] varchar(5) PRIMARY KEY,--LT001
  [RequestID] varchar(5) NOT NULL,
  [TreatmentPlantID] varchar(5) NOT NULL,
  [TestName] nvarchar(100) NOT NULL,
  [TestCode] varchar(50) UNIQUE,
  [TestType] varchar(50),
  [ResultValue] nvarchar(100),
  [CD4Initial] int,
  [ViralLoadInitial] int,
  [Status] varchar(20) DEFAULT N'Đang chờ' CHECK (Status IN (N'Đang chờ', N'Xác nhận', N'Từ chối')),
  [Description] nvarchar
)
GO

CREATE TABLE [Payment] (
  [PaymentID] varchar(5) PRIMARY KEY, -- PM001
  [BookID] varchar(5) NOT NULL,
  [Amount] decimal(10,2) NOT NULL,
  [PaymentDate] datetime NOT NULL,
  [PaymentMethod] nvarchar(50) NOT NULL, -- Phương thức thanh toán, không phải status
  [Status] nvarchar(20) DEFAULT N'Đang chờ' CHECK (Status IN (N'Đang chờ', N'Xác nhận', N'Từ chối')),
  [Description] nvarchar,
  [CreatedAt] datetime DEFAULT (CURRENT_TIMESTAMP)
)
GO

ALTER TABLE [Users] ADD FOREIGN KEY ([RoleID]) REFERENCES [Roles] ([RoleID])
GO

ALTER TABLE [Patients] ADD FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
GO

ALTER TABLE [Doctors] ADD FOREIGN KEY ([UserID]) REFERENCES [Users] ([UserID])
GO

ALTER TABLE [DoctorWorkSchedule] ADD FOREIGN KEY ([DoctorID]) REFERENCES [Doctors] ([DoctorID])
GO

ALTER TABLE [DoctorWorkSchedule] ADD FOREIGN KEY ([SlotID]) REFERENCES [Slot] ([SlotID])
GO

ALTER TABLE [Booking] ADD FOREIGN KEY ([PatientID]) REFERENCES [Patients] ([PatientID])
GO

ALTER TABLE [Booking] ADD FOREIGN KEY ([DoctorID]) REFERENCES [Doctors] ([DoctorID])
GO

ALTER TABLE [TreatmentPlan] ADD FOREIGN KEY ([PatientID]) REFERENCES [Patients] ([PatientID])
GO

ALTER TABLE [TreatmentPlan] ADD FOREIGN KEY ([DoctorID]) REFERENCES [Doctors] ([DoctorID])
GO

ALTER TABLE [TreatmentPlan] ADD FOREIGN KEY ([ARVProtocol]) REFERENCES [ARVProtocol] ([ARVID])
GO

ALTER TABLE [Prescription] ADD FOREIGN KEY ([MedicalRecordID]) REFERENCES [TreatmentPlan] ([TreatmentPlanID])
GO

ALTER TABLE [Prescription] ADD FOREIGN KEY ([MedicationID]) REFERENCES [Medication] ([MedicationID])
GO

ALTER TABLE [Prescription] ADD FOREIGN KEY ([DoctorID]) REFERENCES [Doctors] ([DoctorID])
GO

ALTER TABLE [Reminder] ADD FOREIGN KEY ([PatientID]) REFERENCES [Patients] ([PatientID])
GO

ALTER TABLE [Reminder] ADD FOREIGN KEY ([TreatmentPlantID]) REFERENCES [TreatmentPlan] ([TreatmentPlanID])
GO

ALTER TABLE [Reminder] ADD FOREIGN KEY ([PrescriptionID]) REFERENCES [Prescription] ([PrescriptionID])
GO

ALTER TABLE [LabTests] ADD FOREIGN KEY ([RequestID]) REFERENCES [Booking] ([BookID])
GO

ALTER TABLE [LabTests] ADD FOREIGN KEY ([TreatmentPlantID]) REFERENCES [TreatmentPlan] ([TreatmentPlanID])
GO

ALTER TABLE [Payment] ADD FOREIGN KEY ([BookID]) REFERENCES [Booking] ([BookID])
GO
-- Roles
insert into Roles values ('R001', 'Admin');
insert into Roles values ('R002', 'Manager');
insert into Roles values ('R003', 'Doctor');
insert into Roles values ('R004', 'Staff');
insert into Roles values ('R005', 'Patient');

-- Admin
INSERT INTO Users VALUES ('UI001', 'R001', N'Lê Quốc Việt', '123456', 'lequocviet@gmail.com', N'Hà Nội', '/image/admin.png');
INSERT INTO Users VALUES ('UI002', 'R001', N'Nguyễn Văn Nguyên', '123456', 'nguyenvannguyen@gmail.com', N'Hồ Chí Minh', 'admin2.png');
INSERT INTO Users VALUES ('UI003', 'R001', N'Hoàng Anh Tuấn', '123456', 'hoanganhtuan@gmail.com', N'Lâm Đồng', 'admin3.jpg');
INSERT INTO Users VALUES ('UI004', 'R001', N'Võ Việt Dũng', '123456', 'vovietdung@gmail.com', N'Hồ Chí Minh', 'admin4.jpg');
INSERT INTO Users VALUES ('UI005', 'R001', N'Nguyễn Ngọc Tín', '123456', 'nguyenngoctin@gmail.com', N'Hồ Chí Minh', 'admin5.jpg');
INSERT INTO Users VALUES ('UI006', 'R001', N'Đỗ Thị Nhung', '123456', 'dothinhung@gmail.com', N'Hà Nội', 'admin6.jpg');
INSERT INTO Users VALUES ('UI007', 'R001', N'Vũ Văn Thái', '123456', 'vuvanthai7@gmail.com', N'Hà Nội', 'admin7.jpg');
INSERT INTO Users VALUES ('UI008', 'R001', N'Đặng Thị Xuân', '123456', 'dothixuan@gmail.com', N'Hồ Chí Minh', 'admin8.jpg');
INSERT INTO Users VALUES ('UI009', 'R001', N'Mai Văn Tuyền', '123456', 'maivantuyen@gmail.com', N'Hồ Chí Minh', 'admin9.jpg');
INSERT INTO Users VALUES ('UI010', 'R001', N'Trịnh Thị Yến', '123456', 'trinhthiyen@gmail.com', N'Hà Nội', 'admin10.jpg');

INSERT INTO Users VALUES ('UI051', 'R003', N'Lê Minh Tuấn', '123456', 'leminhtuan@gmail.com', N'Hà Nội', '/image/doctor51.png');
INSERT INTO Users VALUES ('UI052', 'R003', N'Phạm Thu Hằng', '123456', 'phamthuhang@gmail.com', N'Hồ Chí Minh', '/image/doctor52.png');
INSERT INTO Users VALUES ('UI053', 'R003', N'Ngô Văn Huy', '123456', 'ngovanhuy@gmail.com', N'Đà Nẵng', '/image/doctor53.png');
INSERT INTO Users VALUES ('UI054', 'R003', N'Trần Hải Yến', '123456', 'tranhaiyen@gmail.com', N'Hải Phòng', '/image/doctor54.png');
INSERT INTO Users VALUES ('UI055', 'R003', N'Hoàng Đức Anh', '123456', 'hoangducanh@gmail.com', N'Cần Thơ', '/image/doctor55.png');
INSERT INTO Users VALUES ('UI056', 'R003', N'Đỗ Thị Mai', '123456', 'dothimai@gmail.com', N'Nghệ An', '/image/doctor56.png');
INSERT INTO Users VALUES ('UI057', 'R003', N'Vũ Ngọc Long', '123456', 'vungoclong@gmail.com', N'Thái Bình', '/image/doctor57.png');
INSERT INTO Users VALUES ('UI058', 'R003', N'Bùi Thanh Hương', '123456', 'buithanhhuong@gmail.com', N'Hưng Yên', '/image/doctor58.png');
INSERT INTO Users VALUES ('UI059', 'R003', N'Tạ Quang Dũng', '123456', 'taquangdung@gmail.com', N'Lâm Đồng', '/image/doctor59.png');
INSERT INTO Users VALUES ('UI060', 'R003', N'Nguyễn Thị Kim', '123456', 'nguyenthikim@gmail.com', N'Bình Dương', '/image/doctor60.png');


-- Manager
INSERT INTO Users VALUES ('UI011', 'R002', N'Nguyễn Văn Nguyên', '123456', 'nguyenvannguyen@gmail.com', N'Hà Nội', 'manager1.jpg');
INSERT INTO Users VALUES ('UI012', 'R002', N'Trần Thị Hạnh', '123456', 'tranthihanh@gmail.com', N'Hồ Chí Minh', 'manager2.jpg');
INSERT INTO Users VALUES ('UI013', 'R002', N'Lê Quốc Hùng', '123456', 'lequochung@gmail.com', N'Đà Nẵng', 'manager3.jpg');
INSERT INTO Users VALUES ('UI014', 'R002', N'Phạm Thị Duyên', '123456', 'phamthiduyen@gmail.com', N'Cần Thơ', 'manager4.jpg');
INSERT INTO Users VALUES ('UI015', 'R002', N'Hoàng Văn Thái', '123456', 'hoangvanthai@gmail.com', N'Hải Phòng', 'manager5.jpg');
INSERT INTO Users VALUES ('UI016', 'R002', N'Đỗ Thị Hạnh', '123456', 'dithihanh@gmail.com', N'Nha Trang', 'manager6.jpg');
INSERT INTO Users VALUES ('UI017', 'R002', N'Vũ Văn Tuyền', '123456', 'levantuyen@gmail.com', N'Đà Lạt', 'manager7.jpg');
INSERT INTO Users VALUES ('UI018', 'R002', N'Đặng Thị Thu', '123456', 'dangthithu@gmail.com', N'Huế', 'manager8.jpg');
INSERT INTO Users VALUES ('UI019', 'R002', N'Mai Quốc Khánh', '123456', 'maiquockhanh9@gmail.com', N'Quảng Ninh', 'manager9.jpg');
INSERT INTO Users VALUES ('UI020', 'R002', N'Trịnh Thị Hồng', '123456', 'trinhthihong@gmail.com', N'Bắc Ninh', 'manager10.jpg');

-- Doctor
INSERT INTO Users VALUES ('UI021', 'R003', N'Nguyễn Văn An', '123456', 'nguyenvanan@gmail.com', N'Hà Nội', '/image/doctor21.png');
INSERT INTO Users VALUES ('UI022', 'R003', N'Trần Bình An', '123456', 'tranbinhan@gmail.com', N'Hồ Chí Minh', '/image/doctor22.png');
INSERT INTO Users VALUES ('UI023', 'R003', N'Lê Văn Cường', '123456', 'levancuong@gmail.com', N'Đà Nẵng', '/image/doctor23.png');
INSERT INTO Users VALUES ('UI024', 'R003', N'Phạm Thị Dung', '123456', 'phamthidung@gmail.com', N'Cần Thơ', '/image/doctor24.png');
INSERT INTO Users VALUES ('UI025', 'R003', N'Hoàng Văn Anh', '123456', 'hoangvananh@gmail.com', N'Hải Phòng', '/image/doctor25.png');
INSERT INTO Users VALUES ('UI026', 'R003', N'Đỗ Thị Phương', '123456', 'dothiphuong@gmail.com', N'Nha Trang', '/image/doctor26.png');
INSERT INTO Users VALUES ('UI027', 'R003', N'Vũ Văn Giang', '123456', 'vuvangiang@gmail.com', N'Đà Lạt', '/image/doctor27.png');
INSERT INTO Users VALUES ('UI028', 'R003', N'Đặng Thị Hương', '123456', 'dangthihuong@gmail.com', N'Huế', '/image/doctor28.png');
INSERT INTO Users VALUES ('UI029', 'R003', N'Mai Văn Khoa', '123456', 'maivankhoa@gmail.com', N'Quảng Ninh', '/image/doctor29.png');
INSERT INTO Users VALUES ('UI030', 'R003', N'Trịnh Thị Linh', '123456', 'trinhthilinh@gmail.com', N'Bắc Ninh', '/image/doctor30.png');


-- Staff
INSERT INTO Users VALUES ('UI031', 'R004', N'Nguyễn Văn Nam', '123456', 'nguyenvannam@gmail.com', N'Hà Nội', 'staff1.jpg');
INSERT INTO Users VALUES ('UI032', 'R004', N'Trần Thị Oanh', '123456', 'tranthioanh@gmail.com', N'Hồ Chí Minh', 'staff2.jpg');
INSERT INTO Users VALUES ('UI033', 'R004', N'Lê Văn Phong', '123456', 'levanphong@gmail.com', N'Đà Nẵng', 'staff3.jpg');
INSERT INTO Users VALUES ('UI034', 'R004', N'Phạm Thị Quỳnh', '123456', 'phamthiquynh@gmail.com', N'Cần Thơ', 'staff4.jpg');
INSERT INTO Users VALUES ('UI035', 'R004', N'Hoàng Văn Phong', '123456', 'hoangvanphong@gmail.com', N'Hải Phòng', 'staff5.jpg');
INSERT INTO Users VALUES ('UI036', 'R004', N'Đỗ Thị Sương', '123456', 'dothisuong@gmail.com', N'Nha Trang', 'staff6.jpg');
INSERT INTO Users VALUES ('UI037', 'R004', N'Vũ Văn Toàn', '123456', 'vuvantoan@gmail.com', N'Đà Lạt', 'staff7.jpg');
INSERT INTO Users VALUES ('UI038', 'R004', N'Đặng Thị Uyên', '123456', 'dangthiuyen@gmail.com', N'Huế', 'staff8.jpg');
INSERT INTO Users VALUES ('UI039', 'R004', N'Mai Văn Vinh', '123456', 'maivanvinh@gmail.com', N'Quảng Ninh', 'staff9.jpg');
INSERT INTO Users VALUES ('UI040', 'R004', N'Trịnh Trần Xuân', '123456', 'trinhtranxuan@gmail.com', N'Bắc Ninh', 'staff10.jpg');

-- Patient
INSERT INTO Users VALUES ('UI041', 'R005', N'Ngô Bá khá', '123456', 'ngobakha@gmail.com', N'Hà Nội', 'patient1.jpg');
INSERT INTO Users VALUES ('UI042', 'R005', N'Trần Thị Thắm', '123456', 'tranthitham@gmail.com', N'Hồ Chí Minh', 'patient2.jpg');
INSERT INTO Users VALUES ('UI043', 'R005', N'Lê Văn Anh', '123456', 'levananh@gmail.com', N'Đà Nẵng', 'patient3.jpg');
INSERT INTO Users VALUES ('UI044', 'R005', N'Phạm Thị Bích', '123456', 'phamthibich@gmail.com', N'Cần Thơ', 'patient4.jpg');
INSERT INTO Users VALUES ('UI045', 'R005', N'Hoàng Văn Cảnh', '123456', 'hoangvancanh@gmail.com', N'Hải Phòng', 'patient5.jpg');
INSERT INTO Users VALUES ('UI046', 'R005', N'Đỗ Thị Diệp', '123456', 'dothihiep@gmail.com', N'Nha Trang', 'patient6.jpg');
INSERT INTO Users VALUES ('UI047', 'R005', N'Vũ Văn Em', '123456', 'vuvanem@gmail.com', N'Đà Lạt', 'patient7.jpg');
INSERT INTO Users VALUES ('UI048', 'R005', N'Đặng Thị Phúc', '123456', 'phamthiphuc@gmail.com', N'Huế', 'patient8.jpg');
INSERT INTO Users VALUES ('UI049', 'R005', N'Mai Văn Giáp', '123456', 'maivangiap@gmail.com', N'Quảng Ninh', 'patient9.jpg');
INSERT INTO Users VALUES ('UI050', 'R005', N'Trịnh Thị Hòa', '123456', 'trinhthihoa@gmail.com', N'Bắc Ninh', 'patient10.jpg');


-- 3. Bảng Patients

INSERT INTO Patients VALUES ('PT001', 'UI041', '1985-05-15', N'Nam', '0901234567', 'A+', N'Không có dị ứng');
INSERT INTO Patients VALUES ('PT002', 'UI042', '1990-08-20', N'Nữ', '0912345678', 'O+', N'Dị ứng với Penicillin');
INSERT INTO Patients VALUES ('PT003', 'UI043', '1978-03-10', N'Nam', '0923456789', 'B+', N'Dị ứng với hải sản');
INSERT INTO Patients VALUES ('PT004', 'UI044', '1995-11-25', N'Nữ', '0934567890', 'AB+', N'Không có dị ứng');
INSERT INTO Patients VALUES ('PT005', 'UI045', '1982-07-08', N'Nam', '0945678901', 'A-', N'Dị ứng với mật ong');
INSERT INTO Patients VALUES ('PT006', 'UI046', '1998-02-14', N'Nữ', '0956789012', 'O-', N'Dị ứng với phấn hoa');
INSERT INTO Patients VALUES ('PT007', 'UI047', '1975-09-30', N'Nam', '0967890123', 'B-', N'Không có dị ứng');
INSERT INTO Patients VALUES ('PT008', 'UI048', '1993-04-18', N'Nữ', '0978901234', 'AB-', N'Dị ứng với thịt bò');
INSERT INTO Patients VALUES ('PT009', 'UI049', '1980-12-05', N'Nam', '0989012345', 'A+', N'Dị ứng với đậu phộng');
INSERT INTO Patients VALUES ('PT010', 'UI050', '1988-06-22', N'Nữ', '0990123456', 'O+', N'Không có dị ứng');

 --4. Bảng Doctors

INSERT INTO Doctors VALUES ('DT001', 'UI021', 'HIV Specialist', 'LN12345', 15);
INSERT INTO Doctors VALUES ('DT002', 'UI022', 'Infectious Disease', 'LN23456', 10);
INSERT INTO Doctors VALUES ('DT003', 'UI023', 'Immunology', 'LN34567', 12);
INSERT INTO Doctors VALUES ('DT004', 'UI024', 'Internal Medicine', 'LN45678', 8);
INSERT INTO Doctors VALUES ('DT005', 'UI025', 'HIV Prevention', 'LN56789', 14);
INSERT INTO Doctors VALUES ('DT006', 'UI026', 'Virology', 'LN67890', 9);
INSERT INTO Doctors VALUES ('DT007', 'UI027', 'Antiretroviral Therapy', 'LN78901', 11);
INSERT INTO Doctors VALUES ('DT008', 'UI028', 'Family Medicine', 'LN89012', 7);
INSERT INTO Doctors VALUES ('DT009', 'UI029', 'Pediatric HIV', 'LN90123', 13);
INSERT INTO Doctors VALUES ('DT010', 'UI030', 'HIV Research', 'LN01234', 16);

--5. Bảng Slot

INSERT INTO Slot VALUES ('SL001', 1, '06:00:00', '08:00:00');
INSERT INTO Slot VALUES ('SL002', 2, '08:00:00', '10:00:00');
INSERT INTO Slot VALUES ('SL003', 3, '10:00:00', '12:00:00');
INSERT INTO Slot VALUES ('SL004', 4, '12:00:00', '14:00:00');
INSERT INTO Slot VALUES ('SL005', 5, '14:00:00', '16:00:00');
INSERT INTO Slot VALUES ('SL006', 6, '16:00:00', '18:00:00');


-- 6. Bảng DoctorWorkSchedule

INSERT INTO DoctorWorkSchedule VALUES ('DW001', 'DT001', 'SL001', N'Thứ hai');
INSERT INTO DoctorWorkSchedule VALUES ('DW002', 'DT002', 'SL002', N'Thứ ba');
INSERT INTO DoctorWorkSchedule VALUES ('DW003', 'DT003', 'SL003', N'Thứ tư');
INSERT INTO DoctorWorkSchedule VALUES ('DW004', 'DT004', 'SL004', N'Thứ năm');
INSERT INTO DoctorWorkSchedule VALUES ('DW005', 'DT005', 'SL005', N'Thứ sáu');
INSERT INTO DoctorWorkSchedule VALUES ('DW006', 'DT006', 'SL006', N'Thứ bảy');
INSERT INTO DoctorWorkSchedule VALUES ('DW007', 'DT007', 'SL002', N'Chủ nhật');
INSERT INTO DoctorWorkSchedule VALUES ('DW008', 'DT008', 'SL003', N'Thứ hai');
INSERT INTO DoctorWorkSchedule VALUES ('DW009', 'DT009', 'SL001', N'Thứ ba');
INSERT INTO DoctorWorkSchedule VALUES ('DW010', 'DT010', 'SL006', N'Thứ tư');

-- 7. Bảng ARVProtocol

INSERT INTO ARVProtocol VALUES ('AP001', 'ARV-TLD', 'Tenofovir-Lamivudine-Dolutegravir', 'First-line regimen recommended by WHO', '18+', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP002', 'ARV-TLE', 'Tenofovir-Lamivudine-Efavirenz', 'Alternative first-line regimen', '15+', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP003', 'ARV-AZT', 'Zidovudine-Lamivudine-Nevirapine', 'For patients with kidney problems', '12+', 'Adults & Adolescents', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP004', 'ARV-ABC', 'Abacavir-Lamivudine-Dolutegravir', 'For pediatric patients', '3-12', 'Children', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP005', 'ARV-DTG', 'Dolutegravir-based regimen', 'High genetic barrier to resistance', '18+', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP006', 'ARV-RAL', 'Raltegravir-based regimen', 'For pregnant women', '18+', 'Pregnant Women', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP007', 'ARV-DRV', 'Darunavir-Ritonavir regimen', 'Second-line treatment', '18+', 'Adults with resistance', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP008', 'ARV-LPV', 'Lopinavir-Ritonavir regimen', 'For children and adolescents', '5-18', 'Children & Adolescents', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP009', 'ARV-ATV', 'Atazanavir-based regimen', 'For adults with comorbidities', '18+', 'Adults with TB', CURRENT_TIMESTAMP);
INSERT INTO ARVProtocol VALUES ('AP010', 'ARV-BIC', 'Bictegravir-based regimen', 'Latest recommendation', '18+', 'Adults', CURRENT_TIMESTAMP);

-- 8. Bảng TreatmentPlan

INSERT INTO TreatmentPlan VALUES ('TP001', 'PT001', 'DT001', 'AP001', 1, 'HIV stage 1', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP002', 'PT002', 'DT002', 'AP002', 1, 'HIV stage 2', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP003', 'PT003', 'DT003', 'AP003', 2, 'HIV stage 3', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP004', 'PT004', 'DT004', 'AP004', 1, 'HIV stage 1', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP005', 'PT005', 'DT005', 'AP005', 2, 'HIV stage 2', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP006', 'PT006', 'DT006', 'AP006', 1, 'HIV stage 1', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP007', 'PT007', 'DT007', 'AP007', 3, 'HIV stage 3', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP008', 'PT008', 'DT008', 'AP008', 2, 'HIV stage 2', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP009', 'PT009', 'DT009', 'AP009', 1, 'HIV stage 1', N'Đang điều trị', CURRENT_TIMESTAMP);
INSERT INTO TreatmentPlan VALUES ('TP010', 'PT010', 'DT010', 'AP010', 2, 'HIV stage 2', N'Đang điều trị', CURRENT_TIMESTAMP);


-- 9. Bảng Medication

INSERT INTO Medication VALUES ('MD001', 'Tenofovir', 'Tablet', '300mg', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD002', 'Lamivudine', 'Tablet', '150mg', 'Adults & Children', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD003', 'Dolutegravir', 'Tablet', '50mg', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD004', 'Efavirenz', 'Tablet', '600mg', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD005', 'Zidovudine', 'Capsule', '100mg', 'Adults & Children', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD006', 'Nevirapine', 'Tablet', '200mg', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD007', 'Abacavir', 'Tablet', '300mg', 'Adults & Children', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD008', 'Raltegravir', 'Tablet', '400mg', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD009', 'Darunavir', 'Tablet', '600mg', 'Adults', CURRENT_TIMESTAMP);
INSERT INTO Medication VALUES ('MD010', 'Ritonavir', 'Tablet', '100mg', 'Adults', CURRENT_TIMESTAMP);

-- 10. Bảng Prescription

INSERT INTO Prescription VALUES ('PR001', 'TP001', 'MD001', 'DT001', '2023-01-10', '2023-07-10', '1 viên mỗi ngày', 'First-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR002', 'TP002', 'MD002', 'DT002', '2023-02-15', '2023-08-15', '1 viên 2 lần mỗi ngày', 'First-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR003', 'TP003', 'MD003', 'DT003', '2023-03-20', '2023-09-20', '1 viên mỗi ngày', 'Second-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR004', 'TP004', 'MD004', 'DT004', '2023-04-25', '2023-10-25', '1 viên mỗi tối', 'First-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR005', 'TP005', 'MD005', 'DT005', '2023-05-05', '2023-11-05', '1 viên 2 lần mỗi ngày', 'Second-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR006', 'TP006', 'MD006', 'DT006', '2023-06-10', '2023-12-10', '1 viên 2 lần mỗi ngày', 'First-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR007', 'TP007', 'MD007', 'DT007', '2023-07-15', '2024-01-15', '1 viên 2 lần mỗi ngày', 'Third-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR008', 'TP008', 'MD008', 'DT008', '2023-08-20', '2024-02-20', '1 viên 2 lần mỗi ngày', 'Second-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR009', 'TP009', 'MD009', 'DT009', '2023-09-25', '2024-03-25', '1 viên 2 lần mỗi ngày', 'First-line', CURRENT_TIMESTAMP);
INSERT INTO Prescription VALUES ('PR010', 'TP010', 'MD010', 'DT010', '2023-10-30', '2024-04-30', '1 viên mỗi ngày với thức ăn', 'Second-line', CURRENT_TIMESTAMP);

-- 11. Bảng Booking
INSERT INTO Booking VALUES ('BK001', 'PT001', 'DT001', N'Tư vấn', 'CD4, Viral Load', '2023-06-15 09:00:00', N'Xác nhận', 'https://meet.google.com/abc-defg-hij', N'Tái khám định kỳ');
INSERT INTO Booking VALUES ('BK002', 'PT002', 'DT002', N'Khám mới', 'None', '2023-06-16 10:00:00', N'Đang chờ', NULL, N'Khám lần đầu');
INSERT INTO Booking VALUES ('BK003', 'PT003', 'DT003', N'Tái khám', 'CD4', '2023-06-17 11:00:00', N'Xác nhận', 'https://meet.google.com/klm-nopq-rst', N'Đánh giá sau 3 tháng');
INSERT INTO Booking VALUES ('BK004', 'PT004', 'DT004', N'Cấp cứu', 'Full Panel', '2023-06-18 08:00:00', N'Xác nhận', NULL, N'Sốt cao, khó thở');
INSERT INTO Booking VALUES ('BK005', 'PT005', 'DT005', N'Tư vấn', 'None', '2023-06-19 14:00:00', N'Từ chối', NULL, N'Tư vấn dinh dưỡng');
INSERT INTO Booking VALUES ('BK006', 'PT006', 'DT006', N'Tái khám', 'Viral Load', '2023-06-20 15:00:00', N'Đang chờ', NULL, N'Đánh giá sau 6 tháng');
INSERT INTO Booking VALUES ('BK007', 'PT007', 'DT007', N'Khám mới', 'CD4, Viral Load', '2023-06-21 16:00:00', N'Xác nhận', 'https://meet.google.com/uvw-xyza-bcd', N'Chuyển từ cơ sở khác');
INSERT INTO Booking VALUES ('BK008', 'PT008', 'DT008', N'Theo dõi', 'CD4', '2023-06-22 11:00:00', N'Đang chờ', NULL, N'Theo dõi tác dụng phụ');
INSERT INTO Booking VALUES ('BK009', 'PT009', 'DT009', N'Tư vấn', 'None', '2023-06-23 09:00:00', N'Xác nhận', 'https://meet.google.com/efg-hijk-lmn', N'Tư vấn tâm lý');
INSERT INTO Booking VALUES ('BK010', 'PT010', 'DT010', N'Tái khám', 'Full Panel', '2023-06-24 10:00:00', N'Đang chờ', NULL, N'Đánh giá toàn diện sau 1 năm');
--12. Bảng LabTests

INSERT INTO LabTests VALUES ('LT001', 'BK001', 'TP001', 'CD4 Count', 'CD4-001', 'Immunology', '450 cells/mm³', 350, NULL, N'Xác nhận', N'Cần theo dõi');
INSERT INTO LabTests VALUES ('LT002', 'BK003', 'TP003', 'Viral Load', 'VL-002', 'Virology', 'Undetectable <20 copies/ml', NULL, 100000, N'Xác nhận', N'Kết quả tốt');
INSERT INTO LabTests VALUES ('LT003', 'BK004', 'TP004', 'Complete Blood Count', 'CBC-003', 'Hematology', 'Normal', NULL, NULL, N'Đang chờ', N'Đang chờ kết quả');
INSERT INTO LabTests VALUES ('LT004', 'BK007', 'TP007', 'Liver Function Test', 'LFT-004', 'Biochemistry', 'ALT: 45, AST: 42', NULL, NULL, N'Xác nhận', N'Chức năng gan bình thường');
INSERT INTO LabTests VALUES ('LT005', 'BK008', 'TP008', 'Kidney Function Test', 'KFT-005', 'Biochemistry', 'Creatinine: 0.9', NULL, NULL, N'Đang chờ', N'Đang chờ kết quả');
INSERT INTO LabTests VALUES ('LT006', 'BK010', 'TP010', 'CD4 Count', 'CD4-006', 'Immunology', '650 cells/mm³', 600, NULL, N'Xác nhận', N'Kết quả tốt');
INSERT INTO LabTests VALUES ('LT007', 'BK001', 'TP001', 'HIV Resistance Test', 'RES-007', 'Molecular', 'No resistance detected', NULL, NULL, N'Xác nhận', N'Không phát hiện kháng thuốc');
INSERT INTO LabTests VALUES ('LT008', 'BK004', 'TP004', 'Tuberculosis Test', 'TB-008', 'Microbiology', 'Negative', NULL, NULL, N'Từ chối', N'Cần thực hiện lại');
INSERT INTO LabTests VALUES ('LT009', 'BK010', 'TP010', 'Viral Load', 'VL-009', 'Virology', '<50 copies/ml', NULL, 5000, N'Xác nhận', N'Kiểm soát virus tốt');
INSERT INTO LabTests VALUES ('LT010', 'BK003', 'TP003', 'Hepatitis Co-infection', 'HEP-010', 'Serology', 'Negative for Hep B and C', NULL, NULL, N'Đang chờ', N'Đang chờ kết quả');

--13. Bảng Reminder

INSERT INTO Reminder VALUES ('RM001', 'PT001', 'TP001', 'PR001', 'Medication', '2023-06-25 08:00:00', N'Nhắc uống thuốc Tenofovir');
INSERT INTO Reminder VALUES ('RM002', 'PT002', 'TP002', 'PR002', 'Appointment', '2023-06-15 20:00:00', N'Nhắc lịch hẹn ngày mai lúc 10:00');
INSERT INTO Reminder VALUES ('RM003', 'PT003', 'TP003', 'PR003', 'Medication', '2023-06-25 08:00:00', N'Nhắc uống thuốc Dolutegravir');
INSERT INTO Reminder VALUES ('RM004', 'PT004', 'TP004', 'PR004', 'Lab Test', '2023-06-16 18:00:00', N'Nhắc xét nghiệm CD4 vào ngày 18/06');
INSERT INTO Reminder VALUES ('RM005', 'PT005', 'TP005', 'PR005', 'Medication', '2023-06-25 08:00:00', N'Nhắc uống thuốc Zidovudine');
INSERT INTO Reminder VALUES ('RM006', 'PT006', 'TP006', 'PR006', 'Appointment', '2023-06-19 20:00:00', N'Nhắc lịch hẹn ngày mai lúc 15:00');
INSERT INTO Reminder VALUES ('RM007', 'PT007', 'TP007', 'PR007', 'Medication', '2023-06-25 08:00:00', N'Nhắc uống thuốc Abacavir');
INSERT INTO Reminder VALUES ('RM008', 'PT008', 'TP008', 'PR008', 'Lab Test', '2023-06-20 18:00:00', N'Nhắc xét nghiệm CD4 vào ngày 22/06');
INSERT INTO Reminder VALUES ('RM009', 'PT009', 'TP009', 'PR009', 'Medication', '2023-06-25 08:00:00', N'Nhắc uống thuốc Darunavir');
INSERT INTO Reminder VALUES ('RM010', 'PT010', 'TP010', 'PR010', 'Appointment', '2023-06-23 20:00:00', N'Nhắc lịch hẹn ngày mai lúc 10:00');

--14. Bảng Payment

INSERT INTO Payment VALUES ('PM001', 'BK001', 500000, '2023-06-15 10:00:00', N'Tiền mặt', N'Xác nhận', N'Thanh toán đầy đủ', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM002', 'BK002', 750000, '2023-06-16 11:30:00', N'Chuyển khoản', N'Đang chờ', N'Chờ xác nhận từ ngân hàng', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM003', 'BK003', 450000, '2023-06-17 12:00:00', N'Thẻ tín dụng', N'Xác nhận', N'Thanh toán đầy đủ', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM004', 'BK004', 1200000, '2023-06-18 09:30:00', N'Bảo hiểm', N'Xác nhận', N'Bảo hiểm chi trả 80%', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM005', 'BK005', 300000, '2023-06-19 15:30:00', N'Tiền mặt', N'Từ chối', N'Hủy lịch hẹn', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM006', 'BK006', 600000, '2023-06-20 16:30:00', N'Chuyển khoản', N'Đang chờ', N'Chờ xác nhận từ ngân hàng', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM007', 'BK007', 800000, '2023-06-21 17:30:00', N'Thẻ tín dụng', N'Xác nhận', N'Thanh toán đầy đủ', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM008', 'BK008', 550000, '2023-06-22 12:30:00', N'Tiền mặt', N'Đang chờ', N'Thanh toán một phần', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM009', 'BK009', 250000, '2023-06-23 10:30:00', N'Chuyển khoản', N'Xác nhận', N'Thanh toán đầy đủ', CURRENT_TIMESTAMP);
INSERT INTO Payment VALUES ('PM010', 'BK010', 900000, '2023-06-24 11:30:00', N'Bảo hiểm', N'Đang chờ', N'Chờ xác nhận từ bảo hiểm', CURRENT_TIMESTAMP);