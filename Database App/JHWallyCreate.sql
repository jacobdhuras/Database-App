/*
* FILE:				JHWallyCreate.sql
* PROJECT:			Relational Databases - Wally's World A4
* PROGRAMMER:		Jacob Huras
* FIRST VERSION:	12/05/2016
* DESCRIPTION:
*	This file contains queries that creates the jhwally database.
*/

create database JHWally;
use JHWally;

#drop database jhwally;

grant all privileges on JHWally.* To 'root'@'localhost' IDENTIFIED BY 'Conestoga1';

# Create Customer table
create table Customer(
	`ID` int not null AUTO_INCREMENT,
    `FirstName` varchar(20) not null,
    `LastName` varchar(20) not null,
    `PhoneNumber` varchar(11) not null,
    primary key (`ID`));
    
# Create Branch table
create table Branch(
	`ID` int not null AUTO_INCREMENT,
    `BranchName` varchar(30) not null,
    primary key (`ID`));
    
create table Inventory(
	`ID` int not null AUTO_INCREMENT,
    `BranchID` int not null,
    primary key (`ID`),
    foreign key (`BranchID`) references Branch(`ID`));

# Create Product table
create table Product(
	`ID` int not null AUTO_INCREMENT,
    `InventoryID` int not null,
    `Name` varchar(40) not null,
    `Price` double not null,
    `Quantity` int not null,
    `Discontinued` bool not null,
    primary key (`ID`),
    foreign key (`InventoryID`) references Inventory(`ID`));
    
# Create Order table
create table `Order`(
	`ID` int not null AUTO_INCREMENT,
    `CustomerID` int,
    `BranchID` int,
    `Date` date not null,
    `Status` varchar(4) not null,
    primary key (`ID`),
    foreign key (`CustomerID`) references Customer(`ID`),
    foreign key (`BranchID`) references Branch(`ID`));
    
# Create OrderLine table
create table OrderLine(
	`ID` int not null AUTO_INCREMENT,
    `OrderID` int,
    `ProductID` int,
    `Quantity` int not null,
    primary key (`ID`),
    foreign key (`OrderID`) references `Order`(`ID`),
    foreign key (`ProductID`) references `Product`(`ID`));
    
# Initalize starting table data
insert into Customer (FirstName, LastName, PhoneNumber)
values ('Norbert', 'Mika', '4165551111');
insert into Customer (FirstName, LastName, PhoneNumber)
values ('Russel', 'Foubert', '5195552222');
insert into Customer (FirstName, LastName, PhoneNumber)
values ('Sean', 'Clarke', '5195553333');

insert into Branch (BranchName)
values ('Sports World');
insert into Branch (BranchName)
values ('Cambridge Mall');
insert into Branch (BranchName)
values ('St. Jacobs');

insert into Inventory (BranchID)
values (1);
insert into Inventory (BranchID)
values (2);
insert into Inventory (BranchID)
values (3);

# setup for Sports World
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Disco Queen Wallpaper (roll)', 12.95, 56, 1, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Countryside Wallpaper (roll)', 11.95, 24, 1, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Victorian Lace Wallpaper (roll)', 14.95, 44, 1, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Tape (roll)', 3.95, 120, 1, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Tape (pkg 10)', 36.95, 30, 1, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Repair Compound (tube)', 6.95, 90, 1, false);

# setup for Cambridge Mall
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Disco Queen Wallpaper (roll)', 12.95, 20, 2, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Countryside Wallpaper (roll)', 11.95, 20, 2, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Victorian Lace Wallpaper (roll)', 14.95, 20, 2, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Tape (roll)', 3.95, 20, 2, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Tape (pkg 10)', 36.95, 20, 2, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Repair Compound (tube)', 6.95, 20, 2, false);

# setup for St. Jacobs
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Disco Queen Wallpaper (roll)', 12.95, 20, 3, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Countryside Wallpaper (roll)', 11.95, 20, 3, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Victorian Lace Wallpaper (roll)', 14.95, 20, 3, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Tape (roll)', 3.95, 20, 3, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Tape (pkg 10)', 36.95, 20, 3, false);
insert into Product (`Name`, Price, Quantity, InventoryID, Discontinued)
values ('Drywall Repair Compound (tube)', 6.95, 20, 3, false);

insert into `Order` (CustomerID, `Date`, `Status`, BranchID)
values ((select ID from Customer where LastName='Clarke'), '2016-09-20', 'PAID', 1); # sports world
insert into `OrderLine` (OrderID, ProductID, Quantity)
values (1, 3, 4);

insert into `Order` (CustomerID, `Date`, `Status`, BranchID)
values ((select ID from Customer where LastName='Foubert'), '2016-10-06', 'PEND', 2); # cambridge
insert into `OrderLine` (OrderID, ProductID, Quantity)
values (2, 2, 10);

update `Order` set `Status`='CNCL', `Date`='2016-11-06' where ID=2;

insert into `Order` (CustomerID, `Date`, `Status`, BranchID)
values ((select ID from Customer where LastName='Mika'), '2016-11-02', 'PAID', 3); # st jacobs
insert into `OrderLine` (OrderID, ProductID, Quantity)
values (3, 1, 12);

update `Order` set `Status`='RFND', `Date`='2016-11-04' where ID=3;
