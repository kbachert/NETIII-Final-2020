/*
	  AUTHOR: Kaleb Bachert
    FILENAME: dq_inventory.sql
		DATE: 4/26/2019
 DESCRIPTION: 
		This database stores inventory, sales, and order data for a Dairy Queen store's Inventory
*/

/* Check whether the database exists and delete if so */
IF EXISTS(SELECT 1 FROM master.dbo.sysdatabases
		  WHERE name = 'dq_inventory')
BEGIN
	DROP DATABASE [dq_inventory]
	print '' print '*** Dropping dq_inventory'
END
GO

print '' print '*** Creating dq_inventory'
GO
CREATE DATABASE [dq_inventory]
GO

print '' print '*** Using dq_inventory'
GO
USE [dq_inventory]
GO

-- Table for holding vendor information
print '' print '*** Creating vendor Table'
GO

CREATE TABLE [dbo].[vendor] (
	[VendorID]		[nvarchar](50)				NOT NULL,
	[VendorPhone]	[nvarchar](12)				NOT NULL,
	[Active]		[bit]			  NOT NULL DEFAULT 1,
	CONSTRAINT [pk_VendorID] PRIMARY KEY([VendorID] ASC)
)
GO

-- Table for holding informataion about each inventory item
print '' print '*** Creating inventoryItem Table'
GO

CREATE TABLE [dbo].[inventoryItem] (
	[InventoryID]				[nvarchar](50) 				NOT NULL,
	[PurchaseUnit]				[nvarchar](50) 				NOT NULL,
	[SaleUnit]					[nvarchar](50) 				NOT NULL,
	[SaleUnitsPerPurchaseUnit]	[decimal](8,2)		 		NOT NULL,
	[QuantityOnHand]			[decimal](8,2)				NOT NULL,
	[ReorderLevel]				[decimal](8,2)			  		NULL,
	[Active]					[bit]			  NOT NULL DEFAULT 1,
	CONSTRAINT [pk_InventoryID] PRIMARY KEY([InventoryID] ASC),
)
GO

-- Table for holding each vendor that sells an inventoryItem
print '' print '*** Creating vendorItem Table'
GO

