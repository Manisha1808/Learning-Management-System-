using System;
namespace LMS.Areas.Employee.Models
{
    public class VideoProgress
    {
        public int VideoId { get; set; }
        public string VideoTitle { get; set; }
        public int SequenceNo { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedDate { get; set; }
    }
}