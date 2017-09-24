using System;
using System.Configuration;
using System.Linq;
using MongoDB.Driver;

namespace MongoIdentity
{
    public class IdentityDbContext<TUser> : IdentityDbContext<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    where TUser : IdentityUser
    {
        public IdentityDbContext() : this("DefaultConnection") { }
        public IdentityDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
    }

    public class IdentityDbContext : IdentityDbContext<IdentityUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public IdentityDbContext() : this("DefaultConnection") { }
        public IdentityDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
    }

    public class IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> : DbContext
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
    {

        public bool RequireUniqueEmail
        {
            get;
            set;
        }
        public virtual IQueryable<TRole> Roles
        {
            get;
            set;
        }
        public virtual IQueryable<TUser> Users
        {
            get;
            set;
        }
        public IdentityDbContext() : base("DefaultConnection") { }
        public IdentityDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }
        public void Dispose()
        {
        }
    }
}