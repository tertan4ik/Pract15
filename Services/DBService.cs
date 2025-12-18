using pract_15.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pract_15.Services
{
    public class DBService
    {
        private static DBService instance;
        public static DBService Instance
        {
            get
            {
                if (instance == null)
                    instance = new DBService();
                return instance;
            }
        }

        public ElectroShopDbContext Context { get; }

        private DBService()
        {
            Context = new ElectroShopDbContext();
        }
    }
}
