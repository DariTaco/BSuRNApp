using System;
using WertheApp.RN;
using Xamarin.Forms;

namespace WertheApp
{
    public class Rechnernetze : ContentPage
    {
        //CONSTRUCTOR
        public Rechnernetze()
        {
            Title = "Rechnernetze";
            var stackLayout = new StackLayout();
            this.Content = stackLayout;

            CreateContent(stackLayout);
        }

		//METHODS
        void CreateContent(StackLayout stackLayout)
		{
            var l_choose = new Label 
            { 
                Text="Choose an App:"
            };
            stackLayout.Children.Add(l_choose);

			var l_space = new Label();
			stackLayout.Children.Add(l_space);
		}
    }
}

