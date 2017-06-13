﻿using System;

using Xamarin.Forms;

namespace WertheApp.RN
{
    public class PipelineProtocolsSettings : ContentPage
    {
		//VARIABLES


		//CONSTRUCTOR
		public PipelineProtocolsSettings()
		{
			Title = "Pipeline Protocols";
			CreateContent();
		}

		//METHODS
		void CreateContent()
		{
			var scrollView = new ScrollView
			{
				Margin = new Thickness(10)
			};
			var stackLayout = new StackLayout();

			this.Content = scrollView;
			scrollView.Content = stackLayout; //Wrap ScrollView around StackLayout to be able to scroll the content
		}
    }
}

