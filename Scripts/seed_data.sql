USE CommerceDB;
GO

-- 1. Insert Users if not exists
-- Hash for 'password123'
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, Role, Email, PhoneNumber, Address)
    VALUES ('admin', '$2a$11$eNfFih.f3OofRk1Q4z0u4uOhIf1l7G5.hXm34wON02f23bOTlStZ.', 'Admin', 'admin@claysys.com', '000-000-0000', 'Office 1, Tech Park');
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'john_doe')
BEGIN
    INSERT INTO Users (Username, PasswordHash, Role, Email, PhoneNumber, Address)
    VALUES ('john_doe', '$2a$11$eNfFih.f3OofRk1Q4z0u4uOhIf1l7G5.hXm34wON02f23bOTlStZ.', 'Customer', 'john@example.com', '123-456-7890', '456 Retail Street');
END

-- 2. Insert Categories if not exists
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Electronics')
BEGIN
    INSERT INTO Categories (Name, Description) VALUES ('Electronics', 'Electronic devices and gadgets');
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'Accessories')
BEGIN
    INSERT INTO Categories (Name, Description) VALUES ('Accessories', 'Device accessories');
END

-- 3. Insert Products if not exists
DECLARE @ElecId INT = (SELECT TOP 1 Id FROM Categories WHERE Name = 'Electronics');
DECLARE @AccId INT = (SELECT TOP 1 Id FROM Categories WHERE Name = 'Accessories');

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Smartphone X')
BEGIN
    INSERT INTO Products (Name, Description, Price, Stock, CategoryId, ImageUrl)
    VALUES ('Smartphone X', 'Latest generation smartphone', 799.99, 50, @ElecId, 'images/phone.jpg');
END

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = 'Bluetooth Headphones')
BEGIN
    INSERT INTO Products (Name, Description, Price, Stock, CategoryId, ImageUrl)
    VALUES ('Bluetooth Headphones', 'Noise-cancelling wireless headphones', 199.50, 100, @AccId, 'images/headphones.jpg');
END

IF NOT EXISTS (SELECT 1 FROM Products WHERE Name = '4K Monitor')
BEGIN
    INSERT INTO Products (Name, Description, Price, Stock, CategoryId, ImageUrl)
    VALUES ('4K Monitor', '27-inch 4K UHD Ultra-Thin Monitor', 349.00, 30, @ElecId, 'images/monitor.jpg');
END

-- 4. Create an Order (if user doesn't have one)
DECLARE @UserId INT = (SELECT TOP 1 Id FROM Users WHERE Username = 'john_doe');

IF NOT EXISTS (SELECT 1 FROM Orders WHERE UserId = @UserId)
BEGIN
    INSERT INTO Orders (UserId, OrderDate, TotalAmount)
    VALUES (@UserId, GETDATE(), 999.49);
    
    DECLARE @OrderId INT = SCOPE_IDENTITY();
    
    -- Pick some products
    DECLARE @P1 INT = (SELECT TOP 1 Id FROM Products WHERE Name = 'Smartphone X');
    DECLARE @P2 INT = (SELECT TOP 1 Id FROM Products WHERE Name = 'Bluetooth Headphones');
    
    -- Insert OrderItems
    IF @P1 IS NOT NULL
    BEGIN
        INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
        VALUES (@OrderId, @P1, 1, 799.99);
    END
    
    IF @P2 IS NOT NULL
    BEGIN
        INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
        VALUES (@OrderId, @P2, 1, 199.50);
    END

    -- Insert Payment
    INSERT INTO Payments (OrderId, Amount, Method, Status, PaidAt)
    VALUES (@OrderId, 999.49, 'Credit Card', 'Completed', GETDATE());
END
GO