CREATE TABLE [dbo].[vendorItem] (
	[InventoryID]		[nvarchar](50)		NOT NULL,
	[VendorID]			[nvarchar](50)		NOT NULL,
	[PreferredVendor]	[bit]	  NOT NULL DEFAULT 0,
	CONSTRAINT [pk_InventoryID_VendorID] 
		PRIMARY KEY ([InventoryID] ASC, [VendorID] ASC),
	CONSTRAINT [fk_vendorItem_inventoryID] FOREIGN KEY([InventoryID])
		REFERENCES [inventoryItem]([InventoryID]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [fk_vendorItem_vendorID] FOREIGN KEY ([VendorID])
		REFERENCES [vendor]([VendorID]) ON UPDATE CASCADE ON DELETE CASCADE
)
GO
print '' print '*** Adding Index for PreferredVendor on VendorItem Table'
GO
CREATE NONCLUSTERED INDEX [ix_vendorItem_preferredVendor]
	ON [vendorItem]([PreferredVendor] ASC)
GO

-- Table for holding roles and a basic description
print '' print '*** Creating role Table'
GO

CREATE TABLE [dbo].[role](
	[RoleID]		nvarchar(50)	NOT NULL,
	[Description]	nvarchar(250)		NULL,
	CONSTRAINT [pk_RoleID] PRIMARY KEY([RoleID] ASC)
)
GO

-- Table for holding employees and basic information about them
print '' print '*** Creating employee Table'
GO

CREATE TABLE [dbo].[employee](
	[EmployeeID] 	[int] IDENTITY(1000000,1) 	NOT NULL,
    [LastName] 		[nvarchar](50)				NOT NULL, 
    [FirstName] 	[nvarchar](50)				NOT NULL,
    [PhoneNumber] 	[nvarchar](12)				NOT NULL,
	[Email]			[nvarchar](250)				NOT NULL,
	[PasswordHash]	[nvarchar](100)		NOT NULL DEFAULT 
	'9C9064C59F1FFA2E174EE754D2979BE80DD30DB552EC03E7E327E9B1A4BD594E',
	[Active]		[bit]			  NOT NULL DEFAULT 1,
	CONSTRAINT [pk_EmployeeID] PRIMARY KEY([EmployeeID] ASC),
	CONSTRAINT [ak_Email] UNIQUE(Email)
)
GO
print '' print '*** Adding Index for LastName on Employee Table'
GO
CREATE NONCLUSTERED INDEX [ix_employee_lastName]
	ON [employee]([LastName] ASC)
GO
print '' print '*** Adding Index for Email on Employee Table'
GO
CREATE NONCLUSTERED INDEX [ix_employee_email]
	ON [employee]([Email] ASC)
GO

-- Table for holding each employees' roles
print '' print '*** Creating employeeRole Table'
GO

CREATE TABLE [dbo].[employeeRole](
	[EmployeeID]	[int]			NOT NULL,
	[RoleId]		[nvarchar](50)	NOT NULL,
	CONSTRAINT [pk_EmployeeID_RoleID] 
		PRIMARY KEY ([EmployeeID] ASC, [RoleID] ASC),
	CONSTRAINT [fk_employeeRole_employeeID] FOREIGN KEY([EmployeeID])
		REFERENCES [employee]([EmployeeID]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [fk_employeeRole_roleID] FOREIGN KEY ([RoleID])
		REFERENCES [role]([RoleID]) ON UPDATE CASCADE ON DELETE CASCADE
)
GO

-- Table for holding orders we've placed and information about them
print '' print '*** Creating order Table'
GO

CREATE TABLE [dbo].[order](
	[PurchaseOrderNumber]	[int] IDENTITY(1000000,1) 	NOT NULL,
    [VendorID] 				[nvarchar](50)					NULL,
	[EmployeeID] 			[int]						NOT NULL,
    [OrderDate] 			[datetime] 					NOT NULL,
	[OrderTotal]			[money]						NOT NULL,
	CONSTRAINT [pk_PurchaseOrderNumber] PRIMARY KEY([PurchaseOrderNumber] ASC),
	CONSTRAINT [fk_order_vendorID] FOREIGN KEY([VendorID])
		REFERENCES [vendor]([VendorID]),
	CONSTRAINT [fk_order_employeeID] FOREIGN KEY([EmployeeID])
		REFERENCES [employee]([EmployeeID]) ON UPDATE CASCADE
)
GO
print '' print '*** Adding Index for VendorID on Order Table'
GO
CREATE NONCLUSTERED INDEX [ix_order_vendorID]
	ON [order]([VendorID] ASC)
GO
print '' print '*** Adding Index for EmployeeID on Order Table'
GO
CREATE NONCLUSTERED INDEX [ix_order_employeeID]
	ON [order]([EmployeeID] ASC)
GO

-- Table for keeping track of the amount of each inventory item we order
print '' print '*** Creating orderLine Table'
GO

CREATE TABLE [dbo].[orderLine](
	[PurchaseOrderNumber] 	[int]			NOT NULL,
    [InventoryID] 			[nvarchar](50) 	NOT NULL,
    [QuantityOrdered] 		[int] 			NOT NULL,
    [PricePerUnit] 			[money] 		NOT NULL, -- Per Purchase Unit
	[LineTotal] 			[money]			NOT NULL, 
	CONSTRAINT [pk_PurchaseOrderNumber_InventoryID] 
		PRIMARY KEY ([PurchaseOrderNumber] ASC, [InventoryID] ASC),
	CONSTRAINT [fk_orderDetails_purchaseOrderNumber] FOREIGN KEY([PurchaseOrderNumber])
		REFERENCES [order]([PurchaseOrderNumber]) ON UPDATE CASCADE,
	CONSTRAINT [fk_orderDetails_inventoryID] FOREIGN KEY ([InventoryID])
		REFERENCES [inventoryItem]([InventoryID]) ON UPDATE CASCADE
)
GO

-- Table for holding items we sell and their price
print '' print '*** Creating saleItem Table'
GO

CREATE TABLE [dbo].[saleItem](
	[SaleItemID] 		[int] IDENTITY(1000000,1) 	NOT NULL,
    [ItemName] 			[nvarchar](50)				NOT NULL, -- Blizzard, Cone etc.
    [ItemSize] 			[nvarchar](50)					NULL, -- Item size, not all items have sizes
	[Flavor]			[nvarchar](50)					NULL, -- Not all items have flavors
    [Price]				[money] 					NOT NULL,
	[Active]			[bit]			  NOT NULL DEFAULT 1,
	CONSTRAINT [pk_SaleItemID] PRIMARY KEY ([SaleItemID] ASC)
)
GO
print '' print '*** Adding Index for ItemName on SaleItem Table'
GO
CREATE NONCLUSTERED INDEX [ix_saleItem_itemName]
	ON [saleItem]([ItemName] ASC)
GO

-- Table for holding how much of which inventory items go into a sale item
print '' print '*** Creating saleItemInventory Table'
GO

CREATE TABLE [dbo].[saleItemInventory](
	[SaleItemID]	[int]			NOT NULL,
	[InventoryID]	[nvarchar](50)	NOT NULL,
	[Quantity]		[decimal](8,2)	NOT NULL,
	CONSTRAINT [pk_SaleItemID_InventoryID]
		PRIMARY KEY ([SaleItemID] ASC, [InventoryID] ASC),
	CONSTRAINT [fk_saleItemInventory_saleItemID] FOREIGN KEY([SaleItemID])
		REFERENCES [saleItem]([SaleItemID])  ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [fk_saleItemInventory_inventoryID] FOREIGN KEY ([InventoryID])
		REFERENCES [inventoryItem]([InventoryID])  ON UPDATE CASCADE ON DELETE CASCADE
)
GO

-- Table for holding every sale we make, how much, who took the order and when
print '' print '*** Creating sale Table'
GO

CREATE TABLE [dbo].[sale](
	[TransactionID]		[int] IDENTITY(1000000,1)	NOT NULL,
	[EmployeeID]		[int]						NOT NULL,
	[TransactionDate]	[datetime]					NOT NULL,
	[SaleTotal]			[money]						NOT NULL,
	CONSTRAINT [pk_TransactionID] PRIMARY KEY ([TransactionID] ASC),
	CONSTRAINT [fk_sale_employeeID] FOREIGN KEY([EmployeeID])
		REFERENCES [employee]([EmployeeID]) ON UPDATE CASCADE
)
GO
print '' print '*** Adding Index for EmployeeID on Sale Table'
GO
CREATE NONCLUSTERED INDEX [ix_sale_employeeID]
	ON [sale]([EmployeeID] ASC)
GO

-- Table for holding multiple items for each sale
print '' print '*** Creating saleLine Table'
GO

CREATE TABLE [dbo].[saleLine](
	[TransactionID]		[int]		NOT NULL,
	[SaleItemID]		[int]		NOT NULL,
	[LineTotal]			[money]		NOT NULL,
	CONSTRAINT [pk_TransactionID_SaleItemID]
		PRIMARY KEY ([TransactionID] ASC, [SaleItemID] ASC),
	CONSTRAINT [fk_saleLine_transactionID] FOREIGN KEY ([TransactionID])
		REFERENCES [sale]([TransactionID]) ON UPDATE CASCADE ON DELETE CASCADE,
	CONSTRAINT [fk_saleLine_saleItemID] FOREIGN KEY([SaleItemID])
		REFERENCES [saleItem]([SaleItemID]) ON UPDATE CASCADE ON DELETE CASCADE
)
GO

-- Table for storing manual changes to the inventory table
-- Values are stored as they were before the change was made
print '' print '*** Creating inventoryAudit Table'
GO

CREATE TABLE [dbo].[inventoryAudit](
	[InventoryID]				[nvarchar](50)		NOT NULL,
	[VendorID]					[nvarchar](50)		NOT NULL,
	[PurchaseUnit]				[nvarchar](50)		NOT NULL,
	[SaleUnit]					[nvarchar](50)		NOT NULL,
	[SaleUnitsPerPurchaseUnit]	[int]				NOT NULL,
	[QuantityOnHand]			[int]				NOT NULL,
	[ReorderLevel]				[int]				NOT NULL,
	[TypeOfChange]				[nvarchar](20)		NOT NULL,
	[DateChanged]				[datetime]			NOT NULL,
	[EmployeeWhoChanged]		[int]				NOT NULL
)
GO

-- STORED PROCEDURES

print '' print '*** Creating sp_insert_employee'
GO
CREATE PROCEDURE [sp_insert_employee]
(
	@FirstName		[nvarchar](50),
	@LastName		[nvarchar](50),
	@PhoneNumber	[nvarchar](12),
	@Email			[nvarchar](250)
)
AS
BEGIN
	INSERT INTO [dbo].[employee]
		([FirstName],[LastName],[PhoneNumber],[Email])
	VALUES
		(@FirstName, @LastName, @PhoneNumber, LOWER(@Email))
	SELECT SCOPE_IDENTITY()
END
GO

print '' print '*** Creating sp_insert_vendor'
GO
CREATE PROCEDURE [sp_insert_vendor]
(
	@VendorID		[nvarchar](50),
	@VendorPhone	[nvarchar](12)
)
AS
BEGIN
	INSERT INTO [dbo].[vendor]
		([VendorID], [VendorPhone])
	VALUES
		(@VendorID, @VendorPhone)
	SELECT SCOPE_IDENTITY()
END
GO

print '' print '*** Creating sp_authenticate_user'
GO
CREATE PROCEDURE [sp_authenticate_user]
(
	@Email			[nvarchar](250),
	@PasswordHash	[nvarchar](100)
)
AS
BEGIN
	SELECT	COUNT([EmployeeID])
	FROM	[dbo].[employee]
	WHERE	[Email] = LOWER(@Email)
	AND		[PasswordHash] = @PasswordHash
	AND		[Active] = 1
END
GO

print '' print '*** Creating sp_update_email'
GO
CREATE PROCEDURE [sp_update_email]
(
	@OldEmail		[nvarchar](250),
	@NewEmail		[nvarchar](250)
)
AS
BEGIN
	UPDATE	[dbo].[Employee]
	SET		[Email] = LOWER(@NewEmail)
	WHERE	[Email] = LOWER(@OldEmail)
	AND		[Active] = 1
	RETURN @@ROWCOUNT
END
GO

print '' print '*** Creating sp_update_employee_phone_number'
GO
CREATE PROCEDURE [sp_update_employee_phone_number]
(
	@EmployeeID			[int],
	@OldPhoneNumber		[nvarchar](12),
	@NewPhoneNumber		[nvarchar](12)
)
AS
BEGIN
	UPDATE	[dbo].[Employee]
	SET		[PhoneNumber] = @NewPhoneNumber
	WHERE	[PhoneNumber] = @OldPhoneNumber
	AND 	[EmployeeID] = @EmployeeID
	AND		[Active] = 1
	RETURN @@ROWCOUNT
END
GO

print '' print '*** Creating sp_update_password'
GO
CREATE PROCEDURE [sp_update_password]
(
	@EmployeeID			[int],
	@OldPasswordHash	[nvarchar](100),
	@NewPasswordHash	[nvarchar](100)
)
AS
BEGIN
	UPDATE	[dbo].[Employee]
	SET		[PasswordHash] = @NewPasswordHash
	WHERE	[EmployeeID] = @EmployeeID
	AND		[PasswordHash] = @OldPasswordHash
	AND		[Active] = 1
	RETURN @@ROWCOUNT
END
GO

print '' print '*** Creating sp_update_vendor'
GO
CREATE PROCEDURE [sp_update_vendor]
(
	@NewVendorID			[nvarchar](50),
	@NewVendorPhone		    [nvarchar](12),
	@NewActiveStatus				 [bit],
	
	@OldVendorID			[nvarchar](50),
	@OldVendorPhone			[nvarchar](12)
)
AS
BEGIN
	UPDATE 	[dbo].[Vendor]
	   SET  [VendorID] =	@NewVendorID,
			[VendorPhone] =	@NewVendorPhone,
			[Active] =		@NewActiveStatus
	 WHERE 	[VendorID] =	@OldVendorID
	   AND	[VendorPhone] =	@OldVendorPhone
	   
	RETURN @@ROWCOUNT
END
GO

print '' print '*** Creating sp_select_roles_by_employeeid'
GO
CREATE PROCEDURE [sp_select_roles_by_employeeid]
(
	@EmployeeID		[int]
)
AS
BEGIN
	SELECT  [RoleID]
	FROM 	[EmployeeRole]
	WHERE 	[EmployeeID] = @EmployeeID
END
GO

print '' print '*** Creating sp_select_user_by_email'
GO
CREATE PROCEDURE [sp_select_user_by_email]
(
	@Email		[nvarchar](250)
)
AS
BEGIN
	SELECT  [EmployeeID], [FirstName], [LastName], [PhoneNumber], [Active]
	FROM 	[Employee]
	WHERE 	[Email] = @Email
END
GO

print '' print '*** Creating sp_select_users_by_active'
GO
CREATE PROCEDURE [sp_select_users_by_active]
(
	@Active		[bit]
)
AS
BEGIN
	SELECT  [EmployeeID], [FirstName], [LastName], [PhoneNumber], [Email], [Active]
	  FROM  [dbo].[employee]
	 WHERE  [Active] = @Active
END
GO

print '' print '*** Creating sp_select_inventory_items_by_active'
GO
CREATE PROCEDURE [sp_select_inventory_items_by_active]
(
	@Active		[bit]
)
AS
BEGIN
	SELECT [InventoryID], [PurchaseUnit], [SaleUnit], 
		   [SaleUnitsPerPurchaseUnit], [QuantityOnHand], [ReorderLevel], [Active]
	  FROM [dbo].[inventoryItem]
	 WHERE [Active] = @Active
END
GO

print '' print '*** Creating sp_select_inventory_item_by_id'
GO
CREATE PROCEDURE [sp_select_inventory_item_by_id]
(
	@InventoryID		[nvarchar](50)
)
AS
BEGIN
	SELECT [InventoryID], [PurchaseUnit], [SaleUnit], 
		   [SaleUnitsPerPurchaseUnit], [QuantityOnHand], [ReorderLevel], [Active]
	  FROM [dbo].[inventoryItem]
	 WHERE [InventoryID] = @InventoryID
END
GO

print '' print '*** Creating sp_select_sale_item_by_id'
GO
CREATE PROCEDURE [sp_select_sale_item_by_id]
(
	@SaleItemID		[int]
)
AS
BEGIN
	SELECT [SaleItemID], [ItemName], [ItemSize], [Flavor], [Price], [Active]
	  FROM [dbo].[saleItem]
	 WHERE [SaleItemID] = @SaleItemID
END
GO

print '' print '*** Creating sp_select_vendors_by_active'
GO
CREATE PROCEDURE [sp_select_vendors_by_active]
(
	@Active		[bit]
)
AS
BEGIN
	SELECT  [VendorID], [VendorPhone], [Active]
	  FROM  [dbo].[vendor]
	 WHERE  [Active] = @Active
END
GO

print '' print '*** Creating sp_select_sale_items_by_active'
GO
CREATE PROCEDURE [sp_select_sale_items_by_active]
(
	@Active		[bit]
)
AS
BEGIN
	SELECT  [SaleItemID], [ItemName], [ItemSize], [Flavor], [Price], [Active]
	  FROM  [dbo].[saleItem]
	 WHERE  [Active] = @Active
END
GO

print '' print '*** Creating sp_select_sale_items_by_active_and_name'
GO
CREATE PROCEDURE [sp_select_sale_items_by_active_and_name]
(
	@Active		[bit],
	@ItemName	[nvarchar](50)
)
AS
BEGIN
	SELECT  [SaleItemID], [ItemName], [ItemSize], [Flavor], [Price], [Active]
	  FROM  [dbo].[saleItem]
	 WHERE  [Active] = @Active
	   AND  [ItemName] = @ItemName
END
GO

print '' print '*** Creating sp_select_inventory_items_by_active_and_quantity'
GO
CREATE PROCEDURE [sp_select_inventory_items_by_active_and_quantity]
(
	@Active			[bit],
	@LowQuantity	[bit]
)
AS
BEGIN
	IF @LowQuantity = 1
		BEGIN
			SELECT [InventoryID], [PurchaseUnit], [SaleUnit], [SaleUnitsPerPurchaseUnit],
				   [QuantityOnHand], [ReorderLevel], [Active]
			  FROM [dbo].[inventoryItem]
			 WHERE [Active] = @Active
			   AND [QuantityOnHand] < [ReorderLevel]
		END
	ELSE
		BEGIN
			SELECT [InventoryID], [PurchaseUnit], [SaleUnit], [SaleUnitsPerPurchaseUnit],
				   [QuantityOnHand], [ReorderLevel], [Active]
			  FROM [dbo].[inventoryItem]
			 WHERE [Active] = @Active
		END
END
GO

print '' print '*** Creating sp_select_employee_by_id'
GO
CREATE PROCEDURE [sp_select_employee_by_id]
(
	@EmployeeID		[int]
)
AS
BEGIN
	SELECT	[EmployeeID], [FirstName], [LastName], [PhoneNumber], [Email], [Active]
	  FROM	[dbo].[Employee]
	 WHERE	[EmployeeID] = @EmployeeID
END
GO

print '' print '*** Creating sp_select_sale_item_names'
GO
CREATE PROCEDURE [sp_select_sale_item_names]
AS
BEGIN
	SELECT	DISTINCT[ItemName]
	  FROM	[dbo].[SaleItem]
END
GO

print '' print '*** Creating sp_update_employee'
GO
CREATE PROCEDURE [sp_update_employee]
(
	@EmployeeID			[int],
	
	@NewFirstName		[nvarchar](50),
	@NewLastName		[nvarchar](50),
	@NewPhoneNumber		[nvarchar](12),
	@NewEmail			[nvarchar](250),
	@NewActiveStatus	[bit],
	
	@OldFirstName		[nvarchar](50),
	@OldLastName		[nvarchar](50),
	@OldPhoneNumber		[nvarchar](12),
	@OldEmail			[nvarchar](250)
)
AS
BEGIN
	UPDATE 	[dbo].[employee]
	   SET  [FirstName] =	@NewFirstName,
			[LastName] =	@NewLastName,
			[PhoneNumber] =	@NewPhoneNumber,
			[Email] =		@NewEmail,
			[Active] =      @NewActiveStatus
	 WHERE 	[EmployeeID] =	@EmployeeID
	   AND	[FirstName] =	@OldFirstName
	   AND	[LastName] =	@OldLastName
	   AND	[PhoneNumber] =	@OldPhoneNumber
	   AND	[Email] =		@OldEmail
	   
	RETURN @@ROWCOUNT
END
GO

print '' print '*** Creating sp_deactivate_employee'
GO
CREATE PROCEDURE [sp_deactivate_employee]
(
	@EmployeeID		[int]
)
AS
BEGIN
	UPDATE  [dbo].[employee]
	   SET	[Active] = 0
	 WHERE	[EmployeeID] = @EmployeeID
	RETURN  @@ROWCOUNT
END
GO

print '' print '*** Creating sp_reactivate_employee'
GO
CREATE PROCEDURE [sp_reactivate_employee]
(
		@EmployeeID		[int]
)
AS
BEGIN
	UPDATE  [dbo].[employee]
	   SET	[Active] = 1
	 WHERE	[EmployeeID] = @EmployeeID
	RETURN  @@ROWCOUNT
END
GO

print '' print '*** Creating sp_deactivate_inventory_item'
GO
CREATE PROCEDURE [sp_deactivate_inventory_item]
(
	@InventoryID		[nvarchar](50)
)
AS
BEGIN
	UPDATE  [dbo].[inventoryItem]
	   SET	[Active] = 0
	 WHERE	[InventoryID] = @InventoryID
	RETURN  @@ROWCOUNT
END
GO

print '' print '*** Creating sp_reactivate_inventory_item'
GO
CREATE PROCEDURE [sp_reactivate_inventory_item]
(
	@InventoryID		[nvarchar](50)
)
AS
BEGIN
	UPDATE  [dbo].[inventoryItem]
	   SET	[Active] = 1
	 WHERE	[InventoryID] = @InventoryID
	RETURN  @@ROWCOUNT
END
GO

print '' print '*** Creating sp_deactivate_vendor'
GO
CREATE PROCEDURE [sp_deactivate_vendor]
(
	@VendorID		[nvarchar](50)
)
AS
BEGIN
	UPDATE  [dbo].[vendor]
	   SET	[Active] = 0
	 WHERE	[VendorID] = @VendorID
	RETURN  @@ROWCOUNT
END
GO

print '' print '*** Creating sp_reactivate_vendor'
GO
CREATE PROCEDURE [sp_reactivate_vendor]
(
		@VendorID		[nvarchar](50)
)
AS
BEGIN
	UPDATE  [dbo].[vendor]
	   SET	[Active] = 1
	 WHERE	[VendorID] = @VendorID
	RETURN  @@ROWCOUNT
END
GO

print '' print '*** Creating sp_deactivate_sale_item'
GO
CREATE PROCEDURE [sp_deactivate_sale_item]
(
	@SaleItemID		[int]
)
AS
BEGIN
	UPDATE  [dbo].[saleItem]
	   SET	[Active] = 0
	 WHERE	[SaleItemID] = @SaleItemID
	RETURN  @@ROWCOUNT
END
GO

print '' print '*** Creating sp_reactivate_sale_item'
GO
CREATE PROCEDURE [sp_reactivate_sale_item]
(
	@SaleItemID		[int]
)
AS
BEGIN
	UPDATE  [dbo].[saleItem]
	   SET	[Active] = 1
	 WHERE	[SaleItemID] = @SaleItemID
	RETURN  @@ROWCOUNT
END
GO

print '' print '*** Creating sp_delete_vendor_from_vendor_items'
GO
CREATE PROCEDURE [sp_delete_vendor_from_vendor_items]
(
	@VendorID		[nvarchar](50)
)
AS
BEGIN
	DELETE FROM [dbo].[vendorItem]
		  WHERE [VendorID] = @VendorID
END
GO

print '' print '*** Creating sp_delete_inventory_item_from_vendor_items'
GO
CREATE PROCEDURE [sp_delete_inventory_item_from_vendor_items]
(
	@InventoryID	[nvarchar](50)	
)
AS
BEGIN
	DELETE FROM [dbo].[vendorItem]
		  WHERE [InventoryID] = @InventoryID
END
GO

print '' print '*** Creating sp_insert_employee_role'
GO
CREATE PROCEDURE [sp_insert_employee_role]
(
	@EmployeeID		[int],
	@RoleID			[nvarchar](50)
)
AS
BEGIN
	INSERT INTO [dbo].[EmployeeRole]
	([EmployeeID], [RoleID])
	VALUES
	(@EmployeeID, @RoleID)
END
GO

print '' print '*** Creating sp_insert_vendor_item'
GO
CREATE PROCEDURE [sp_insert_vendor_item]
(
	@InventoryID		[nvarchar](50),
	@VendorID			[nvarchar](50)
)
AS
BEGIN
	INSERT INTO [dbo].[vendorItem]
	([InventoryID], [VendorID])
	VALUES
	(@InventoryID, @VendorID)
END
GO

print '' print '*** Creating sp_insert_sale_item_inventory'
GO
CREATE PROCEDURE [sp_insert_sale_item_inventory]
(
	@SaleItemID		[int],
	@InventoryID	[nvarchar](50),
	@Quantity		[decimal](8,2)
)
AS
BEGIN
	INSERT INTO [dbo].[saleItemInventory]
	([SaleItemID], [InventoryID], [Quantity])
	VALUES
	(@SaleItemID, @InventoryID, @Quantity)
END
GO

print '' print '*** Creating sp_delete_employee_role'
GO
CREATE PROCEDURE [sp_delete_employee_role]
(
	@EmployeeID		[int],
	@RoleID			[nvarchar](50)
)
AS
BEGIN
	DELETE FROM  [dbo].[employeeRole]
		  WHERE	 [EmployeeID] =  @EmployeeID
			AND	 [RoleID] = 	 @RoleID
END
GO

print '' print '*** Creating sp_delete_employees_roles'
GO
CREATE PROCEDURE [sp_delete_employees_roles]
(
	@EmployeeID		[int]
)
AS
BEGIN
	DELETE FROM  [dbo].[employeeRole]
		  WHERE	 [EmployeeID] =  @EmployeeID
END
GO

print '' print '*** Creating sp_delete_sale_item_inventory_by_sale_item'
GO
CREATE PROCEDURE [sp_delete_sale_item_inventory_by_sale_item]
(
	@SaleItemID		[int]
)
AS
BEGIN
	DELETE FROM  [dbo].[saleItemInventory]
		  WHERE	 [SaleItemID] =  @SaleItemID
END
GO

print '' print '*** Creating sp_select_all_roles'
GO
CREATE PROCEDURE [sp_select_all_roles]
AS
BEGIN
	  SELECT  [RoleID]
		FROM  [dbo].[Role]
	ORDER BY  [RoleID]
END
GO

print '' print '*** Creating sp_select_all_vendors'
GO
CREATE PROCEDURE [sp_select_all_vendors]
AS
BEGIN
	  SELECT  [VendorID], [VendorPhone], [Active]
		FROM  [dbo].[Vendor]
	ORDER BY  [VendorID]
END
GO

print '' print '*** Creating sp_select_active_vendor_names'
GO
CREATE PROCEDURE [sp_select_active_vendor_names]
AS
BEGIN
	  SELECT  [VendorID]
		FROM  [dbo].[Vendor]
	   WHERE  [Active] = 1
	ORDER BY  [VendorID]
END
GO

print '' print '*** Creating sp_select_vendor_names_by_inventory_item'
GO
CREATE PROCEDURE [sp_select_vendor_names_by_inventory_item]
(
	@InventoryID	[nvarchar](50)
)
AS
BEGIN
	  SELECT  [VendorID]
		FROM  [dbo].[VendorItem]
	   WHERE  [InventoryID] = @InventoryID
	ORDER BY  [VendorID]
END
GO

print '' print '*** Creating sp_select_vendor_id_by_name'
GO
CREATE PROCEDURE [sp_select_vendor_id_by_name]
(
	@VendorID		[nvarchar](50)
)
AS
BEGIN
	SELECT [VendorID]
	  FROM [dbo].[vendor]
	 WHERE [VendorID] = @VendorID
END
GO

print '' print '*** Creating sp_select_vendor_by_name'
GO
CREATE PROCEDURE [sp_select_vendor_by_name]
(
	@VendorID		[nvarchar](50)
)
AS
BEGIN
	SELECT [VendorID], [VendorPhone], [Active]
	  FROM [dbo].[vendor]
	 WHERE [VendorID] = @VendorID
END
GO

print '' print '*** Creating sp_select_inventory_quantities_by_sale_item'
GO
CREATE PROCEDURE [sp_select_inventory_quantities_by_sale_item]
(
	@SaleItemID		[int]
)
AS
BEGIN
	SELECT [InventoryID], [Quantity]
	  FROM [dbo].[saleItemInventory]
	 WHERE [SaleItemID] = @SaleItemID
END
GO

print '' print '*** Creating sp_delete_vendor'
GO
CREATE PROCEDURE [sp_delete_vendor]
(
	@VendorID	[nvarchar](50)
)
AS
BEGIN
	DELETE FROM  [dbo].[vendor]
		  WHERE	 [VendorID] =  @VendorID
END
GO

print '' print '*** Creating sp_insert_inventory_item'
GO
CREATE PROCEDURE [sp_insert_inventory_item]
(
	@InventoryID 					[nvarchar](50),
	@PurchaseUnit					[nvarchar](50),
	@SaleUnit						[nvarchar](50),
	@SaleUnitsPerPurchaseUnit		[decimal](8,2),
	@QuantityOnHand					[decimal](8,2),
	@ReorderLevel					[decimal](8,2)
)
AS
BEGIN
	INSERT INTO [dbo].[inventoryItem]
	([InventoryID], [PurchaseUnit], [SaleUnit], 
	[SaleUnitsPerPurchaseUnit], [QuantityOnHand], [ReorderLevel])
	VALUES
	(@InventoryID, @PurchaseUnit, @SaleUnit, 
	@SaleUnitsPerPurchaseUnit, @QuantityOnHand, @ReorderLevel)
	SELECT @InventoryID
END
GO

print '' print '*** Creating sp_insert_sale_item'
GO
CREATE PROCEDURE [sp_insert_sale_item]
(
	@ItemName			[nvarchar](50),
	@ItemSize			[nvarchar](50),
	@Flavor				[nvarchar](50),
	@Price				[money]
)
AS
BEGIN
	INSERT INTO [dbo].[saleItem]
	([ItemName], [ItemSize], [Flavor], [Price])
	VALUES
	(@ItemName, @ItemSize, @Flavor, @Price)
	SELECT SCOPE_IDENTITY()
END
GO

print '' print '*** Creating sp_select_all_inventory_items'
GO
CREATE PROCEDURE [sp_select_all_inventory_items]
AS
BEGIN
	  SELECT  [InventoryID], [PurchaseUnit], [SaleUnit], 
			  [SaleUnitsPerPurchaseUnit], [QuantityOnHand], [ReorderLevel]
		FROM  [dbo].[InventoryItem]
	ORDER BY  [InventoryID]
END
GO

print '' print '*** Creating sp_select_low_quantity_inventory_items'
GO
CREATE PROCEDURE [sp_select_low_quantity_inventory_items]
AS
BEGIN
	  SELECT  [InventoryID], [PurchaseUnit], [SaleUnit], 
			  [SaleUnitsPerPurchaseUnit], [QuantityOnHand], [ReorderLevel]
		FROM  [dbo].[InventoryItem]
	   WHERE  [QuantityOnHand] < [ReorderLevel]
	ORDER BY  [QuantityOnHand]
END
GO

print '' print '*** Creating sp_update_inventory_item'
GO
CREATE PROCEDURE [sp_update_inventory_item]
(
	@NewItemName					[nvarchar](50),
	@NewPurchaseUnit				[nvarchar](50),
	@NewSaleUnit					[nvarchar](50),
	@NewSaleUnitsPerPurchaseUnit	[decimal](8,2),
	@NewQuantityOnHand				[decimal](8,2),
	@NewReorderLevel				[decimal](8,2),
	@NewActiveStatus					 	 [bit],
	
	@OldItemName					[nvarchar](50),
	@OldPurchaseUnit				[nvarchar](50),
	@OldSaleUnit					[nvarchar](50),
	@OldSaleUnitsPerPurchaseUnit	[decimal](8,2),
	@OldQuantityOnHand				[decimal](8,2),
	@OldReorderLevel				[decimal](8,2)
)
AS
BEGIN
	UPDATE 	[dbo].[inventoryItem]
	   SET  [InventoryID] =					@NewItemName,
			[PurchaseUnit] =				@NewPurchaseUnit,
			[SaleUnit] =					@NewSaleUnit,
			[SaleUnitsPerPurchaseUnit] =	@NewSaleUnitsPerPurchaseUnit,
			[QuantityOnHand] =      		@NewQuantityOnHand,
			[ReorderLevel] =				@NewReorderLevel,
			[Active] =						@NewActiveStatus
	 WHERE 	[InventoryID] =					@OldItemName
	   AND	[PurchaseUnit] =				@OldPurchaseUnit
	   AND	[SaleUnit] =					@OldSaleUnit
	   AND	[SaleUnitsPerPurchaseUnit] =	@OldSaleUnitsPerPurchaseUnit
	   AND  [QuantityOnHand] =				@OldQuantityOnHand
	   AND  [ReorderLevel] = 				@OldReorderLevel
	   
	RETURN @@ROWCOUNT
END
GO

print '' print '*** Creating sp_update_sale_item'
GO
CREATE PROCEDURE [sp_update_sale_item]
(
	@SaleItemID					   [int],

	@NewItemName		  [nvarchar](50),
	@NewItemSize		  [nvarchar](50),
	@NewFlavor			  [nvarchar](50),
	@NewPrice		 			 [money],
	@NewActiveStatus			   [bit],
	
	@OldItemName		  [nvarchar](50),
	@OldPrice				      [money]
)
AS
BEGIN
	UPDATE 	[dbo].[saleItem]
	   SET  [ItemName] =			@NewItemName,
			[ItemSize] =			@NewItemSize,
			[Flavor] =				@NewFlavor,
			[Price] =				@NewPrice,
			[Active] =				@NewActiveStatus
	 WHERE 	[SaleItemID] =			@SaleItemID
	   AND	[ItemName] =			@OldItemName
	   AND  [Price] =				@OldPrice
	RETURN @@ROWCOUNT
END
GO

print '' print '*** Creating sp_delete_inventory_item'
GO
CREATE PROCEDURE [sp_delete_inventory_item]
(
	@InventoryID	[nvarchar](50)
)
AS
BEGIN
	DELETE FROM  [dbo].[inventoryItem]
		  WHERE	 [InventoryID] =  @InventoryID
END
GO

print '' print '*** Creating sp_delete_sale_item'
GO
CREATE PROCEDURE [sp_delete_sale_item]
(
	@SaleItemID		[int]
)
AS
BEGIN
	DELETE FROM  [dbo].[saleItem]
		  WHERE	 [SaleItemID] =  @SaleItemID
END
GO

print '' print '*** Creating sp_delete_employee'
GO
CREATE PROCEDURE [sp_delete_employee]
(
	@EmployeeID		[int]
)
AS
BEGIN
	DELETE FROM  [dbo].[employee]
		  WHERE	 [EmployeeID] =  @EmployeeID
END
GO

print '' print '*** Creating sp_select_preferred_vendor_by_inventory_item'
GO
CREATE PROCEDURE [sp_select_preferred_vendor_by_inventory_item]
(
	@InventoryID	[nvarchar](50)
)
AS
BEGIN
		SELECT [vendor].[VendorID]
		  FROM [dbo].[vendorItem]
	INNER JOIN [dbo].[vendor]
		    ON [vendorItem].[vendorID] = [vendor].[vendorID]
		 WHERE [InventoryID] = @InventoryID
		   AND [PreferredVendor] = 1
END
GO

print '' print '*** Creating sp_set_preferred_vendor'
GO
CREATE PROCEDURE [sp_set_preferred_vendor]
(
	@InventoryID	[nvarchar](50),
	@VendorID		[nvarchar](50)
)
AS
BEGIN
	IF EXISTS(
		SELECT [VendorID]
		  FROM [vendorItem]
		 WHERE [InventoryID] = @InventoryID
		   AND [PreferredVendor] = 1
		) 
		BEGIN
			UPDATE [vendorItem]
		       SET [PreferredVendor] = 0
		     WHERE [InventoryID] = @InventoryID
		       AND [PreferredVendor] = 1
		END 
	IF EXISTS(
		SELECT [VendorID]
		  FROM [vendorItem]
		 WHERE [InventoryID] = @InventoryID
		   AND [VendorID] = @VendorID
		)
		BEGIN
			UPDATE [vendorItem]
			   SET [PreferredVendor] = 1
			 WHERE [InventoryID] = @InventoryID
			   AND [VendorID] = @VendorID
		END
	ELSE BEGIN
		INSERT INTO [dbo].[vendorItem]
		([InventoryID], [VendorID], [PreferredVendor])
		VALUES
		(@InventoryID, @VendorID, 1)
	END
END
GO

print '' print '*** Creating sp_remove_preferred_vendor'
GO
CREATE PROCEDURE [sp_remove_preferred_vendor]
(
	@InventoryID	[nvarchar](50)
)
AS
BEGIN
	UPDATE [vendorItem]
	   SET [PreferredVendor] = 0
	 WHERE [InventoryID] = @InventoryID
END
GO


-- SAMPLE DATA

print '' print '*** Creating Sample Role Records'
GO
INSERT INTO [dbo].[role]
	([RoleID])
	VALUES
	('Administrator'),
	('General Manager'),
	('Shift Manager'),
	('Employee')
GO

print '' print '*** Creating Sample employee Records'
GO

INSERT INTO [dbo].[employee]
	([FirstName], [LastName], [PhoneNumber], [Email])
	VALUES
	('System', 'Admin', '13191111111', 'admin@company.com'),
	('John', 'Manager', '13192222222', 'john@company.com'),
	('Al', 'Coholic', '13193333333', 'al@company.com'),
	('Hugh', 'Jass', '13194444444', 'hugh@company.com')
GO

print '' print '*** Creating Sample Deactivated Employee'
GO
INSERT INTO [dbo].[employee]
	([FirstName], [LastName], [PhoneNumber], [Email], [Active])
	VALUES
	('Charles', 'Manson', '13196666666', 'charles@company.com', 0)
GO

print '' print '*** Inserting Sample employeeRole Records'
GO
INSERT INTO [dbo].[employeeRole]
	([EmployeeID], [RoleID])
	VALUES
	(1000000, 'Administrator'),
	(1000000, 'General Manager'),
	(1000000, 'Shift Manager'),
	(1000001, 'General Manager'),
	(1000002, 'Shift Manager'),
	(1000003, 'Employee')
GO

print '' print '*** Inserting Sample vendor Records'
GO
INSERT INTO [dbo].[vendor]
	([VendorID], [VendorPhone], [Active])
	VALUES
	('US Foods', '18001234567', 1),
	('Reinhart FoodService', '13191234567', 1),
	('Town and Country Wholesale', '13197654321', 1),
	('Terrible Food Distributor', '1111111611', 0)
GO

print '' print '*** Inserting Sample inventoryItem Records'
GO
INSERT INTO [dbo].[inventoryItem]
	([InventoryID], [PurchaseUnit], [SaleUnit], 
	[SaleUnitsPerPurchaseUnit], [QuantityOnHand], [ReorderLevel])
	VALUES
	('Cookie Dough', 'Box', 'Scoop', 400, 682, 200),
	('Oreo', 'Box', 'Scoop', 600, 1254, 400),
	('Cheesecake', 'Box', 'Scoop', 400, 715, 300),
	('Brownie Pieces', 'Box', 'Scoop', 600, 280, 300), -- Needs reorder
	('Chocolate Chunks', 'Box', 'Scoop', 150, 212, 100),
	('Hot Fudge', 'Box', 'Pump', 1000, 628, 200),
	('Cocoa Fudge', 'Box', 'Pump', 1000, 256, 200),
	('Strawberry', 'Box', 'Ladle', 500, 110, 50),
	('Small Cone', 'Box', 'Cone', 400, 195, 100),
	('Large Cone', 'Box', 'Cone', 400, 149, 100),
	('Vanilla Mix', 'Bag', 'Ounce', 640, 6400, 2560), -- Reorder when 4 bags are left
	('Chocolate Mix', 'Bag', 'Ounce', 640, 2560, 1280), -- Reorder when 2 bags are left
	('Chicken Strips', 'Box', 'Strip', 80, 478, 160), 
	('Fries', 'Box', 'Bag', 200, 119, 80),
	('Cheeseballs', 'Box', 'Bag', 100, 78, 40),
	('Mini Cup', 'Box', 'Cup', 500, 298, 100),
	('Small Cup', 'Box', 'Cup', 500, 99, 100), -- Needs reorder
	('Medium Cup', 'Box', 'Cup', 500, 313, 100),
	('Large Cup', 'Box', 'Cup', 500, 414, 100),
	('Large Drink Cup', 'Box', 'Cup', 500, 338, 100),
	('Wax Paper', 'Box', 'Sheet', 1000, 798, 150),
	('Meal Basket', 'Box', 'Basket', 500, 286, 100),
	('Bread', 'Loaf', 'Slice', 25, 96, 100), -- Needs reorder
	('Short Spoon', 'Box', 'Spoon', 1000, 656, 200),
	('Long Spoon', 'Box', 'Spoon', 1000, 964, 200),
	('Pie Chips', 'Box', 'Scoop', 600, 792, 600)
GO

print '' print '*** Inserting Sample Deactivated inventoryItem Records'
GO
INSERT INTO [dbo].[inventoryItem]
	([InventoryID], [PurchaseUnit], [SaleUnit], [SaleUnitsPerPurchaseUnit], 
	[QuantityOnHand], [ReorderLevel], [Active])
	VALUES
	('Pumpkin Pie Filling', 'Box', 'Pump', 1000, 0, 150, 0),
	('Candy Cane Chunks', 'Box', 'Scoop', 200, 100, 50, 0)
GO

print '' print '*** Inserting Sample vendorItem Records'
GO
INSERT INTO [dbo].[vendorItem]
	([InventoryID], [VendorID], [PreferredVendor])
	VALUES
	('Brownie Pieces', 'US Foods', 1),
	('Brownie Pieces', 'Reinhart FoodService', 0),
	('Cheesecake', 'US Foods', 0),
	('Oreo', 'Reinhart FoodService', 0),
	('Cookie Dough', 'Town and Country Wholesale', 1),
	('Chicken Strips', 'Reinhart FoodService', 0),
	('Chicken Strips', 'US Foods', 1),
	('Vanilla Mix', 'US Foods', 0),
	('Vanilla Mix', 'Town and Country Wholesale', 0),
	('Fries', 'Reinhart FoodService', 1),
	('Cheeseballs', 'Reinhart FoodService', 1),
	('Mini Cup', 'US Foods', 1),
	('Mini Cup', 'Reinhart FoodService', 0)
GO

print '' print '*** Inserting Sample saleItem Records'
GO
INSERT INTO [dbo].[saleItem]
	([ItemName], [ItemSize], [Flavor], [Price])
	VALUES
	('Blizzard', 'Mini', 'Chocolate Extreme', 3.06),
	('Blizzard', 'Small', 'Chocolate Extreme', 4.12),
	('Blizzard', 'Medium', 'Chocolate Extreme', 4.97),
	('Blizzard', 'Large', 'Chocolate Extreme', 5.76),
	('Cone', 'Small', 'Chocolate', 2.11),
	('Cone', 'Small', 'Vanilla', 2.11),
	('Cone', 'Small', 'Twist', 2.11),
	('Chicken Strip Basket', '4 Piece', null, 5.76),
	('Chicken Strip Basket', '6 Piece', null, 9.16),
	('Fries', null, null, 2.75),
	('Cheeseballs', null, null, 3.19)
GO

print '' print '*** Inserting Sample Deactivated saleItem Records'
GO
INSERT INTO [dbo].[saleItem]
	([ItemName], [ItemSize], [Flavor], [Price], [Active])
	VALUES
	('Blizzard', 'Mini', 'Pumpkin Pie', 3.06, 0),
	('Blizzard', 'Small', 'Pumpkin Pie', 4.12, 0),
	('Blizzard', 'Medium', 'Pumpkin Pie', 4.97, 0),
	('Blizzard', 'Large', 'Pumpkin Pie', 5.76, 0),
	('Blizzard', 'Mini', 'Candy Cane Oreo', 3.06, 0),
	('Blizzard', 'Small', 'Candy Cane Oreo', 4.12, 0),
	('Blizzard', 'Medium', 'Candy Cane Oreo', 4.97, 0),
	('Blizzard', 'Large', 'Candy Cane Oreo', 5.76, 0)
GO

print '' print '*** Inserting Sample saleItemInventory Records'
GO
INSERT INTO [dbo].[saleItemInventory]
	([SaleItemID], [InventoryID], [Quantity])
	VALUES
	(1000000, 'Brownie Pieces', .5), (1000000, 'Chocolate Chunks', .5), (1000000, 'Cocoa Fudge', .5), (1000000, 'Vanilla Mix', 3.5), (1000000, 'Mini Cup', 1), (1000000, 'Short Spoon', 1),
	(1000001, 'Brownie Pieces', 1), (1000001, 'Chocolate Chunks', 1), (1000001, 'Cocoa Fudge', 1), (1000001, 'Vanilla Mix', 5), (1000001, 'Small Cup', 1), (1000001, 'Short Spoon', 1),
	(1000002, 'Brownie Pieces', 2), (1000002, 'Chocolate Chunks', 2), (1000002, 'Cocoa Fudge', 2), (1000002, 'Vanilla Mix', 8.5), (1000002, 'Medium Cup', 1), (1000002, 'Long Spoon', 1),
	(1000003, 'Brownie Pieces', 3), (1000003, 'Chocolate Chunks', 3), (1000003, 'Cocoa Fudge', 3), (1000003, 'Vanilla Mix', 14), (1000003, 'Large Cup', 1), (1000003, 'Long Spoon', 1),
	(1000004, 'Small Cone', 1), (1000004, 'Chocolate Mix', 5),
	(1000005, 'Small Cone', 1), (1000005, 'Vanilla Mix', 5),
	(1000006, 'Small Cone', 1), (1000006, 'Chocolate Mix', 2.5), (1000006, 'Vanilla Mix', 2.5),
	(1000007, 'Chicken Strips', 4), (1000007, 'Fries', 1), (1000007, 'Bread', 1), (1000007, 'Meal Basket', 1), (1000007, 'Wax Paper', 1),
	(1000008, 'Chicken Strips', 6), (1000008, 'Fries', 1), (1000008, 'Bread', 1), (1000008, 'Meal Basket', 1), (1000008, 'Wax Paper', 1),
	(1000009, 'Fries', 1), (1000009, 'Small Cup', 1),
	(1000010, 'Cheeseballs', 1), (1000010, 'Mini Cup', 1),
	(1000011, 'Pumpkin Pie Filling', .5), (1000011, 'Pie Chips', .5), (1000011, 'Vanilla Mix', 3.5), (1000011, 'Mini Cup', 1), (1000011, 'Short Spoon', 1),
	(1000012, 'Pumpkin Pie Filling', 1), (1000012, 'Pie Chips', 1), (1000012, 'Vanilla Mix', 5), (1000012, 'Small Cup', 1), (1000012, 'Short Spoon', 1),
	(1000013, 'Pumpkin Pie Filling', 2), (1000013, 'Pie Chips', 2), (1000013, 'Vanilla Mix', 8.5), (1000013, 'Medium Cup', 1), (1000013, 'Long Spoon', 1),
	(1000014, 'Pumpkin Pie Filling', 3), (1000014, 'Pie Chips', 3), (1000014, 'Vanilla Mix', 14), (1000014, 'Large Cup', 1), (1000014, 'Long Spoon', 1),
	(1000015, 'Candy Cane Chunks', .5), (1000015, 'Oreo', .5), (1000015, 'Vanilla Mix', 3.5), (1000015, 'Mini Cup', 1), (1000015, 'Short Spoon', 1),
	(1000016, 'Candy Cane Chunks', 1), (1000016, 'Oreo', 1), (1000016, 'Vanilla Mix', 5), (1000016, 'Small Cup', 1), (1000016, 'Short Spoon', 1),
	(1000017, 'Candy Cane Chunks', 2), (1000017, 'Oreo', 2), (1000017, 'Vanilla Mix', 8.5), (1000017, 'Medium Cup', 1), (1000017, 'Long Spoon', 1),
	(1000018, 'Candy Cane Chunks', 3), (1000018, 'Oreo', 3), (1000018, 'Vanilla Mix', 14), (1000018, 'Large Cup', 1), (1000018, 'Long Spoon', 1)
GO