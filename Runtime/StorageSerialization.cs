﻿namespace Egsp.Core
{
    public partial class Storage
    {
        public static void SwitchSerializer(Option<ISerializer> serializer)
        {
            if (!serializer)
                return;
            
            Common.Serializer = serializer.option;
            Current.Serializer = serializer.option;
        }
    }
}