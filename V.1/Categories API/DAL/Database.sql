CREATE TABLE Categories (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NULL,
    IconUrl NVARCHAR(300) NULL,
    Color NVARCHAR(20) NULL,
    CourseCount INT DEFAULT 0,
    IsActive BIT DEFAULT 1,
    DisplayOrder INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- Insert sample categories
INSERT INTO Categories (Name, Description, IconUrl, Color, CourseCount, DisplayOrder) VALUES
('Software Development', 'Learn programming languages, frameworks, and development tools', 'https://cdn-icons-png.flaticon.com/512/1005/1005141.png', '#007bff', 15, 1),
('Data Science', 'Master data analysis, machine learning, and statistics', 'https://cdn-icons-png.flaticon.com/512/2103/2103665.png', '#28a745', 12, 2),
('Mathematics', 'Explore mathematical concepts from basics to advanced topics', 'https://cdn-icons-png.flaticon.com/512/3771/3771275.png', '#dc3545', 8, 3),
('Business', 'Develop business skills, management, and entrepreneurship', 'https://cdn-icons-png.flaticon.com/512/1041/1041883.png', '#ffc107', 10, 4),
('Design', 'Creative design, UI/UX, graphic design, and digital arts', 'https://cdn-icons-png.flaticon.com/512/2620/2620419.png', '#e83e8c', 7, 5),
('Marketing', 'Digital marketing, social media, and advertising strategies', 'https://cdn-icons-png.flaticon.com/512/2966/2966327.png', '#fd7e14', 6, 6),
('Personal Development', 'Self-improvement, productivity, and life skills', 'https://cdn-icons-png.flaticon.com/512/3135/3135715.png', '#6f42c1', 5, 7),
('Language Learning', 'Master new languages and communication skills', 'https://cdn-icons-png.flaticon.com/512/3002/3002543.png', '#20c997', 9, 8),
('Health & Fitness', 'Physical fitness, nutrition, and wellness courses', 'https://cdn-icons-png.flaticon.com/512/2382/2382533.png', '#17a2b8', 4, 9),
('Photography', 'Photography techniques, editing, and visual storytelling', 'https://cdn-icons-png.flaticon.com/512/685/685655.png', '#6c757d', 3, 10);

-- Create trigger to update course count automatically
CREATE TRIGGER UpdateCategoryCount
ON Courses
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Update counts for affected categories
    UPDATE Categories 
    SET CourseCount = (
        SELECT COUNT(*) 
        FROM Courses 
        WHERE Category = Categories.Name
    )
    WHERE Name IN (
        SELECT DISTINCT Category FROM inserted
        UNION
        SELECT DISTINCT Category FROM deleted
    );
END;
