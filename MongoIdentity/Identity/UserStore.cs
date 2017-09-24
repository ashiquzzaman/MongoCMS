﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace MongoIdentity
{

    /// <summary>
    /// Class UserStore.
    /// </summary>
    /// <typeparam name="TUser">The type of the t user.</typeparam>
    public class UserStore<TUser> : UserStore<TUser, IdentityRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>, IUserStore<TUser>, IUserStore<TUser, string>, IDisposable
    where TUser : IdentityUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserStore{TUser}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserStore(IdentityDbContext context) : base(context.Db) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStore{TUser}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserStore(DbContext context) : base(context.Db) { }


        /// <summary>
        /// Initializes a new instance of the <see cref="UserStore{TUser}"/> class.
        /// </summary>
        /// <param name="db">The database.</param>
        public UserStore(MongoDatabase db) : base(db) { }
    }



    /// <summary>
    /// Class UserStore.
    /// </summary>
    /// <typeparam name="TUser">The type of the t user.</typeparam>
    /// <typeparam name="TRole">The type of the t role.</typeparam>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TUserLogin">The type of the t user login.</typeparam>
    /// <typeparam name="TUserRole">The type of the t user role.</typeparam>
    /// <typeparam name="TUserClaim">The type of the t user claim.</typeparam>
    public class UserStore<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> :
        IUserLoginStore<TUser, TKey>, IUserClaimStore<TUser, TKey>, IUserRoleStore<TUser, TKey>,
        IUserPasswordStore<TUser, TKey>, IUserSecurityStampStore<TUser, TKey>,
        IQueryableUserStore<TUser, TKey>, IUserEmailStore<TUser, TKey>, IUserLockoutStore<TUser, TKey>,
        IUserPhoneNumberStore<TUser, TKey>, IUserTwoFactorStore<TUser, TKey>,IUserStore<TUser, TKey>, IDisposable
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin<TKey>, new()
        where TUserRole : IdentityUserRole<TKey>, new()
        where TUserClaim : IdentityUserClaim<TKey>, new()

    {
        #region Private Methods & Variables



        /// <summary>
        ///     The database
        /// </summary>
        private readonly MongoDatabase db;

        /// <summary>
        ///     The _disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The AspNetUsers collection name
        /// </summary>
        private const string collectionName = "AspNetUsers";


        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStore{TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim}"/> class.
        /// </summary>
        public UserStore(): this(new IdentityDbContext().Db) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="UserStore{TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public UserStore(MongoDatabase context)
        {
            db = context;
        }


        #endregion

        #region Methods

        /// <summary>
        ///     Adds the claim asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="claim">The claim.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task AddClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Claims.Any(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value))
            {
                user.Claims.Add(new IdentityUserClaim
                {
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }


            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets the claims asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{IList{Claim}}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            IList<Claim> result = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult(result);
        }

        /// <summary>
        ///     Removes the claim asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="claim">The claim.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.Claims.RemoveAll(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
            return Task.FromResult(0);
        }


        /// <summary>
        ///     Creates the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task CreateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            db.GetCollection<TUser>(collectionName).Insert(user);

            return Task.FromResult(user);
        }

        /// <summary>
        ///     Deletes the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            db.GetCollection(collectionName).Remove((Query.EQ("_id", ObjectId.Parse(user.Id.ToString()))));
            return Task.FromResult(true);
        }

        /// <summary>
        /// Finds the by identifier asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task{`0}.</returns>
        public Task<TUser> FindByIdAsync(TKey userId)
        {
            ThrowIfDisposed();
            TUser user = db.GetCollection<TUser>(collectionName).FindOne((Query.EQ("_id", ObjectId.Parse(userId.ToString()))));
            return Task.FromResult(user);
        }

        /// <summary>
        ///     Finds the by identifier asynchronous.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <returns>Task{`0}.</returns>
        public Task<TUser> FindByIdAsync(string userId)
        {
            ThrowIfDisposed();
            TUser user = db.GetCollection<TUser>(collectionName).FindOne((Query.EQ("_id", ObjectId.Parse(userId))));
            return Task.FromResult(user);
        }

        /// <summary>
        ///     Finds the by name asynchronous.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns>Task{`0}.</returns>
        public Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            
            TUser user = db.GetCollection<TUser>(collectionName).FindOne((Query.EQ("UserName", userName)));
            return Task.FromResult(user);
        }

        /// <summary>
        /// Sets the email confirmed asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="confirmed">if set to <c>true</c> [confirmed].</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Finds the by email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Task{`0}.</returns>
        public Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();

            TUser user = db.GetCollection<TUser>(collectionName).FindOne((Query.EQ("Email", email)));
            return Task.FromResult(user);
        }

        /// <summary>
        /// Sets the email asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="email">The email.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task SetEmailAsync(TUser user, string email)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the email asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.String}.</returns>
        /// <exception cref="System.ArgumentException">user</exception>
        public Task<string> GetEmailAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            return Task.FromResult<string>(user.Email);
        }

        /// <summary>
        /// Gets the email confirmed asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.Boolean}.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Updates the user asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task UpdateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            db.GetCollection<TUser>(collectionName).Update(Query.EQ("_id", ObjectId.Parse(user.Id.ToString())), Update.Replace(user), UpdateFlags.Upsert);

            return Task.FromResult(user);
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
        }

        /// <summary>
        ///     Adds the login asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="login">The login.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Logins.Any(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey))
            {
                user.Logins.Add(login);
            }

            return Task.FromResult(true);
        }

        /// <summary>
        ///     Finds the user asynchronous.
        /// </summary>
        /// <param name="login">The login.</param>
        /// <returns>Task{`0}.</returns>
        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            TUser user = null;
            user =
                db.GetCollection<TUser>(collectionName)
                    .FindOne(Query.And(Query.EQ("Logins.LoginProvider", login.LoginProvider),
                        Query.EQ("Logins.ProviderKey", login.ProviderKey)));

            return Task.FromResult(user);
        }

        /// <summary>
        ///     Gets the logins asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{IList{UserLoginInfo}}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Logins.ToIList());
        }

        /// <summary>
        ///     Removes the login asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="login">The login.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.Logins.RemoveAll(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);

            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets the password hash asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.String}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<string> GetPasswordHashAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        /// <summary>
        ///     Determines whether [has password asynchronous] [the specified user].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<bool> HasPasswordAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash != null);
        }

        /// <summary>
        ///     Sets the password hash asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="passwordHash">The password hash.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Adds to role asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task AddToRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase))
                user.Roles.Add(role);

            return Task.FromResult(true);
        }

        /// <summary>
        ///     Gets the roles asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{IList{System.String}}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult<IList<string>>(user.Roles);
        }

        /// <summary>
        ///     Determines whether [is in role asynchronous] [the specified user].
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role.</param>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<bool> IsInRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Roles.Contains(role, StringComparer.InvariantCultureIgnoreCase));
        }

        /// <summary>
        ///     Removes from role asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="role">The role.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task RemoveFromRoleAsync(TUser user, string role)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.Roles.RemoveAll(r => String.Equals(r, role, StringComparison.InvariantCultureIgnoreCase));

            return Task.FromResult(0);
        }

        /// <summary>
        ///     Gets the security stamp asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.String}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<string> GetSecurityStampAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        ///     Sets the security stamp asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="stamp">The stamp.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        /// <summary>
        ///     Throws if disposed.
        /// </summary>
        /// <exception cref="System.ObjectDisposedException"></exception>
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        #endregion

        #region LockoutStore
        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.Factory.StartNew<int>(() => user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {

            return Task.Factory.StartNew<bool>(() => false);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return Task.Factory.StartNew<DateTimeOffset>(() => user.LockoutEndDateUtc.Value);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return Task.Factory.StartNew<int>(() => user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            throw new NotImplementedException();
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            throw new NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Gets the users.
        /// </summary>
        /// <value>The users.</value>
        public IQueryable<TUser> Users { get; private set; }

        /// <summary>
        /// Sets the phone number asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentException">user</exception>
        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentException("user");
            }

            user.PhoneNumber = phoneNumber;
            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Gets the phone number asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.String}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            
            return Task.FromResult<string>(user.PhoneNumber);
        }

        /// <summary>
        /// Gets the phone number confirmed asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.Boolean}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.PhoneNumberConfirmed);
        }

        /// <summary>
        /// Sets the phone number confirmed asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="confirmed">if set to <c>true</c> [confirmed].</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Sets the two factor enabled asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        /// <returns>Task.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult<int>(0);
        }

        /// <summary>
        /// Gets the two factor enabled asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task{System.Boolean}.</returns>
        /// <exception cref="System.ArgumentNullException">user</exception>
        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult<bool>(user.TwoFactorEnabled);
        }
    }
}
        