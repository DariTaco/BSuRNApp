using System;
using Xamarin.Forms;
using SkiaSharp.Views.Forms;
namespace WertheApp.BS
{
    public class BuddySystemViewCell : ViewCell
    {
        private SKCanvasView skiaview;

        public BuddySystemViewCell()
        {
            // crate the canvas
            skiaview = new SKCanvasView();
            skiaview.BackgroundColor = Color.Aquamarine;
            //skiaview.HeightRequest = 100;

            // do the drawing
            skiaview.PaintSurface += (object sender, SKPaintSurfaceEventArgs e) =>
            {
            };

            // assign the canvas to the cell
            View = skiaview;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            // update the canvas when the data changes
            skiaview.InvalidateSurface();
        }
    }
}
