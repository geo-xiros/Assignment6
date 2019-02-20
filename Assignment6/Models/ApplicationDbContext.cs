using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;

namespace Assignment6.Models
{
    public partial class ApplicationDbContext
    {
        private string _connectionString;
        public ApplicationDbContext()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["ApplicationDbContext"].ConnectionString;
            //Court = new CourtManager(this);
            //Branch = new BranchManager(this);
            Users = new UserManager(this);
            Roles = new RoleManager(this);
            //Facility = new FacilityManager(this);
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
