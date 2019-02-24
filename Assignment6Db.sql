use master
go
drop database Assignment6DB 
go

CREATE DATABASE Assignment6DB 
GO

USE Assignment6DB
GO

CREATE TABLE [User] (
	Id int identity(1,1) not null,
	Username varchar(50) not null,
	Password varchar(50) not null,
	constraint PK_User primary key (Id)
)
go

CREATE TABLE Role (
	Id int Identity(1,1) not null,
	Name varchar(50) not null,
	constraint PK_Role Primary key (Id)
)

CREATE UNIQUE INDEX IX_RoleNameUnique on Role(Name)
go

CREATE TABLE UserRoles (
	UserId int not null,
	RoleId int not null,
	constraint PK_UserRoles primary key (UserId,RoleId),
	constraint FK_UserRolesUserId foreign key (UserId) references [User](Id),
	constraint FK_UserRolesRoleId foreign key (RoleId) references Role(Id)
)
go

CREATE TABLE Registration (
	Id int Identity(1,1) not null,
	UserId int not null,
	RoleId int not null,
	RegisteredByUserId int null,
	Status varchar(10) not null,
	constraint PK_Registration primary key (Id),
	constraint FK_RegistrationUserId foreign key (UserId) references [User](Id),
	constraint FK_RegistrationRegisteredByUserId foreign key (RegisteredByUserId) references [User](Id),
	constraint FK_RegistrationRoleId foreign key (RoleId) references Role(Id)
)

CREATE TABLE Document (
	Id int Identity(1,1) not null,
	Title varchar(50) not null,
	Body Text null,
	constraint PK_Document Primary key (Id)
)
go

CREATE TABLE DocumentAssign(
	Id int identity(1,1) not null,
	DocumentId int not null,
	AssignedToRoleId int not null,
	PurchasedByUserId int null,
	Status varchar(20) not null,
	constraint PK_DocumentAssign primary key (Id),
	constraint FK_DocumentAssignDocumentId foreign key (DocumentId) references Document(Id),
	constraint FK_DocumentAssignPurchasedByUserId foreign key (PurchasedByUserId) references [User](Id),
	constraint FK_DocumentAssignAssignedToRoleId foreign key (AssignedToRoleId) references Role(Id)
)
go

INSERT INTO [User] (Username,Password) values ('geo.xiros','1234')
go
INSERT INTO Role (Name) Values ('Manager'),('Architect'),('Analyst'),('Programmer'),('Tester')
go

INSERT INTO UserRoles (UserId, RoleId) VALUES (1, 1)
go

