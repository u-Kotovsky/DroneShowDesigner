namespace Runtime.Core.Skybrush
{
    public class SkybrushTrajectory
    {
        public int version;
        public object[] points; // [ 0.0, [ 0.0, 0.0, 0.0 ] ] // time, xyz
        public float takeoffTime;
        public float landingTime;
    }
}
