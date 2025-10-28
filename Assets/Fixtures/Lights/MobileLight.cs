namespace Fixtures.Lights
{
    // TODO: Boundary box visuals/limit for editor usage
    public class MobileLight : BaseMobile
    {
        public int index;

        private void Awake()
        {
            Buffer = new byte[6];
            
            // TODO: MinXPosition, MinYPosition, MinZPosition as well Max values
            MinPosition = -50;
            MaxPosition = 50;
        }
        
        public new byte[] GetDmxData()
        {
            WriteDmxPosition(0, transform.localPosition);
            
            return Buffer;
        }
    }
}
