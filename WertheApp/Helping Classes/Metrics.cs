using System;
namespace WertheApp.HelpingClasses
{
    //supposed to be a singleton class
    //Which helps to aquire the correct Screen Size after the phone was rotated
    public class Metrics
    {

        //VARIBLES
        private static Metrics _instance;

		public double Width { get; set; } //Width
		public double Height { get; set; } //Height


        //METHODS
        public static Metrics Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Metrics();
                }
                return _instance;
            }
        }

    }

}