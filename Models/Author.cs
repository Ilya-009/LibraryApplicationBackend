using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApplication.Models
{
    [Table(name: "Authors")]
    public class Author
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}