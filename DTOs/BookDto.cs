using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryApplication.DTOs
{
    public class BookDto
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public short Year { get; set; }
        public long GenreId { get; set; }
        public long AuthorId { get; set; }
    }
}
