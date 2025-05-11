using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORE.APP.Domain;
using System.ComponentModel.DataAnnotations;

namespace APP.Projects.Domain
{
    public class Work : Entity
    {
        [Required, StringLength(300)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime DueDate { get; set; }

        public int? ProjectId { get; set; }

        public Project Project { get; set; }
    }
}