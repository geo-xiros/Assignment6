CREATE DATABASE Assignment6DB 
GO

USE Assignment6DB
GO

CREATE TABLE [User] (
	Id int identity(1,1) not null,
	Username varchar(50) not null,
	Password varchar(50) not null,
	Registered bit not null default(0),
	constraint pk_User primary key (Id)
)
go

CREATE TABLE Role (
	Id int Identity(1,1) not null,
	Name varchar(50) not null,
	constraint pk_Role Primary key (Id)
)

CREATE UNIQUE INDEX Ix_RoleNameUnique on Role(Name)
go

CREATE TABLE UserRoles (
	UserId int not null,
	RoleId int not null,
	constraint pk_UserRoles primary key (UserId,RoleId),
	constraint fk_UserId foreign key (UserId) references [User](Id),
	constraint fk_RoleId foreign key (RoleId) references Role(Id)
)
go

CREATE TABLE Document (
	Id int Identity(1,1) not null,
	Name varchar(50) not null,
	constraint pk_Document Primary key (Id)
)
go

CREATE TABLE AssignedDocuments(
	Id int identity(1,1) not null,
	DocumentId int not null,
	RoleId int not null,
	Completed bit not null default(0),
	constraint pk_AssignedDocuments primary key (Id),
	constraint fk_AssignedDocumentsDocumentId foreign key (DocumentId) references Document(Id),
	constraint fk_AssignedDocumentsRoleId foreign key (RoleId) references Role(Id)
)
go

INSERT INTO [User] (Username,Password,Registered) values ('geo.xiros','1234', 1)
go
INSERT INTO Role (Name) Values ('Architect'),('Analyst'),('Programmer'),('Tester'),('Manager')
go