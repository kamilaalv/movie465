using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.Projects.Domain
{
    /// <summary>
    /// Represents a project entity with details such as name, description, URL, version, and associated tags.
    /// Inherits from the base class Entity.
    /// </summary>
    public class Project : Entity
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// This field is required and must be between 5 and 200 characters long.
        /// </summary>
        [Required, Length(5, 200)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the project.
        /// This field is optional.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the URL of the project.
        /// The URL is optional and must be a maximum of 400 characters long.
        /// </summary>
        [StringLength(400)]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the version of the project.
        /// This field is optional and can be null.
        /// </summary>
        public double? Version { get; set; } // float no1 = 1.2f, decimal no2 = 3.4m, double no3 = 5.6

        /// <summary>
        /// Gets or sets the collection of relational project tags.
        /// This is a navigational property representing the tags associated with the project.
        /// </summary>
        public List<ProjectTag> ProjectTags { get; set; } = new List<ProjectTag>();

        /// <summary>
        /// Gets or sets the list of tag IDs associated with the project by returning relational tag IDs 
        /// from the relational ProjectTags collection or setting the relational ProjectTags collection 
        /// by assigning from the tag IDs (value) set to this property.
        /// This property is not mapped to the database therefore there won't be a column in the table.
        /// </summary>
        [NotMapped]
        public List<int> TagIds
        {
            get => ProjectTags.Select(pt => pt.TagId).ToList();
            set => ProjectTags = value.Select(v => new ProjectTag() { TagId = v }).ToList();
        }

        public List<Work> Works { get; set; } = new List<Work>();
    }
}