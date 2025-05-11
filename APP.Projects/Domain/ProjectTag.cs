using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORE.APP.Domain;

namespace APP.Projects.Domain
{
    /// <summary>
    /// Represents a linking entity between Project and Tag entities.
    /// Inherits from the base class Entity.
    /// </summary>
    public class ProjectTag : Entity
    {
        /// <summary>
        /// Gets or sets the project ID.
        /// This field is a foreign key linking to the Project entity.
        /// </summary>
        public int ProjectId { get; set; }

        /// <summary>
        /// Gets or sets the project associated with the project tag.
        /// This is a navigational property.
        /// </summary>
        public Project Project { get; set; }

        /// <summary>
        /// Gets or sets the tag ID.
        /// This field is a foreign key linking to the Tag entity.
        /// </summary>
        public int TagId { get; set; }

        /// <summary>
        /// Gets or sets the tag associated with the project tag.
        /// This is a navigational property.
        /// </summary>
        public Tag Tag { get; set; }
    }

}