using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoIdentity
{
   public class DbContext:IDisposable
    {


       [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
       [SuppressMessageAttribute("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
       protected DbContext(): this("DefaultConnection")
       {
           
       }

       [SuppressMessageAttribute("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
       [SuppressMessageAttribute("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
       public DbContext(DbConnection existingConnection, bool contextOwnsConnection)
       {

       }


       [SuppressMessageAttribute("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
       [SuppressMessageAttribute("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
       public DbContext(string nameOrConnectionString)
       {
           if (nameOrConnectionString.ToLower().StartsWith("mongodb://"))
           {
               Db = GetDatabaseFromUrl(new MongoUrl(nameOrConnectionString));
           }
           else
           {
               string connStringFromManager =
                   ConfigurationManager.ConnectionStrings[nameOrConnectionString].ConnectionString;
               Db = connStringFromManager.ToLower().StartsWith("mongodb://") ? GetDatabaseFromUrl(new MongoUrl(connStringFromManager)) : GetDatabaseFromSqlStyle(connStringFromManager);
           }
       }


       internal readonly MongoDatabase Db;
       private bool _disposed;
       

       [SuppressMessageAttribute("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Set")]
       public MongoCollection<TEntity> Set<TEntity>()
           where TEntity : class
       {
           return DataBase.GetCollection<TEntity>(GetCollectionName<TEntity>());
       }
       [SuppressMessageAttribute("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get")]
       public MongoCollection<TEntity> Get<TEntity>()
           where TEntity : class
       {
           return DataBase.GetCollection<TEntity>(GetCollectionName<TEntity>());
       }
       public MongoCollection<TEntity> GetCollection<TEntity>(string name) where TEntity : class
       {
           return DataBase.GetCollection<TEntity>(name);
       }

       public MongoDatabase DataBase { get { return Db; } }


       /// <summary>
       ///     Gets the database from connection string.
       /// </summary>
       /// <param name="connectionString">The connection string.</param>
       /// <returns>MongoDatabase.</returns>
       /// <exception cref="System.Exception">No database name specified in connection string</exception>
       private MongoDatabase GetDatabaseFromSqlStyle(string connectionString)
       {
           //var conString = new MongoConnectionStringBuilder(connectionString);
           //MongoClientSettings settings = MongoClientSettings.FromUrl(conString);
           //MongoServer server = new MongoClient(settings).GetServer();
           //if (conString.DatabaseName == null)
           //{
           //    throw new Exception("No database name specified in connection string");
           //}
           //return server.GetDatabase(conString.DatabaseName);
           var databaseName = MongoUrl.Create(connectionString).DatabaseName;
           var client = new MongoClient(connectionString);
           var server = client.GetServer();
           var database = server.GetDatabase(databaseName);
           return database;

       }

       /// <summary>
       ///     Gets the database from URL.
       /// </summary>
       /// <param name="url">The URL.</param>
       /// <returns>MongoDatabase.</returns>
       private MongoDatabase GetDatabaseFromUrl(MongoUrl url)
       {
           var client = new MongoClient(url);
           MongoServer server = client.GetServer();
           if (url.DatabaseName == null)
           {
               throw new Exception("No database name specified in connection string");
           }
           return server.GetDatabase(url.DatabaseName); // WriteConcern defaulted to Acknowledged
       }

       /// <summary>
       ///     Uses connectionString to connect to server and then uses databae name specified.
       /// </summary>
       /// <param name="connectionString">The connection string.</param>
       /// <param name="dbName">Name of the database.</param>
       /// <returns>MongoDatabase.</returns>
       private MongoDatabase GetDatabase(string connectionString, string dbName)
       {
           var client = new MongoClient(connectionString);
           MongoServer server = client.GetServer();
           return server.GetDatabase(dbName);
       }

       protected virtual void Dispose(bool disposing)
       {
           _disposed = true;
       }

       public void Dispose()
       {
          Dispose(true);
       }

       public string GetCollectionName<T>() where T : class
       {
           string collectionName;
           collectionName = typeof(T).BaseType == typeof(object) ? GetCollectioNameFromInterface<T>() : GetCollectionNameFromType(typeof(T));

           if (string.IsNullOrEmpty(collectionName))
           {
               throw new ArgumentException("Collection name cannot be empty for this entity");
           }
           return collectionName;
       }
       public  string GetCollectioNameFromInterface<T>()
       {
           // Check to see if the object (inherited from Entity) has a CollectionName attribute
           var att = Attribute.GetCustomAttribute(typeof(T), typeof(CollectionName));
           var collectionname = att != null ? ((CollectionName)att).Name : typeof(T).Name;

           return collectionname;
       }

       public string GetCollectionNameFromType(Type entitytype)
       {
           string collectionname;

           // Check to see if the object (inherited from Entity) has a CollectionName attribute
           var att = Attribute.GetCustomAttribute(entitytype, typeof(CollectionName));
           collectionname = att != null ? ((CollectionName)att).Name : entitytype.Name;

           return collectionname;
       }
    }
}
