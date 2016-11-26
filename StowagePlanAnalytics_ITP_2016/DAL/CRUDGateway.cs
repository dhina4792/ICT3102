using StowagePlanAnalytics_ITP_2016.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace StowagePlanAnalytics_ITP_2016.DAL
{
    public class CRUDGateway<T> : ICRUDGateway<T> where T : class
    {
        // Database context
        protected StowageDBContext db = new StowageDBContext();
        protected DbSet<T> data = null;

        //Testing
        private ApplicationDbContext dbUser = new ApplicationDbContext();

        public CRUDGateway()
        {
            this.data = db.Set<T>();
        }

        public IEnumerable<T> SelectAll()
        {
            return data.ToList();
        }
        public T[] SelectAllArray()
        {
            return data.ToArray();
        }

        public virtual T SelectByPrimaryKey(string key)
        {
            return data.Find(key);
        }

        public T SelectByPrimaryKey(int? key)
        {
            return data.Find(key);
        }

        public virtual void Insert(T obj)
        {
            try
            {
                data.Add(obj);
                db.SaveChanges();
            }
            /*Display Database Entity Validation Error Messages*/
            catch (DbEntityValidationException dbEx)
            {
                string errorMessages = string.Join("; ", dbEx.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
                throw new DbEntityValidationException(errorMessages);
            }
        }

        public void Update(T obj)
        {
            db.Entry(obj).State = EntityState.Modified;
            db.SaveChanges();
        }

        public virtual T Delete(string key)
        {
            T obj = data.Find(key);
            data.Remove(obj);
            db.SaveChanges();
            return obj;
        }

        public void Save()
        {
            db.SaveChanges();
        }

        /*Check whether is there the same existing port code in the system*/
        public bool VesselCodeExist(string VesselCode)
        {
            var pc = db.Vessel.FirstOrDefault(d => d.VesselCode == VesselCode);
            return (pc != null);
        }

        /*Check whether is there the same existing Role name in the system*/
        public bool RoleNameExist(string RoleName)
        {
            var pc = dbUser.Roles.FirstOrDefault(d => d.Name == RoleName);
            return (pc != null);
        }
    }
}