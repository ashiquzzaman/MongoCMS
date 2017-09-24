using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CMS.Repositories
{
    public interface IRepository<T> : IDisposable where T : class
    {
        #region LINQ
        MongoCollection<T> DbSet { get; set; }
        MongoCollection<T> Collection { get; }
        string CollectionName { get; }
        T GetById(ObjectId id);
        void Add(IEnumerable<T> entities);

        IQueryable<T> All();
        bool Any(Expression<Func<T, bool>> predicate);
        T Select(Expression<Func<T, bool>> predicate);
        List<string> Select(Expression<Func<T, string>> predicate);
        T Find(params object[] keys);
        T Find(Expression<Func<T, bool>> predicate);
        T First(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        bool IsExist(Expression<Func<T, bool>> predicate);
        List<string> Where(Expression<Func<T, string>> predicate);
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAll(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindAll(Expression<Func<T, bool>> predicate, int index, int size);
        long Count { get; }
        int Counting(Expression<Func<T, bool>> predicate);
        string MaxValue(Expression<Func<T, string>> predicate);
        T Create(T t);
        int Update(T t);
        int Update(Expression<Func<T, bool>> predicate);
        void Update(IEnumerable<T> entities);
        int Delete(T t);
        int Delete(Expression<Func<T, bool>> predicate);
        void Delete(ObjectId id);
        void DeleteAll();
        /// <summary>
        /// Add item into repository
        /// </summary>
        /// <param name="item">Item to add to repository</param>
        void Add(T item);

        /// <summary>
        /// Delete item 
        /// </summary>
        /// <param name="item">Item to delete</param>
        void Remove(T item);

        /// <summary>
        /// Set item as modified
        /// </summary>
        /// <param name="item">Item to modify</param>
        void Modify(T item);

        /// <summary>
        ///Track entity into this repository, really in UnitOfWork. 
        ///In EF this can be done with Attach and with Update in NH
        /// </summary>
        /// <param name="item">Item to attach</param>
        void TrackItem(T item);

        /// <summary>
        /// Sets modified entity into the repository. 
        /// When calling Commit() method in UnitOfWork 
        /// these changes will be saved into the storage
        /// </summary>
        /// <param name="persisted">The persisted item</param>
        /// <param name="current">The current item</param>
        void Merge(T persisted, T current);


        /// <summary>
        /// Get all elements of type T in repository
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Number of elements in each page</param>
        /// <param name="orderByExpression">Order by expression for this query</param>
        /// <param name="ascending">Specify if order is ascending</param>
        /// <returns>List of selected elements</returns>
        IEnumerable<T> GetPaged<TProperty>(int pageIndex, int pageCount, Expression<Func<T, TProperty>> orderByExpression, bool ascending);

        
        #endregion

        IEnumerator<T> GetEnumerator();
        Type ElementType { get; }
        Expression Expression { get; }
         IQueryProvider Provider { get; }


        /// <summary>
        /// Gets a value indicating whether the collection already exists.
        /// </summary>
        /// <value>Returns true when the collection already exists, false otherwise.</value>
        bool Exists { get; }


        /// <summary>
        /// Drops the repository.
        /// </summary>
        void Drop();

        /// <summary>
        /// Tests whether the repository is capped.
        /// </summary>
        /// <returns>Returns true when the repository is capped, false otherwise.</returns>
        bool IsCapped();

        /// <summary>
        /// Drops specified index on the repository.
        /// </summary>
        /// <param name="keyname">The name of the indexed field.</param>
        void DropIndex(string keyname);

        /// <summary>
        /// Drops specified indexes on the repository.
        /// </summary>
        /// <param name="keynames">The names of the indexed fields.</param>
        void DropIndexes(IEnumerable<string> keynames);

        /// <summary>
        /// Drops all indexes on this repository.
        /// </summary>
        void DropAllIndexes();

        /// <summary>
        /// Ensures that the desired index exist and creates it if it doesn't exist.
        /// </summary>
        /// <param name="keyname">The indexed field.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse.
        /// </remarks>
        void EnsureIndex(string keyname);

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
        void EnsureIndex(string keyname, bool descending, bool unique, bool sparse);

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist.
        /// </summary>
        /// <param name="keynames">The indexed fields.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse.
        /// </remarks>
        void EnsureIndexes(IEnumerable<string> keynames);

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
        void EnsureIndexes(IEnumerable<string> keynames, bool descending, bool unique, bool sparse);

        /// <summary>
        /// Ensures that the desired indexes exist and creates them if they don't exist.
        /// </summary>
        /// <param name="keys">The indexed fields.</param>
        /// <param name="options">The index options.</param>
        /// <remarks>
        /// This is a convenience method for EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options).
        /// Index will be ascending order, non-unique, non-sparse.
        /// </remarks>
        void EnsureIndexes(IMongoIndexKeys keys, IMongoIndexOptions options);

        /// <summary>
        /// Tests whether indexes exist.
        /// </summary>
        /// <param name="keyname">The indexed fields.</param>
        /// <returns>Returns true when the indexes exist, false otherwise.</returns>
        bool IndexExists(string keyname);

        /// <summary>
        /// Tests whether indexes exist.
        /// </summary>
        /// <param name="keynames">The indexed fields.</param>
        /// <returns>Returns true when the indexes exist, false otherwise.</returns>
        bool IndexesExists(IEnumerable<string> keynames);

        /// <summary>
        /// Runs the ReIndex command on this repository.
        /// </summary>
        void ReIndex();

        /// <summary>
        /// Gets the total size for the repository (data + indexes).
        /// </summary>
        /// <returns>Returns total size for the repository (data + indexes).</returns>
        long GetTotalDataSize();

        /// <summary>
        /// Gets the total storage size for the repository (data + indexes).
        /// </summary>
        /// <returns>Returns total storage size for the repository (data + indexes).</returns>
        long GetTotalStorageSize();

        /// <summary>
        /// Validates the integrity of the repository.
        /// </summary>
        /// <returns>Returns a ValidateCollectionResult.</returns>
        /// <remarks>You will need to reference MongoDb.Driver.</remarks>
        ValidateCollectionResult Validate();

        /// <summary>
        /// Gets stats for this repository.
        /// </summary>
        /// <returns>Returns a CollectionStatsResult.</returns>
        /// <remarks>You will need to reference MongoDb.Driver.</remarks>
        CollectionStatsResult GetStats();

        /// <summary>
        /// Gets the indexes for this repository.
        /// </summary>
        /// <returns>Returns the indexes for this repository.</returns>
        GetIndexesResult GetIndexes();





    }
}
