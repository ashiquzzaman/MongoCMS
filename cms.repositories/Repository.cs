using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoIdentity;

namespace CMS.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext Context;
        protected readonly bool ShareContext;//For Related Class

        public Repository(DbContext context) : this(context, false) { }

        protected Repository(DbContext context, bool sharedContext)
        {
            Context = context;
            ShareContext = sharedContext;
        }

        private MongoCollection<T> collection;
        //For connect Data base.
        public MongoCollection<T> DbSet
        {
            get{return collection ?? Context.Set<T>();}
            set { collection = value ;}
        }

        public MongoCollection<T> Collection
        {
            get { return this.collection; }
        }
        public string CollectionName
        {
            get { return this.collection.Name; }
        }
        public virtual T GetById(ObjectId id)
        {
            return this.collection.FindOneByIdAs<T>(id);
        }
        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public virtual void Add(IEnumerable<T> entities)
        {
            this.collection.InsertBatch<T>(entities);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> All()
        {
            return collection.FindAll().AsQueryable();
        }

        public bool Any(Expression<Func<T, bool>> predicate)
        {
            return this.collection.AsQueryable<T>().Any(predicate);
        }

        public T Select(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public List<string> Select(Expression<Func<T, string>> predicate)
        {
            throw new NotImplementedException();
        }

        public T Find(params object[] keys)
        {
            throw new NotImplementedException();
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public T First(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public bool IsExist(Expression<Func<T, bool>> predicate)
        {
            return this.collection.AsQueryable<T>().Any(predicate);
        }

        public List<string> Where(Expression<Func<T, string>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return collection.AsQueryable<T>().Where(predicate).AsQueryable();
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate)
        {
            return collection.AsQueryable<T>().Where(predicate).AsQueryable();
        }

        public IQueryable<T> FindAll(Expression<Func<T, bool>> predicate, int index, int size)
        {
            var skip = index * size;

            IQueryable<T> query = collection.AsQueryable<T>();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (skip != 0)
            {
                query = query.Skip(skip);
            }

            return query.Take(size).AsQueryable();
        }

        public long Count
        {
            get
            {
                   return collection.Count();
            }
        }
        /// <summary>
        /// Gets a value indicating whether the collection already exists.
        /// </summary>
        /// <value>Returns true when the collection already exists, false otherwise.</value>
        public virtual bool Exists
        {
            get { return this.collection.Exists(); }
        }

        /// <summary>
        /// Drops the collection.
        /// </summary>
        public virtual void Drop()
        {
            this.collection.Drop();
        }

        /// <summary>
        /// Tests whether the repository is capped.
        /// </summary>
        /// <returns>Returns true when the repository is capped, false otherwise.</returns>
        public virtual bool IsCapped()
        {
            return this.collection.IsCapped();
        }

        /// <summary>
        /// Drops specified index on the repository.
        /// </summary>
        /// <param name="keyname">The name of the indexed field.</param>
        public virtual void DropIndex(string keyname)
        {
            this.DropIndexes(new string[] { keyname });
        }

        /// <summary>
        /// Drops specified indexes on the repository.
        /// </summary>
        /// <param name="keynames">The names of the indexed fields.</param>
        public virtual void DropIndexes(IEnumerable<string> keynames)
        {
            this.collection.DropIndex(keynames.ToArray());
        }

        /// <summary>
        /// Drops all indexes on this repository.
        /// </summary>
        public virtual void DropAllIndexes()
        {
            this.collection.DropAllIndexes();
        }

        /// <summary>
        /// Ensures that the desired index exist and creates it if it doesn't exist.
        /// </summary>
        /// <param name="keyname">The indexed field.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse.
        /// </remarks>
        public virtual void EnsureIndex(string keyname)
        {
            this.EnsureIndexes(new string[] { keyname });
        }

        /// <summary>
        /// Ensures that the desired index exist and creates it if it doesn't exist.
        /// </summary>
        /// <param name="keyname">The indexed field.</param>
        /// <param name="descending">Set to true to make index descending, false for ascending.</param>
        /// <param name="unique">Set to true to ensure index enforces unique values.</param>
        /// <param name="sparse">Set to true to specify the index is sparse.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// </remarks>
        public virtual void EnsureIndex(string keyname, bool descending, bool unique, bool sparse)
        {
            this.EnsureIndexes(new string[] { keyname }, descending, unique, sparse);
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist.
        /// </summary>
        /// <param name="keynames">The indexed fields.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse.
        /// </remarks>
        public virtual void EnsureIndexes(IEnumerable<string> keynames)
        {
            this.EnsureIndexes(keynames, false, false, false);
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist.
        /// </summary>
        /// <param name="keynames">The indexed fields.</param>
        /// <param name="descending">Set to true to make index descending, false for ascending.</param>
        /// <param name="unique">Set to true to ensure index enforces unique values.</param>
        /// <param name="sparse">Set to true to specify the index is sparse.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// </remarks>
        public virtual void EnsureIndexes(IEnumerable<string> keynames, bool descending, bool unique, bool sparse)
        {
            var ixk = new IndexKeysBuilder();
            if (descending)
            {
                ixk.Descending(keynames.ToArray());
            }
            else
            {
                ixk.Ascending(keynames.ToArray());
            }

            this.EnsureIndexes(
                ixk,
                new IndexOptionsBuilder().SetUnique(unique).SetSparse(sparse));
        }

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist.
        /// </summary>
        /// <param name="keys">The indexed fields.</param>
        /// <param name="options">The index options.</param>
        /// <remarks>
        /// This method allows ultimate control but does "leak" some MongoDb specific implementation details.
        /// </remarks>
        public virtual void EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options)
        {
            this.collection.CreateIndex(keys, options);
        }

        /// <summary>
        /// Tests whether indexes exist.
        /// </summary>
        /// <param name="keyname">The indexed fields.</param>
        /// <returns>Returns true when the indexes exist, false otherwise.</returns>
        public virtual bool IndexExists(string keyname)
        {
            return this.IndexesExists(new string[] { keyname });
        }

        /// <summary>
        /// Tests whether indexes exist.
        /// </summary>
        /// <param name="keynames">The indexed fields.</param>
        /// <returns>Returns true when the indexes exist, false otherwise.</returns>
        public virtual bool IndexesExists(IEnumerable<string> keynames)
        {
            return this.collection.IndexExists(keynames.ToArray());
        }

        /// <summary>
        /// Runs the ReIndex command on this repository.
        /// </summary>
        public virtual void ReIndex()
        {
            this.collection.ReIndex();
        }

        /// <summary>
        /// Gets the total size for the repository (data + indexes).
        /// </summary>
        /// <returns>Returns total size for the repository (data + indexes).</returns>
        [Obsolete("This method will be removed in the next version of the driver")]
        public virtual long GetTotalDataSize()
        {
            return 0;
            //return this.collection.GetTotalDataSize();
        }

        /// <summary>
        /// Gets the total storage size for the repository (data + indexes).
        /// </summary>
        /// <returns>Returns total storage size for the repository (data + indexes).</returns>
        [Obsolete("This method will be removed in the next version of the driver")]
        public virtual long GetTotalStorageSize()
        {
            return 0;
            // return this.collection.GetTotalStorageSize();
        }

        /// <summary>
        /// Validates the integrity of the repository.
        /// </summary>
        /// <returns>Returns a ValidateCollectionResult.</returns>
        /// <remarks>You will need to reference MongoDb.Driver.</remarks>
        public virtual ValidateCollectionResult Validate()
        {
            return this.collection.Validate();
        }

        /// <summary>
        /// Gets stats for this repository.
        /// </summary>
        /// <returns>Returns a CollectionStatsResult.</returns>
        /// <remarks>You will need to reference MongoDb.Driver.</remarks>
        public virtual CollectionStatsResult GetStats()
        {
            return this.collection.GetStats();
        }

        /// <summary>
        /// Gets the indexes for this repository.
        /// </summary>
        /// <returns>Returns the indexes for this repository.</returns>
        public virtual GetIndexesResult GetIndexes()
        {
            return this.collection.GetIndexes();
        }




        public int Counting(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public string MaxValue(Expression<Func<T, string>> predicate)
        {
            throw new NotImplementedException();
        }

        public T Create(T t)
        {
            this.collection.Insert<T>(t);
            return t;
        }

        public int Update(T t)
        {
            this.collection.Save<T>(t);

            return 1;
        }
        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                this.collection.Save<T>(entity);
            }
        }
        public int Update(Expression<Func<T, bool>> predicate)
        {
            foreach (T entity in this.collection.AsQueryable<T>().Where(predicate))
            {
                Collection.Save(entity);
            }
            return 1;
        }

        public int Delete(T t)
        {
            this.Delete(t);
            return 1;
        }
        /// <summary>
        /// Deletes an entity from the repository by its ObjectId.
        /// </summary>
        /// <param name="id">The ObjectId of the entity.</param>
        public virtual void Delete(ObjectId id)
        {
            this.collection.Remove(Query.EQ("_id", id));
        }

        public int Delete(Expression<Func<T, bool>> predicate)
        {
            foreach (T entity in this.collection.AsQueryable<T>().Where(predicate))
            {
                this.Delete(entity);
            }
            return 1;
        }
        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public virtual void DeleteAll()
        {
            this.collection.RemoveAll();
        }
        public void Add(T item)
        {
            this.collection.Insert<T>(item);
        }



        public void Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void Modify(T item)
        {
            throw new NotImplementedException();
        }

        public void TrackItem(T item)
        {
            throw new NotImplementedException();
        }

        public void Merge(T persisted, T current)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetPaged<TProperty>(int pageIndex, int pageCount, Expression<Func<T, TProperty>> orderByExpression, bool @ascending)
        {
            throw new NotImplementedException();
        }

        #region IQueryable<T>
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }
     
        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get { return this.collection.AsQueryable<T>().ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { return this.collection.AsQueryable<T>().Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get { return this.collection.AsQueryable<T>().Provider; }
        }
        #endregion
    }
}
