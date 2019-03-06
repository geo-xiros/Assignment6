using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Data.Common;

namespace Assignment6.Models
{
    public partial class ApplicationDbContext
    {
        private string _connectionString;
        private string _connectionStringMaster { get => _connectionString.Replace("Assignment6DB", "master"); }
        public string Error { get; private set; }
        public ApplicationDbContext()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"].ConnectionString;

            Users = new UserManager(this);
            Roles = new RoleManager(this);
            Registrations = new RegistrationManager(this);
            Documents = new DocumentManager(this);
            DocumentAssigns = new DocumentAssignManager(this);
            if (!DatabaseExists())
            {
                if (CreateDatabase())
                {
                    CreateDbSchema();
                }
            }
        }
        private void CreateDbSchema()
        {
            UsingConnection((dbCon)=>{
                dbCon.Execute("CREATE TABLE [User] (Id int identity(1,1) not null,Username varchar(50) not null,	Password varchar(50) not null,	constraint PK_User primary key (Id))");
                dbCon.Execute("CREATE TABLE Role (Id int Identity(1,1) not null,Name varchar(50) not null,	constraint PK_Role Primary key (Id))");
                dbCon.Execute("CREATE UNIQUE INDEX IX_RoleNameUnique on Role(Name)");
                dbCon.Execute("CREATE TABLE UserRoles (	UserId int not null,RoleId int not null,constraint PK_UserRoles primary key (UserId,RoleId),	constraint FK_UserRolesUserId foreign key (UserId) references [User](Id),	constraint FK_UserRolesRoleId foreign key (RoleId) references Role(Id))");
                dbCon.Execute("CREATE TABLE Registration (	Id int Identity(1,1) not null,	UserId int not null,	RoleId int not null,	RegisteredByUserId int null,	Status varchar(10) not null,	constraint PK_Registration primary key (Id),	constraint FK_RegistrationUserId foreign key (UserId) references [User](Id),	constraint FK_RegistrationRegisteredByUserId foreign key (RegisteredByUserId) references [User](Id),	constraint FK_RegistrationRoleId foreign key (RoleId) references Role(Id))");
                dbCon.Execute("CREATE TABLE Document (	Id int Identity(1,1) not null,	Title varchar(50) not null,	Body Text null,	constraint PK_Document Primary key (Id))");
                dbCon.Execute("CREATE TABLE DocumentAssign(	Id int identity(1,1) not null,	DocumentId int not null,	AssignedToRoleId int not null,	PurchasedByUserId int null,	Status varchar(20) not null,	constraint PK_DocumentAssign primary key (Id),	constraint FK_DocumentAssignDocumentId foreign key (DocumentId) references Document(Id),	constraint FK_DocumentAssignPurchasedByUserId foreign key (PurchasedByUserId) references [User](Id),	constraint FK_DocumentAssignAssignedToRoleId foreign key (AssignedToRoleId) references Role(Id))");
                dbCon.Execute("INSERT INTO [User] (Username,Password) values ('manager','1234')");
                dbCon.Execute("INSERT INTO Role (Name) Values ('Manager'),('Architect'),('Analyst'),('Programmer'),('Tester')");
                dbCon.Execute("INSERT INTO UserRoles (userId, RoleId) SELECT U.Id, R.Id FROM (SELECT Id FROM [USER] WHERE Username = 'Manager') AS U, (SELECT Id FROM [Role] WHERE Name='Manager') R");
            });
        }
        private bool CreateDatabase()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionStringMaster))
                {
                    sqlConnection.Execute("CREATE DATABASE Assignment6DB");
                }
                return true;
            }
            catch (DbException)
            {
                return false;
            }
        }
        private bool DatabaseExists()
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionStringMaster))
                {
                    sqlConnection.Execute("USE Assignment6DB");
                }
                return true;
            }
            catch (DbException)
            {
                return false;
            }

        }
        public bool UsingConnection(Action<SqlConnection> action)
        {
            Error = string.Empty;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    action(sqlConnection);
                }
            }
            catch (System.Data.Common.DbException e)
            {
                Error = e.Message;
            }

            return Error == string.Empty;
        }

        public UserManager Users { get; set; }
        public RoleManager Roles { get; set; }
        public RegistrationManager Registrations { get; set; }
        public DocumentManager Documents { get; set; }
        public DocumentAssignManager DocumentAssigns { get; set; }
    }

    public abstract class TableManager<T> : IDatabaseActions<T>
    {
        protected ApplicationDbContext _db;
        protected Dictionary<string, string> _queryParts;
        public IEnumerable<T> Court
        {
            get
            {
                return Get();
            }
        }

        public virtual T Add(T row)
        {
            T addedCourt = default(T);
            _db.UsingConnection((dbCon) =>
            {
                addedCourt = dbCon
                    .Query<T>(_queryParts["InsertQuery"], row)
                    .FirstOrDefault();
            });

            return addedCourt;
        }

        public T Find(int id)
        {
            return Get(_queryParts["FindById"], new { id }).FirstOrDefault();
        }
        public bool Find(int id, out T table)
        {
            table = Find(id);
            return table != null;
        }

        public abstract IEnumerable<T> Get(string query = null, object parameters = null);

        public bool Remove(int id)
        {
            int rowsAffected = 0;
            _db.UsingConnection((dbCon) =>
            {
                rowsAffected = dbCon
                    .Execute(_queryParts["RemoveQuery"], new { id });
            });
            return rowsAffected > 0;
        }

        public bool Update(T row)
        {
            int rowsAffected = 0;
            _db.UsingConnection((dbCon) =>
            {
                rowsAffected = dbCon
                    .Execute(_queryParts["UpdateQuery"], row);
            });
            return rowsAffected > 0;
        }
    }

    public interface IDatabaseActions<T>
    {
        IEnumerable<T> Get(string query, object parameters);
        T Find(int id);
        T Add(T row);
        bool Remove(int id);
        bool Update(T row);
    }
}
