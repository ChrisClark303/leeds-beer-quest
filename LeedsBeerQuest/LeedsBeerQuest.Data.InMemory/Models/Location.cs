namespace LeedsBeerQuest.App.Models
{
    public class Location
    {
        public double Lat { get; set; }
        public double Long { get; set; }
        //oddly, MongoDBs driver does not use get-only properties, apparently.
        public double[] Coordinates
        {
            get { return new[] { Long, Lat }; }
            set { Lat = value[1]; Long = value[0]; }
        }
    }
}
