using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text;

namespace CrivServer.Data.Models
{
    public class DbContentModel
    {
        [Key]
        public int content_id { get; set; }
        public int? site_id { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string canonical { get; set; }
        public string page_title { get; set; }
        public string meta_description { get; set; }
        public string main_content { get; set; }
        public string additional_content { get; set; }
        public string layout { get; set; }
        public DateTime? published_date { get; set; }
        public int? content_type { get; set; }
        public int? status { get; set; }
        public string redirect_url { get; set; }
    }
}
