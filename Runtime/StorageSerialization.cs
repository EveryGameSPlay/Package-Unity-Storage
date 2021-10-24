namespace Egsp.Core
{
    public partial class Storage
    {
        public static void SwitchSerializer(Option<ISerializer> serializer)
        {
            if (!serializer)
                return;
            
            General.Serializer = serializer.option;
            Specified.Serializer = serializer.option;
        }
    }
}