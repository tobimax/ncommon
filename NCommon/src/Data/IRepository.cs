#region license
//Copyright 2010 Ritesh Rao 

//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NCommon.Specifications;

namespace NCommon.Data
{
    /// <summary>
    /// The <see cref="IRepository{TEntity}"/> interface defines a standard contract that repository
    /// components should implement.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity that the repository encapsulates.</typeparam>
    public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        /// <summary>
        /// Determines the synchronization option for sending or receiving entities. 
        /// </summary>
        /// <value>
        /// The merge option.
        /// </value>
        MergeOption MergeOption { get; set; }

        /// <summary>
        /// Gets the a <see cref="IUnitOfWork"/> of <typeparamref name="T"/> that
        /// the repository will use to query the underlying store.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IUnitOfWork"/> implementation to retrieve.</typeparam>
        /// <returns>The <see cref="IUnitOfWork"/> implementation.</returns>
        T UnitOfWork<T>() where T : IUnitOfWork;

        /// <summary>
        /// Adds a transient instance of <paramref name="entity"/> to be tracked
        /// and persisted by the repository.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> to be persisted.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Marks the changes of an existing entity to be saved to the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be
        /// updated in the database.</param>
        /// <remarks>Implementors of this method must handle the Update scneario. </remarks>
        void Delete(TEntity entity);

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        /// <exception cref="NotSupportedException">Implentors should throw the NotImplementedException if Detaching
        /// entities is not supported.</exception>
        void Detach(TEntity entity);

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        /// <exception cref="NotSupportedException">Implementors should throw the NotImplementedException if Attaching
        /// entities is not supported.</exception>
        void Attach(TEntity entity);

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="Detach"/> method.
        /// </summary>
        /// <param name="entity">The modified entity instance to attach back to the repository.</param>
        /// <param name="original">The original entity instance to attach back to the repository.</param>
        /// <exception cref="NotSupportedException">Implementors should throw the NotImplementedException if Attaching
        /// entities is not supported.</exception>
        void Attach(TEntity entity, TEntity original);

        /// <summary>
        /// Attaches a collection of detached entities, previously detached via the <see cref="Detach"/> method.all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void AttachAll(IEnumerable<TEntity> entities);

        /// <summary>
        /// Attaches a collection of detached entities as modified, previously detached via the <see cref="Detach"/> method.all.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <param name="asModified">if set to <c>true</c> [as modified].</param>
        void AttachAll(IEnumerable<TEntity> entities, bool asModified);

        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        /// <exception cref="NotSupportedException">Implementors should throw the NotImplementedException if Refreshing
        /// entities is not supported.</exception>
        void Refresh(TEntity entity);

        /// <summary>
        /// Queries the repository based on the provided specification and returns results that
        /// are only satisfied by the specification.
        /// </summary>
        /// <param name="specification">A <see cref="ISpecification{TEntity}"/> instance used to filter results
        /// that only satisfy the specification.</param>
        /// <returns>A <see cref="IEnumerable{TEntity}"/> that can be used to enumerate over the results
        /// of the query.</returns>
        IQueryable<TEntity> Query(ISpecification<TEntity> specification);

        /// <summary>
        /// Defines the service context under which the repository will execute.
        /// </summary>
        /// <typeparam name="TService">The service type that defines the context of the repository.</typeparam>
        /// <returns>The same <see cref="IRepository{TEntity}"/> instance.</returns>
        /// <remarks>
        /// Implementors should perform context specific actions within this method call and return
        /// the exact same instance.
        /// </remarks>
        IQueryable<TEntity> For<TService>();

        /// <summary>
        /// Queries the repository based on the provided specification and returns results that
        /// are only satisfied by the specification and defines the service context under which the repository will execute.
        /// </summary>
        /// <typeparam name="TService">The type of the service.</typeparam>
        /// <param name="specification">The specification.</param>
        /// <returns></returns>
        IQueryable<TEntity> QueryFor<TService>(ISpecification<TEntity> specification);

    }
}