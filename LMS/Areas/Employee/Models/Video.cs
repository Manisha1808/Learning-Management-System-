using System;

namespace LMS.Areas.Employee.Models
{
    public class Video
    {
        public int VideoId { get; set; }

        public int CourseId { get; set; }

        public string VideoTitle { get; set; }

        public string VideoUrl { get; set; }

        public string Description { get; set; }

        public int SequenceNo { get; set; }

        public bool IsActive { get; set; }
    }
}
