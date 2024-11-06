-- Create the database
CREATE DATABASE StoreManager;
GO

-- Use the newly created database
USE StoreManager;
GO

-- Create UserRoles table
CREATE TABLE UserRoles (
    id INT NOT NULL IDENTITY(0,1) PRIMARY KEY,
    name NVARCHAR(255) NOT NULL
);
GO

-- Insert predefined roles into UserRoles
INSERT INTO dbo.UserRoles (name)
VALUES
('Admin'), ('Seller'), ('Customer');
GO

-- Create Users table (General user information)
CREATE TABLE Users (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(255) NOT NULL,
    email NVARCHAR(255) NOT NULL UNIQUE,
    password NVARCHAR(255) NOT NULL,
    role INT,
    status BIT NOT NULL,
    remember_token NVARCHAR(100) DEFAULT NULL,
    reset_password_token NVARCHAR(100) DEFAULT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
    FOREIGN KEY (role) REFERENCES UserRoles(id)
);
GO

-- Create UserDetails table (Detailed information separated from Users table)
CREATE TABLE UserDetails (
    user_id INT NOT NULL PRIMARY KEY,
    fullname NVARCHAR(255) NOT NULL,
    address NVARCHAR(255) NOT NULL,
    phone NVARCHAR(255) NOT NULL,
    dob DATE NOT NULL,
    gender BIT NOT NULL,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);
GO
-- Create Categories table
CREATE TABLE Categories (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(255) NOT NULL,
    Description NVARCHAR(255) NULL,
	status BIT NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
	seller_id INT,
    FOREIGN KEY (seller_id) REFERENCES Users(id),
);
GO
-- Create Products table (General product information)
CREATE TABLE Products (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	category_id INT,
    ProductName NVARCHAR(255) NOT NULL,
    MeasureUnit NVARCHAR(255) NOT NULL,
    cost MONEY NOT NULL,
    status BIT NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
    seller_id INT,
    FOREIGN KEY (seller_id) REFERENCES Users(id),
	FOREIGN KEY (category_id) REFERENCES Categories(id)
);
GO
-- Create ProductDetails table (Additional product information)
CREATE TABLE ProductDetails (
    product_id INT NOT NULL PRIMARY KEY,
    description NVARCHAR(MAX) NULL,
    additional_info NVARCHAR(MAX) NULL,
    FOREIGN KEY (product_id) REFERENCES Products(id)
);
GO

-- Create Customers table (General customer information)
CREATE TABLE Customers (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(MAX) NOT NULL,
    address NVARCHAR(MAX) NOT NULL,
    phone NVARCHAR(MAX) NOT NULL,
    note NVARCHAR(MAX) NULL,
    status BIT NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
    seller_id INT,
    FOREIGN KEY (seller_id) REFERENCES Users(id)
);
GO

-- Create Orders table (General order information)
CREATE TABLE ExportOrders (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    customer_id INT,
    orderDate DATE NOT NULL,
    complete BIT NOT NULL,
    orderTotal MONEY NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
    seller_id INT,
    FOREIGN KEY (seller_id) REFERENCES Users(id),
    FOREIGN KEY (customer_id) REFERENCES Customers(id)
);
GO

-- Create OrderItems table (Details separated from Orders table)
CREATE TABLE ExportOrderItems (
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    volume INT NOT NULL,
	real_volume INT NOT NULL DEFAULT 0,
    product_price MONEY NOT NULL,
    total MONEY NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
    seller_id INT,
	status BIT NOT NULL DEFAULT 0,
	PRIMARY KEY(order_id,product_id),
    FOREIGN KEY (order_id) REFERENCES ExportOrders(id),
    FOREIGN KEY (product_id) REFERENCES Products(id),
    FOREIGN KEY (seller_id) REFERENCES Users(id)
);
GO
-- Create Reports table (General report information)
CREATE TABLE Reports (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    customer_id INT,
    TotalPrice MONEY NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
    seller_id INT,
    FOREIGN KEY (seller_id) REFERENCES Users(id),
    FOREIGN KEY (customer_id) REFERENCES Customers(id)
);
GO

-- Create ReportDetails table (Details separated from Reports table)
CREATE TABLE ReportDetails (
    report_id INT NOT NULL,
    product_id INT NOT NULL,
	Volume INT NOT NULL,
	TotalPrice MONEY NOT NULL,
	orderDate DATE,
	PRIMARY KEY(report_id,product_id),
    FOREIGN KEY (report_id) REFERENCES Reports(id),
	FOREIGN KEY (product_id) REFERENCES Products(id)
);
GO

-- Create SellerDetails table (Separated from Users table specifically for seller details)
CREATE TABLE SellerDetails (
    user_id INT NOT NULL PRIMARY KEY,
    business_name NVARCHAR(255) NULL,
    business_address NVARCHAR(255) NULL,
	ImageData VARBINARY(MAX) NULL,
    FOREIGN KEY (user_id) REFERENCES Users(id)
);
GO
-- Create Customers table (General customer information)
CREATE TABLE Suppliers (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(MAX) NOT NULL,
    address NVARCHAR(MAX) NOT NULL,
    phone NVARCHAR(MAX) NOT NULL,
    note NVARCHAR(MAX) NULL,
    status BIT NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
    seller_id INT,
    FOREIGN KEY (seller_id) REFERENCES Users(id)
);
GO
-- Create Orders table (General order information)
CREATE TABLE ImportOrders (
    id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    supplier_id INT,
    orderDate DATE NOT NULL,
    complete BIT NOT NULL,
    orderTotal MONEY NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
    seller_id INT,
    FOREIGN KEY (seller_id) REFERENCES Users(id),
    FOREIGN KEY (supplier_id) REFERENCES Suppliers(id)
);
GO

-- Create OrderItems table (Details separated from Orders table)
CREATE TABLE ImportOrderItems (
    order_id INT NOT NULL,
    product_id INT NOT NULL,
    volume INT NOT NULL,
	real_volume INT NOT NULL DEFAULT 0,
    product_cost MONEY NOT NULL,
    created_at DATETIME NULL DEFAULT NULL,
    updated_at DATETIME NULL DEFAULT NULL,
	status BIT NOT NULL DEFAULT 0,
	PRIMARY KEY(order_id,product_id),
    FOREIGN KEY (order_id) REFERENCES ImportOrders(id),
    FOREIGN KEY (product_id) REFERENCES Products(id),
);
GO
CREATE TABLE InventoryManagement(
	product_id INT NOT NULL PRIMARY KEY,
	remaining_volume INT NOT NULL DEFAULT 0,
	allocated_volume INT NOT NULL DEFAULT 0,
	shipped_volume INT NOT NULL DEFAULT 0,
	FOREIGN KEY (product_id) REFERENCES Products(id),
);