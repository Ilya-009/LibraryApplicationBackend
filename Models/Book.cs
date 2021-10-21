using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApplication.Models
{
    [Table(name: "Books")]
    public class Book
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column(name: "Name")]
        public string Name { get; set; }

        [Column(name: "Year")]
        public short Year { get; set; }

        public Genre Genre { get; set; }
        public Author Author { get; set; }
    }
}
