using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DbEntities
{
    public class Video
    {
        public int VideoId { get; set; }
        public string VideoTitle{ get; set; } = null!;
        public string VideoUrl { get; set; } =  null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}