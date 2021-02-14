using WertheApp.CN;
using Xamarin.Forms;

namespace WertheApp
{
    public class ComputerNetworks : ContentPage
    {
        //CONSTRUCTOR
        public ComputerNetworks()
        {
            Title = "Computer Networks";
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
            listView.BackgroundColor = Color.Transparent;
            listView.ItemsSource = new string[]
			{
                //Reno Fast Retransmit/Recovery
				"Congestion Control", "Pipeline Protocols", "Reno Fast Recovery", "Ack Generation", "Dijkstra"
            };
            //after an item was clicked, open the respective app 
			listView.ItemTapped += async (sender, e) =>
            {
                var appName = e.Item.ToString ();

                switch(appName)
                {
                    case "Congestion Control":
                        await Navigation.PushAsync(new CongestionControlSettings());
                        break;
                    case "Pipeline Protocols":
                        await Navigation.PushAsync(new PipelineProtocolsSettings());
                        break;
                    case "Reno Fast Recovery":                        
                        await Navigation.PushAsync(new RenoFastRecovery());
                        break;
                    case "Ack Generation":
                        await Navigation.PushAsync(new AckGeneration());
                        break;
                    case "Dijkstra":
                        DijkstraSettings.ClearNetworkList();
                        await Navigation.PushAsync(new DijkstraSettings());
                        break;
                }

            };

			stackLayout.Children.Add(listView);
		}
        protected override void OnAppearing()
        {
            base.OnAppearing();
            MessagingCenter.Send<object>(this, "Unspecified");
        }
    }
}