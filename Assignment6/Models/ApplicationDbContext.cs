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
                CreateDatabase();
            }
        }
        private void CreateDatabase()
        {

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
        public void UsingConnection(Action<SqlConnection> action)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    action(sqlConnection);
                }
            }
            catch (Exception)
            {

                throw;
            }
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
