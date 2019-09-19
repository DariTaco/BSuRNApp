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
            var stackLayout = new StackLayout { Margin = new Thickness(10) };
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

            //creates a list with all Apps of the course
			var listView = new ListView();
			listView.ItemsSource = new string[]
			{
                //Reno Fast Retransmit/Recovery
				"Congestion Control", "Pipeline Protocols", "TCP"
			};
            //after an item was clicked, open the respective app 
			listView.ItemTapped += async (sender, e) =>
            {
                var appName = e.Item.ToString ();

                switch(appName)
                {
                    case "Congestion Control":
                        await Navigation.PushAsync(new CongestionAvoidanceSettings());
                        break;
                    case "Pipeline Protocols":
                        await Navigation.PushAsync(new PipelineProtocolsSettings());
                        break;
                    case "TCP":
                        await Navigation.PushAsync(new TCPSettings());
                        break;
                }

            };

			stackLayout.Children.Add(listView);
		}
    }
}