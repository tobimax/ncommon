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
using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using Microsoft.Practices.ServiceLocation;

namespace NCommon.Data.Db4o
{
    /// <summary>
    /// Inherits from the <see cref="RepositoryBase{TEntity}"/> class to provide an implementation of a
    /// repository that uses Db4o.
    /// </summary>
    public class Db4ORepository<TEntity> : RepositoryBase<TEntity> where TEntity : class
    {
        readonly IObjectContainer _privateContainer;

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="Db4ORepository{TEntity}"/> class.
        /// </summary>
        public Db4ORepository()
        {
            if (ServiceLocator.Current != null)
            {
                var containers = ServiceLocator.Current.GetAllInstances<IObjectContainer>();

                if (containers != null)
                {
                    var objectContainers = containers as List<IObjectContainer> ?? containers.ToList();
                    if (objectContainers.Any())
                        _privateContainer = objectContainers.FirstOrDefault();
                }
            }
            
        }

        /// <summary>
        /// Gets the <see cref="IObjectContainer"/> instance that is used by the repository.
        /// </summary>
        public IObjectContainer ObjectContainer
        {
            get { return _privateContainer ?? UnitOfWork<Db4oUnitOfWork>().ObjectContainer; }
        }

        /// <summary>
        /// Gets the <see cref="IQueryable{TEntity}"/> used by the <see cref="RepositoryBase{TEntity}"/> 
        /// to execute Linq queries.
        /// </summary>
        /// <value>A <see cref="IQueryable{TEntity}"/> instance.</value>
        /// <remarks>
        /// Inheritos of this base class should return a valid non-null <see cref="IQueryable{TEntity}"/> instance.
        /// </remarks>
        protected override IQueryable<TEntity> RepositoryQuery
        {
            get { return ObjectContainer.Cast<TEntity>().AsQueryable(); }
        }

        /// <summary>
        /// Gets or sets the merge option.
        /// </summary>
        /// <value>The merge option.</value>
        /// <remarks></remarks>
        public override MergeOption MergeOption
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void Add(TEntity entity)
        {
            ObjectContainer.Store(entity);
        }

        /// <summary>
        /// Marks the entity instance to be deleted from the store.
        /// </summary>
        /// <param name="entity">An instance of <typeparamref name="TEntity"/> that should be deleted.</param>
        public override void Delete(TEntity entity)
        {
            ObjectContainer.Delete(entity);
        }

        /// <summary>
        /// Detaches a instance from the repository.
        /// </summary>
        /// <param name="entity">The entity instance, currently being tracked via the repository, to detach.</param>
        public override void Detach(TEntity entity)
        {
            ObjectContainer.Ext().Purge(entity);
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="IRepository{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The entity instance to attach back to the repository.</param>
        public override void Attach(TEntity entity)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Attaches a detached entity, previously detached via the <see cref="RepositoryBase{TEntity}.Detach"/> method.
        /// </summary>
        /// <param name="entity">The modified entity instance to attach back to the repository.</param>
        /// <param name="orignial">The original entity instance to attach back to the repository.</param>
        /// <exception cref="NotSupportedException">Implementors should throw the NotImplementedException if Attaching
        /// entities is not supported.</exception>
        public override void Attach(TEntity entity, TEntity orignial)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Attaches a collection of detached entities, previously detached via the <see cref="RepositoryBase{TEntity}.Detach"/> method.all.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The entities.</param>
        public override void AttachAll(IEnumerable<TEntity> entities)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Attaches a collection of detached entities as modified, previously detached via the <see cref="RepositoryBase{TEntity}.Detach"/> method.all.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The entities.</param>
        /// <param name="asModified">if set to <c>true</c> [as modified].</param>
        public override void AttachAll(IEnumerable<TEntity> entities, bool asModified)
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Refreshes a entity instance.
        /// </summary>
        /// <param name="entity">The entity to refresh.</param>
        public override void Refresh(TEntity entity)
        {
            ObjectContainer.Ext().Refresh(entity, 0);
        }
    }
}