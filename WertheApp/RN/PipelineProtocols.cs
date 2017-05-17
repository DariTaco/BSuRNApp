using System;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class PipelineProtocols : ContentPage
    {
        public PipelineProtocols()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Pipeline Protocols" }
                }
            };
        }
    }
}

