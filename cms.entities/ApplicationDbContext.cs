using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoIdentity;

namespace CMS.Entities
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")//base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        //public MongoCollection<Country> Countries { get; set; }
        public MongoCollection<Country> Countries
        {
            get
            {
               //   var countries = Get<Country>();
              var countries = GetCollection<Country>("Countries");
                return countries;
            }
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
