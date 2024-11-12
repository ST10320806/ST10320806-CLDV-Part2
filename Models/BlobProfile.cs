namespace ST10320806_Part1.Models
{
    public class BlobProfile
    {
        public int BlobID { get; set; } // This is your primary key
        public byte[] BlobImage { get; set; } // This will hold the image data
    }

}
