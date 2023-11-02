use warehouseManagement

CREATE TABLE Users (
    id INT IDENTITY(1,1) NOT NULL,
    Username VARCHAR(45) NOT NULL,
    Role VARCHAR(45) NOT NULL,  -- You can use VARCHAR for Role
    Password VARCHAR(64) NOT NULL,
    Email VARCHAR(200) NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT Username_UNIQUE UNIQUE (Username)
);

CREATE TABLE Categories (
    id INT IDENTITY(1,1) NOT NULL,
    Category VARCHAR(200) NOT NULL,
    PRIMARY KEY (id),
    CONSTRAINT Category_UNIQUE UNIQUE (Category)
);

USE warehouseManagement;

CREATE TABLE Items (
    id INT IDENTITY(1,1) NOT NULL,
    Itemname VARCHAR(200) NOT NULL,
    Category_Id INT NOT NULL,
    Unit VARCHAR(45) NULL,
    Location VARCHAR(200) NULL,
    Description VARCHAR(500) NULL,
    PRIMARY KEY (id),
    CONSTRAINT Itemname_UNIQUE UNIQUE (Itemname),
    INDEX Items_Category_idx (Category_Id),
    CONSTRAINT Items_Category
        FOREIGN KEY (Category_Id)
        REFERENCES Categories (id)
        ON DELETE CASCADE
        ON UPDATE NO ACTION
);

CREATE TABLE Settlement (
    id INT IDENTITY(1,1) NOT NULL,
    Item_Id INT NOT NULL,
    Quantity INT NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    Settle_Date DATE NOT NULL,
    PRIMARY KEY (id),
    INDEX Settlement_Item_Id_idx (Item_Id),
    CONSTRAINT Settlement_Item_Id
        FOREIGN KEY (Item_Id)
        REFERENCES Items (id)
        ON DELETE CASCADE
        ON UPDATE NO ACTION
);

CREATE TABLE Transactions (
    id INT IDENTITY(1,1) NOT NULL,
    Item_Id INT NOT NULL,
    Quantity INT NOT NULL,
    Total DECIMAL(10,2) NOT NULL,
    Transaction_Date DATE NOT NULL,
    User_Id INT NOT NULL,
    PRIMARY KEY (id),
    INDEX user_id_idx (User_Id),
    INDEX item_id_idx (Item_Id),
    CONSTRAINT user_id
        FOREIGN KEY (User_Id)
        REFERENCES Users (id)
        ON DELETE NO ACTION
        ON UPDATE NO ACTION,
    CONSTRAINT item_id
        FOREIGN KEY (Item_Id)
        REFERENCES Items (id)
        ON DELETE CASCADE
        ON UPDATE NO ACTION
);