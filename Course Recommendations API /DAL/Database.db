CREATE TABLE Courses (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    CourseName NVARCHAR(200) NOT NULL,
    Category NVARCHAR(100) NOT NULL,
    ImageUrl NVARCHAR(500) NULL,
    Description NVARCHAR(1000) NULL,
    Instructor NVARCHAR(100) NULL,
    Duration NVARCHAR(50) NULL,
    Rating DECIMAL(3,2) NULL,
    Price DECIMAL(10,2) NULL,
    IsRecommended BIT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Insert sample course data
INSERT INTO Courses (CourseName, Category, ImageUrl, Description, Instructor, Duration, Rating, Price, IsRecommended) VALUES
('Complete Python Programming', 'Software Development', 'https://images.unsplash.com/photo-1526379879527-8559ecfcaec0?w=400', 'Learn Python from scratch to advanced level', 'Dr. Ahmed Hassan', '40 hours', 4.8, 99.99, 1),
('Data Science with R', 'Data Science', 'https://images.unsplash.com/photo-1551288049-bebda4e38f71?w=400', 'Master data analysis and visualization with R', 'Sarah Johnson', '35 hours', 4.6, 89.99, 1),
('Calculus I - Fundamentals', 'Mathematics', 'https://images.unsplash.com/photo-1635070041078-e363dbe005cb?w=400', 'Complete introduction to differential calculus', 'Prof. Mohamed Ali', '30 hours', 4.7, 79.99, 1),
('Machine Learning Basics', 'Data Science', 'https://images.unsplash.com/photo-1555949963-aa79dcee981c?w=400', 'Introduction to ML algorithms and applications', 'Dr. Lisa Chen', '45 hours', 4.9, 129.99, 1),
('Web Development with React', 'Software Development', 'https://images.unsplash.com/photo-1633356122544-f134324a6cee?w=400', 'Build modern web applications with React', 'John Smith', '50 hours', 4.5, 109.99, 1),
('Statistics for Beginners', 'Mathematics', 'https://images.unsplash.com/photo-1543286386-713bdd548da4?w=400', 'Essential statistical concepts and methods', 'Dr. Emma Wilson', '25 hours', 4.4, 69.99, 0),
('Advanced JavaScript', 'Software Development', 'https://images.unsplash.com/photo-1579468118864-1b9ea3c0db4a?w=400', 'Master advanced JavaScript concepts', 'Mike Johnson', '38 hours', 4.7, 94.99, 1),
('Linear Algebra', 'Mathematics', 'https://images.unsplash.com/photo-1596495578065-6e0763fa1178?w=400', 'Comprehensive linear algebra course', 'Prof. David Lee', '42 hours', 4.8, 84.99, 0),
('Deep Learning with TensorFlow', 'Data Science', 'https://images.unsplash.com/photo-1555949963-f7c13c6b9eba?w=400', 'Build neural networks with TensorFlow', 'Dr. Anna Rodriguez', '55 hours', 4.9, 149.99, 1),
('Mobile App Development', 'Software Development', 'https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c?w=400', 'Create mobile apps for iOS and Android', 'Robert Brown', '60 hours', 4.6, 119.99, 0);

-- Create user course recommendations junction table (optional for personalized recommendations)
CREATE TABLE UserCourseRecommendations (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    CourseID INT NOT NULL,
    RecommendationScore DECIMAL(5,2) DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserID) REFERENCES Users(ID),
    FOREIGN KEY (CourseID) REFERENCES Courses(ID),
    UNIQUE(UserID, CourseID)
);
