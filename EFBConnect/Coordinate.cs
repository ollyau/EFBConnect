namespace EFBConnect
{
    struct Coordinate
    {
        public double Latitude;
        public double Longitude;

        public Coordinate(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var comparisonObj = (Coordinate)obj;
            return (comparisonObj.Latitude == this.Latitude) && (comparisonObj.Longitude == this.Longitude);
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + Latitude.GetHashCode();
            hash = hash * 23 + Longitude.GetHashCode();
            return hash;
        }
    }
}
