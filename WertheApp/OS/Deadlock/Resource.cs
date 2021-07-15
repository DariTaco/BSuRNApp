using System;
using System.Collections.Generic;

namespace WertheApp.OS
{
    public class Resource
    {
        //VARIABLES
        static Dictionary<String, Vector> vectorsDict = new Dictionary<String, Vector>();
        String name;

        //each resource has several pickers

        //CONSTRUCTOR
        public Resource(String p_name)
        {
            this.name = p_name;
        }

        //METHODS
    }
}
