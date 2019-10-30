using System;
using System.Diagnostics; //Debug.WriteLine("");
using Xamarin.Forms;
namespace WertheApp.RN
{
    public class DijkstraSettings: TabbedPage
    {
        public DijkstraSettings()
        {
            //Child Pages
            var child1 = new NavigationPage(new DijkstraSettingsChild1("Network 1"));
            var child2 = new NavigationPage(new DijkstraSettingsChild1("Network 2"));
            var child3 = new NavigationPage(new DijkstraSettingsChild1("Network 3"));
            var child4 = new NavigationPage(new DijkstraSettingsChild1("Network 4"));

            //Titles for Tabs
            child1.Title = "child1";
            child2.Title = "child2";
            child3.Title = "child3";
            child4.Title = "child4";
            //navigationPage.IconImageSource = "schedule.png";

            //Add Tabs with Child Pages
            Children.Add(child1);
            Children.Add(child2);
            Children.Add(child3);
            Children.Add(child4);
        }
    }
}
