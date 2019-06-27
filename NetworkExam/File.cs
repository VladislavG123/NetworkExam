using MaterialDesignThemes.Wpf;
using System;

namespace NetworkExam
{
    public class File
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string LocalPath { get; set; }
        public string Url { get; set; }
        public PackIconKind Icon { get; set; }
    }
}
