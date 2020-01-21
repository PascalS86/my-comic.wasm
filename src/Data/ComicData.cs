using System;

namespace Csandra.Comics.App.wasm.Data
{
    public class ComicData
    {
        public string ID {get;set;}
        public string Isbn { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string PictureUri { get; set; }
        public string Heroes { get; set; }
        public string Publisher { get; set; }
        public string Authors { get; set; }
        public string Artists { get; set; }
        public string Categories { get; set; }
        public string Medium { get; set; }
        public string LentTo { get; set; }
        public DateTime? LentSince { get; set; }
        public bool IsLent { get; set; }
        public bool IsKindle { get; set; }
        public int? Rating { get; set; } = 0;
        public int EditableRating { get=> Rating.GetValueOrDefault(0); set=> Rating = value; } 
        public string Genres { get; set; }
        public string Tags { get; set; } 
    }
}