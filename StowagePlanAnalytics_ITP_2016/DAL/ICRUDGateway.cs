using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StowagePlanAnalytics_ITP_2016.DAL
{
    interface ICRUDGateway<T> where T : class
    {
        IEnumerable<T> SelectAll();
        T SelectByPrimaryKey(string key);
        T SelectByPrimaryKey(int? key);
        void Insert(T obj);
        void Update(T obj);
        T Delete(string key);
        void Save();
    }
}
