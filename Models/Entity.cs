using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Face.Models
{
    public class Entity
    {
        public string file { get; set; }
        public string id { get; set; }
        public bool success { get; set; }
        public string imgsrc { get; set; }
        public string scores { get; set; }
    }

    public class scores {
        public float top { get;set;}
        public string classs { get;set;}
        public float width { get;set;}
        public float score { get;set;}
        public float height { get;set;}
        public int id { get;set;}
        public float left { get;set; }
    }
}
