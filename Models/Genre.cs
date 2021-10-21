using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApplication.Models
{
    [Table(name: "Genres")]
    public class Genre
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column(name: "Name")]
        public string Name { get; set; }
    }
}