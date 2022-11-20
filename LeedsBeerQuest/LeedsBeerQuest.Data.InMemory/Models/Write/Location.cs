using LocationReadModel = LeedsBeerQuest.App.Models.Read.Location;

namespace LeedsBeerQuest.App.Models.Write
{
    public class Location : LocationReadModel
    {
        //oddly, MongoDBs driver does not use get-only properties, apparently.
        public double[] Coordinates
        {
            get { return new[] { Long, Lat }; }
            set { Lat = value[1]; Long = value[0]; }
        }
    }
}
