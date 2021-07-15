using System;
using System.Collections.Generic;

namespace WertheApp.OS
{
    public class Vector
    {
        static Dictionary<String, Vector> vectorsDict = new Dictionary<String, Vector>();
        String name;
        int dvd, laser, usb, bluray, inkjet, printer3d;

        //CONSTRUCTOR
        public Vector(String p_name,
            int p_dvd,
            int p_laser,
            int p_usb,
            int p_bluray,
            int p_inkjet,
            int p_printer3d)
        {
            this.name = p_name;
            this.dvd = p_dvd;
            this.laser = p_laser;
            this.usb = p_usb;
            this.bluray = p_bluray;
            this.inkjet = p_inkjet;
            this.printer3d = p_printer3d;

            vectorsDict.Add(this.name, this);
        }

        //MERHODS
        public void ChangeResources(int p_dvd, int p_laser, int p_usb, int p_bluray, int p_inkjet, int p_printer3d)
        {
            this.dvd = p_dvd;
            this.laser = p_laser;
            this.usb = p_usb;
            this.bluray = p_bluray;
            this.inkjet = p_inkjet;
            this.printer3d = p_printer3d;
        }

        public static Vector GetVector(String p_name)
        {
            Vector v;
            if (vectorsDict.TryGetValue(p_name, out v))
            {
                return v;
            }
            return null;
        }

        public String GetVectorString()
        {
            String s = ""
                + ConvertResourceToString(this.dvd)
                + ConvertResourceToString(this.laser)
                + ConvertResourceToString(this.usb)
                + ConvertResourceToString(this.bluray)
                + ConvertResourceToString(this.inkjet)
                + ConvertResourceToString(this.printer3d);

            return s;
        }

        String ConvertResourceToString(int r)
        {
            return r >= 0 ? r.ToString() + "   " : null;
        }

        public static void DeleteAll()
        {
            vectorsDict.Clear();
        }
    }
}