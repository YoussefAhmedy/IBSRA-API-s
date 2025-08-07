ALTER TABLE Users ADD 
    Email NVARCHAR(255) UNIQUE NOT NULL,
    Username NVARCHAR(100) UNIQUE NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    ResetToken NVARCHAR(255) NULL,
    ResetTokenExpiry DATETIME NULL;

-- Sample data
INSERT INTO Users (Name, Age, PhoneNumber, Email, Username, Password) VALUES 
('Mohamed Ali', 25, '01234567890', 'mohamed@example.com', 'mohamed_ali', '$2a$11$hashedpassword1'),
('Sarah Ahmed', 30, '01098765432', 'sarah@example.com', 'sarah_ahmed', '$2a$11$hashedpassword2'),
('John Smith', 28, '01555123456', 'john@example.com', 'john_smith', '$2a$11$hashedpassword3');
