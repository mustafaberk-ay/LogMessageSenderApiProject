﻿namespace LogMessageSenderApiProject.Models
{
    public class Quote
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string[] Tags { get; set; }
        public int Length { get; set; }
        public string DateAdded { get; set; }
        public string DateModified { get; set; }
    }
}
