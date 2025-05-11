using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace APP.Projects.Domain
{
    /// <summary>
    /// Represents a tag, inheriting from <see cref="Entity"/>.
    /// </summary>
    public class Tag : Entity
    {
        /// <summary>
        /// Gets or sets the name of the tag.
        /// The name is required and has a maximum length of 150 characters.
        /// </summary>
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the collection of relational project tags.
        /// This is a navigational property representing the projects associated with the tag.
        /// </summary>
        public List<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();
    }
}