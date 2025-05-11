using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.APP.Domain
{
    /// <summary>
    /// Abstract base class for all entities.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Gets or sets the ID of the entity.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class.
        /// </summary>
        protected Entity() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Entity"/> class with a specified ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        protected Entity(int id)
        {
            Id = id;
        }
    }
}
