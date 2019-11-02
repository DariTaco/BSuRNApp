using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
namespace WertheApp.RN
{
    public class DijkstraSettings: TabbedPage
    {
        //VARIABLES
        private DijkstraSettingsDraw skiaview0, skiaview1, skiaview2, skiaview3;
        private SKCanvasView canvas0, canvas1, canvas2, canvas3;

        //CONSTRUCTOR
        public DijkstraSettings()
        {
            Title = "Dijkstra";

            CreateContent();
            //navigationPage.IconImageSource = "schedule.png";
        }

        //METHODS
        /**********************************************************************
        *********************************************************************/
        void CreateContent()
        {
            //get canvas to draw on
            skiaview0 = new DijkstraSettingsDraw();
            canvas0 = skiaview0.ReturnCanvas();
            skiaview1 = new DijkstraSettingsDraw();
            canvas1 = skiaview1.ReturnCanvas();
            skiaview2 = new DijkstraSettingsDraw();
            canvas2 = skiaview2.ReturnCanvas();
            skiaview3 = new DijkstraSettingsDraw();
            canvas3 = skiaview3.ReturnCanvas();

            //Child Pages (Tabs)
            var child0 = new ContentPage();
            var child1 = new ContentPage();
            var child2 = new ContentPage();
            var child3 = new ContentPage();

            //assign canvas to Tab
            child0.Content = canvas0;
            child1.Content = canvas1;
            child2.Content = canvas2;
            child3.Content = canvas3;

            //Tab titles
            child0.Title = "network 1";
            child1.Title = "network 2";
            child2.Title = "network 3";
            child3.Title = "network 4";

            //Add Tabs to TabbedPage
            Children.Add(child0);
            Children.Add(child1);
            Children.Add(child2);
            Children.Add(child3);

        }
    }
}
