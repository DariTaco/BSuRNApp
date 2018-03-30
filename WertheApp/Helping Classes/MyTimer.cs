using System;
using Xamarin.Forms;
using System.Threading;

namespace WertheApp.HelpingClasses
{
    public class MyTimer
    {
        int counter;
        TimeSpan timespan;

        public MyTimer()
        {
            counter = 0;
            timespan = new TimeSpan(10000000);
        }

        public static void StartTimer(TimeSpan interval, Func<bool> callback){
            
        }
    }
}
